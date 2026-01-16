using System.Collections.Generic;

using AlgorithmsDataStructures2;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DFSTests
{
    [TestClass]
    public class TenVerticesGraph
    {
        [TestMethod]
        public void ExistingPathFirst()
        {
            var graph = new SimpleGraph<int>(13);

            GraphSeeder.SeedThirteenVertices(graph);

            List<Vertex<int>> path = graph.BreadthFirstSearch(0, 12);

            Assert.AreEqual(6, path.Count);
            Assert.AreEqual(1, path[0].Value);
            Assert.AreEqual(3, path[1].Value);
            Assert.AreEqual(5, path[2].Value);
            Assert.AreEqual(8, path[3].Value);
            Assert.AreEqual(9, path[4].Value);
            Assert.AreEqual(13, path[5].Value);
        }

        [TestMethod]
        public void ExistingPathSecond()
        {
            var graph = new SimpleGraph<int>(13);

            GraphSeeder.SeedThirteenVertices(graph);

            List<Vertex<int>> path = graph.BreadthFirstSearch(10, 4);

            Assert.AreEqual(4, path.Count);
            Assert.AreEqual(11, path[0].Value);
            Assert.AreEqual(10, path[1].Value);
            Assert.AreEqual(8, path[2].Value);
            Assert.AreEqual(5, path[3].Value);
        }

        [TestMethod]
        public void NonExistingPath()
        {
            var graph = new SimpleGraph<int>(14);

            GraphSeeder.SeedThirteenVertices(graph);

            graph.AddVertex(14);

            List<Vertex<int>> path = graph.BreadthFirstSearch(0, 13);

            Assert.AreEqual(0, path.Count);
        }

        [TestMethod]
        public void ExistingCyclicPath()
        {
            var graph = new SimpleGraph<int>(13);

            GraphSeeder.SeedThirteenVertices(graph);

            List<Vertex<int>> path = graph.BreadthFirstSearch(0, 0);

            Assert.AreEqual(1, path.Count);
            Assert.AreEqual(1, path[0].Value);
        }

        [TestMethod]
        public void NonExistingCyclicPath()
        {
            var graph = new SimpleGraph<int>(13);

            GraphSeeder.SeedThirteenVertices(graph);

            List<Vertex<int>> path = graph.BreadthFirstSearch(2, 2);

            Assert.AreEqual(0, path.Count);
        }
    }

    public static class GraphSeeder
    {
        public static void SeedThirteenVertices(SimpleGraph<int> graph)
        {
            for (int i = 1; i <= 13; i++)
            {
                graph.AddVertex(i);
            }

            graph.AddEdge(0, 0);
            graph.AddEdge(0, 1);
            graph.AddEdge(0, 2);
            graph.AddEdge(0, 3);

            graph.AddEdge(1, 2);

            graph.AddEdge(3, 2);

            graph.AddEdge(2, 4);
            graph.AddEdge(2, 5);

            graph.AddEdge(5, 5);
            graph.AddEdge(5, 7);

            graph.AddEdge(4, 6);
            graph.AddEdge(4, 7);

            graph.AddEdge(6, 6); // Cyclic edge for vertex 7
            graph.AddEdge(6, 7);

            graph.AddEdge(7, 8);
            graph.AddEdge(7, 9);

            graph.AddEdge(8, 12);
            graph.AddEdge(8, 11);
            graph.AddEdge(8, 9);

            graph.AddEdge(9, 10);

            graph.AddEdge(10, 11);

            graph.AddEdge(12, 12);
        }
    }
}