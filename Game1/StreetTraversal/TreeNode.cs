using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.StreetTraversal
{
    public class TreeNode<T>
    {
        public TreeNode<T> leftChild, rightChild;
        public T item;
        

        public TreeNode(T _item)
        {
            item = _item;
        }





    }
}
