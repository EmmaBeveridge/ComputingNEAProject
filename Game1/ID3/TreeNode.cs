using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.ID3
{
    public class TreeNode
    {
        /// <summary>
        /// String name of attribute represented by the node (e.g. HUNGER or SLEEP_PRIORITISED etc). Values to which the edges out of the node refer.
        /// </summary>
        public string Name;

        /// <summary>
        /// Index of the attribute represented by the node in attribute list constructed from the data table.
        /// </summary>
        public int AttributeListIndex;

        /// <summary>
        /// NodeAttribute object containing data for attribute represented in the tree node object.
        /// </summary>
        public NodeAttribute Attribute;

        /// <summary>
        /// Value on edge connecting node to parent node.
        /// </summary>
        public string Edge;
        /// <summary>
        /// List of TreeNode objects representing the children of the node.
        /// </summary>
        public List<TreeNode> Children;

        /// <summary>
        /// Stores whether or not the decision node is also a leaf node i.e. if all records in data set with this value for the particular attribute represented by the node have the same result for the selected need to be fulfilled.
        /// </summary>
        public bool IsLeaf;

        /// <summary>
        /// Index of column containing data for attribute described by node in data set (e.g. HUNGER or SLEEP_PRIORITISED etc)
        /// </summary>
        public int TableIndex;

        /// <summary>
        /// Constructor for new TreeNode object.
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_index"></param>
        /// <param name="_attribute"></param>
        /// <param name="_edge"></param>
        public TreeNode(string _name, int _index, NodeAttribute _attribute, string _edge)
        {
            Name = _name;
            
            AttributeListIndex = _index;
            TableIndex = _index + 1; //ID column not included in attribute index so must increment
            Attribute = _attribute;
            Edge = _edge;
            Children = new List<TreeNode>();
        }

        /// <summary>
        /// Constructor for new TreeNode object.
        /// </summary>
        /// <param name="_isLeaf"></param>
        /// <param name="_name"></param>
        /// <param name="_edge"></param>
        public TreeNode(bool _isLeaf, string _name, string _edge)
        {
            IsLeaf = _isLeaf;
            Name = _name;
            Edge = _edge;

        }




    }
}
