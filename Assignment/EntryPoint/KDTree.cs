using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace EntryPoint

{
    public class KDNode
    {
        public Vector2 dt { get; set; }
        public KDNode left { get; set; }
        public KDNode right { get; set; }
    }

    class KDTree
    {
        public List<Vector2> PotentialBuildingsInRange;

        public KDTree()
        {
            PotentialBuildingsInRange = new List<Vector2>();
        }


        public KDNode MakeTree(List<Vector2> points, int depth)
        {

            if (points.Count == 0)
            {
                return null;
            }

            Vector2 middle;
            List<Vector2> left = new List<Vector2>();
            List<Vector2> right = new List<Vector2>();

            if (depth % 2 == 0) // If depth is even, sort on X
            {
                middle = SortByCoordinate(points, p => p.X, left, right);
            }
            else // If depth is oneven, sort on Y
            {
                middle = SortByCoordinate(points, p => p.Y, left, right);
            }

            return new KDNode
            {
                dt = middle,
                left = MakeTree(left, depth + 1),
                right = MakeTree(right, depth + 1)
            };
        }

        private static Vector2 SortByCoordinate(List<Vector2> points, Func<Vector2, float> coordinate, List<Vector2> left, List<Vector2> right)
        {
            points = points.OrderBy(coordinate).ToList();
            var middle = points[points.Count/2]; // Middle Element of list
            points.Remove(middle);
            foreach (var item in points)
            {
                if (coordinate(item) <= coordinate(middle))
                {
                    left.Add(item);
                }
                else if (coordinate(item) > coordinate(middle))
                {
                    right.Add(item);
                }
            }
            return middle;
        }

        public void RangeSearch(Vector2 house, KDNode root, float maxDistance, int depth, Func<Vector2, float>[] coordinates )
        {
            if (root == null)
            {
                return;
            }
            var rootVector = coordinates[depth % 2](root.dt);
            var houseVector = coordinates[depth % 2](house);

            if (rootVector < (houseVector - maxDistance))
            {
                RangeSearch(house, root.right, maxDistance, depth + 1, coordinates);
            }
            else if (rootVector > (houseVector + maxDistance))
            {
                RangeSearch(house, root.left, maxDistance, depth + 1, coordinates);
            }
            else if ((rootVector >= (houseVector - maxDistance)) && (rootVector <= (houseVector + maxDistance)))
            {
                PotentialBuildingsInRange.Add(root.dt);
                RangeSearch(house, root.left, maxDistance, depth + 1, coordinates);
                RangeSearch(house, root.right, maxDistance, depth + 1, coordinates);
            }
        }
    }
}
