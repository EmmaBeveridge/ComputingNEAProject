using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Characters
{
    class PathPoint
    {
        /// <summary>
        /// Static class attribute. List of PathPoint objects that are currently to be displayed.
        /// </summary>
        static List<PathPoint> PathPoints = new List<PathPoint>();


        /// <summary>
        /// Static class attribute. 3D model object rendered to represent a point on the path.
        /// </summary>
        static Model PathPointModel;

        /// <summary>
        /// Avatar for path point, used to render 3D model for path point at position of the path point.
        /// </summary>
        Avatar PathPointAvatar;

        /// <summary>
        /// Town location of the path point
        /// </summary>
        Vector3 PathPointPosition;

        /// <summary>
        /// Sets 3D model for path points to Model object supplied as a parameter
        /// </summary>
        /// <param name="model">path point model</param>
        public static void SetPathPointModel(Model model) { PathPointModel = model; }


        /// <summary>
        /// COnstructor for new PathPoint object. Creates PathPointAvatar using static PathPointModel attribute and position parameter
        /// </summary>
        /// <param name="position">Position of path point</param>
        public PathPoint(Vector3 position)
        {
            PathPointPosition = position;
            PathPointAvatar = new Avatar(PathPointModel, position);
            
        }


        /// <summary>
        /// Static method. Calls DrawPathPoint method on each PathPoint object to draw A* path
        /// </summary>
        /// <param name="view">View matrix</param>
        /// <param name="projection">Projection matrix</param>
        public static void DrawPath(Matrix view, Matrix projection)
        {
            foreach (PathPoint pathPoint in PathPoints)
            {

                pathPoint.DrawPathPoint(view, projection);

            }
        }


        /// <summary>
        /// Draws path point avatar using Draw method on PathPointAvatar attribute
        /// </summary>
        /// <param name="view"></param>
        /// <param name="projection"></param>
        private void DrawPathPoint(Matrix view, Matrix projection)
        {
            PathPointAvatar.Draw(view, projection);
        }

        
        /// <summary>
        /// Clears static class list of path points
        /// </summary>
        public static void ClearPathPoints() { PathPoints.Clear(); }

        /// <summary>
        /// CLears PathPoints list. Creates PathPoint objects at specified positions and adds to static class list PathPoints
        /// </summary>
        /// <param name="pathPointPositions">List of positions of path points</param>
        public static void BuildRangePathPoints(List<Vector3> pathPointPositions)
        {
            ClearPathPoints();

            foreach (Vector3 pos in pathPointPositions)
            {
                PathPoints.Add(new PathPoint(pos));
            }

        }

        /// <summary>
        /// Removes PathPoint object with the vector position specified by the parameter from static PathPoints list 
        /// </summary>
        /// <param name="position"></param>
        public static void RemovePathPointAtPosition(Vector3 position)
        {
            PathPoints.Remove(PathPoints.Find(pp => pp.PathPointPosition == position));

        }
        



        


    }


   


}
