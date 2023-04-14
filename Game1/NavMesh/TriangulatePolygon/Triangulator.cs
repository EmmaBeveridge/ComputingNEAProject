using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.IO;
using System.Data;
using Game1.DataClasses;
using Game1.Town;
using Game1.Town.Buildings;
using System.Text.RegularExpressions;

namespace Game1.NavMesh.TriangulatePolygon
{
    class Triangulator
    {
         string housePolygonVerticesFileName = "Polygon data.xlsx";
        /// <summary>
        /// List of vertices for polygon to be triangulated, not including hole vertices. 
        /// </summary>
        public List<Vector2> shapeVertices = new List<Vector2>();

        
         public List<Vertex> resultVertices;
         public int[] indices;
        public List<LineSegment> polygonEdges = new List<LineSegment>();


         readonly CircularLinkedListIndexed<Vertex> polygonVertices = new CircularLinkedListIndexed<Vertex>();
         readonly CircularLinkedListIndexed<Vertex> earVertices = new CircularLinkedListIndexed<Vertex>();
         readonly CircularList<Vertex> convexVertices = new CircularList<Vertex>();
         readonly CircularList<Vertex> reflexVertices = new CircularList<Vertex>();

        const int yValue = 0;

        int holeCount;





        /// <summary>
        /// Triangulates the house layout using house coordinates from file. Calls CutHoleInShape() method before triangulation to allow polygon to be triangulated properly. 
        /// </summary>
        public void BuildHouseTriangles()
        {
            List<Vector2> holeVertices = new List<Vector2>();


            GetVerticesFromFile(housePolygonVerticesFileName, ref holeVertices);
            
            Vector2[] newVertices = CutHoleInShape(shapeVertices.ToArray(), holeVertices.ToArray());

            Triangulate(newVertices);
           
           
        }

        /// <summary>
        /// Generates town polygon vertices and hole polygon vertices (e.g. a house is a hole in the navmesh). As the town polygon has multiple holes within it, each hole must be cut in the shape in term beginning with the hole with the largest X coordinate and then working towards the hole with the least X coordinate. Once all holes have been cut in the polygon, the polygon can be triangulated. 
        /// </summary>
        public void BuildTownTriangles()
        {
            List<List<Vector2>> holeVertices = new List<List<Vector2>>();




            //for (int i = 2; i <= 2; i++)
            //{
            //    Building building = Building.buildings.Find(b => b.GetType() == typeof(Office));
            //    //Regex regex = new Regex($@"B{i}$");
            //    //Building building = Building.buildings.Find(b => regex.IsMatch(b.id));
            //    List<Vector2> buildingVertices = new List<Vector2>();
            //    holeCount++;
            //    Console.WriteLine(building.id);

            //    foreach (Vector3 corner in building.groundCorners)
            //    {

            //        buildingVertices.Add(new Vector2(corner.X , corner.Z));

            //    }


            //    holeVertices.Add(buildingVertices);

            //}





            //holeVertices.Add(new List<Vector2> { new Vector2(100, 0), new Vector2(0, 0), new Vector2(0, -100), new Vector2(100, -100) });


            //foreach (Building building in Building.buildings)
            //{
            //    List<Vector2> buildingVertices = new List<Vector2>();
            //    holeCount++;

            //    foreach (Vector3 corner in building.groundCorners)
            //    {

            //        buildingVertices.Add(new Vector2(corner.X, corner.Z));

            //    }

            //    buildingVertices.Reverse();

            //    holeVertices.Add(buildingVertices);


            //}

            foreach (House house in House.houses)
            {
                List<Vector2> houseVertices = new List<Vector2>();
                holeCount++;

                foreach (Vector3 corner in house.groundCorners)
                {

                    houseVertices.Add(new Vector2(corner.X, corner.Z));

                }

                holeVertices.Add(houseVertices);

            }



            foreach (Vector3 corner in Town.Town.corners)
            {
                shapeVertices.Add(new Vector2(corner.X, corner.Z));
            }



            //holeVertices.Remove(holeVertices.Last());
            //holeVertices.Add(new List<Vector2> { new Vector2(2070, -30), new Vector2(2070, 860), new Vector2(1610, 860), new Vector2(1610, -30) });
            //holeVertices.Add(new List<Vector2> { new Vector2(-1055, 1230), new Vector2(-1330, 1080), new Vector2(-1235, 900), new Vector2(-950, 1060) });
            //holeVertices.Add(new List<Vector2> { new Vector2(360, 295), new Vector2(0, 295), new Vector2(0, 0), new Vector2(360, 0) });




            #region Translate all vertices so all Z coordinates negative

            //float ZTranslation = holeVertices.OrderByDescending(h => h.Max(v => v.Y)).First().Max(v => v.Y)+100; //find largest z coordinate i.e. amount all vertices must be translated by
            float ZTranslation = shapeVertices.Max(v => v.Y)+100; //find largest z coordinate i.e. amount all vertices must be translated by

            List<List<Vector2>> translatedHoles = new List<List<Vector2>>();
            foreach (List<Vector2> hole in holeVertices)
            {

                List<Vector2> translatedHole = new List<Vector2>();

                foreach (Vector2 vertex in hole)
                {
                    translatedHole.Add(new Vector2(vertex.X, vertex.Y - ZTranslation));
                }


                translatedHoles.Add(translatedHole);

            }


            List<Vector2> translatedShapeVertices = new List<Vector2>();


            foreach (Vector2 vertex in shapeVertices)
            {
                translatedShapeVertices.Add(new Vector2(vertex.X, vertex.Y - ZTranslation));

            }




            holeVertices = translatedHoles;
            shapeVertices = translatedShapeVertices;



            #endregion
























            

            //for (int i = 0; i < holeCount; i++)
            while (holeVertices.Count>0)
            {

                List<Vector2> nextHole = new List<Vector2>();

                nextHole = holeVertices.OrderByDescending(h => h.Max(v => v.X)).First();

                //float maxX = holeVertices[0][0].X;

                //foreach (var hole in holeVertices)
                //{
                //    if (hole.Max( h => h.X) >= maxX) //for some reason this line has broken since adding buildings - making >= seems to have fixed it??
                //    {
                //        nextHole = hole;
                //        maxX = hole.Max(h => h.X);
                //    }
                //}

                holeVertices.Remove(nextHole);
                
                shapeVertices = CutHoleInShape(shapeVertices.ToArray(), nextHole.ToArray()).ToList<Vector2>();

                //Console.WriteLine("\n\n\nHOLE:" + i);
                //foreach( var vertex in shapeVertices)
                //{
                //    Console.WriteLine("v : "+vertex.ToString());
                //}

            }



            //FixVertices(ref shapeVertices);


            



            Triangulate(shapeVertices.ToArray(), ZTranslation);
            
        }







        private void FixVertices(ref List<Vector2> shapeVertices)
        {
            Vector2[] iterVertices = new Vector2[shapeVertices.Count];
            shapeVertices.CopyTo(iterVertices);
            int insertIndex = 0;
            House prevVertexHouse = null;
            House vertexHouse = null;
            Vector2 prevVertex = iterVertices[0];
            List<Vector2> openHouses = new List<Vector2>();
            foreach (Vector2 vertex in iterVertices)
            {
                vertexHouse =  House.getHouseContainingPoint(new Vector3(vertex.X, 0, vertex.Y));
                
                if (vertexHouse == null)
                {
                    if (openHouses.Count > 0)
                    {
                        shapeVertices.Insert(insertIndex, vertex);
                        insertIndex++;
                        openHouses.RemoveAt(0);
                    }

                }

                else
                { 

                    if (vertexHouse == prevVertexHouse)
                    {

                    }

                    else if (vertexHouse != prevVertexHouse)
                    {
                        openHouses.Add(prevVertex);
                        openHouses.Add(vertex);


                    }

                }

                prevVertex = vertex;
                insertIndex++;

            }




        }

        /// <summary>
        /// Uses ExcelFileManager class to read in polygon and hole vertices. If the vertex is marked as belonging to a hole they are added to the holeVertices list, otherwise they are added to the shapeVertices list. 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="holeVertices"></param>
        private void GetVerticesFromFile(string fileName, ref List<Vector2> holeVertices)
        {
            DataTable sheet = ExcelFileManager.ReadExcelFile(fileName);
            foreach (DataRow row in sheet.Rows)
            {
                if (row["HoleBool"].ToString() == "0")
                {
                    shapeVertices.Add(new Vector2(int.Parse(row["X"].ToString()), int.Parse(row["Z"].ToString())));

                }
                else
                {
                    holeVertices.Add(new Vector2(int.Parse(row["X"].ToString()), int.Parse(row["Z"].ToString())));
                }

            }
            //Console.WriteLine("Shape vertices");
            //foreach (var item in shapeVertices)
            //{
            //    Console.WriteLine(item.ToString());
            //}

            //Console.WriteLine("Hole vertices");

            //foreach (var item in holeVertices)
            //{
            //    Console.WriteLine(item.ToString());
            //}
        }

        /// <summary>
        /// Begins by identifying all convex and reflex vertices and calculating the ear vertices of the polygon. Method then enters loop calling ClipNextEar method until there are no more ear vertices or there are fewer than 3 polygon vertices remaining. If there are exactly 3 polygon vertices remaining, a final triangle is added made from these three vertices. 
        /// </summary>
        /// <param name="inputVertices"></param>
        public void Triangulate(Vector2[] inputVertices, float ZtranslationToApply = 0)//Vector2 used where Y = Z
        {



            List<Triangle> triangles = new List<Triangle>();
            //uses anticlockwise winding order

            Vector2[] outputVertices = (Vector2[])inputVertices.Clone();

            polygonVertices.Clear();
            earVertices.Clear();
            convexVertices.Clear();
            reflexVertices.Clear();


            for (int i = 0; i < outputVertices.Length; i++)
            {
                polygonVertices.AddLast(new Vertex(new Vector3(outputVertices[i].X, yValue, outputVertices[i].Y), i));
            }




            #region CreateLineSegments

            for (int i = 0; i < polygonVertices.Count; i++)
            {
                polygonEdges.Add(new LineSegment(polygonVertices[i].Value, polygonVertices[i + 1].Value));

            }

            #endregion







            #region Translate vertices back

            if (ZtranslationToApply != 0)
            {
                resultVertices = new List<Vertex>();

                foreach (Vertex polyVertex in polygonVertices)
                {
                    resultVertices.Add(new Vertex(new Vector3(polyVertex.position.X, polyVertex.position.Y, polyVertex.position.Z + ZtranslationToApply), polyVertex.index));
                }

            }
            else
            {
                resultVertices = polygonVertices.ToList();
            }


            #endregion



            FindConvexAndReflexVertices();
            FindEarVertices();



            foreach (Vertex item in polygonVertices)
            {
                Console.WriteLine(item.position.ToString());
            }



            while (polygonVertices.Count > 3 && earVertices.Count > 0)
            {
                ClipNextEar(triangles);
            }

            //bool recheckEars = true;
            //while (recheckEars)
            //{
            //    while (polygonVertices.Count > 3 && earVertices.Count > 0)
            //    {
            //        ClipNextEar(triangles);
            //    }

            //    if (polygonVertices.Count > 3)
            //    {
            //        FindEarVertices();

            //        if (earVertices.Count == 0)
            //        {
            //            recheckEars = false;
            //        }

            //    }
            //    else
            //    {
            //        recheckEars = false;
            //    }


            //}

            FindEarVertices();

            if (polygonVertices.Count == 3)
            {
                triangles.Add(new Triangle(polygonVertices[0].Value, polygonVertices[1].Value, polygonVertices[2].Value));
            }


            indices = new int[triangles.Count * 3];
            for (int i = 0; i < triangles.Count; i++)
            {

                indices[(i * 3)] = triangles[i].A.index;
                indices[(i * 3) + 1] = triangles[i].B.index;
                indices[(i * 3) + 2] = triangles[i].C.index;

            }


            
        }

        struct PolygonTree
        {
            public List<Vector2> polygon;
            public List<PolygonTree> children;
        }

        //public Vector2[] CutManyHolesInShape(PolygonTree polyTree, Vector2[] shapeVertices)
        //{
        //    List<Triangle> triangles;
        //    PolygonTree tree = polyTree;
        //    Queue<PolygonTree> treeQueue = new Queue<PolygonTree>();
        //    treeQueue.Enqueue(polyTree);

        //    while(treeQueue.Count != 0)
        //    {
        //        PolygonTree outerNode = treeQueue.Dequeue();
        //        int numChildren = outerNode.children.Count;

        //        if (numChildren == 0)
        //        {
        //            return shapeVertices;
        //        }
        //        else
        //        {
        //            for (int i = 0; i < numChildren; i++)
        //            {
        //                PolygonTree innerNode = outerNode.children[i];

        //                List<List<Vector2>> innerPolygons = new List<List<Vector2>>();
        //                int numGrandchildren = innerNode.children.Count;
        //                for (int j = 0; j < numGrandchildren; j++)
        //                {
        //                    innerPolygons.Append(innerNode.polygon);
        //                    treeQueue.Prepend(innerNode.children[j]);
        //                }



        //            }
        //            List<Vector2> combined = MakeSimple(outerNode.polygon, innerPolygons);
        //        }


        //    }

        //}

        //public List<Vector2> MakeSimple(List<Vector2> outer, List<List<Vector2>> inner)
        //{

        //}




        /// <summary>
        /// In order to triangulate the polygon, we must cut open the polygon by introducing 2 new coincident edges between two mutually visible vertices, one vertex from shape vertices and one from hole vertices. Once the mutually visible vertices have been identified, the vertices of the hole are injected into the list of shape vertices in order to create a structure of vertices on which triangulation can occur.
        /// </summary>
        /// <param name="shapeVertices"></param>
        /// <param name="holeVertices"></param>
        /// <returns></returns>
        public Vector2[] CutHoleInShape(Vector2[] shapeVertices, Vector2[] holeVertices)
        {
            //shape vertices wound counterclockwise, hole vertices wound clockwise

            polygonVertices.Clear();
            earVertices.Clear();
            convexVertices.Clear();
            reflexVertices.Clear();


            for (int i = 0; i < shapeVertices.Length; i++)
            {

                polygonVertices.AddLast(new Vertex(new Vector3(shapeVertices[i].X, yValue, shapeVertices[i].Y), i));

            }


            CircularList<Vertex> holePolygon = new CircularList<Vertex>();

            for (int i = 0; i < holeVertices.Length; i++)
            {
                holePolygon.Add(new Vertex(new Vector3(holeVertices[i].X, yValue, holeVertices[i].Y), i + polygonVertices.Count));

            }

            FindConvexAndReflexVertices();
            FindEarVertices();



            Vertex holeVertexMaxX = holePolygon.OrderByDescending(v => v.position.X).First();


            //Vertex holeVertexMaxX = holePolygon[0];
            //foreach (Vertex vertex in holePolygon)
            //{
            //    if (vertex.position.X > holeVertexMaxX.position.X)
            //    {
            //        holeVertexMaxX = vertex;
            //    }
            //}


            List<LineSegment> segmentsToTest = new List<LineSegment>();

            //builds list of segments where at least one vertex had an x value greater than biggest x value of hole vertices and where one vertex has greater z value and one a smaller z value than hole vertex

            for (int i = 0; i < polygonVertices.Count; i++)
            {

                Vertex a = polygonVertices[i].Value;
                Vertex b = polygonVertices[i + 1].Value;

                if ((a.position.X > holeVertexMaxX.position.X || b.position.X > holeVertexMaxX.position.X) && ((a.position.Z >= holeVertexMaxX.position.Z && b.position.Z <= holeVertexMaxX.position.Z) || (a.position.Z <= holeVertexMaxX.position.Z && b.position.Z >= holeVertexMaxX.position.Z))) 
                {
                    segmentsToTest.Add(new LineSegment(a, b));

                }

            }



            float? shortestDistance = null;
            LineSegment closestSegment = new LineSegment();

            foreach (LineSegment segment in segmentsToTest)
            {
                Vector3 intersection;
                bool intersects = segment.Intersects(holeVertexMaxX.position, holeVertexMaxX.position+Vector3.UnitX, out intersection, true); 
                if (intersects)
                {
                    float distance = (intersection - holeVertexMaxX.position).Length();

                    if (shortestDistance == null || distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        closestSegment = segment;
                    }
                }
            }

            if (shortestDistance == null)
            {
                return shapeVertices;
            }

            Vector3 I = holeVertexMaxX.position + Vector3.UnitX * shortestDistance.Value;
            Vertex P = (closestSegment.v1.position.X> closestSegment.v2.position.X) ? closestSegment.v1: closestSegment.v2;



            //https://www.geometrictools.com/Documentation/TriangulationByEarClipping.pdf pg8

            Triangle mip = new Triangle(holeVertexMaxX, new Vertex(I, 1), P);

            List<Vertex> reflexVerticesInmip = new List<Vertex>();

            foreach (Vertex vertex  in reflexVertices)
            {
                if (vertex.Equals(P)) { continue; } //don't add reflex P

                if (mip.Encloses(vertex.position))
                {

                    reflexVerticesInmip.Add(vertex);


                }


            }

            //if (reflexVerticesInmip.Count > 0)
            //{
            //    float smallestDotProduct = -1f; //smallest dot product => smallest angle
            //    foreach (Vertex vertex in reflexVerticesInmip)
            //    {
            //        Vector3 d = Vector3.Normalize(vertex.position - holeVertexMaxX.position);
            //        float dotProduct = Vector3.Dot(Vector3.UnitX, d);

            //        if (dotProduct < smallestDotProduct)
            //        {
            //            smallestDotProduct = dotProduct;
            //            P = vertex;
            //        }



            //    }



            //}

            if (reflexVerticesInmip.Count > 0)
            {
                float smallestDistance = float.MaxValue; //smallest dot product => smallest angle
                foreach (Vertex vertex in reflexVerticesInmip)
                {
                    float dist = (vertex.position - holeVertexMaxX.position).Length();
                    
                    if (dist < smallestDistance)
                    {
                        smallestDistance = dist;
                        P = vertex;
                    }



                }



            }






            int startHoleIndex = holePolygon.IndexOf(holeVertexMaxX);
            //int injectPoint = polygonVertices.IndexOf(P);

            int injectPoint = FindLastIndex(polygonVertices.ToList(), P);

            for (int i = startHoleIndex; i < startHoleIndex+holePolygon.Count; i++)
            {
                polygonVertices.AddAfter(polygonVertices[injectPoint++], holePolygon[i]);
            }
            polygonVertices.AddAfter(polygonVertices[injectPoint++], holePolygon[startHoleIndex]);
            polygonVertices.AddAfter(polygonVertices[injectPoint], new Vertex(P.position));

            Vector2[] newShapeVertices = new Vector2[polygonVertices.Count];

            for (int i = 0; i < polygonVertices.Count; i++)
            {

                newShapeVertices[i] = new Vector2(polygonVertices[i].Value.position.X, polygonVertices[i].Value.position.Z);


            }


            return newShapeVertices;


        }


        /// <summary>
        /// In the case of polygons with multiple holes, some vertices may appear in the list of polygon vertices more than once. In order to correctly inject the hole vertices, if one of the vertices appearing more than once in the list of polygon vertices is chosen as the mutually visible vertex, the hole vertices must be injected after the last instance of this vertex in the list. The method returns the index of the last instance of the specified vertex in the list. 
        /// </summary>
        /// <param name="polyVertices"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        private int FindLastIndex(List<Vertex> polyVertices, Vertex v)
        {
            int foundIndex = -1;

            for (int i = 0; i < polyVertices.Count; i++)
            {

                if (polyVertices[i].position == v.position)
                {
                    foundIndex = i;
                }

            }

            return foundIndex;

        }




        //private int FindEarWithSmallestAngle(Vertex ear)
        //{
        //    double smallestAngle = double.MaxValue;
        //    int indexOfSmallestEarinPolygonList = -1;
           
        //    int instanceCount = 0;

        //    for (int i = 0; i < polygonVertices.Count; i++)
        //    {
        //        if (polygonVertices[i].Value.position != ear.position)
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            instanceCount++;


                   
        //            Vertex previous = polygonVertices[i - 1].Value;
        //            Vertex next = polygonVertices[i + 1].Value;


        //            Vector2 earPos = new Vector2(ear.position.X, ear.position.Z);
        //            Vector2 prevPos = new Vector2(previous.position.X, previous.position.Z);
        //            Vector2 nextPos = new Vector2(next.position.X, next.position.Z);


        //            Vector2 d1 = Vector2.Normalize(prevPos - earPos); //points from earPos towards prevPos
        //            Vector2 d2 = Vector2.Normalize(nextPos - earPos); //points from earPos towards nextPos

        //            double angle = Math.Acos(Vector2.Dot(d1, d2) / (d1.Length() * d2.Length()));

        //            if (angle > Math.PI / 2) { angle = Math.PI - angle; }


        //            if (angle < smallestAngle)
        //            {
        //                indexOfSmallestEarinPolygonList = i;
                        
        //                smallestAngle = angle;
        //            }



        //        }

        //    }




        //    return indexOfSmallestEarinPolygonList;

        //}




        /// <summary>
        /// Removes ear vertex from polygon vertices and adds ear triangle to the list of triangles. Proceeds to make a call to ValidateAdjacentVertex() for each of the adjacent vertices. This is because, after removing the ear vertex, a convex adjacent vertex may now also be an ear and a reflex vertex may now be convex and possibly an ear as well.  
        /// </summary>
        /// <param name="triangles"></param>
        private void ClipNextEar(ICollection<Triangle> triangles)
        {
            Vertex ear = earVertices[0].Value;

            //List<int> InstanceIndexes = new List<int>();

            //for (int i = 0; i < earVertices.Count; i++)
            //{
            //    if (earVertices[i].Value.position == ear.position)
            //    {
            //        InstanceIndexes.Add(i);
            //    }
            //}

            //if (InstanceIndexes.Count > 1)
            //{
            //    int indexOfNextEar = FindEarWithSmallestAngle(InstanceIndexes, ear);
            //    ear = earVertices[indexOfNextEar].Value;
            //}




            Vertex previous = polygonVertices[polygonVertices.IndexOf(ear) - 1].Value;
            Vertex next = polygonVertices[polygonVertices.IndexOf(ear) + 1].Value;

            triangles.Add(new Triangle(ear, next, previous));

            Console.WriteLine("Removed:"+ear.position.ToString());

            earVertices.RemoveAt(0); 
            polygonVertices.RemoveAt(polygonVertices.IndexOf(ear));

            if (earVertices.Count == 0)
            {
                Console.WriteLine();
            }


            ValidateAdjacentVertex(previous);
            ValidateAdjacentVertex(next);


        }

        /// <summary>
        /// Called after an ear has been clipped for each adjacent vertex in order to update list of convex, reflex and ear vertices following removal of ear vertex. Any adjacent convex vertex must be tested as to whether it is now an ear, and any adjacent reflex vertex must be tested as to whether it is now convex and, if so, whether it is now also an ear. 
        /// </summary>
        /// <param name="vertex"></param>
        private void ValidateAdjacentVertex(Vertex vertex)
        {
            if (reflexVertices.Contains(vertex))
            {
                if (IsConvex(vertex))
                {
                    reflexVertices.Remove(vertex);
                    convexVertices.Add(vertex);
                }
            }

            if (convexVertices.Contains(vertex))
            {
                bool wasEar = earVertices.Contains(vertex);
                bool isEar = IsEar(vertex);

                if (wasEar && !isEar) 
                {
                    earVertices.Remove(vertex);

                }
                else if (!wasEar && isEar) 
                {
                    earVertices.AddFirst(vertex);
                }


            }


        }



        /// <summary>
        /// Iterates over each vertex in the list of polygonVertices using IsConvex() method to test if vertex is convex or reflex. 
        /// </summary>
        public void FindConvexAndReflexVertices()
        {
            foreach (Vertex vertex in polygonVertices)
            {
                if (IsConvex(vertex))
                {
                    convexVertices.Add(vertex);
                   //Console.WriteLine($"{vertex.position.ToString()}: convex");
                    

                }
                else
                {
                    reflexVertices.Add(vertex);
                    //Console.WriteLine($"{vertex.position.ToString()}: reflex");

                }
            }
        }


        /// <summary>
        ///  Determines if a vertex is convex (less than 180 degrees) by first finding the vertex’s adjacent vertices and then calculating the direction vectors between these vertices and the vertex (one vector from the previous vertex to the tested vertex and one from the tested vertex to the next vertex). The vector from the test vertex to the next vertex is then rotated through 270 degrees and therefore the dot product between the vector from the previous vertex to the test vertex and the rotated vertex will be greater than zero if the angle at the test vertex is convex. 
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private bool IsConvex(Vertex vertex)
        {

            List<int> InstanceIndexes = new List<int>();

            for (int i = 0; i < polygonVertices.Count; i++)
            {
                if (polygonVertices[i].Value.position == vertex.position)
                {
                    InstanceIndexes.Add(i);
                }
            }


            Vertex previous = polygonVertices[polygonVertices.IndexOf(vertex) - 1].Value;
            Vertex next = polygonVertices[polygonVertices.IndexOf(vertex) + 1].Value;

            Vector2 vertexPos = new Vector2(vertex.position.X, vertex.position.Z);
            Vector2 prevPos = new Vector2(previous.position.X, previous.position.Z);
            Vector2 nextPos = new Vector2(next.position.X, next.position.Z);

            //float det = (vertexPos.X - prevPos.X) * (nextPos.Y - vertexPos.Y) - (nextPos.X - vertexPos.X) * (vertexPos.Y - prevPos.Y);
            //return (det < 0);



            
            Vector2 d1 = Vector2.Normalize(vertexPos - prevPos); //points from prevPos towards vertexPos
            Vector2 d2 = Vector2.Normalize(nextPos - vertexPos); //points from vertexPos towards nextPos
            Vector2 n2 = new Vector2(-d2.Y, d2.X); //rotates d2 through 270, makes it so that if angle is either obtuse or acute (ie. convex) dot product between vectors >0 as either vector can be projected onto the other so that length of projection in direction of vector >0

            return (Vector2.Dot(d1, n2) > 0f);
            


        }

        private  bool IsReflex (Vertex vertex)
        {
            return !IsConvex(vertex);
        }

        /// <summary>
        /// Iterates through each vertex calling IsEar() method to determine if the vertex is an ear vertex. If the vertex is an ear vertex, it is added to the list of earVertices.
        /// </summary>
        public void FindEarVertices() 
        {
            foreach (Vertex vertex in convexVertices)
            {


                if (IsEar(vertex))
                {


                    Console.WriteLine("\t" + vertex.position.ToString());
                    earVertices.AddLast(vertex);
                }
                else
                {
                    Console.WriteLine(vertex.position.ToString());
                }
            }
        }



        /// <summary>
        /// Returns if the vertex is an ear vertex. An ear vertex forms part of the ear of the polygon. An ear of the polygon is a triangle formed by three consecutive vertices <P, V, N> where V is a convex vertex and the line segment from PN lies fully within the polygon (I.e. is a diagonal of the polygon). This method therefore identifies all of the ear vertices in the polygon by iterating over each convex vertex V and checking that for the triangle PVN the segment PN is contained within the polygon. We can determine if PN is within the polygon by checking if any of the polygon’s reflex vertices lies within PVN using the static Encloses() method of the Triangle class on PVN and the position of the reflex vertex. If any of the reflex vertices lie within PVN, then PVN is not an ear and therefore V is not an ear vertex. 
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private bool IsEar(Vertex vertex)
        {
            
            Vertex previous = polygonVertices[polygonVertices.IndexOf(vertex) - 1].Value;
            Vertex next = polygonVertices[polygonVertices.IndexOf(vertex) + 1].Value;

            //foreach (Vertex reflexVertex in reflexVertices)
            //{
            //    if (reflexVertex.Equals(previous) || reflexVertex.Equals(vertex) || reflexVertex.Equals(next))
            //    {
            //        continue;
            //    }

            //    if (Triangle.Encloses(previous, vertex, next, reflexVertex))
            //    {
            //        return false;
            //    }


            //}

            foreach (Vertex reflexVertex in reflexVertices)
            {
                if (reflexVertex.Equals(previous) || reflexVertex.Equals(vertex) || reflexVertex.Equals(next))
                {
                    continue;
                }

                if (Triangle.Encloses(previous, vertex, next, reflexVertex))
                {
                    return false;
                }


            }


            Triangle EarTriangle = new Triangle(next, vertex, previous);

            foreach (LineSegment segment in polygonEdges)
            {
                if (!EarTriangle.HasVertex(segment.v1) || !EarTriangle.HasVertex(segment.v2))
                {
                    if (segment.Intersects(EarTriangle.edges)) { return false; }


                }
                    
                    
            }










            return true;



        }





        




    }
}
