using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.AStar
{
    class PriorityQueue<T> //using binary tree
    {
        List<T> innerList = new List<T>();
        IComparer<T> comparer;
        public int Count { get { return innerList.Count; } }
        
        
        public T this[int index] 
        { 
            get { return innerList[index]; }
            set { innerList[index] = value; Update(index); } 
        }




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

        void Update(int i)
        {
            if (BubbleUpTree(i) < i)
            {
                return;
            }

            BubbleDownTree();
        }

        private int Compare(int a, int b)
        {
            return comparer.Compare(innerList[a], innerList[b]);
        }



        private void BubbleDownTree()
        {
            int priority = 0, nodePriority, leftChildPriority, rightChildPriority;

            do
            {
                nodePriority = priority;
                leftChildPriority = 2 * priority + 1;
                rightChildPriority = 2 * priority + 2;

                if (innerList.Count > leftChildPriority && Compare(priority, leftChildPriority) > 0) //innerList[priority] > innerList[leftChildPriority]
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




        private int BubbleUpTree(int i)
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


        void Switch(int a, int b)
        {
            T temp = innerList[a];
            innerList[a] = innerList[b];
            innerList[b] = temp;
        }

        public int Push(T item)
        {
            innerList.Add(item);
            return BubbleUpTree(innerList.Count - 1);
        }


        public T Pop()
        {
            var result = innerList[0];
            innerList[0] = innerList[innerList.Count - 1];
            innerList.RemoveAt(innerList.Count - 1);
            if (innerList.Count != 0)
            {
                BubbleDownTree();

            }
            
            return result;
        
        }





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
