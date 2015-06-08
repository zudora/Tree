using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;
using SymbolTree;
using System.Diagnostics;

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
            valFreq.Add(42, 1);
            valFreq.Add(15, 12);
            valFreq.Add(12, 9);
            valFreq.Add(13, 6);
            valFreq.Add(3, 4);
            valFreq.Add(200, 12);
            valFreq.Add(122, 3);
            valFreq.Add(86, 20);
            valFreq.Add(41, 12);

            //submit input to tree builder
            // Order value/freq pairs and use to build tree         
            List<treeNode> nodes1 = Program.listBuild(valFreq);            
            treeNode root = new treeNode();
            Dictionary<int, treeNode> testTreeDict = Program.treeBuild(nodes1, out root);

            Dictionary<int, string> testSymbolBuild = Program.SymbolBuild(testTreeDict, root);

            Dictionary<int, string> testValues = Program.nodesWithValues(testSymbolBuild, testTreeDict);

            bool prefixMatch = prefixMatchTest(testValues);

            Assert.IsFalse(prefixMatch, "Prefix found");

            testBadPrefixes();
            blockTest();
        }

        public void testBadPrefixes()
        {
            //making sure prefixes are caught by test code. This is intended to fail!
            Dictionary<int, string> testValues = new Dictionary<int, string>();

            testValues.Add(0, "0000");
            testValues.Add(1, "00");
            testValues.Add(2, "0010");
            testValues.Add(3, "0100");

            bool prefixMatch = prefixMatchTest(testValues);

            Assert.IsTrue(prefixMatch, "Prefix not found");

        }

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
    
        public void blockTest()
        {
            int[,] testRect = new int[27,30];
            for (int x = 0; x < testRect.GetLength(0); x++)
            {
                for (int y = 0; y < testRect.GetLength(1); y++)
                {
                    string coordStr = (x.ToString() + y.ToString());
                    int coordInt = Convert.ToInt16(coordStr);
                    testRect[x, y] = coordInt;
                }
            }
            Assert.IsTrue(testRect[0,0] == 0);
            Assert.IsTrue(testRect[26, 29] == 2629);

            int[,] paddedRect = Program.edgePad(testRect);

            Assert.IsTrue(paddedRect.GetLength(0) == );

            double initBlocksWide = paddedRect.GetLength(0) / 8;
            double initBlocksHigh = paddedRect.GetLength(1) / 8;

            int intBlocksWide = (int)Math.Floor(initBlocksWide);
            int intBlocksHigh = (int)Math.Floor(initBlocksHigh);

            int widthMod = (paddedRect.GetLength(0) % 8);
            int heightMod = (paddedRect.GetLength(1) % 8);

            int paddedBlocksWide = paddedRect.GetLength(0) / 8;
            int paddedBlocksHigh = paddedRect.GetLength(1) / 8;
             
            for (int outx = 0; outx < testRect.GetLength(0); outx++)
            {
                for (int outy = 0; outy < testRect.GetLength(1); outy++)
                {                    
                    Debug.Write(testRect[outx, outy] + ", ");                    
                }
                Debug.Write("\n");
            }
            
            List<int[,]> imageBlocks = Program.blockSplit(paddedRect);
        }
    }
}
