using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Xna.Framework;

namespace EntryPoint
{
    class Graph
    {
        Dictionary<Vector2, int> adjList = new Dictionary<Vector2, int>();
        private int[,] _floyd;
        private int[,] _matrix;

        public Graph(IEnumerable<Tuple<Vector2, Vector2>> roads)
        {
            var enumerable = roads as IList<Tuple<Vector2, Vector2>> ?? roads.ToList();
            var road = enumerable.ToList();
            var j = 0;
            foreach (var t in enumerable.Where(t => !adjList.ContainsKey(t.Item1)))
            {
                adjList.Add(t.Item1, j);
                j++;
            }
            foreach (var t in enumerable.Where(t => !adjList.ContainsKey(t.Item2)))
            {
                adjList.Add(t.Item2, j);
                j++;
            }
            _matrix = CreateAdjMatrix(road);
            _floyd = FloydWarshall(_matrix);
        }



        private int[,] CreateAdjMatrix(List<Tuple<Vector2, Vector2>> roads)
        {
            var adj = new int[adjList.Count, adjList.Count];

            foreach (var road in roads)
            {
                adj[adjList[road.Item1], adjList[road.Item2]] = (int)Vector2.Distance(road.Item1, road.Item2);
            }
            for (var u = 0; u < adj.GetLength(0); u++) // Vertices
            {
                for (var v = 0; v < adj.GetLength(0); v++) //Edge
                {
                    if (adj[u, v] == 0 && u != v)
                    {
                        adj[u, v] = 5000000; //Infinity
                    }
                }
            }
            return adj;
        }


        private int[,] CreatePredecessorMatrix(int[,] matrix)
        {
            var loopCount = matrix.GetLength(0);
            var next = new int[loopCount, loopCount];

            for (var u = 0; u < loopCount; u++) // Vertices
            {
                for (var v = 0; v < loopCount; v++) //Edge
                {
                    if (matrix[u, v] != 5000000 && u != v)
                    {
                        next[u, v] = u;
                    }
                    else
                    {
                        next[u, v] = -1;
                    }
                }
            }
            return next;
        }


        private int[,] FloydWarshall(int[,] matrix)
        {
            var loopCount = matrix.GetLength(0);
            var next = CreatePredecessorMatrix(matrix);
            var w = new Stopwatch();

            for (var k = 0; k < loopCount; k++)
            {
                w.Start();
                Console.WriteLine(k);
                for (var i = 0; i < loopCount; i++)
                {
                    for (var j = 0; j < loopCount; j++)
                    {
                        if (matrix[i, j] > matrix[i, k] + matrix[k, j])
                        {
                            matrix[i, j] = matrix[i, k] + matrix[k, j];
                            next[i, j] = next[k, j];
                        }
                    }
                }
            }

            w.Stop();
            Console.WriteLine(w.Elapsed);
            return next;
        }

        private int VectorToInt(Vector2 v)
        {
            return adjList[v];
        }

        public List<Vector2> GetPath(Vector2 v1, Vector2 v2)
        {
            List<Vector2> result = new List<Vector2>();
            var start = VectorToInt(v1);
            var end = VectorToInt(v2);
            PrintPath(result, _floyd, start, end);
            return result;
        }

        private void PrintPath(List<Vector2> result, int[,] next, int i, int j)
        {
            if (next[i, j] == -1)
            {
                result.Add(adjList.FirstOrDefault(x => x.Value == i).Key);
            }
            else
            {
                var pre = next[i, j];
                PrintPath(result, next, i, pre);
                result.Add(adjList.FirstOrDefault(x => x.Value == j).Key);
            }
        }
    }


}