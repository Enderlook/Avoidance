using AvalonStudios.Additions.Extensions;

using System.Collections.Generic;
using UnityEngine;

namespace AvalonStudios.Additions.Utils.HandlePoint
{
    [System.Serializable]
    public class HandlePoint
    {
        [SerializeField]
        private List<Vector3> points = new List<Vector3>();

        public Vector3 this[int i] => points[i];

        /// <summary>
        /// Get the counts of points of the list.
        /// </summary>
        public int CountOfPoints => points.Count;

        /// <summary>
        /// Return an array of points positions.
        /// </summary>
        public Vector3[] GetPositionsPoints
        {
            get
            {
                if (points != null)
                    return points.ToArray();

                return null;
            }
        }

        // Methods -----

        /// <summary>
        /// Initialize the points
        /// </summary>
        /// <param name="center"><seealso cref="Vector3"/> to draw the points</param>
        /// <param name="amountOfPoints">Amount of points to draw in scene.</param>
        /// <param name="reset">True then create a new point. False add a new point</param>
        public void Initialize(Vector3 center, int amountOfPoints = 1, bool reset = false)
        {
            if (!reset)
            {
                for (int x = 0; x < amountOfPoints; x++)
                    points.Add(center);
            }
            else
                points = new List<Vector3> { center };
        }

        /// <summary>
        /// Get actual position of a point of the list.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Vector3 GetPositionPoint(int i) => new Vector3(points[i].x, points[i].y, points[i].z);

        /// <summary>
        /// Add a Point into the list.
        /// </summary>
        /// <param name="p"></param>
        public void AddPoint(Vector3 p) => points.Add(p);

        /// <summary>
        /// Remove a point of the list. ...Still in progress...
        /// </summary>
        /// <param name="p"></param>
        public void RemovePoint(int p)
        {
            points.RemoveAt(p);
        }

        /// <summary>
        /// Remove all the points from the list.
        /// </summary>
        public void RemoveAllPoints() => points.Clear();

        /// <summary>
        /// Update the position of the point when is moved.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="p"></param>
        public void MovePoint(int i, Vector3 p) => points[i] = p;
    }
}
