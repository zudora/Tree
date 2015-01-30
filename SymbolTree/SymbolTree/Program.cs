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
            List<treeNode> nodes1 = nodes1 = treeBuild(valFreq);

            Dictionary<string, int> valFreq2 = buildDic2();
            List<treeNode> nodes2 = treeBuild(valFreq2);
        }

        public static List<treeNode> treeBuild(Dictionary<string, int> valFreq)
        {
            List<treeNode> workList = new List<treeNode>();
            int nodeCount = 0;

            foreach (KeyValuePair<string, int> entry in valFreq)
            {
                //create new treeNode
                treeNode newNode = new treeNode(-1, -1, nodeCount, entry.Value, entry.Key);

                //check freq value and insert                
                int freq = entry.Value;
                int pos = 0;

                while (pos < workList.Count && freq < workList[pos].freq)
                {

                    pos++;

                }

                if (workList.Count > 0 && pos < workList.Count && freq == workList[pos].freq)
                {
                    //freq values are the same. Sort by value.
                    while (pos < workList.Count && freq == workList[pos].freq && string.Compare(entry.Key, workList[pos].value) == 1)
                    {

                        pos++;

                    }

                }
                workList.Insert(pos, newNode);
                nodeCount++;
            }

            foreach (treeNode item in workList)
            {
                Debug.WriteLine(item.value + ": " + item.freq);

            }

            return workList;
        }

        static Dictionary<string, int> buildDic()
        {
            //using a string here
            Dictionary<string, int> valFreq = new Dictionary<string, int>();

            valFreq.Add("D", 12);
            valFreq.Add("B", 14);
            valFreq.Add("I", 1);
            valFreq.Add("C", 12);
            valFreq.Add("E", 9);
            valFreq.Add("R", 12);
            valFreq.Add("F", 6);
            valFreq.Add("G", 4);
            valFreq.Add("M", 12);
            valFreq.Add("H", 3);
            valFreq.Add("A", 20);

            return valFreq;
        }

        static Dictionary<string, int> buildDic2()
        {
            //using a string here
            Dictionary<string, int> valFreq2 = new Dictionary<string, int>();

            valFreq2.Add("R", 12);
            valFreq2.Add("B", 14);
            valFreq2.Add("I", 1);
            valFreq2.Add("C", 12);
            valFreq2.Add("E", 9);       
            valFreq2.Add("F", 6);
            valFreq2.Add("G", 4);
            valFreq2.Add("M", 12);
            valFreq2.Add("H", 3);
            valFreq2.Add("A", 20);
            valFreq2.Add("D", 12);

            return valFreq2;
        }
        
        public struct treeNode
        {
            public int id;
            public int lChild;
            public int rChild;
            public int freq;
            public string value;
                     
            public treeNode(int lChild, int rChild, int nodeCount, int freq, string value)
            {
                this.id = nodeCount++;
                this.lChild = lChild;
                this.rChild = rChild;
                this.freq = freq;
                this.value = value;

            }



        }
    
    }
}
