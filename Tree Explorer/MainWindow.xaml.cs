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
            foreach(Tree node in tree.children)
            {
                Node graphNode = new Node(path + "\\" + node.info);
                graphNode.LabelText = node.info;
                this.graph.AddNode(graphNode);
                this.graph.AddEdge(path, graphNode.Id);
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
                this.dirTree.children.Add(new Tree(fileInfo.Name));
            }

            DirectoryInfo[] directoryInfos = new DirectoryInfo(path).GetDirectories();
            Tree originalTree = this.dirTree;
            foreach (DirectoryInfo directoryInfo in directoryInfos)
            {
                Tree tree = new Tree(directoryInfo.Name, TreeColor.RED);
                this.dirTree = tree;
                constructDirectoryTree(directoryInfo.FullName);
                originalTree.children.Add(tree);
            }
            this.dirTree = originalTree;
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            string path = startingFolderName.Text;
            if (!string.IsNullOrEmpty(path))
            {
                dirTree = new Tree(path, TreeColor.RED);
                constructDirectoryTree(path);

                this.graph = new Graph();
                graph.AddNode(path);
                updateGraph(path, this.dirTree);
                graphControl.Graph = this.graph;
            }
        }
    }
}
