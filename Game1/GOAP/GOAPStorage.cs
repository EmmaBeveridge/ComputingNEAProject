using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GOAP
{
    public class GOAPStorage
    {
        // The maximum number of nodes we can store
        const int MaxNodes = 128;

        private readonly GOAPNode[] opened = new GOAPNode[MaxNodes];
        private readonly GOAPNode[] closed = new GOAPNode[MaxNodes];

        private int numOpened;
        private int numClosed;

        private int lastFoundOpened;
        private int lastFoundClosed;

        /// <summary>
        /// Constructor for new GOAPStorage object 
        /// </summary>
        internal GOAPStorage()
        {
        }


        /// <summary>
        /// Clears GOAPStorage. Sets all values in opened and closed arrays to null. Sets numOpened and numClosed attributes to 0. Sets lastFoundClosed and lastFoundOpened attributes to 0.
        /// </summary>
        public void Clear()
        {
            for (var i = 0; i < this.numOpened; i++)
            {
                this.opened[i] = null;
            }

            for (var i = 0; i < this.numClosed; i++)
            {
                this.closed[i] = null;
            }

            this.numOpened = this.numClosed = 0;
            this.lastFoundClosed = this.lastFoundOpened = 0;
        }

        /// <summary>
        /// Find node in opened list with matching cared about bits in its world state using Equals method on node.WorldState attribute for each node in opened list. 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public GOAPNode FindOpened(GOAPNode node)
        {
            for (var i = 0; i < this.numOpened; i++)
            {
                //var care = node.WorldState.DontCare ^ -1L;
                //if ((node.WorldState.Values & care) == (this.opened[i].WorldState.Values & care))
                //{
                //    this.lastFoundOpened = i;
                //    return this.opened[i];


                //}

                if (node.WorldState.Equals(this.opened[i].WorldState))
                {
                    this.lastFoundOpened = i;
                    return this.opened[i];

                }

            }
            return null;
        }

        /// <summary>
        /// Find node in closed list with matching cared about bits in its world state using Equals method on node.WorldState attribute for each node in closed list. 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public GOAPNode FindClosed(GOAPNode node)
        {
            for (var i = 0; i < this.numClosed; i++)
            {
                //long care = node.WorldState.DontCare ^ -1L;
                //if ((node.WorldState.Values & care) == (this.closed[i].WorldState.Values & care))
                //{
                //    this.lastFoundClosed = i;
                //    return this.closed[i];
                //}


                if (node.WorldState.Equals(this.closed[i].WorldState))
                {
                    this.lastFoundClosed = i;
                    return this.closed[i];

                }

            }
            return null;
        }

        /// <summary>
        /// Checks if storage has opened nodes by returning if numOpened greater than 0. 
        /// </summary>
        /// <returns></returns>
        public bool HasOpened()
        {
            return this.numOpened > 0;
        }

        /// <summary>
        ///  Removes node at index of lastFoundOpened from opened list and decrements numOpened counter. 
        /// </summary>
        /// <param name="node">Not necessary as always node at last found opened index</param>
        public void RemoveOpened(GOAPNode node)
        {
            
            if (this.numOpened > 0)
            {
                this.opened[this.lastFoundOpened] = this.opened[this.numOpened - 1];
            }
            this.numOpened--;
        }


        /// <summary>
        /// Removes node at index of lastFoundClosed from closed list and decrements numClosed counter.
        /// </summary>
        /// <param name="node">Not necessary as always node at last found closed index</param>
        public void RemoveClosed(GOAPNode node)
        {
            


            if (this.numClosed > 0)
            {
                this.closed[this.lastFoundClosed] = this.closed[this.numClosed - 1];
            }
            this.numClosed--;
        }

        /// <summary>
        /// Returns if specified node is in the opened array. 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsOpen(GOAPNode node)
        {
            return Array.IndexOf(this.opened, node) > -1;
        }

        /// <summary>
        /// Returns if the specified node is in the closed array. 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsClosed(GOAPNode node)
        {
            return Array.IndexOf(this.closed, node) > -1;
        }

        /// <summary>
        /// Increments numOpened and adds node to opened array at index of new numOpened. 
        /// </summary>
        /// <param name="node"></param>
        public void AddToOpenList(GOAPNode node)
        {
            this.opened[this.numOpened++] = node;
        }

        /// <summary>
        /// Increments numClosed and adds node to closed array at index of new numClosed. 
        /// </summary>
        /// <param name="node"></param>
        public void AddToClosedList(GOAPNode node)
        {
            this.closed[this.numClosed++] = node;
            
        }

        /// <summary>
        /// Finds the node in open list with the lowest combined cost and heuristic. Removes this node from the opened list and returns the node. 
        /// </summary>
        /// <returns></returns>
        public GOAPNode RemoveCheapestOpenNode()
        {
            var lowestVal = float.MaxValue;
            this.lastFoundOpened = -1;
            for (var i = 0; i < this.numOpened; i++)
            {
                if (this.opened[i].CostSoFarAndHeuristicCost < lowestVal)
                {
                    lowestVal = this.opened[i].CostSoFarAndHeuristicCost;
                    this.lastFoundOpened = i;
                }
            }
            var val = this.opened[this.lastFoundOpened];
            this.RemoveOpened(val);

            return val;
        }

    }
}
