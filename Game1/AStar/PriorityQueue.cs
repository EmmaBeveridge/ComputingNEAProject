using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.AStar
{

    /// <summary>
    /// Implementation of a priority queue using a binary heap. The heap structure requires that the root node of the heap has higher priority than the other nodes in the heap which is then also recursively true for all root nodes of all sub-heaps. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class PriorityQueue<T> //using binary heap
    {
        List<T> innerList = new List<T>();
        IComparer<T> comparer;
        public int Count { get { return innerList.Count; } }

        /// <summary>
        /// get method: returns item from innerList at specified index.
        /// Set method: assigns value at specified index of innerList to value parameter. Calls Update method to reorder heap.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index] 
        { 
            get { return innerList[index]; }
            set { innerList[index] = value; Update(index); } 
        }



        /// <summary>
        /// Constructor for new PriorityQueue object.
        /// </summary>
        public PriorityQueue()
        {
            comparer = Comparer<T>.Default;
        }


        public PriorityQueue(IComparer<T> argComparer)
        {
            comparer = argComparer;
        }


        public PriorityQueue(IComparer argComparer)
        {
            comparer = (IComparer<T>)argComparer;
        }


        /// <summary>
        /// Called whenever an item in the queue is altered or removed in order to maintain heap property using BubbleUpHeap and BubbleDownHeap methods.
        /// </summary>
        /// <param name="i"></param>
        void Update(int i)
        {
            if (BubbleUpHeap(i) < i)
            {
                return;
            }

            BubbleDownHeap();
        }

        /// <summary>
        /// Compares two nodes in order to determine priority. In this case uses AStar class Compare method to compare the estimated total cost of the node to reach the goal node, the lower of which is given higher priority. 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private int Compare(int a, int b)
        {
            return comparer.Compare(innerList[a], innerList[b]);
        }


        /// <summary>
        /// Bubbles the root node of the heap down the heap to the correct position by swapping with a child node of higher priority until there are no child nodes or child nodes are of lower priority. 
        /// </summary>
        private void BubbleDownHeap()
        {
            int priority = 0, nodePriority, leftChildPriority, rightChildPriority;

            do
            {
                nodePriority = priority;
                leftChildPriority = 2 * priority + 1;
                rightChildPriority = 2 * priority + 2;

                if (innerList.Count > leftChildPriority && Compare(priority, leftChildPriority) > 0) //innerList[priority] > innerList[leftChildPriority] i.e if total estimated cost of node at index priority is greater than that at leftChildPriority
                {
                    priority = leftChildPriority;
                }

                if (innerList.Count > rightChildPriority && Compare(priority, rightChildPriority) > 0)
                {
                    priority = rightChildPriority;
                }


                Switch(priority, nodePriority);


            }
            while (priority != nodePriority);



        }



        /// <summary>
        /// Swaps the specified node with its parent node until the parent node has higher priority than the specified node. 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private int BubbleUpHeap(int i)
        {
            int priority = i, parentPriority;

            while (true)
            {
                if (priority == 0)
                {
                    break;
                }
                parentPriority = (priority - 1) / 2; //parent node index in binary tree

                if (Compare(priority, parentPriority) < 0) 
                {

                    Switch(priority, parentPriority);
                    priority = parentPriority;

                }
                else
                {
                    break;
                }



            }

            return priority;


        }

        /// <summary>
        /// Switches the items within the innerList at the specified indices. Called whenever nodes are swapped in BubbleUpHeap and BubbleDownHeap methods. 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        void Switch(int a, int b)
        {
            T temp = innerList[a];
            innerList[a] = innerList[b];
            innerList[b] = temp;
        }

        /// <summary>
        /// Adds item to priority queue by first adding to the item to the end on the innerList, then using the BubbleUpHeap method in order to correctly place the node in the tree due to its priority.  
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int Push(T item)
        {
            innerList.Add(item);
            return BubbleUpHeap(innerList.Count - 1);
        }

        /// <summary>
        /// Returns the node of highest priority (at index 0 of innerList). This value is stored at the root of the heap so cannot be removed from the heap directly – it must first be swapped with one of the leaf nodes within the heap (e.g. the last item in the innerList). The leaf node is now in the position of the root node in the heap so must be bubbled down the heap to the correct position. As the former root is now a leaf node, it can be removed without needing to alter the tree further. 
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            var result = innerList[0];
            innerList[0] = innerList[innerList.Count - 1];
            innerList.RemoveAt(innerList.Count - 1);
            if (innerList.Count != 0)
            {
                BubbleDownHeap();

            }
            
            return result;
        
        }




        /// <summary>
        ///  Performs linear search to find item in heap fulfilling condition supplied as parameter. 
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public int Find(Func<T, bool> condition)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (condition(this[i]))
                {
                    return i;
                }

            }

            return -1;
        }


        /// <summary>
        /// Removes a node from the priority queue at the specified index. 
        /// </summary>
        /// <param name="node"></param>
        public void Remove (T node)
        {
            T formerLastNode = innerList.Last();

            int nodeIndex = innerList.IndexOf(node);

            Switch(nodeIndex, innerList.Count-1);

            innerList.Remove(node);

            Update(nodeIndex);

            


        }







        public void Clear()
        {
            innerList.Clear();
        }
        



    }
}
