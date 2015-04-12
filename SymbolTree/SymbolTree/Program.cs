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
            Dictionary<string, int> valFreq = buildDic();
            List<treeNode> nodes1 = nodes1 = listBuild(valFreq);            
            List<treeNode> treeList = new List<treeNode>();
            treeNode root = new treeNode();
            Dictionary<int, treeNode> TreeDict = treeBuild(nodes1, out root);
            Dictionary<int, string> SymbolDict = new Dictionary<int, string>();
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

                //Debug.WriteLine("New round");write
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
            // Only assign symbols to leaf nodes
            // Add one symbol length for each level

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
                    }

                    else
                    {
                        //right side
                        childId = parent.rChild;
                    }

                    Debug.WriteLine("dir: " + dir);
                    Debug.WriteLine("childId: " + childId);
                    Debug.WriteLine("level: " + level);

                    treeNode childNode = new treeNode();
                    if (TreeDict.TryGetValue(childId, out childNode))
                    {
                        level++;
                        childNode.encoding = encString + dir;
                        SymbolDict.Add(childNode.id, childNode.encoding);
                        if (level >= 0)
                        {
                            treeAssign(childNode, level, TreeDict, SymbolDict, childNode.encoding);
                        }
                    }
                    else
                    {
                        //it's a leaf. If on the right side, go back up.
                        Debug.WriteLine("Node not found");
                        level--;
                        if (level < 0)
                        {
                            break;
                        }
                    }
                }
            }
            foreach (KeyValuePair<int, string> kvp in SymbolDict)
            {
                Debug.WriteLine(kvp.Key + ": " + kvp.Value);
            }
            Debug.WriteLine ("done");
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
            //using a string here
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
