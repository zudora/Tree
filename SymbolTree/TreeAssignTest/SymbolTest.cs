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

            //test matrix for levels and DCT
            int[,] testMatrix = matFill();
            List<int[,]> testMatList = new List<int[,]>();
            testMatList.Add(testMatrix);
            List<int[,]> leveledMatList = Program.levelOff(testMatList);
            int[,] leveledMatrix = leveledMatList[0];

            
        }

        public static int[,] matFill()
        {
            int[,] testMatrix = new int[8, 8];

            testMatrix[0, 0] =154;	
            testMatrix[1, 0] =123;	
            testMatrix[2, 0] =123;	
            testMatrix[3, 0] =123;	
            testMatrix[4, 0] =123;	
            testMatrix[5, 0] =123;	
            testMatrix[6, 0] =123;	
            testMatrix[7, 0] =136;
                     
            testMatrix[0, 1] =192;	
            testMatrix[1, 1] =180;	
            testMatrix[2, 1] =136;	
            testMatrix[3, 1] =154;	
            testMatrix[4, 1] =154;	
            testMatrix[5, 1] =154;	
            testMatrix[6, 1] =136;	
            testMatrix[7, 1] =110;
                     
            testMatrix[0, 2] =254;	
            testMatrix[1, 2] =198;	
            testMatrix[2, 2] =154;	
            testMatrix[3, 2] =154;	
            testMatrix[4, 2] =180;	
            testMatrix[5, 2] =154;	
            testMatrix[6, 2] =123;	
            testMatrix[7, 2] =123;
                     
            testMatrix[0, 3] =239;	
            testMatrix[1, 3] =180;	
            testMatrix[2, 3] =136;	
            testMatrix[3, 3] =180;	
            testMatrix[4, 3] =180;	
            testMatrix[5, 3] =166;	
            testMatrix[6, 3] =123;	
            testMatrix[7, 3] =123;
                     
            testMatrix[0, 4] =180;	
            testMatrix[1, 4] =154;	
            testMatrix[2, 4] =136;	
            testMatrix[3, 4] =167;	
            testMatrix[4, 4] =166;	
            testMatrix[5, 4] =149;	
            testMatrix[6, 4] =136;	
            testMatrix[7, 4] =136;
                     
            testMatrix[0, 5] =128;	
            testMatrix[1, 5] =136;	
            testMatrix[2, 5] =123;	
            testMatrix[3, 5] =136;	
            testMatrix[4, 5] =154;	
            testMatrix[5, 5] =180;	
            testMatrix[6, 5] =198;	
            testMatrix[7, 5] =154;
                     
            testMatrix[0, 6] =123;	
            testMatrix[1, 6] =105;	
            testMatrix[2, 6] =110;	
            testMatrix[3, 6] =149;	
            testMatrix[4, 6] =136;	
            testMatrix[5, 6] =136;	
            testMatrix[6, 6] =180;	
            testMatrix[7, 6] =166;
                     
            testMatrix[0, 7] =110;	
            testMatrix[1, 7] =136;	
            testMatrix[2, 7] =123;	
            testMatrix[3, 7] =123;	
            testMatrix[4, 7] =123;	
            testMatrix[5, 7] =136;	
            testMatrix[6, 7] =154;
            testMatrix[7, 7] = 136;
            
            return testMatrix;

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
