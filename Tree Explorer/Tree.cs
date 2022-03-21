using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree_Explorer
{
    enum TreeColor
    {
        BLACK,
        RED,
        BLUE
    }

    internal class TreeColorToRGB
    {
        public TreeColor color;
        public byte r;
        public byte g;
        public byte b;
        public TreeColorToRGB()
        {
            this.color = TreeColor.BLACK;
            this.r = 0;
            this.g = 0;
            this.b = 0;
        }
        public TreeColorToRGB(TreeColor color)
        {
            this.color = color;
            if (color == TreeColor.BLACK)
            {
                r = 0;
                g = 0;
                b = 0;
            }
            else if (color == TreeColor.RED)
            {
                r = 255;
                g = 0;
                b = 0;
            } else if(color == TreeColor.BLUE)
            {
                r = 0;
                g = 0;
                b = 255;
            }
        }
        public TreeColorToRGB(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
    }

    internal class Tree
    {
        public string info; // nama file atau folder
        public TreeColor nodeColor;
        public List<Tree> children;

        public Tree()
        {
            this.info = "";
            this.nodeColor = TreeColor.BLACK;
            this.children = new List<Tree>();
        }

        public Tree(string info)
        {
            this.info = info;
            this.nodeColor = TreeColor.BLACK;
            this.children = new List<Tree>();
        }

        public Tree(string info, TreeColor nodeColor)
        {
            this.info = info;
            this.nodeColor = nodeColor;
            this.children = new List<Tree>();
        }

        public bool isLeaf()
        {
            return this.children.Count == 0;
        }

        public void changeToRED()
        {
            if(this.nodeColor == TreeColor.BLACK)
            {
                this.nodeColor = TreeColor.RED;
            }
        }

        public void changeToBLUE()
        {
            this.nodeColor = TreeColor.BLUE;
        }

        public Tree? getChild(string nodeInfo)
        {
            // seharusnya ini pasti ketemu
            foreach(Tree child in this.children)
            {
                if(child.info.Equals(nodeInfo))
                {
                    return child;
                }
            }
            return null;
        }
    }

    internal class DFS
    {
        public string searchedFile;
        public Tree startingTree;
        public bool isOneOccurance;
        public int totalOccurance;

        public DFS(string searchedFile, Tree startingTree, bool isOneOccurance)
        {
            this.searchedFile = searchedFile;
            this.startingTree = startingTree;
            this.isOneOccurance = isOneOccurance;
            this.totalOccurance = 0;
            List<string> p = new List<string>() { this.startingTree.info};
            this.searchFile(this.startingTree, p);
        }
        // asumsi masih nyari semua file di semua subfolder
        public void searchFile(Tree tree, List<string> path)
        {
            if(!(this.isOneOccurance && this.totalOccurance == 1))
            {
                if (!tree.info.Equals(startingTree.info))
                {
                    path.Add(tree.info);
                }
                tree.changeToRED();
                if (tree.info.Equals(this.searchedFile) && tree.isLeaf())
                {
                    this.treeColoring(path);
                    this.totalOccurance++;
                }
                if (!tree.isLeaf())
                {
                    foreach (Tree child in tree.children)
                    {
                        List<string> newP = new List<string>();
                        for (int i = 0; i < path.Count; i++)
                        {
                            newP.Add(path[i]);
                        }
                        searchFile(child, newP);
                    }
                }
            }
        }

        public void treeColoring(List<string> path)
        {
            Tree nodeTree = this.startingTree;
            int i = 1;
            while(!nodeTree.isLeaf())
            {
                nodeTree.changeToBLUE();
                Tree nextNode = nodeTree.getChild(path[i]);
                i++;
                if(nextNode != null)
                {
                    nodeTree = nextNode;
                }
            }
            nodeTree.changeToBLUE();
        }
    }

    internal class BFS
    {
        public string searchedFile;
        public Tree startingTree;
        public bool isOneOccurance;

        private List<string> path;
        private Queue<Tree> queue;
        private Queue<List<string>> childPath;

        public BFS(string searchedFile, Tree startingTree, bool isOneOccurance)
        {
            this.searchedFile = searchedFile;
            this.startingTree = startingTree;
            this.isOneOccurance = isOneOccurance;
            Initial(startingTree);
            this.searchFile(this.startingTree);
        }

        public void searchFile(Tree tree)
        {
            if (this.isOneOccurance)
            {
                if (!tree.info.Equals(this.searchedFile) || !tree.isLeaf())
                {
                    BFSRecursive(tree);
                    if(queue.Count > 0)
                    {
                        searchFile(queue.Peek());
                    }
                }
                tree.changeToRED();
                if (tree.info.Equals(this.searchedFile) && tree.isLeaf())
                {
                    treeColoring(childPath.Peek());
                }
            }
            else
            {
                if (!tree.info.Equals(this.searchedFile) || !tree.isLeaf())
                {
                    BFSRecursive(tree);
                    if (queue.Count > 0)
                    {
                        searchFile(queue.Peek());
                    }
                }
                tree.changeToRED();
                if (tree.info.Equals(this.searchedFile) && tree.isLeaf())
                {
                    treeColoring(childPath.Peek());
                    queue.Dequeue();
                    path = childPath.Dequeue();
                    if (queue.Count > 0)
                    {
                        searchFile(queue.Peek());
                    }
                }
            }
        }

        private void BFSRecursive(Tree tree)
        {
            queue.Dequeue();
            path = childPath.Dequeue();
            foreach (Tree childRecursive in tree.children)
            {
                queue.Enqueue(childRecursive);
                path.Add(childRecursive.info);
                List<string> newP = new List<string>(path);
                childPath.Enqueue(newP);
                path.Remove(childRecursive.info);
            }
        }

        private void Initial(Tree tree)
        {
            path = new List<string>();
            queue = new Queue<Tree>();
            childPath = new Queue<List<string>>();
            List<string> currentPath = new List<string>();

            currentPath.Add(tree.info);
            queue.Enqueue(tree);
            childPath.Enqueue(currentPath);
        }

        public void treeColoring(List<string> path)
        {
            Tree nodeTree = this.startingTree;
            int i = 1;
            while (!nodeTree.isLeaf())
            {
                nodeTree.changeToBLUE();
                Tree nextNode = nodeTree.getChild(path[i]);
                i++;
                if (nextNode != null)
                {
                    nodeTree = nextNode;
                }
            }
            nodeTree.changeToBLUE();
        }
    }
}