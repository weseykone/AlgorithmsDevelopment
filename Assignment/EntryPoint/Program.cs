using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace EntryPoint
{
#if WINDOWS || LINUX
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            var fullscreen = false;
            read_input:
            switch (
                Microsoft.VisualBasic.Interaction.InputBox(
                    "Which assignment shall run next? (1, 2, 3, 4, or q for quit)", "Choose assignment",
                    VirtualCity.GetInitialValue()))
            {
                case "1":
                    using (var game = VirtualCity.RunAssignment1(SortSpecialBuildingsByDistance, fullscreen))
                        game.Run();
                    break;
                case "2":
                    using (var game = VirtualCity.RunAssignment2(FindSpecialBuildingsWithinDistanceFromHouse, fullscreen))
                        game.Run();
                    break;
                case "3":
                    using (var game = VirtualCity.RunAssignment3(FindRoute, fullscreen))
                        game.Run();
                    break;
                case "4":
                    using (var game = VirtualCity.RunAssignment4(FindRoutesToAll, fullscreen))
                        game.Run();
                    break;
                case "q":
                    return;
            }
            goto read_input;
        }

        private static IEnumerable<Vector2> SortSpecialBuildingsByDistance(Vector2 house,
            IEnumerable<Vector2> specialBuildings)
        {
            List<Vector2> buildings = specialBuildings.ToList();
            List<Tuple<Vector2, float>> unsortedlist = new List<Tuple<Vector2, float>>();

            for (var i = 0; i < buildings.Count(); i++)
            {
                unsortedlist.Add(Tuple.Create(buildings[i], Vector2.Distance(buildings[i], house)));

            }
            Merge m = new Merge();
            List<Tuple<Vector2, float>> sortedlist = m.mergesort(unsortedlist);
            buildings = null;
            buildings = new List<Vector2>();
            for (int i = 0; i < sortedlist.Count; i++)
            {
                buildings.Add(sortedlist[i].Item1);
            }
            return buildings;
            //return specialBuildings.OrderBy(v => Vector2.Distance(v, house));
        }



        private static IEnumerable<IEnumerable<Vector2>> FindSpecialBuildingsWithinDistanceFromHouse(
            IEnumerable<Vector2> specialBuildings,
            IEnumerable<Tuple<Vector2, float>> housesAndDistances)
        {
            List<List<Vector2>> specialBuildingsInRange = new List<List<Vector2>>();
            List<Tuple<Vector2, float>> buildingList = housesAndDistances.ToList();

            var a = new KDTree();
            var b = a.MakeTree(specialBuildings.ToList(), 0);

            foreach (var building in buildingList)
            {
                var specialBuildingsInBox = new List<Vector2>();

                a.RangeSearch(building.Item1, b, building.Item2, 0, new Func<Vector2, float>[] { p => p.X, p => p.Y });

                specialBuildingsInBox.AddRange(
                    from x in a.PotentialBuildingsInRange
                    where Vector2.Distance(building.Item1, x) <= building.Item2
                    select x);

                specialBuildingsInRange.Add(specialBuildingsInBox);
            }
    
            Console.WriteLine(specialBuildingsInRange.Count);

            return specialBuildingsInRange;

            /* 
              return from h in housesAndDistances 
              select
              from s in specialBuildings
              where Vector2.Distance(h.Item1, s) <= h.Item2
              select s;
            */
        }



        private static IEnumerable<Tuple<Vector2, Vector2>> FindRoute(Vector2 startingBuilding,
            Vector2 destinationBuilding, IEnumerable<Tuple<Vector2, Vector2>> roads)
        {
            List<Tuple<Vector2, Vector2>> roadsList = roads.ToList();

            for (int x = 0; x < roadsList.Count; x++)
            {
                Console.WriteLine("Item {0}: {1} \n Distance: {2}", x, roadsList[x], Vector2.Distance(roadsList[x].Item1, roadsList[x].Item2));
            }



            return new List<Tuple<Vector2, Vector2>>();

            /*
            var startingRoad = roads.Where(x => x.Item1.Equals(startingBuilding)).First();
            List<Tuple<Vector2, Vector2>> fakeBestPath = new List<Tuple<Vector2, Vector2>>() { startingRoad };
            var prevRoad = startingRoad;

            for (int i = 0; i < 30; i++)
            {
                prevRoad = (roads.Where(x => x.Item1.Equals(prevRoad.Item2)).OrderBy(x => Vector2.Distance(x.Item2, destinationBuilding)).First());
                fakeBestPath.Add(prevRoad);
            }

            return fakeBestPath;
            */
        }

        private static IEnumerable<IEnumerable<Tuple<Vector2, Vector2>>> FindRoutesToAll(Vector2 startingBuilding,
            IEnumerable<Vector2> destinationBuildings, IEnumerable<Tuple<Vector2, Vector2>> roads)
        {
            Warshall w = new Warshall();
            Dictionary<Vector2, int> adjList2 = new Dictionary<Vector2, int>();
            var allRoads = roads.ToList();
            var roadsTuplesList = new List<Tuple<Vector2, Vector2>>();
            List<List<Tuple<Vector2, Vector2>>> allRoadsToBuildings = new List<List<Tuple<Vector2, Vector2>>>();

            var j = 0;
            foreach (var t in roads.Where(t => !adjList2.ContainsKey(t.Item1)))
            {
                adjList2.Add(t.Item1, j);
                j++;
            }

            var matrix = w.CreateAdjMatrix(adjList2, allRoads);
            var next = w.FloydWarshall(matrix, adjList2);

            var startPoint = adjList2.FirstOrDefault(x => x.Key == startingBuilding).Value;

            foreach (var destination in destinationBuildings)
            {
                var endPoint = adjList2.FirstOrDefault(x => x.Key == destination).Value;
                var roadToBuilding = w.PrintPath(next, adjList2, startPoint, endPoint);

                for (int i = 0; i < roadToBuilding.Count; i++)
                {
                    if(i + 1 < roadToBuilding.Count)
                        roadsTuplesList.Add(Tuple.Create(roadToBuilding[i], roadToBuilding[i+1]));
                }

                allRoadsToBuildings.Add(roadsTuplesList);
            }

            return allRoadsToBuildings;
        }
    }
#endif
}
