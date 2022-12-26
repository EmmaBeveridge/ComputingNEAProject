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

        public string mapName;

        public List<Street> streets;

        public BinaryTree<Street> StreetTree;


        public Model mapModel;

        public Avatar avatar;


        public void GenerateAvatar()
        {

            
            avatar = new Avatar(mapModel, TownLocation);


           



        }

        public District() { }







        public void BuildStreets()
        {

            StreetTree = new BinaryTree<Street>(streets.Count);
            StreetTree.root = new TreeNode<Street>(streets.Find(x => x.parent == null));
            AddChildrenToTree(StreetTree.root);

            StreetTree.PrepareTree();



        }



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



        protected List<Vector3> FindStreetSequence(House start, House end)
        {
            Street startStreet = start.street;
            Street endStreet = end.street;

            List<Vector3> streetPoints = new List<Vector3>();
            
            
            
            if (startStreet == endStreet)
            {
                
                return streetPoints;

            }



            Street LCA = StreetTree.FindLowestCommonAncestor(startStreet, endStreet);

            Street parent = startStreet; 

            while (parent != LCA)
            {
                streetPoints.Add(parent.Start); 
                parent = parent.parent;
            }
            
            
            List<Vector3> goalToLCA = new List<Vector3>();
            //goalToLCA.Add(endStreet);

            parent = endStreet;

            do
            {
                parent = parent.parent; //will add LCA to street points too
                goalToLCA.Add(parent.End);
                
            } while (parent != LCA);


            //goalToLCA.RemoveAt(0); //Dont want to go to end of last street so remove it

            goalToLCA.Reverse();
            streetPoints.AddRange(goalToLCA);

            return streetPoints;


        }


        public List<Vector3> FindStreetPathPoints(House start, House end)
        {

           

            List<Vector3> pathPoints = new List<Vector3>();
            pathPoints.Add(start.street.FindClosestPointOnStreet(start.TownLocation));
            pathPoints.AddRange(FindStreetSequence(start, end));
            pathPoints.Add(end.street.FindClosestPointOnStreet(end.TownLocation));

            return pathPoints;





        }










    }
}
