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
            int level = 0;
            Node root = new Node(nodeCount, "");
            Node parent = root;
            nodeHash.Add(root.nodeId, root);

            branch(parent, level, ref nodeCount, nodeHash);
            List<string> permList = new List<string>(); 

            foreach (int key in nodeHash.Keys)
            {
                string aggText = nodeHash[key].nodeText;
                Debug.WriteLine(aggText);
                if (!string.IsNullOrEmpty(aggText))
                { 
                    if (aggText.Length == 6)
                    {
                        permList.Add(aggText);
                    }
                }
            }

            List<string> doubleList = new List<String>();
            List<string> ringList = new List<String>();
            List<string> dupes = new List<String>();

            foreach (string perm in permList)
            {
                int flag = -1;
                foreach (string doub in doubleList)
                {                   
                    if (doub.IndexOf(perm) > -1)
                    {
                        //found a dupe. Add Perm value to dupes and break
                        dupes.Add(perm);
                        flag = 1;
                        break;
                    }
                }
                if (flag == -1)
                {
                    doubleList.Add(perm + perm);
                }
            }
            
            //Debug.WriteLine("Before"); 
            foreach (string item in permList)
            {
              //  Debug.Write(item + " ");
            }

            //dedupe
            foreach (string dupeItem in dupes)
            {
                permList.Remove(dupeItem);
            }
            //Debug.WriteLine("\n");
            //Debug.WriteLine("After");
            foreach (string item in permList)
            {
                //Debug.Write(item + " ");
            }
        }
        
        public static void branch(Node parent, int level, ref int nodeCount, Dictionary<int, Node> nodeHash)
        {
            if (level <= 5)
            {
                level++;

                for (int i = 0; i < 2; i++)
                {                                                
                    nodeCount++;
                    string newText;
                    int branchNo;
                    if (i == 0)
                    {
                        newText = parent.nodeText + "A";
                        branchNo = 0;
                    }
                    else if (i == 1)
                    {
                        newText = parent.nodeText + "B";
                        branchNo = 1;
                    }
                    else
                    {
                        newText = "";
                        branchNo = -1;
                    }

                    Node newChild = new Node(nodeCount, newText);
                    parent.childAdd(nodeCount, branchNo);
                    nodeHash.Add(newChild.nodeId, newChild);

                    //call branch on left branch
                    branch(newChild, level, ref nodeCount, nodeHash);

                }
                    

                ////add right branch
                //nodeCount++;
                //Node newB = new Node(nodeCount, parent.nodeText + "B");
                //parent.childAdd(nodeCount, 1);
                //nodeHash.Add(newB.nodeId, newB);

                ////call branch on left branch
                //branch(newB, level, ref nodeCount, nodeHash);
            }
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
