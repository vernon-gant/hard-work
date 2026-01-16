using System;
using System.Collections.Generic;
using System.IO;

namespace TSP_NearestNeighbourCode
{
    public class Point
    {
        public double X { get; }
        public double Y { get; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public static class CitiesLoader
    {
        public static double[,] LoadFromFile(string fileName)
        {
            var lines = File.ReadAllLines(fileName);
            var n = int.Parse(lines[0]);
            var cities = new List<Point>(n);

            for (int i = 1; i <= n; i++)
            {
                var parts = lines[i].Split(' ');
                cities.Add(new Point(double.Parse(parts[0]), double.Parse(parts[1])));
            }

            double[,] matrix = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i != j) matrix[i, j] = CalculateDistance(cities[i], cities[j]);
                    else matrix[i, j] = 0;  // Distance to itself is 0.
                }
            }

            return matrix;
        }

        // Calculate the distance between two points using the Euclidean distance formula
        private static double CalculateDistance(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
        }
    }

}