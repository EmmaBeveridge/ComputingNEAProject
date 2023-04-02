using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.NavMesh;
using Microsoft.Xna.Framework;

namespace Game1.AStar
{
    public class NodeCollection
    {
        HashSet<Vertex> nodes = new HashSet<Vertex>();

        List<Vertex> nodesDynamic = new List<Vertex>();
        NodeLinks linksStatic = new NodeLinks();
        NodeLinks linksDynamic = new NodeLinks();
        HashSet<Vertex> getLinksResult = new HashSet<Vertex>();

        /// <summary>
        /// Adds a new vertex to the node collection by adding to nodes attribute. 
        /// </summary>
        /// <param name="node"></param>
        public void Add(Vertex node)
        {
            nodes.Add(node);
        }

        /// <summary>
        /// Uses CalculateLinks method to calculate the links between the static vertices in the navmesh. 
        /// </summary>
        /// <param name="condition"></param>
        public void CalculateStaticLinks(Func<Vector3, Vector3, bool> condition)
        {
            CalculateLinks(condition, nodes, linksStatic); // static link letting walk through wall??? why?
        }


        /// <summary>
        /// Used to calculate the links between the start and end nodes (dynamic nodes) and the other static nodes within the collection. Method first checks if either the start or end nodes are contained within the static link collection. If not, the CalculateLinks method is used to calculate the dynamic links. 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="condition"></param>
        public void CalculateDynamicLinks(Vertex start, Vertex end, Func<Vector3, Vector3, bool> condition)
        {
            if (!linksStatic.Contains(start))
            {
                nodesDynamic.Add(start);
            }
            if (!linksStatic.Contains(end) && start != end)
            {
                nodesDynamic.Add(end);
            }

            CalculateLinks(condition, nodesDynamic, linksDynamic);

        }


        /// <summary>
        /// Returns all of the links between the specified node and the other nodes in the collection using the GetLinks method on both the linksStatic and linksDynamic objects.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public IEnumerable<Vertex> GetLinks(Vertex point)
        {
            getLinksResult.Clear();
            var dynamicLinks = linksDynamic.GetLinks(point);
            var staticLinks = linksStatic.GetLinks(point);

            if (dynamicLinks != null)
            {
                foreach (var link in dynamicLinks)
                {
                    getLinksResult.Add(link);
                }
            }

            if (staticLinks != null)
            {
                foreach (var link in staticLinks)
                {
                    getLinksResult.Add(link);
                }
            }


            return getLinksResult;
        }


        /// <summary>
        /// Examines all node pairs and adds a link between them if they fulfil the specified condition – in this case, the InLineofSight function defined in the Path class is used. 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="newNodes"></param>
        /// <param name="links"></param>
        void CalculateLinks(Func<Vector3, Vector3, bool> condition, IEnumerable<Vertex> newNodes, NodeLinks links)
        {
            links.Clear();

            foreach(var outer in nodes)
            {
                foreach (var inner in newNodes.Where(n => n != outer))
                {
                    



                    if (condition(outer.position, inner.position))
                    {   
                        if (outer.position.X == 345 && outer.position.Z == -105 &&  inner.position.X == 345 && inner.position.Z == 25)
                        {
                            
                        }
                        else if (outer.position.X == 345 && outer.position.Z == 25 && inner.position.X == 345 && inner.position.Z == -105) 
                        { }
                         
                        //quick fix, needs addressed later - I think because pts are collinear?? when checking if in line of sight, says they are as Crosses test goes wrong, numerator 2 evaluates to 0 when checking intersection with wall x= 345 -> 200
                        // this just stops link from these two nodes being added so cannot be traversed 

                       
                        else
                        {
                            links.AddLink(outer, inner);

                        }
                        
                    }
                }
            }

        }








    }
}
