using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymbolTree
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, int> valFreq = buildDic();
            
            List<treeNode>workList = new List<treeNode>();

            foreach(KeyValuePair<string,int> entry in valFreq)
            {

                

            }
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
            valFreq.Add("F", 6);
            valFreq.Add("G", 4);
            valFreq.Add("H", 3);
            valFreq.Add("A", 20);

            return valFreq;
        }
        
        
        public struct treeNode
        {
            public treeNode(int leftId, int rightId, int nodeCount)
            {
                int id = nodeCount++;
                int lChild = leftId;
                int rChild = rightId;

            }



        }
    
    }
}
