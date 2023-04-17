using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.StreetTraversal
{
    public class BinaryTree<T> where T:Town.Street
    {

        /// <summary>
        /// Root node for binary tree. Represents main street in district street network.
        /// </summary>
        public TreeNode<T> root;
        int capacity;

        /// <summary>
        /// Array of street objects in the order the streets would be traversed in a Euler tour of the network. A Euler tour is a Eulerian circuit through the Euler tour representation (ETR) of the tree.
        /// </summary>
        T[] eulerTour;

        /// <summary>
        /// The level of the node as it is visited in the Euler tour.
        /// </summary>
        int[] level;

        /// <summary>
        /// Dictionary with street as key and value being the index of its first appearance in Euler tour. 
        /// </summary>
        Dictionary<T, int> firstOccurence;
        int filler;
        SegmentTree segmentTree = new SegmentTree();



        /// <summary>
        /// Constructor for new binary tree object. Supplied with capacity as a parameter. Initialises Euler tour and level arrays as maximum size of a binary tree with given capacity (2*capacity –1), initialises firstOcurrence dictionary. 
        /// </summary>
        /// <param name="_capacity"></param>
        public BinaryTree(int _capacity)
        {
            capacity = _capacity;
            eulerTour = new T[2 * capacity - 1];
            level = new int[2 * capacity - 1];
            firstOccurence = new Dictionary<T, int>();
            

        }


        /// <summary>
        /// Swaps values of two parameters passed by reference. 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        void swap(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;



            return;
        }



        ///<summary>Recursive method to return the minimum value in a given range of array indices. Base case if the segment represented by the node is completely contained within the query segment or if segment node is outside of range of query segment. Makes recursive call to left and right child by splitting the segment. Each chid if queried.</summary>
        ///<param name="segmentPointer"> Pointer to segment tree </param>
        ///<param name="index"> Index of the current node in the segment tree. Begins at root i.e. index 0</param>
        ///<param name="segmentStart"> start index of the segment represented by the current node</param>
        ///<param name="segmentEnd"> end index of the segment represented by the current node</param>
        ///<param name="queryStart"> starting index of query range</param>
        ///<param name="queryEnd"> end index of query range</param>
        int RangeMinimumQueryRecurse(int index, int segmentStart, int segmentEnd, int queryStart, int queryEnd, SegmentTree segmentPointer)
        {

            if (queryStart<= segmentStart && queryEnd>= segmentEnd) //if segment represented by node is contained completely by the query segment
            {
                return segmentPointer.segmentTreeT[index];
            }

            else if (segmentEnd < queryStart || segmentStart > queryEnd)//segment of node is outside of range of query segment
            { return -1; }

            int segmentMid = (segmentStart + segmentEnd) / 2;

            int queryLeftChild = RangeMinimumQueryRecurse(2 * index + 1, segmentStart, segmentMid, queryStart, queryEnd, segmentPointer);
            int queryRightChild = RangeMinimumQueryRecurse(2 * index + 2, segmentMid + 1, segmentEnd, queryStart, queryEnd, segmentPointer);

            if (queryLeftChild == -1) { return queryRightChild; } // if query segment partly contained in parent node but not in leftChildNode then must be partly contained by rightChildNode
            else if (queryRightChild == -1) { return queryLeftChild; }
            //if query segment split across left and right children
            return (level[queryLeftChild] < level[queryRightChild] ? queryLeftChild : queryRightChild);

        }

        /// <summary>
        /// Returns the minimum of the elements in the range of the query
        /// </summary>
        /// <param name="segmentTree"></param>
        /// <param name="n"></param>
        /// <param name="queryStart">starting index of query range</param>
        /// <param name="queryEnd">end index of query range</param>
        /// <returns></returns>

        int RangeMinimumQuery(SegmentTree segmentTree, int n, int queryStart, int queryEnd)
        {

            if (queryStart<0||queryEnd> n-1||queryStart>queryEnd)
            {
                throw new IndexOutOfRangeException();
            }

            return RangeMinimumQueryRecurse(0, 0, n - 1, queryStart, queryEnd, segmentTree);


        }

        /// <summary>
        /// Recursive method to construct the segment tree. Base case for if the segment start is equal to the segment end (I.e. only one element in segment), this is a leaf in the segment tree and the value stored in SegmentTree.segmentTreeT at index of the segmentIndex is the index of the street in the levels array. When right and left children recursive calls return, it is determined if the value (level) stored in the levels array is smaller for the index stored at the left child index in the segment tree or the right child index. The levels array index (value stored in segmentTreeT at index of left/right child) of the smaller of these two values is stored at the index of the parent node in segmentTreeT array. 
        /// </summary>
        /// <param name="segmentIndex"> index of current node in segment tree</param>
        /// <param name="segmentStart"></param>
        /// <param name="segmentEnd"></param>
        /// <param name="array"> level array</param>
        /// <param name="segmentTree"></param>
        void ConstructSegmentTreeRecurse(int segmentIndex, int segmentStart, int segmentEnd, int[] array, SegmentTree segmentTree) //segmentTree is initially sent as gloabal class variable so updates in place- do not need to return calls or pass by reference
        {
            if (segmentStart == segmentEnd) //only one element in the array
            {
                segmentTree.segmentTreeT[segmentIndex] = segmentStart; //leaf nodes of segment tree are indexes of levels array https://www.geeksforgeeks.org/segment-tree-range-minimum-query/
            }
            else
            {
                int midpoint = (segmentStart + segmentEnd) / 2;
                ConstructSegmentTreeRecurse(segmentIndex * 2 + 1, segmentStart, midpoint, array, segmentTree); //constructs left child subtree
                ConstructSegmentTreeRecurse(segmentIndex * 2 + 2, midpoint + 1, segmentEnd, array, segmentTree); //constructs right child subtree
                //Each internal node represents minimum of all leaves under it
                if (array[segmentTree.segmentTreeT[segmentIndex * 2 + 1]] < array[segmentTree.segmentTreeT[segmentIndex * 2 + 2]]) 
                {
                    segmentTree.segmentTreeT[segmentIndex] = segmentTree.segmentTreeT[2 * segmentIndex + 1]; //stores minimum value of the segment as minimum occurs in left subtree
                }
                else
                {
                    segmentTree.segmentTreeT[segmentIndex] = segmentTree.segmentTreeT[segmentIndex * 2 + 2]; //stores right child as minimum occurs in right subtree
                }


            }

        }


        /// <summary>
        /// Constructs a segment tree using ConstructSegmentTreeRecurse() method to build a segment tree based on levels array from Euler tour wherein each node contains the index of the minimum value in the segment of level array represented by the node.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        int ConstructSegmentTree(int[] array, int n)
        {
            int treeHeight = (int)(Math.Log(n, 2) + 1);
            int maxSize = (1 << treeHeight + 1) - 1;
            segmentTree.segmentTreeT = new int[maxSize];
            ConstructSegmentTreeRecurse(0, 0, n - 1, array, segmentTree);
            return segmentTree.segmentTree;

        }


        /// <summary>
        /// Recursive method to find the Euler tour of street tree. (A Euler tour on an undirected tree is where the tree is thought of as a directed graph with each undirected arc being replaced by two opposite directed arcs. The Euler tour representation (ETR) of the tree is then the Eulerian circuit of the directed graph.) Method works beginning at the root node and setting the element at eulerTour array index 0 (as filler variable initialised to 0) as the root node street and setting the element at level array index 0 as the current level of the tour (the root being at level 0 and increasing down the tree). The filler variable is then incremented to indicate we are filling the next space in the arrays. The first occurrence of the node is recorded with the street used as the key and the index of first occurrence of the street in eulerTour as the value. The method is then called recursively on the left child if it exists with the current level incremented and leftChild node supplied as root. When recursive call returns, the parent is readded to the tour arrays again as it must be revisited before touring the right subtree. This is then repeated on the right subtree. 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="currentLevel"></param>
        void FindEulerTour(TreeNode<T> node, int currentLevel)
        {
            if (node != null)
            {
                eulerTour[filler] = node.item; //set to street represented by node
                level[filler] = currentLevel;
                filler++;

                if (!firstOccurence.ContainsKey(node.item)) { firstOccurence[node.item] = filler - 1; } //mark first occurence of node

                if (node.leftChild != null)
                {
                    FindEulerTour(node.leftChild, currentLevel + 1); //tour left subtree
                    
                    //add return to parent node at end of subtree tour
                    eulerTour[filler] = node.item; 
                    level[filler] = currentLevel; 
                    filler++;

                }
                if (node.rightChild != null)
                {
                    //repeat with right subtree
                    FindEulerTour(node.rightChild, currentLevel + 1);
                    eulerTour[filler] = node.item;
                    level[filler] = currentLevel;
                    filler++;

                }
            }
        }

        /// <summary>
        /// Returns the lowest common ancestor in the street tree for the start and end streets supplied as parameters. Algorithm requires the first occurrence of the start item to be earlier than for the end item, so if this is not the case the values of the start and end items are swapped first using the static swap method. The method determines the query start and query end points for the RangeMinimumQuery method as the first occurrences of the start and end items respectively. The return of RangeMinimumQuery is the index of the lowest common ancestor in the Euler tour. The street at the index of the lowest common ancestor in eulerTour is then returned. 
        /// </summary>
        /// <param name="startItem"></param>
        /// <param name="endItem"></param>
        /// <returns></returns>
        public T FindLowestCommonAncestor(T startItem, T endItem)
        {
            

            

            if (firstOccurence[startItem] > firstOccurence[endItem])
            {
                swap(ref startItem, ref endItem);


            }
            int queryStart = firstOccurence[startItem];
            int queryEnd = firstOccurence[endItem];

            int LCAIndex = RangeMinimumQuery(segmentTree, 2 * capacity - 1, queryStart, queryEnd);

            return eulerTour[LCAIndex];


        }

        /// <summary>
        /// Initialises filler to 0 to begin filling tour arrays from index 0. Calls FindEulerTour and ConstructSegmentTree, assigning return value to segmentTree.segmentTree. 
        /// </summary>
        public void PrepareTree()
        {
            filler = 0; //begin at index 0
            FindEulerTour(root, 0); //find Euler tour beginning from the root of the tree
            segmentTree.segmentTree = ConstructSegmentTree(level, 2 * capacity - 1);
        }



    }


    class SegmentTree 
    {
        public int segmentTree;
        /// <summary>
        /// Array representing segment tree. Value at leaf indices are indices of levels array, values at indices of internal nodes contain levels array index of child with the lowest level in Euler tour I.e. child highest up the tree. 
        /// </summary>
        public int[] segmentTreeT = new int[1000];
    }


}
