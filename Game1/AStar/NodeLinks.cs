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
        public Dictionary<Vertex, List<Vertex>> links = new Dictionary<Vertex, List<Vertex>>();

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

        public bool Contains(Vertex vertex)
        {
            return links.Keys.Any(v => v == vertex);
        }

        public List<Vertex> GetLinks(Vertex from)
        {
            var resultLinks = links.FirstOrDefault(l => l.Key == from);
            if (resultLinks.Equals(default(KeyValuePair<Vertex, List<Vertex>>)))
            {
                return null;
            }

            return resultLinks.Value;

        }

        public void Clear()
        {
            links.Clear();
        }



    }
}
