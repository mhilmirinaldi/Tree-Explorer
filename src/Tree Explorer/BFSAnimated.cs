using Microsoft.Msagl.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tree_Explorer
{
    internal class BFSAnimated
    {

        public MainWindow mainWindow;
        public Graph graph;
        public int delayTime;

        public string searchedFile;
        public Tree startingTree;
        public bool isOneOccurance;
        public List<string> resultPaths;

        private List<string> path;
        private Queue<Tree> queue;
        private Queue<List<string>> childPath;

        public BFSAnimated(string searchedFile, Tree startingTree, bool isOneOccurance, MainWindow mainWindow, int speed)
        {
            this.mainWindow = mainWindow;
            this.delayTime = 1000 / speed;
            this.graph = new Graph();

            this.searchedFile = searchedFile;
            this.startingTree = startingTree;
            this.isOneOccurance = isOneOccurance;
            this.resultPaths = new List<string>();
        }

        public void start()
        {
            Initial(startingTree);
            this.searchFile(this.startingTree);
            mainWindow.notifyAnimatedFinished();
        }

        public void searchFile(Tree tree)
        {
            string currentPath = generateCumulativePath(childPath.Peek());
            Node n = graph.FindNode(currentPath);
            if(n == null)
            {
                addToGraph(tree, "");
                n = graph.FindNode(currentPath);
            }
            n.Attr.Color = Color.Yellow;

            mainWindow.updateGraphControl(graph);
            Thread.Sleep(delayTime);

            if (this.isOneOccurance)
            {
                if (!tree.info.Equals(this.searchedFile) || !tree.isLeaf())
                {
                    BFSRecursive(tree);

                    changeGraphNodeColor(currentPath, TreeColor.RED);
                    if (queue.Count > 0)
                    {
                        searchFile(queue.Peek());
                    }
                }
                tree.changeToRED();
                if (tree.info.Equals(this.searchedFile) && tree.isLeaf())
                {
                    treeColoring(childPath.Peek());
                    while (this.queue.Count > 0)
                    {
                        Tree temp = queue.Dequeue();
                        temp.children.Clear();
                    }
                }
            }
            else
            {
                if (!tree.info.Equals(this.searchedFile) || !tree.isLeaf())
                {
                    BFSRecursive(tree);

                    changeGraphNodeColor(currentPath, TreeColor.RED);
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
                addToGraph(childRecursive, generateCumulativePath(path));
                queue.Enqueue(childRecursive);
                path.Add(childRecursive.info);
                List<string> newP = new List<string>(path);
                childPath.Enqueue(newP);
                path.Remove(childRecursive.info);
                mainWindow.updateGraphControl(this.graph);
                Thread.Sleep(delayTime);
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

        public static string generateCumulativePath(List<string> paths)
        {
            string cumulativePath = "";
            for(int i = 0; i < paths.Count-1; i++)
            {
                cumulativePath += (paths[i] + "\\");
            }
            cumulativePath += paths[paths.Count-1];
            return cumulativePath;
        }

        public void treeColoring(List<string> path)
        {
            Tree nodeTree = this.startingTree;
            string cumulPath = "";
            int i = 1;
            while (!nodeTree.isLeaf())
            {
                nodeTree.changeToBLUE();
                changeGraphNodeColor(cumulPath + nodeTree.info, TreeColor.BLUE);
                cumulPath += nodeTree.info + "\\";
                Tree nextNode = nodeTree.getChild(path[i]);
                i++;
                if (nextNode != null)
                {
                    nodeTree = nextNode;
                }
                if(this.resultPaths.Count == 0)
                {
                    mainWindow.updateGraphControl(this.graph);
                    Thread.Sleep(delayTime);
                }
            }
            nodeTree.changeToBLUE();
            cumulPath += nodeTree.info;
            changeGraphNodeColor(cumulPath, TreeColor.BLUE);
            mainWindow.updateGraphControl(graph);
            Thread.Sleep(delayTime);
            this.resultPaths.Add(cumulPath);
        }

        public void addToGraph(Tree node, string parentPath)
        {
            Node graphNode = null;
            if (string.IsNullOrEmpty(parentPath))
            {
                graphNode = new Node(node.info);
            }
            else
            {
                graphNode = new Node(parentPath + "\\" + node.info);
            }
            graphNode.LabelText = node.info;
            TreeColorToRGB nodeColor = new TreeColorToRGB(node.nodeColor);
            Color nodeEdgeColor = new Color(nodeColor.r, nodeColor.g, nodeColor.b);
            graphNode.Attr.Color = nodeEdgeColor;
            graphNode.Attr.LabelMargin = 5;
            graphNode.Attr.LineWidth = 2;
            graph.AddNode(graphNode);

            if (!string.IsNullOrEmpty(parentPath))
            {
                Edge newEdge = this.graph.AddEdge(parentPath, graphNode.Id);
                newEdge.Attr.Color = nodeEdgeColor;
            }
        }

        public void changeGraphNodeColor(string nodeId, TreeColor newColor)
        {
            Node graphNode = this.graph.FindNode(nodeId);
            TreeColorToRGB nodeColor = new TreeColorToRGB(newColor);
            Color nodeEdgeColor = new Color(nodeColor.r, nodeColor.g, nodeColor.b);
            graphNode.Attr.Color = nodeEdgeColor;
            foreach (Edge x in graphNode.InEdges)
            {
                x.Attr.Color = nodeEdgeColor;
            }
        }
    }
}
