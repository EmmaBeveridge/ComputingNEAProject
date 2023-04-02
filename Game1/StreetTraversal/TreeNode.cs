using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.StreetTraversal
{
    public class TreeNode<T>
    {
        /// <summary>
        /// Left child of node in street tree. 
        /// </summary>
        public TreeNode<T> leftChild;

        /// <summary>
        /// Right child of node in street tree
        /// </summary>
        public TreeNode<T> rightChild;

        /// <summary>
        /// Item stored at the tree node I.e the street represented by the node 
        /// </summary>
        public T item;

        /// <summary>
        /// Constructor for new TreeNode object; stores item parameter in item attribute. 
        /// </summary>
        /// <param name="_item"></param>
        public TreeNode(T _item)
        {
            item = _item;
        }





    }
}
