using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics;

namespace SymbolTree
{
    public struct treeNode
    {
        public int id;
        public int lChild;
        public int rChild;
        public int freq;
        public int value;
        public string encoding;

        public treeNode(int lChild, int rChild, int id, int freq, int value, string encoding)
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
            //import image file. Get the pixel data in channels.
            string imageFile = "C:\\Users\\Betsy\\Pictures\\testCat.jpg";
            List<int[,]> imageChannels = imageData(imageFile);
            List<float[,]> dctChannels = new List<float[,]>();

            //use imageSave to test atatus 
            //imageSave(imageChannels);

            //split data into blocks
            int[,] paddedRect = edgePad(imageChannels[0]);
            List<int[,]> imageBlocks = blockSplit(paddedRect);
            
            //generate transform matrices
            float[,] basisMatrix = dctBasis();
            float[,] basisTrans = transposeMatrix(basisMatrix);

            //perform DCT
            //level off inputs by subtracting 128 from each value


            //quantize

            //reduce with run-length encoding if possible 

            //build value frequencies and assign Huffman symbols
            Dictionary<int, int> redFreqCollapse = freqDict(imageChannels[0]);

            foreach (KeyValuePair<int, int> kvp in redFreqCollapse)
            {
                //Debug.WriteLine("Pixel value:" + kvp.Key + ", frequency:" + kvp.Value);
            }

            List<treeNode> nodes1 = listBuild(redFreqCollapse);
            List<treeNode> treeList = new List<treeNode>();
            treeNode root = new treeNode(-1, -1, -1, -1, -1, null);
            Dictionary<int, treeNode> TreeDict = treeBuild(nodes1, out root);

            Dictionary<int, string> SymbolDict = SymbolBuild(TreeDict, root);

            foreach (KeyValuePair<int, string> symbol in SymbolDict)
            {
                int origVal = TreeDict[symbol.Key].value;
                int origFreq = TreeDict[symbol.Key].freq;

                //Debug.WriteLine("Key: " + symbol.Key + " Symbol: \"" + symbol.Value + "\" Freq: " + origFreq + " Value: " + origVal);

            }            
        }

        public static List<int[,]> imageData(string imagePath)
        {
            Bitmap loadImage = new Bitmap(imagePath);

            //Debug.WriteLine(loadImage.PixelFormat);

            int[,] redChannel = new int[loadImage.Width, loadImage.Height];
            int[,] greenChannel = new int[loadImage.Width, loadImage.Height];
            int[,] blueChannel = new int[loadImage.Width, loadImage.Height];

            for (int x = 0; x < loadImage.Width; x++)
            {
                for (int y = 0; y < loadImage.Height; y++)
                {
                    Color pixColor = loadImage.GetPixel(x, y);                                        
                                        
                    redChannel[x, y] = pixColor.R;
                    greenChannel[x, y] = pixColor.G;
                    blueChannel[x, y] = pixColor.B;
                }
            }

            List<int[,]> imageData = new List<int[,]>(3);
            imageData.Add(redChannel);
            imageData.Add(greenChannel);
            imageData.Add(blueChannel);
                 
            return imageData;
        }      

        public static void imageSave(List<int[,]> imageData)
        {
            //full
            int width = imageData[0].GetLength(0);
            int height = imageData[0].GetLength(1);
            Bitmap output = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Bitmap greenOutput = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int red = imageData[0][x, y];
                    int green = imageData[1][x, y];
                    int blue = imageData[2][x, y];

                    output.SetPixel(x, y, Color.FromArgb(0, red, green, blue));
                    greenOutput.SetPixel(x, y, Color.FromArgb(0, 0, green, 0));                    
                }
            }
            
            output.Save("C:\\Users\\Betsy\\Pictures\\outCat.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            greenOutput.Save("C:\\Users\\Betsy\\Pictures\\greenCat.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }

        public static Dictionary<int, int> freqDict(int[,] inputData)
        {
            Dictionary<int, int> builtFreq = new Dictionary<int, int>();
            for (int x = 0; x < inputData.GetLength(0); x++)
            {
                for (int y = 0; y < inputData.GetLength(1); y++)
                {
                    int pix = inputData[x, y];
                    int freq;
                    
                    if (builtFreq.TryGetValue(pix, out freq))
                    {
                        //increment
                        builtFreq[pix]++;
                    }
                    else
                    {
                        //add new entry
                        builtFreq.Add(pix, 1);
                    }
                }
            }

            return builtFreq;
        }

        public static List<treeNode> listBuild(Dictionary<int, int> valFreq)
        {
            List<treeNode> workList = new List<treeNode>();
            int nodeCount = 0;

            foreach (KeyValuePair<int, int> entry in valFreq)
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
                //Debug.WriteLine(parent.id + "(" + parent.value + ") : " + symbol);
            }
            depth++;
            treeNode child = new treeNode(-1, -1, -1, -1, -1, null);

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
                    //Debug.WriteLine(child.id + "(" + child.value + ") : " + symbol);
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
                treeNode node = new treeNode(-1, -1, -1, -1, -1, null);
                if (TreeDict.TryGetValue(kvp.Key, out node))
                {
                    int valInt;
                    if (node.value == -1)
                    {
                        valInt = 0;
                    }
                    else
                    {
                        valInt = node.value;
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

                treeNode childNode = new treeNode(-1, -1, -1, -1, -1, null);
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
                if (inTree[kvp.Key].value != -1)
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
                treeNode newNode = new treeNode(penult.id, ult.id, nodeCount, sumFreq, -1, null);
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

        public static int nodeInsert(int freq, int valInt, List<treeNode> workList)
        {
            int pos = 0;

            while (pos < workList.Count && freq < workList[pos].freq)
            {
                pos++;
            }

            if (workList.Count > 0 && pos < workList.Count && freq == workList[pos].freq)
            {
                //freq values are the same. Sort by value.
                while (pos < workList.Count && freq == workList[pos].freq && (valInt==workList[pos].value))
                {
                    pos++;
                }
            }
            return pos;
        }

        public static int[,] edgePad(int[,] imagePixels)
        {
            //fill in dummy pixels at edge. Make sure to transmit original pixel size to reverse this later
            int width = imagePixels.GetLength(0);
            int height = imagePixels.GetLength(1);

            //get number of blocks each way
            int blocksHigh = (int)(Math.Ceiling(height / 8.0F));
            int blocksWide = (int)(Math.Ceiling(width / 8.0F));

            int widthMod = 8 - (width % 8);
            int heightMod = 8 - (height % 8);

            int[,] paddedPixels = new int[blocksWide * 8, blocksHigh * 8];

            // so bad. why did you do it this way?
            // how about:
            // start with a copy the whole thing filling the top left
            // figure out what remains:
            // rectangle 1 is the extra rows below
            // rectange 2 is the extra columns to the right
            // rectangle 3 is the intersection
            // for 1 and 2, pull off the matching array and reflect it: top/bottom for 1, LR for 2
            // for 3, maybe:
            // average the bottom 3.height and the right 3.width pixels from 1 and 2?

            for (int x = 0; x < width + widthMod; x++)
            {
                for (int y = 0; y < height + heightMod; y++)
                {
                    if (x < width && y < height)
                    {
                        //just clone the existing x
                        paddedPixels[x, y] = imagePixels[x, y];
                    }
                    else if (x >= width)
                    {
                        if (y < height)
                        {
                            //copy over dummy fill
                            paddedPixels[x, y] = imagePixels[x - widthMod, y];
                        }
                        else
                        {
                            //blend
                            paddedPixels[x, y] = imagePixels[x - widthMod, y - heightMod];
                        }
                    }
                    else
                    {
                        // This is a basic copy. Would be better to flip to avoid ringing at border
                        paddedPixels[x, y] = imagePixels[x, y - heightMod];
                    }
                }
            }
            
            return paddedPixels;
        }

        public static List<int[,]> blockSplit(int[,] paddedPixels)
        {                          
            //blocks are added left to right and top to bottom 
            List<int[,]> blocks = new List<int[,]>();
            
            int width = paddedPixels.GetLength(0);
            int height = paddedPixels.GetLength(1);

            //get number of blocks each way
            int blocksHigh = (int)(Math.Ceiling(height / 8.0F));
            int blocksWide = (int)(Math.Ceiling(width / 8.0F));
            int xPos = 0;
            int yPos = 0;

            int totalBlocks = blocksWide * blocksHigh;
            
            // add blocks like this:
            // top corner
            // [0, 0] [8, 0] [16, 0]
            // [0, 8] [8, 8] [16, 8]

            // iterate through x in inner loop
            Debug.WriteLine("Expected blocks: " + blocksHigh * blocksWide);
            
            while (yPos < height)
            {
                
                while (xPos < width)
                {
                    int[,] newBlock = new int[8, 8]; 
                    Debug.WriteLine("xPos = " + xPos + ", yPos = " + yPos);
                    for (int yBit = 0; yBit < 8; yBit++)
                    {
                        for (int xBit = 0; xBit < 8; xBit++)
                        {
                            newBlock[xBit, yBit] = paddedPixels[xPos + xBit, yPos + yBit];                            
                        }
                    }

                    blocks.Add(newBlock);
                    xPos += 8;
    
                    for (int x = 0; x < 8; x++)
                    {
                        for (int y = 0; y < 8; y++)
                        {
                            Debug.Write(newBlock[x, y] + ", ");
                        }
                    Debug.Write("\n");
                    }
                    Debug.WriteLine("Next block");
                }
                
                                               
                yPos += 8;
                xPos = 0;
            }            
            return blocks;
        }

        public static float[,] dctBasis()
        {
            float[,] cellBasis = new float[8, 8];
            
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (i == 0)
                    {
                        float basis = 1.0F / ((float)Math.Sqrt(8.0F));
                        cellBasis[i, j] = basis;
                    }
                    else
                    {
                        float basis = (float)(Math.Sqrt(2.0F / 8.0F) * Math.Cos(((2.0F * j + 1.0F) * i * Math.PI) / 16.0F));
                        cellBasis[i, j] = basis;
                    }                  
                }                
            }
            return cellBasis;
        }

        public static float[,] transposeMatrix(float[,] inputMatrix)
        {
            float[,] transMatrix = new float[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    transMatrix[j, i] = inputMatrix[i, j];
                }
            }

            return transMatrix;
        }
        
        public static int[,] encodedImage(List<int[,]> rawImage, Dictionary<int, string> symbolTree)
        {
            int[,] pixelEncoding = null;

            
            foreach (int[,] pixel in rawImage)
            {
                
            }            
            
            return pixelEncoding;
        }

        public static void symbolDecode(Dictionary<int, int> symbolTee)
        {
            
        }

        //static Dictionary<string, int> buildDic()
        //{
        //    //Dummy function
        //    //using a string here
        //    Dictionary<string, int> valFreq = new Dictionary<string, int>();

        //    valFreq.Add("D", 12);
        //    valFreq.Add("B", 14);
        //    valFreq.Add("K", 1);
        //    valFreq.Add("C", 12);
        //    valFreq.Add("G", 9);
        //    valFreq.Add("H", 6);
        //    valFreq.Add("I", 4);
        //    valFreq.Add("F", 12);
        //    valFreq.Add("J", 3);
        //    valFreq.Add("A", 20);
        //    valFreq.Add("E", 12);

        //    return valFreq;
        //}

        //static Dictionary<string, int> buildDic2()
        //{
        //    // Same values and freqs as version 1
        //    // Inserted in different order to make sure results are same
        //    Dictionary<string, int> valFreq2 = new Dictionary<string, int>();

        //    valFreq2.Add("D", 12);
        //    valFreq2.Add("B", 14);
        //    valFreq2.Add("K", 1);
        //    valFreq2.Add("C", 12);
        //    valFreq2.Add("G", 9);
        //    valFreq2.Add("H", 6);
        //    valFreq2.Add("I", 4);
        //    valFreq2.Add("F", 12);
        //    valFreq2.Add("J", 3);
        //    valFreq2.Add("A", 20);
        //    valFreq2.Add("E", 12);

        //    return valFreq2;
        //}       
    }
}
