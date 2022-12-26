using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.ID3
{
    public class TreeNode
    {
        public string Name;
        public int AttributeListIndex;
        public NodeAttribute Attribute;
        public string Edge;
        public List<TreeNode> Children;
        public bool IsLeaf;


        public int TableIndex;


        public TreeNode(string _name, int _index, NodeAttribute _attribute, string _edge)
        {
            Name = _name;
            AttributeListIndex = _index;
            TableIndex = _index + 1; //ID column not included in attribute index so must increment
            Attribute = _attribute;
            Edge = _edge;
            Children = new List<TreeNode>();
        }

        public TreeNode(bool _isLeaf, string _name, string _edge)
        {
            IsLeaf = _isLeaf;
            Name = _name;
            Edge = _edge;

        }




    }
}
