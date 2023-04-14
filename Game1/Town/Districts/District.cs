using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Game1.StreetTraversal;

namespace Game1.Town.Districts
{
    public class District
    {

        [JsonProperty("id")]
        public string id;

        [JsonProperty("townLocation")]
        public Neo4j.Driver.Point townLoc { set { TownLocation.X = (float)value.X; TownLocation.Z = (float)value.Y; } }


        public Vector3 TownLocation;

        public string districtClass;

        /// <summary>
        /// String containing filename of model of district. 
        /// </summary>
        public string mapName;

        /// <summary>
        /// List of street objects of streets within the district. 
        /// </summary>
        public List<Street> streets;

        /// <summary>
        ///  Binary tree object representing streets in district. Used to preform range minimum queries for street navigation. 
        /// </summary>
        public BinaryTree<Street> StreetTree;

        /// <summary>
        /// Model of district. Model consists of road network for street.
        /// </summary>
        public Model mapModel;

        public Avatar avatar;

        public Vector3 origin;

        /// <summary>
        /// sets avatar using model and town location to allow model to be rendered.
        /// </summary>
        public void GenerateAvatar()
        {

            
            avatar = new Avatar(mapModel, TownLocation);


           



        }

        public District() { }






        /// <summary>
        /// Creates a new binary tree to represent the district’s street network. Determines and sets the origin of the district as the start of the street with no parent. Sets TreeNode object representing the start street as the origin of the tree. Calls AddChildrenToTree method and PrepareTree() method on binary street tree object. 
        /// </summary>
        public void BuildStreets()
        {

            StreetTree = new BinaryTree<Street>(streets.Count);

            Street startStreet = streets.Find(x => x.parent == null);
            origin = startStreet.Start;
            StreetTree.root = new TreeNode<Street>(startStreet);

            AddChildrenToTree(StreetTree.root);
            StreetTree.PrepareTree();



        }


        /// <summary>
        /// Recursive subroutine to create and assign child nodes in binary street tree. Creates and adds left/right child nodes (if required) and then calls method using left /right child as parent node to populate tree.  
        /// </summary>
        /// <param name="parentNode">Parent node to add children to tree for</param>
        private void AddChildrenToTree(TreeNode<Street> parentNode)
        {
            parentNode.leftChild = parentNode.item.children.Count > 0 ? new TreeNode<Street>(parentNode.item.children[0]) : null;
            parentNode.rightChild = parentNode.item.children.Count > 1 ? new TreeNode<Street>(parentNode.item.children[1]) : null;

            if (parentNode.leftChild != null)
            {
                AddChildrenToTree(parentNode.leftChild);
            }

            if (parentNode.rightChild != null)
            {
                AddChildrenToTree(parentNode.rightChild);
            }






        }


        /// <summary>
        /// Overloaded method, supplied with either start and end buildings. Method establishes start/end street and finds sequence of streets to be traversed in order to move from start to end. Method uses StreetTree.FindLowestCommonAncestor method to obtain lowest common ancestor (LCA) of streets in tree. List of points on street constructed by backtracking from start street through street parents until the LCA is reached, each time adding the start of the street to list of coordinates. We then backtrack from the end street through street parents up to and including LCA, adding end of street to list of coordinates each time – reversing this list when finished to go from LCA to goal. Combining these lists gives final output of street points character should follow to reach goal. 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        protected List<Vector3> FindStreetSequence(Building start, Building end)
        {
            Street startStreet = start.street;
            Street endStreet = end.street;
            return FindStreetSequence(startStreet, endStreet);

        }

        /// <summary>
        /// Overloaded method, supplied with either start and end houses. Method establishes start/end street and finds sequence of streets to be traversed in order to move from start to end. Method uses StreetTree.FindLowestCommonAncestor method to obtain lowest common ancestor (LCA) of streets in tree. List of points on street constructed by backtracking from start street through street parents until the LCA is reached, each time adding the start of the street to list of coordinates. We then backtrack from the end street through street parents up to and including LCA, adding end of street to list of coordinates each time – reversing this list when finished to go from LCA to goal. Combining these lists gives final output of street points character should follow to reach goal. 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        protected List<Vector3> FindStreetSequence(House start, House end)
        {
            Street startStreet = start.street;
            Street endStreet = end.street;
            return FindStreetSequence(startStreet, endStreet);
        }


        /// <summary>
        /// Returns list of coordinates navigating from current street to district origin (start of starting street in district). List obtained by backtracking from start street through street parents until parent is null, each time adding the start of the street to the list of coordinates.  
        /// </summary>
        /// <param name="startStreet"></param>
        /// <returns></returns>
        public List<Vector3> StreetPathToDistrictOrigin(Street startStreet)
        {
            List<Vector3> streetPoints = new List<Vector3>();
            Street parent = startStreet;

            while (parent != null)
            {
                streetPoints.Add(parent.Start);
                parent = parent.parent;
            }


            streetPoints.Add(origin);
            return streetPoints;


        }


        /// <summary>
        /// Overloaded method, supplied with start house. Returns list of coordinates navigating from current street to district origin (start of main street in district). List obtained by backtracking from start street through street parents until parent is null, each time adding the start of the street to the list of coordinates.  
        /// </summary>
        /// <param name="startHouse"></param>
        /// <returns></returns>
        public List<Vector3> StreetPathToDistrictOrigin(House startHouse)
        {
            List<Vector3> path = new List<Vector3>();

            path.Add(startHouse.TownLocation);

            path.Add(startHouse.street.FindClosestPointOnStreet(startHouse.TownLocation));

            path.AddRange(StreetPathToDistrictOrigin(startHouse.street));
            return path;
        }


        /// <summary>
        /// Overloaded method, supplied with start building. Returns list of coordinates navigating from current street to district origin (start of main street in district). List obtained by backtracking from start street through street parents until parent is null, each time adding the start of the street to the list of coordinates.  
        /// </summary>
        /// <param name="startBuilding"></param>
        /// <returns></returns>
        public List<Vector3> StreetPathToDistrictOrigin(Building startBuilding)
        {
            List<Vector3> path = new List<Vector3>();

            path.Add(startBuilding.TownLocation);

            path.Add(startBuilding.street.FindClosestPointOnStreet(startBuilding.TownLocation));

            path.AddRange(StreetPathToDistrictOrigin(startBuilding.street));
            return path;
        }

        /// <summary>
        /// Overloaded method, supplied with either start and end streets. Method establishes start/end street and finds sequence of streets to be traversed in order to move from start to end. Method uses StreetTree.FindLowestCommonAncestor method to obtain lowest common ancestor (LCA) of streets in tree. List of points on street constructed by backtracking from start street through street parents until the LCA is reached, each time adding the start of the street to list of coordinates. We then backtrack from the end street through street parents up to and including LCA, adding end of street to list of coordinates each time – reversing this list when finished to go from LCA to goal. Combining these lists gives final output of street points character should follow to reach goal. 
        /// </summary>
        /// <param name="startStreet"></param>
        /// <param name="endStreet"></param>
        /// <returns></returns>
        protected List<Vector3> FindStreetSequence(Street startStreet, Street endStreet)
        {
            
            List<Vector3> streetPoints = new List<Vector3>();
            
            
            
            if (startStreet == endStreet)
            {
                
                return streetPoints;

            }



            Street LCA = StreetTree.FindLowestCommonAncestor(startStreet, endStreet);

            Street parent = startStreet; 

            while (parent != LCA && parent != null) //dont add start of lca street
            {
                streetPoints.Add(parent.Start); 
                parent = parent.parent;
            }
            
            
            
            List<Vector3> goalToLCA = new List<Vector3>();
            //goalToLCA.Add(endStreet);

            parent = endStreet.parent; //dont want to add end point of end street to list

            if (endStreet.parent != null)
            {


                while (parent != LCA && parent != null)
                {
                    goalToLCA.Add(parent.End);
                    parent = parent.parent; 
                    

                } 

            }

            goalToLCA.Add(LCA.End); //want to go to end of lca street

            

            goalToLCA.Reverse();
            streetPoints.AddRange(goalToLCA);

            return streetPoints;


        }



        /// <summary>
        /// Overloaded method, supplied start and end houses, to find path via streets. First adds closest point on start street to house/building origin using FindClosestPointOnStreet method on street object. Sequence of street points from start to end determined using FindStreetSequence method. Finally, closest point on end street added to list of path points which is then returned. 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<Vector3> FindStreetPathPoints(House start, House end)
        {

           

            List<Vector3> pathPoints = new List<Vector3>();
            pathPoints.Add(start.street.FindClosestPointOnStreet(start.TownLocation));
            pathPoints.AddRange(FindStreetSequence(start, end));
            pathPoints.Add(end.street.FindClosestPointOnStreet(end.TownLocation));

            return pathPoints;





        }


        /// <summary>
        /// Overloaded method, supplied start and end buildings, to find path via streets. First adds closest point on start street to house/building origin using FindClosestPointOnStreet method on street object. Sequence of street points from start to end determined using FindStreetSequence method. Finally, closest point on end street added to list of path points which is then returned. 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<Vector3> FindStreetPathPoints(Building start, Building end)
        {
            List<Vector3> pathPoints = new List<Vector3>();
            pathPoints.Add(start.street.FindClosestPointOnStreet(start.TownLocation));
            pathPoints.AddRange(FindStreetSequence(start, end));
            pathPoints.Add(end.street.FindClosestPointOnStreet(end.TownLocation));

            return pathPoints;


        }















    }
}
