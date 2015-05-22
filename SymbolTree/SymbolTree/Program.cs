using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace SymbolTree
{
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
        public class Program
    {
        static void Main(string[] args)
        {
            // Build the starter dictionary of values and frequencies. This is dummy input for now.
            Dictionary<string, int> valFreq = buildDic();
            
            // Order value/freq pairs and use to build tree
            List<treeNode> nodes1 = listBuild(valFreq);
            List<treeNode> treeList = new List<treeNode>();
            treeNode root = new treeNode();
            Dictionary<int, treeNode> TreeDict = treeBuild(nodes1, out root);

            Dictionary<int, string> SymbolDict = SymbolBuild(TreeDict, root);
        }

        static Dictionary<string, int> freqDict(float[,] inputData)
        {
            Dictionary<string, int> builtFreq = new Dictionary<string, int>();
            return builtFreq;
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

        public static void nodeDump(treeNode parent, Dictionary<int, treeNode> TreeDict, int depth, Dictionary<int, string> SymbolDict)
        {
            string symbol;
            if (depth == 0)
            {
                symbol = SymbolDict[parent.id];
                Debug.WriteLine(parent.id + "(" + parent.value + ") : " + symbol);
            }
            depth++;
            treeNode child = new treeNode(-1, -1, -1, -1, null, null);

            for (int i = 0; i <= 1; i++)
            {
                if (i == 0)
                {
                    if (parent.lChild != -1)
                    {
                        child = TreeDict[parent.lChild];
                        //Debug.Write("\t");
                    }
                }
                else
                {
                    if (parent.rChild != -1)
                    {
                        child = TreeDict[parent.rChild];
                        StringBuilder indent = new StringBuilder();
                        for (int j = 0; j < depth; j++)
                        {
                            indent.Append("\t");
                        }
                            //Debug.Write("\n" + indent + "    ");
                    }
                }
                
                if (child.id != -1)
                {
                    symbol = SymbolDict[child.id];
                    Debug.WriteLine(child.id + "(" + child.value + ") : " + symbol);
                    nodeDump(child, TreeDict, depth, SymbolDict);
                }
            }
        }

        public static Dictionary<int, string> SymbolBuild(Dictionary<int, treeNode> TreeDict, treeNode root)
        {            
            Dictionary<int, string> SymbolBuild = new Dictionary<int, string>();
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
                    //Debug.WriteLine("ID: " + node.id + " val: " + valString + " L: " + node.lChild + " R: " + node.rChild);
                }
            }
            treeAssign(root, 0, TreeDict, SymbolBuild, null);
             
            //Dump tree
            nodeDump(root, TreeDict, 0, SymbolBuild);
            
            return SymbolBuild;
        }
        
        public static void treeAssign(treeNode parent, int level, Dictionary<int, treeNode> TreeDict, Dictionary<int, string> SymbolDict, string encString)
        {
            if (level == 0)
            {
                SymbolDict.Add(parent.id, parent.encoding);
            }
            level++;
            for (int dir = 0; dir <= 1; dir++)
            {
                int childId;
                //Debug.WriteLine(level);
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
                
                treeNode childNode = new treeNode();
                if (TreeDict.TryGetValue(childId, out childNode))
                {                    
                    childNode.encoding = encString + dir;

                    //Debug.WriteLine("Child found: " + childNode.value + " Encoding: " + childNode.encoding);

                    SymbolDict.Add(childNode.id, childNode.encoding);
                
                    treeAssign(childNode, level, TreeDict, SymbolDict, childNode.encoding);                    
                }
            }
        }

        public static Dictionary<int, string> nodesWithValues(Dictionary<int, string> AssignedSymbols, Dictionary<int, treeNode> inTree)
        {
            Dictionary<int, string> nodesWithValues = new Dictionary<int, string>();

            foreach (KeyValuePair<int, string> kvp in AssignedSymbols)
            {
                if (inTree[kvp.Key].value != null)
                {
                    nodesWithValues.Add(kvp.Key, kvp.Value);
                }
            }

            return nodesWithValues;
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
                nodeCount++;
            }

            Debug.Assert(workList.Count == 1);

            root = workList[0];
            treeDic.Add(root.id, root);
            foreach (KeyValuePair<int, treeNode> kvp in treeDic)
            {
                //Debug.WriteLine(kvp.Key, kvp.Value.value);
            }
            return treeDic;
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
    }
}
