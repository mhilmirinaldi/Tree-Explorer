using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Microsoft.Msagl.Drawing;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;

namespace Tree_Explorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Tree dirTree;
        Graph graph;
        DFSAnimated dfsanim;
        BFSAnimated bfsanim;
        Stopwatch stopwatch;

        public MainWindow()
        {
            InitializeComponent();
            
            graph = new Graph();
            var edge = graph.AddEdge("A", "B");
            graphControl.Graph = graph;
        }

        public static Graph cloneGraph(Graph graph)
        {
            Graph newGraph = new Graph();
            foreach (Node n in graph.Nodes)
            {
                Node nn = new Node(n.Id);
                nn.Attr.Color = new Color(n.Attr.Color.R, n.Attr.Color.G, n.Attr.Color.B);
                nn.Label.Text = n.Label.Text;
                nn.Attr.LabelMargin = n.Attr.LabelMargin;
                nn.Attr.LineWidth = n.Attr.LineWidth;
                newGraph.AddNode(nn);
            }
            
            Stack<Edge> s = new Stack<Edge>();
            foreach(Edge e in graph.Edges)
            {
                s.Push(e);
            }
            while(s.Count > 0)
            {
                Edge e = s.Pop();
                Edge ne = newGraph.AddEdge(e.Source, e.Target);
                ne.Attr.Color = new Color(e.Attr.Color.R, e.Attr.Color.G, e.Attr.Color.B);
            }
            return newGraph;
        }

        public void updateGraphControl(Graph graph)
        {
            Graph newGraph = cloneGraph(graph);
            this.Dispatcher.Invoke(() =>
            {
                graphControl.Graph = new Graph();
                graphControl.Graph = newGraph;
            });
        }

        public void notifyAnimatedFinished()
        {
            this.Dispatcher.Invoke(() =>
            {
                if(stopwatch != null)
                {
                    searchButton.IsEnabled = true;
                    stopwatch.Stop();
                    waktuEksekusiLabel.Content = "Waktu eksekusi: " + stopwatch.ElapsedMilliseconds + " ms";
                }

                skipButton.Visibility = Visibility.Hidden;

                List<string> resultPaths = null;
                if(dfsanim != null)
                {
                    resultPaths = dfsanim.resultPaths;
                } else if(bfsanim != null)
                {
                    resultPaths = bfsanim.resultPaths;
                }

                foreach (string result in resultPaths)
                {
                    ListBoxItem listItem = new ListBoxItem();
                    StackPanel sp = new StackPanel();
                    sp.Orientation = System.Windows.Controls.Orientation.Horizontal;

                    System.Windows.Controls.Label label = new System.Windows.Controls.Label();
                    label.Content = result;

                    System.Windows.Controls.Button button = new System.Windows.Controls.Button();
                    button.DataContext = result;
                    button.Content = "Open";
                    button.Click += (o, e) =>
                    {
                        System.Windows.Controls.Button button = (System.Windows.Controls.Button)o;
                        DirectoryInfo dirInfo = new DirectoryInfo((string)button.DataContext);
                        ProcessStartInfo startInfo = new ProcessStartInfo("explorer.exe", dirInfo.Parent.FullName);
                        Process.Start(startInfo);
                    };

                    sp.Children.Add(button);
                    sp.Children.Add(label);
                    matchPathList.Items.Add(sp);
                }

                dfsanim = null;
                bfsanim = null;
                stopwatch = null;
            });
        }

        private void chooseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog picker = new FolderBrowserDialog();
            DialogResult result = picker.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.OK)
            {
                startingFolderName.Text = picker.SelectedPath;
            }
        }

        void updateGraph(string path, Tree tree)
        {
            for(int i = tree.children.Count - 1; i >= 0; i--)
            {
                Tree node = tree.children[i];
                Node graphNode = new Node(path + "\\" + node.info);
                graphNode.LabelText = node.info;
                TreeColorToRGB nodeColor = new TreeColorToRGB(node.nodeColor);
                Color nodeEdgeColor = new Color(nodeColor.r, nodeColor.g, nodeColor.b);
                graphNode.Attr.Color = nodeEdgeColor;
                graphNode.Attr.LabelMargin = 5;
                graphNode.Attr.LineWidth = 2;
                this.graph.AddNode(graphNode);

                Edge newEdge = this.graph.AddEdge(path, graphNode.Id);
                newEdge.Attr.Color = nodeEdgeColor;
                if(node.children.Count > 0)
                {
                    updateGraph(path + "\\" + node.info, node);
                }
            }
        }

        // Memodifikasi atribut dirTree kelas ini
        void constructDirectoryTree(string path)
        {
            FileInfo[]  fileInfos = new DirectoryInfo(path).GetFiles();
            foreach(FileInfo fileInfo in fileInfos)
            {
                this.dirTree.children.Add(new Tree(fileInfo.Name, TreeColor.BLACK));
            }

            DirectoryInfo[] directoryInfos = new DirectoryInfo(path).GetDirectories();
            Tree originalTree = this.dirTree;
            foreach (DirectoryInfo directoryInfo in directoryInfos)
            {
                Tree tree = new Tree(directoryInfo.Name, TreeColor.BLACK);
                this.dirTree = tree;
                constructDirectoryTree(directoryInfo.FullName);
                originalTree.children.Add(tree);
            }
            this.dirTree = originalTree;
        }


        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            string path = startingFolderName.Text;
            string fileName = inputFileName.Text;
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            bool isOneOccurance = !isAllOccurence.IsChecked.Value;
            matchPathList.Items.Clear();

            if (directoryInfo.Exists && !string.IsNullOrEmpty(fileName))
            {
                dirTree = new Tree(path, TreeColor.BLACK);
                constructDirectoryTree(path);

                if (checkBoxAnimated.IsChecked.Value)
                {
                    this.stopwatch = new Stopwatch();
                    stopwatch.Start();

                    searchButton.IsEnabled = false;
                    skipButton.Visibility = Visibility.Visible;
                    int speed = (int)sliderValue.Value;
                    if (metodeDFS.IsChecked.Value)
                    {
                        dfsanim = new DFSAnimated(fileName, this.dirTree, isOneOccurance, this, speed);
                        Thread dfsanimThread = new Thread(dfsanim.start);
                        dfsanimThread.Start();
                    }
                    else if(metodeBFS.IsChecked.Value)
                    {
                        bfsanim = new BFSAnimated(fileName, this.dirTree, isOneOccurance, this, speed);
                        Thread bfsanimThread = new Thread(bfsanim.start);
                        bfsanimThread.Start();
                    }
                }
                else
                {
                    searchButton.IsEnabled = false;
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    if (metodeBFS.IsChecked.Value)
                    {
                        BFS bfs = new BFS(fileName, this.dirTree, isOneOccurance);
                        this.dirTree = bfs.startingTree;

                        foreach (string result in bfs.resultPaths)
                        {
                            ListBoxItem listItem = new ListBoxItem();
                            StackPanel sp = new StackPanel();
                            sp.Orientation = System.Windows.Controls.Orientation.Horizontal;

                            System.Windows.Controls.Label label = new System.Windows.Controls.Label();
                            label.Content = result;

                            System.Windows.Controls.Button button = new System.Windows.Controls.Button();
                            button.DataContext = result;
                            button.Content = "Open";
                            button.Click += (o, e) =>
                            {
                                System.Windows.Controls.Button button = (System.Windows.Controls.Button)o;
                                DirectoryInfo dirInfo = new DirectoryInfo((string)button.DataContext);
                                ProcessStartInfo startInfo = new ProcessStartInfo("explorer.exe", dirInfo.Parent.FullName);
                                Process.Start(startInfo);
                            };

                            sp.Children.Add(button);
                            sp.Children.Add(label);
                            matchPathList.Items.Add(sp);
                        }
                    }
                    else if (metodeDFS.IsChecked.Value)
                    {
                        DFS dfs = new DFS(fileName, this.dirTree, isOneOccurance);
                        this.dirTree = dfs.startingTree;

                        foreach (string result in dfs.resultPaths)
                        {
                            ListBoxItem listItem = new ListBoxItem();
                            StackPanel sp = new StackPanel();
                            sp.Orientation = System.Windows.Controls.Orientation.Horizontal;

                            System.Windows.Controls.Label label = new System.Windows.Controls.Label();
                            label.Content = result;

                            System.Windows.Controls.Button button = new System.Windows.Controls.Button();
                            button.DataContext = result;
                            button.Content = "Open";
                            button.Click += (o, e) =>
                            {
                                System.Windows.Controls.Button button = (System.Windows.Controls.Button)o;
                                DirectoryInfo dirInfo = new DirectoryInfo((string)button.DataContext);
                                ProcessStartInfo startInfo = new ProcessStartInfo("explorer.exe", dirInfo.Parent.FullName);
                                Process.Start(startInfo);
                            };

                            sp.Children.Add(button);
                            sp.Children.Add(label);
                            matchPathList.Items.Add(sp);
                        }
                    }

                    this.graph = new Graph();
                    Node rootNode = new Node(path);
                    TreeColorToRGB rootColor = new TreeColorToRGB(this.dirTree.nodeColor);
                    rootNode.Attr.Color = new Microsoft.Msagl.Drawing.Color(rootColor.r, rootColor.g, rootColor.b);
                    rootNode.Attr.LabelMargin = 5;
                    rootNode.Attr.LineWidth = 2;
                    this.graph.AddNode(rootNode);

                    updateGraph(path, this.dirTree);

                    // Mengupdate graph di UI (graphControl)
                    graphControl.Graph = new Graph();
                    graphControl.Graph = this.graph;

                    stopwatch.Stop();
                    waktuEksekusiLabel.Content = "Waktu eksekusi: " + stopwatch.ElapsedMilliseconds + " ms";
                    searchButton.IsEnabled = true;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Folder awal tidak valid atau nama file kosong!", "Tidak valid", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void skipButton_Click(object sender, RoutedEventArgs e)
        {
            if(dfsanim != null) dfsanim.delayTime = 0;
            if(bfsanim != null) bfsanim.delayTime = 0;
        }
    }
}
