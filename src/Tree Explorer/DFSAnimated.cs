using Microsoft.Msagl.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Tree_Explorer
{
    internal class DFSAnimated
    {
        public string searchedFile;
        public Tree startingTree;
        public bool isOneOccurance;
        public MainWindow mainWindow;
        public Graph graph;
        public int delayTime;

        public int totalOccurance;
        public List<string> resultPaths;

        public DFSAnimated(string searchedFile, Tree startingTree, bool isOneOccurance, MainWindow mainWindow, int speed)
        {
            this.searchedFile = searchedFile;
            this.startingTree = startingTree;
            this.isOneOccurance = isOneOccurance;
            this.mainWindow = mainWindow;
            this.delayTime = 1000 / speed;
            this.graph = new Graph();

            this.totalOccurance = 0;
            this.resultPaths = new List<string>();
        }

        public void start()
        {
            List<string> p = new List<string>() { this.startingTree.info };
            this.searchFile(this.startingTree, p, "");
            mainWindow.updateGraphControl(graph);
            mainWindow.notifyAnimatedFinished();
        }
        // asumsi masih nyari semua file di semua subfolder
        public void searchFile(Tree tree, List<string> path, string cumulPath)
        {
            string currentPath = string.IsNullOrEmpty(cumulPath) ? tree.info : cumulPath + "\\" + tree.info;
            Node n = graph.FindNode(currentPath);
            if(n == null)
            {
                addToGraph(tree, cumulPath);
                n = graph.FindNode(currentPath);
            }
            n.Attr.Color = Color.Yellow;

            mainWindow.updateGraphControl(graph);
            Thread.Sleep(delayTime);

            if (!(this.isOneOccurance && this.totalOccurance == 1))
            {
                if (!tree.info.Equals(startingTree.info))
                {
                    path.Add(tree.info);
                }
                tree.changeToRED();
                n.Attr.Color = Color.Red;
                if (tree.info.Equals(this.searchedFile) && tree.isLeaf())
                {
                    this.treeColoring(path);
                    this.totalOccurance++;
                }
                if (!tree.isLeaf())
                {
                    foreach(Tree child in tree.children)
                    {
                        addToGraph(child, currentPath);
                    }
                    foreach (Tree child in tree.children)
                    {
                        if (this.isOneOccurance && this.totalOccurance == 1)
                        {
                            //child.children.Clear();
                        }
                        else
                        {
                            List<string> newP = new List<string>();
                            for (int i = 0; i < path.Count; i++)
                            {
                                newP.Add(path[i]);
                            }
                            searchFile(child, newP, currentPath);
                        }
                    }
                }
            }
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
                if (this.resultPaths.Count == 0)
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

            if(!string.IsNullOrEmpty(parentPath))
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
            foreach(Edge x in graphNode.InEdges)
            {
                x.Attr.Color = nodeEdgeColor;
            }
        }
    }
}
