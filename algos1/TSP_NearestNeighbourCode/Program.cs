using System;
using System.Diagnostics;
using System.Linq;

namespace TSP_NearestNeighbourCode
{
    class Program
    {
        static void Main(string[] args)
        {
            string algorithmCode = args[0];
            string fileName = args[1];
            double[,] cities = CitiesLoader.LoadFromFile(fileName);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var shortestRoute = ShortestRouteFinder.FindShortestRoute(algorithmCode, cities);

            stopwatch.Stop();

            double totalDistance = 0;

            for (int i = 0; i < shortestRoute.Count - 1; i++)
            {
                double distance = cities[shortestRoute[i], shortestRoute[i + 1]];
                totalDistance += distance;
                Console.WriteLine($"{i + 1}) {shortestRoute[i]} -> {shortestRoute[i + 1]} with (distance: {distance})");
            }

            double lastDistance = cities[shortestRoute.Last(), shortestRoute.First()];
            totalDistance += lastDistance;
            Console.WriteLine($"{shortestRoute.Count}) {shortestRoute.Last()} -> {shortestRoute.First()} with (distance: {lastDistance})");

            Console.WriteLine($"Total distance of the shortest route: {totalDistance}");
            Console.WriteLine($"Time taken to find the shortest route: {stopwatch.ElapsedMilliseconds} milliseconds");
        }
    }
}