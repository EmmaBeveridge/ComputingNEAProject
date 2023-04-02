using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.NavMesh;

namespace Game1.AStar
{
    class NodeLinks
    {
        /// <summary>
        /// Dictionary containing each vertex in the mesh as a key and a list of the vertices with which there is a link as the value.
        /// </summary>
        public Dictionary<Vertex, List<Vertex>> links = new Dictionary<Vertex, List<Vertex>>();



        /// <summary>
        /// Adds a bidirectional link between the two nodes. 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void AddLink(Vertex from, Vertex to)
        {
            if (!links.ContainsKey(from))
            {
                links.Add(from, new List<Vertex>());
            }

            if (!links.ContainsKey(to))
            {
                links.Add(to, new List<Vertex>());
            }

            if (!links[from].Contains(to))
            {
                links[from].Add(to);
            }

            if (!links[to].Contains(from))
            {
                links[to].Add(from);
            }

            //Console.WriteLine($"Link from:{from.position}, to: {to.position}");


        }

        /// <summary>
        /// Returns whether the vertex is contained within the link collection, I.e. if there are any links between this and any other vertex. 
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>

        public bool Contains(Vertex vertex)
        {
            return links.Keys.Any(v => v == vertex);
        }

        /// <summary>
        /// Returns all vertices where there is a link to the specified vertex . 
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public List<Vertex> GetLinks(Vertex from)
        {
            var resultLinks = links.FirstOrDefault(l => l.Key == from);
            if (resultLinks.Equals(default(KeyValuePair<Vertex, List<Vertex>>)))
            {
                return null;
            }

            return resultLinks.Value;

        }

        /// <summary>
        /// Clears links dictionary
        /// </summary>
        public void Clear()
        {
            links.Clear();
        }



    }
}
