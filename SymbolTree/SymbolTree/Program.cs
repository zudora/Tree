using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SymbolTree
{
    class Program
    {
        static void Main(string[] args)
        {
            // Build the starter dictionary of values and frequencies. This is dummy input.
            Dictionary<string, int> valFreq = buildDic();

            // Order value/freq pairs and use to build tree
            List<treeNode> nodes1 = listBuild(valFreq);            
            List<treeNode> treeList = new List<treeNode>();
            treeNode root = new treeNode();
            Dictionary<int, treeNode> TreeDict = treeBuild(nodes1, out root);
            Dictionary<int, string> SymbolDict = new Dictionary<int, string>();
            foreach (KeyValuePair<int, treeNode> kvp in TreeDict)
            {
                //Debug.WriteLine("Key: " + kvp.Key + ", Value: " + kvp.Value.value);
                treeNode node = new treeNode();
                if (TreeDict.TryGetValue(kvp.Key, out node))
                {
                    string valString;
                    if (node.value == null)
                    {
                        valString = "null";
                    }
                    else
                    {
                        valString = node.value;
                    }
                    Debug.WriteLine("ID: " + node.id + " val: " + valString + " L: " + node.lChild + " R: " + node.rChild);
                }
            }
            
            treeAssign(root, 0, TreeDict, SymbolDict, null);
        }

        public static List<treeNode> listBuild(Dictionary<string, int> valFreq)
        {
            List<treeNode> workList = new List<treeNode>();
            int nodeCount = 0;

            foreach (KeyValuePair<string, int> entry in valFreq)
            {
                //create new treeNode
                treeNode newNode = new treeNode(-1, -1, nodeCount, entry.Value, entry.Key, null);

                //check freq value and insert                
                int freq = entry.Value;
                int pos = nodeInsert(freq, entry.Key, workList);

                workList.Insert(pos, newNode);
                nodeCount++;
            }
            return workList;
        }

        public static Dictionary<int, treeNode> treeBuild(List<treeNode> workList, out treeNode root)
        {
            int nodeCount = workList.Count;
            Dictionary<int, treeNode> treeDic = new Dictionary<int, treeNode>();

            while (workList.Count > 1)
            {
                treeNode ult = workList[workList.Count - 1];
                treeNode penult = workList[workList.Count - 2];
                int sumFreq = ult.freq + penult.freq;
                treeNode newNode = new treeNode(penult.id, ult.id, nodeCount, sumFreq, null, null);                
                treeDic.Add(ult.id, ult);
                treeDic.Add(penult.id, penult);                

                //how does a null sort
                int pos = nodeInsert(newNode.freq, newNode.value, workList);

                workList.Insert(pos, newNode);
                workList.Remove(workList[workList.Count - 1]);
                workList.Remove(workList[workList.Count - 1]);

                foreach (treeNode node in workList)
                {
                    //Debug.WriteLine(node.value + ": " + node.freq);                    
                }
                nodeCount++;
            }

            Debug.Assert(workList.Count == 1);

            root = workList[0];
            treeDic.Add(root.id, root);
            Debug.WriteLine(treeDic.Count);
            foreach (KeyValuePair<int, treeNode> kvp in treeDic)
            {
                Debug.WriteLine(kvp.Key, kvp.Value.value);
            }
            return treeDic;
        }

        public static void treeAssign(treeNode parent, int level, Dictionary<int, treeNode> TreeDict, Dictionary<int, string> SymbolDict, string encString)
        {           
            // For each node, get the children. If a leaf, assign a symbol and stop. Otherwise, call treeAssign on child
            // Node level tracked by level variable
            
            while (level >= 0)
            {
                for (int dir = 0; dir <= 1; dir++)
                {
                    int childId;

                    if (dir == 0)
                    {
                        //left side                    
                        childId = parent.lChild;
                        if (childId == 8)
                        {
                            Debug.Write ("8 found");
                        }
                    }

                    else
                    {
                        //right side
                        childId = parent.rChild;
                    }

                    Debug.WriteLine("direction: " + dir + " parentID: " + parent.id + " childId: " + childId + " level: " + level + " Encoding in: " + encString);

                    treeNode childNode = new treeNode();
                    if (TreeDict.TryGetValue(childId, out childNode))
                    {                        
                        level++;
                        childNode.encoding = encString + dir;
                        
                        Debug.WriteLine("Child found: " + childNode.value + " Encoding: " + childNode.encoding);

                        SymbolDict.Add(childNode.id, childNode.encoding);
                        if (level >= 0)
                        {
                            treeAssign(childNode, level, TreeDict, SymbolDict, childNode.encoding);
                        }
                    }
                    else
                    {
                        //it's a leaf. If on the left side, check the right side. If it's on the right side, go back up another level.
                        Debug.WriteLine("Node not found");
                        if (dir == 0 & level < 0)
                        {
                            break;
                        }
                        else
                        {
                            level = level - 2;
                            if (level < 0)
                            {
                                break;
                            }
                        }
                    }
                }
            }          
        }

        public static int nodeInsert(int freq, string valString, List<treeNode> workList)
        {
            int pos = 0;

            while (pos < workList.Count && freq < workList[pos].freq)
            {
                pos++;
            }

            if (workList.Count > 0 && pos < workList.Count && freq == workList[pos].freq)
            {
                //freq values are the same. Sort by value.
                while (pos < workList.Count && freq == workList[pos].freq && string.Compare(valString, workList[pos].value) == 1)
                {
                    pos++;
                }
            }
            return pos;
        }

        static Dictionary<string, int> buildDic()
        {
            //Dummy function
            //using a string here
            Dictionary<string, int> valFreq = new Dictionary<string, int>();

            valFreq.Add("D", 12);
            valFreq.Add("B", 14);
            valFreq.Add("K", 1);
            valFreq.Add("C", 12);
            valFreq.Add("G", 9);
            valFreq.Add("H", 6);
            valFreq.Add("I", 4);
            valFreq.Add("F", 12);
            valFreq.Add("J", 3);
            valFreq.Add("A", 20);
            valFreq.Add("E", 12);

            return valFreq;
        }

        static Dictionary<string, int> buildDic2()
        {
            // Same values and freqs as version 1
            // Inserted in different order to make sure results are same
            Dictionary<string, int> valFreq2 = new Dictionary<string, int>();

            valFreq2.Add("D", 12);
            valFreq2.Add("B", 14);
            valFreq2.Add("K", 1);
            valFreq2.Add("C", 12);
            valFreq2.Add("G", 9);
            valFreq2.Add("H", 6);
            valFreq2.Add("I", 4);
            valFreq2.Add("F", 12);
            valFreq2.Add("J", 3);
            valFreq2.Add("A", 20);
            valFreq2.Add("E", 12);

            return valFreq2;
        }

        public struct treeNode
        {
            public int id;
            public int lChild;
            public int rChild;
            public int freq;
            public string value;
            public string encoding;

            public treeNode(int lChild, int rChild, int id, int freq, string value, string encoding)
            {
                this.id = id;
                this.lChild = lChild;
                this.rChild = rChild;
                this.freq = freq;
                this.value = value;
                this.encoding = encoding;
            }
        }    
    }
}
