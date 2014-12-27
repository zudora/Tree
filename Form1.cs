using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Tree
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
            //Rules:
            // Every Node has zero to two children (in Node definition)
            // Add A on the left and B on the right.
            // Go six deep.
            Dictionary<int, Node> nodeHash= new Dictionary<int,Node>();
            int nodeCount = 0;
            Node root = new Node(nodeCount, "");
            Node parent = root;
            nodeHash.Add(root.nodeId, root);

            //Steps for adding. This adds left and right children.
            nodeCount++;
            Node newA = new Node(nodeCount,"A");
            parent.childAdd(nodeCount, 0);
            nodeHash.Add(newA.nodeId, newA);
            nodeCount++; 
            Node newB = new Node(nodeCount, "B");
            parent.childAdd(nodeCount, 1);
            nodeHash.Add(newB.nodeId, newB);

            // Keep track of iteration level when going bown a branch and compare to desired depth.
            // At root, iteration level is 0.
            // Then add A. Increment level to 1.
            // Then add left child (A), producing sequence root-A-A. Increment level.
            // Continue until level equals desired depth (note that root adds a symbol)
            // Roll up one level and add right branch.
            // Roll up TWO levels and add left and right branches, with roll-ups.
            // How to identify when to roll up and by how much?
            //  If level == depth and branch == left (i.e. text == "A"), roll up 1 and add right branch
            //  If level == depth and branch == right (i.e. text == "B"), roll up 2 and add right branch, but then add left and right as usual

            //For root, just create it
            //Then call add on root
            //That increments and checks the level
            // If it's less than or equal to the depth, create a left branch.
            // If it's greater than the depth, decrement it by two and add the right branch to the parent of the last node. (Keep track or just look up?)
            //
            // Call add on the new node

        }
        
        public static void placeNode(string inText)
        {
            // a tree is a collection of nodes with IDs
            // each node has two pointers, left and right, which each may be null or may contain the ID of another node
            // to find a node without searching the whole tree, maybe we'd have to map it in a dictionary? With int/ID to node?
            // adding a new node is different depending on whether node is leaf or not
            // 
            // determining placement of node
            // need organized way to search tree
            //  take advantage of whatever rules exist. E.g. if tree sorts numbers, certain branches may not need to be searched. 
            //  If all entries at a certain level have something in common (such as number of symbols), skip to correct level on each branch
            // identify root node. Variable keeps track of this.
            // use dictionary to look up child IDs and follow correct branch.
            // 
            // How is tree built?
            //  Is there ever a reason to cut a branch and insert a node between nodes? Why might we do that?

        }
    }
    
    public class Node
    {
        public int nodeId;
        public string nodeText;
        public int left;
        public int right;

        public Node(int inId, string inText)
        {
            nodeId = inId;
            if (inText != "") {nodeText = inText;}
        }

        public void childAdd(int childId, int branch)
        {
            if (branch == 0)
            {
                left = childId;
            }

            else
            {
                right = childId;
            }
        }
    }
}
