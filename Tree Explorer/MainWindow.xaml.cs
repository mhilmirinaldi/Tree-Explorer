using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Msagl;
using System.Drawing;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
using System.IO;
using System.Diagnostics;

namespace Tree_Explorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Tree dirTree;
        Graph graph;

        public MainWindow()
        {
            InitializeComponent();
            
            graph = new Graph();
            var edge = graph.AddEdge("A", "B");
            //graph.Attr.LayerDirection = LayerDirection.LR;
            graphControl.Graph = graph;
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
            //foreach(Tree node in tree.children)
            for(int i = tree.children.Count - 1; i >= 0; i--)
            {
                Tree node = tree.children[i];
                Node graphNode = new Node(path + "\\" + node.info);
                graphNode.LabelText = node.info;
                TreeColorToRGB nodeColor = new TreeColorToRGB(node.nodeColor);
                Microsoft.Msagl.Drawing.Color nodeEdgeColor = new Microsoft.Msagl.Drawing.Color(nodeColor.r, nodeColor.g, nodeColor.b);
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
            matchPathList.Items.Clear();

            if (directoryInfo.Exists && !string.IsNullOrEmpty(fileName))
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                bool isOneOccurance = !isAllOccurence.IsChecked.Value;
                dirTree = new Tree(path, TreeColor.BLACK);
                constructDirectoryTree(path);
                if (metodeBFS.IsChecked.Value)
                {
                    BFS bfs = new BFS(fileName, this.dirTree, isOneOccurance);
                    this.dirTree = bfs.startingTree;

                    foreach(string result in bfs.resultPaths)
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
                } else if (metodeDFS.IsChecked.Value)
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
                graph.AddNode(rootNode);

                updateGraph(path, this.dirTree);
                graphControl.Graph = this.graph;    // Mengupdate graph di UI (graphControl)
                stopwatch.Stop();
                waktuEksekusiLabel.Content = "Waktu eksekusi: " + stopwatch.ElapsedMilliseconds + " ms";
            }
            else
            {
                System.Windows.MessageBox.Show("Folder awal tidak valid atau nama file kosong!", "Tidak valid", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
