using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.StreetTraversal
{
    public class BinaryTree<T> where T:Town.Street
    {

       
        public TreeNode<T> root;
        int capacity;
        T[] eulerTour;
        int[] level;
        Dictionary<T, int> firstOccurence;
        int filler;
        SegmentTree segmentTree = new SegmentTree();




        public BinaryTree(int _capacity)
        {
            capacity = _capacity;
            eulerTour = new T[2 * capacity - 1];
            level = new int[2 * capacity - 1];
            firstOccurence = new Dictionary<T, int>();
            

        }


        void swap(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;



            return;
        }



       ///<summary>Gets minimum value in a given range of array indices</summary>
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

            if (queryLeftChild == -1) { return queryRightChild; } // if query segment partly contained in parent node but not in leftCHildNode then must be partly contained by rightCHildNode
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
        /// Function to construct a segment tree
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
                segmentTree.segmentTreeT[segmentIndex] = segmentStart;
            }
            else
            {
                int midpoint = (segmentStart + segmentEnd) / 2;
                ConstructSegmentTreeRecurse(segmentIndex * 2 + 1, segmentStart, midpoint, array, segmentTree); //constructs left child subtree
                ConstructSegmentTreeRecurse(segmentIndex * 2 + 2, midpoint + 1, segmentEnd, array, segmentTree); //constructs right child subtree

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

        int ConstructSegmentTree(int[] array, int n)
        {
            int treeHeight = (int)(Math.Log(n, 2) + 1);
            int maxSize = (1 << treeHeight + 1) - 1;
            segmentTree.segmentTreeT = new int[maxSize];
            ConstructSegmentTreeRecurse(0, 0, n - 1, array, segmentTree);
            return segmentTree.segmentTree;

        }


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
        public int[] segmentTreeT = new int[1000];
    }


}
