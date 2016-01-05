using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public KDNode rootNode { get; set; }
        public List<Vector2> PotentialBuildingsInRange;
    
        public KDTree()
        {
            rootNode = null;
            PotentialBuildingsInRange = new List<Vector2>();
        }


        public KDNode MakeTree(List<Vector2> points, int depth)
        {
            if (points.Count == 0)
            {
                return null;
            }

            List<Vector2> left = new List<Vector2>();
            List<Vector2> right = new List<Vector2>();


            if (depth % 2 == 0) // If depth is even, sort on X
            {
                points = points.OrderBy(n => n.X).ToList();
                var middle = points[points.Count / 2]; // Middle Element of list
                points.Remove(middle);
                foreach (var item in points)
                {
                    if (item.X <= middle.X)
                    {
                        left.Add(item);
                    }
                    else if (item.X > middle.X)
                    {
                        right.Add(item);
                    }
                }

                return new KDNode
                {
                    dt = middle,
                    left = MakeTree(left, depth + 1),
                    right = MakeTree(right, depth + 1)
                };
            }
            else // If depth is oneven, sort on Y
            {
                points = points.OrderBy(n => n.Y).ToList();
                var middle = points[points.Count / 2]; // Middle Element of list
                points.Remove(middle);
                foreach (var item in points)
                {
                    if (item.Y <= middle.Y)
                    {
                        left.Add(item);
                    }
                    else if (item.Y > middle.Y)
                    {
                        right.Add(item);
                    }
                }

                return new KDNode
                {
                    dt = middle,
                    left = MakeTree(left, depth + 1),
                    right = MakeTree(right, depth + 1)
                };
            }
        }

        public void RangeSearch(Vector2 house, KDNode root, float maxDistance, int depth)
        {
            if (root == null)
            {
                return;
            }

            if (depth % 2 == 0)
            {
                if (root.dt.X < (house.X - maxDistance))
                {
                    RangeSearch(house, root.right, maxDistance, depth + 1);
                }
                else if (root.dt.X > (house.X + maxDistance))
                {
                    RangeSearch(house, root.left, maxDistance, depth + 1);
                }
                else if ((root.dt.X >= (house.X - maxDistance)) && (root.dt.X <= (house.X + maxDistance)))
                {
                    PotentialBuildingsInRange.Add(root.dt);
                    RangeSearch(house, root.left, maxDistance, depth + 1);
                    RangeSearch(house, root.right, maxDistance, depth + 1);
                }
            }
            else
            {
                if (root.dt.Y < (house.Y - maxDistance))
                {
                    RangeSearch(house, root.right, maxDistance, depth + 1);
                }
                else if (root.dt.Y > (house.Y + maxDistance))
                {
                    RangeSearch(house, root.left, maxDistance, depth + 1);
                }
                else if ((root.dt.Y >= (house.Y - maxDistance)) && (root.dt.Y <= (house.Y + maxDistance)))
                {
                    PotentialBuildingsInRange.Add(root.dt);
                    RangeSearch(house, root.left, maxDistance, depth + 1);
                    RangeSearch(house, root.right, maxDistance, depth + 1);
                }
            }
        }
    }
}
