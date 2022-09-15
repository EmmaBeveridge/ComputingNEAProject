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

        public void Add(Vertex node)
        {
            nodes.Add(node);
        }

        public void CalculateStaticLinks(Func<Vector3, Vector3, bool> condition)
        {
            CalculateLinks(condition, nodes, linksStatic); // static link letting walk through wall??? why?
        }

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
