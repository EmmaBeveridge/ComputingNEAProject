using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Game1.DataClasses;
using System.Data;
using Game1.NavMesh.TriangulatePolygon;

namespace Game1.NavMesh
{
    public class Mesh
    {
        public float minX;
        public float minZ;
        public float maxX;
        public float maxZ;

        public List<Vertex> vertices;
        int[] indices;

        Triangulator triangulator = new Triangulator();

        public Triangle[] triangles;
        Vector3[] vertexPositions { get { if (vertexPositions == null) { CacheVertices(); } return vertexPositions; } set { vertexPositions = value; } }

        public Mesh (List<Triangle> triangles)
        {

            

            Vertex[] points = new Vertex[triangles.Count * 3];
            int[] indices = new int[triangles.Count * 3];
            for (int i = 0; i < triangles.Count; i++)
            {
                points[i * 3] = new Vertex(triangles[i].A.position, triangles[i].A.index);
                points[i * 3 + 1] = new Vertex(triangles[i].B.position, triangles[i].B.index);
                points[i * 3 + 2] = new Vertex(triangles[i].C.position, triangles[i].C.index);

                indices[i * 3] = i * 3;
                indices[i * 3 + 1] = i * 3 + 1;
                indices[i * 3 + 2] = i * 3 + 2;

            }

            //SetTriangles(points, indices);

        }

        //public Mesh()
        //{
        //    triangulator.BuildTriangles();

        //    vertices = triangulator.resultVertices;
        //    indices = triangulator.indices;

        //    SetTriangles(vertices, indices);


        //}



        public Mesh(bool town=false, bool house = false)
        {
            if (house == true)
            {
                triangulator.BuildHouseTriangles();
            }
            else if (town == true)
            {
                triangulator.BuildTownTriangles();
            }

            vertices = triangulator.resultVertices;
            indices = triangulator.indices;

            SetTriangles(vertices, indices);

        }


        void SetTriangles(List<Vertex> points, int[] indices)
        {
            if (indices.Length % 3 != 0) 
            {
                throw new ArgumentException("Triangle indices not multiple of 3");


            }

            triangles = new Triangle[indices.Length / 3];
            var triID = 0;
            maxX = points[0].position.X;
            minX = maxX;
            maxZ = points[0].position.Z;
            minZ = maxZ;

            for (int i = 0; i <= indices.Length-3; i+=3)
            {
                ExtendBounds(points[indices[i]].position);
                ExtendBounds(points[indices[i + 1]].position);
                ExtendBounds(points[indices[i + 2]].position);

                triangles[triID] = new Triangle(points[indices[i]], points[indices[i + 1]], points[indices[i + 2]], triID++);

            }
            for (int i = 0; i < triangles.Length; i++)
            {
                Console.WriteLine($"\"tri{i}\":{{\"A\":[{triangles[i].A.position.X} , {triangles[i].A.position.Z}] , \"B\":[{triangles[i].B.position.X} , {triangles[i].B.position.Z}] ,\"C\":[{triangles[i].C.position.X} , {triangles[i].C.position.Z}] }},");
            }

            //Console.WriteLine($"max X:{maxX}\n max Z:{maxZ}\n min X {minX}\n min z {minZ}");


        }


        void ExtendBounds(Vector3 point)
        {
            maxX = Math.Max(maxX, point.X);
            maxZ = Math.Max(maxZ, point.Z);
            minZ = Math.Min(minZ, point.Z);
            minX = Math.Min(minX, point.X);

        }




       











        private void CacheVertices()
        {
            vertexPositions = new Vector3[triangles.Length * 3];

            for (int i = 0; i < triangles.Length; i++)
            {
                vertexPositions[i * 3] = triangles[i].A.position;
                vertexPositions[i * 3 + 1] = triangles[i].B.position;
                vertexPositions[i * 3 + 2] = triangles[i].C.position;


            }


        }


        public Triangle GetTriangleAt (Vector3 point)
        {
            foreach (Triangle triangle in triangles)
            {
                if (triangle.Encloses(point))
                {
                    return triangle;
                }
            }

            return null;

        }


        public bool Contains (Vector3 point)
        {
            if (point.X < minX || point.Z < minZ || point.X > maxX || point.Z > maxZ)
            {
                return false;

            }

            return GetTriangleAt(point) != null;


        }


        public Triangle GetClosestTriangle(Vector3 point)
        {

            float shortestDistance = -1f;
            Triangle closestTriangle = null;

            for (int i = 0; i < triangles.Length; i++)
            {
                
                if (triangles[i].Encloses(point))
                {
                   
                    return triangles[i];
                }

                Vector3 closestPoint = triangles[i].GetClosestPoint(point);

                float distance = (closestPoint - point).LengthSquared();

                if (shortestDistance == -1 || distance< shortestDistance)
                {
                    shortestDistance = distance;
                    closestTriangle = triangles[i];
                }


            }

            return closestTriangle;





        }






    }
}
