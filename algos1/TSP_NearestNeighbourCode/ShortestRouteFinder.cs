using System;
using System.Collections.Generic;
using System.Linq;

namespace TSP_NearestNeighbourCode
{
    public static class ShortestRouteFinder
    {
        private static readonly Dictionary<string, Func<double[,], List<int>>> _algorithms = new ()
        {
            { "-e", FindUsingEnumeration },
            { "-n", FindUsingNearestNeighbour }
        };

        public static List<int> FindShortestRoute(string algorithmCode, double[,] matrix)
        {
            if (_algorithms.TryGetValue(algorithmCode, out var algorithm)) return algorithm.Invoke(matrix);

            throw new ArgumentException($"Unsupported algorithm code: {algorithmCode}");
        }

        private static List<int> FindUsingEnumeration(double[,] adjacencyMatrix)
        {
            double shortestDistance = double.MaxValue;
            List<int> bestRoute = null;

            var routes = GetDistinctCycles(adjacencyMatrix.GetLength(0));

            foreach (var route in routes)
            {
                double currentDistance = CalculateRouteDistance(route, adjacencyMatrix);
                if (currentDistance < shortestDistance)
                {
                    shortestDistance = currentDistance;
                    bestRoute = route;
                }
            }

            return bestRoute;
        }


        private static List<int> FindUsingNearestNeighbour(double[,] adjacencyMatrix)
        {
            double shortestDistance = double.MaxValue;
            List<int> bestRoute = null;
            int numberOfCities = adjacencyMatrix.GetLength(0);

            // Try each city as a starting point
            for (int start = 0; start < numberOfCities; start++)
            {
                // Use a hashset to keep track of visited cities when finding the nearest neighbour
                var visited = new HashSet<int>();
                var currentRoute = new List<int> { start };
                visited.Add(start);

                int currentCity = start;
                for (int i = 1; i < numberOfCities; i++)
                {
                    int nearestNeighbour = GetNearestNeighbour(currentCity, adjacencyMatrix, visited);
                    currentRoute.Add(nearestNeighbour);
                    visited.Add(nearestNeighbour);
                    currentCity = nearestNeighbour;
                }

                double currentDistance = CalculateRouteDistance(currentRoute, adjacencyMatrix);
                if (currentDistance < shortestDistance)
                {
                    shortestDistance = currentDistance;
                    bestRoute = currentRoute;
                }
            }

            return bestRoute;
        }

        private static int GetNearestNeighbour(int currentCity, double[,] adjacencyMatrix, HashSet<int> visited)
        {
            int nearestCity = -1;
            double shortestDistance = double.MaxValue;

            for (int city = 0; city < adjacencyMatrix.GetLength(0); city++)
            {
                if (!visited.Contains(city) && adjacencyMatrix[currentCity, city] < shortestDistance)
                {
                    shortestDistance = adjacencyMatrix[currentCity, city];
                    nearestCity = city;
                }
            }

            return nearestCity;
        }

        private static List<List<int>> GetDistinctCycles(int numberOfCities)
        {
            var citiesIndices = Enumerable.Range(0, numberOfCities).ToList();

            if (numberOfCities <= 1) return new List<List<int>> { citiesIndices };

            // Fix the first city to get distinct cycles
            var fixedCity = citiesIndices[0];

            var permutationsOfRest = GetPermutations(citiesIndices.Skip(1).ToList(), numberOfCities - 1);

            // Add the fixed city to the beginning of each permutation to form cycles
            var distinctCycles = permutationsOfRest.Select(p => new List<int> { fixedCity }.Concat(p).ToList()).ToList();

            return distinctCycles;
        }


        private static List<List<int>> GetPermutations(List<int> list, int length)
        {
            if (length == 0) return new List<List<int>> { new () };

            var permutations = new List<List<int>>();

            foreach (var item in list)
            {
                var remainingItems = list.Except(new List<int> { item }).ToList();
                foreach (var permutationOfRest in GetPermutations(remainingItems, length - 1))
                {
                    permutations.Add(new List<int> { item }.Concat(permutationOfRest).ToList());
                }
            }

            return permutations;
        }


        private static double CalculateRouteDistance(List<int> route, double[,] adjacencyMatrix)
        {
            double totalDistance = 0;

            for (int i = 0; i < route.Count - 1; i++) totalDistance += adjacencyMatrix[route[i], route[i + 1]];

            // Add the distance from the last city back to the first city
            totalDistance += adjacencyMatrix[route[route.Count - 1], route[0]];

            return totalDistance;
        }
    }
}