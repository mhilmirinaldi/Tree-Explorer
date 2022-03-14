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
    }
}
