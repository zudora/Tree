using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;
using SymbolTree;

namespace TreeAssignTest
{
    [TestClass]
    public class SymbolTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            //create input
            Dictionary<int, int> valFreq = new Dictionary<int, int>();

            valFreq.Add(35, 12);
            valFreq.Add(45, 14);
            valFreq.Add(45, 1);
            valFreq.Add(15, 12);
            valFreq.Add(12, 9);
            valFreq.Add(13, 6);
            valFreq.Add(3, 4);
            valFreq.Add(200, 12);
            valFreq.Add(122, 3);
            valFreq.Add(86, 20);
            valFreq.Add(42, 12);

            //submit input to tree builder
            // Order value/freq pairs and use to build tree         
            List<treeNode> nodes1 = Program.listBuild(valFreq);            
            treeNode root = new treeNode();
            Dictionary<int, treeNode> testTreeDict = Program.treeBuild(nodes1, out root);

            Dictionary<int, string> testSymbolBuild = Program.SymbolBuild(testTreeDict, root);

            Dictionary<int, string> testValues = Program.nodesWithValues(testSymbolBuild, testTreeDict);

            bool prefixMatch = prefixMatchTest(testValues);

            Assert.IsFalse(prefixMatch, "Prefix found");

            //testBadPrefixes();
        }
        
        //public void testBadPrefixes()
        //{
        //    //making sure prefixes are caught
        //    Dictionary<int, string> testValues = new Dictionary<int,string>();

        //    testValues.Add(0, "0000");
        //    testValues.Add(1, "00");
        //    testValues.Add(2, "0010");
        //    testValues.Add(3, "0100");

        //    bool prefixMatch = prefixMatchTest(testValues);

        //    Assert.IsFalse(prefixMatch, "Prefix found");

        //}

        public static bool prefixMatchTest (Dictionary<int, string> testSymbols)
        {
            //test for prefixes        
            bool prefixMatch = false;

            foreach (KeyValuePair<int, string> firstKey in testSymbols)
            {
                foreach (KeyValuePair<int, string> secondKey in testSymbols)
                {
                    if (firstKey.Key != secondKey.Key)
                    {
                        if (secondKey.Value.StartsWith(firstKey.Value))
                        {
                            prefixMatch = true;
                        }
                    }
                }
            }
            return prefixMatch;
        }    
    }
}
