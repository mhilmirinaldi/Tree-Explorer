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
    }
}
