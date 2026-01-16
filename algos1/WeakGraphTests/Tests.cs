using System.Collections.Generic;

using AlgorithmsDataStructures2;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WeakGraphTests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void FourWeakVertices()
        {
            SimpleGraph<int> graph = new SimpleGraph<int>(13);

            GraphSeeder.SeedThirteenVertices(graph);

            List<Vertex<int>> weakVertices = graph.WeakVertices();

            Assert.AreEqual(4, weakVertices.Count);
            Assert.AreEqual(6, weakVertices[0].Value);
            Assert.AreEqual(11, weakVertices[1].Value);
            Assert.AreEqual(12, weakVertices[2].Value);
            Assert.AreEqual(13, weakVertices[3].Value);
        }

        [TestMethod]
        public void TwoWeakVertices()
        {
            SimpleGraph<int> graph = new SimpleGraph<int>(9);

            GraphSeeder.SeedNineVertices(graph);

            List<Vertex<int>> weakVertices = graph.WeakVertices();

            Assert.AreEqual(2, weakVertices.Count);
            Assert.AreEqual(5, weakVertices[0].Value);
            Assert.AreEqual(9, weakVertices[1].Value);
        }

        [TestMethod]
        public void ThreeWeakVertices()
        {
            SimpleGraph<int> graph = new SimpleGraph<int>(9);

            GraphSeeder.SeedNineVerticesSecond(graph);

            List<Vertex<int>> weakVertices = graph.WeakVertices();

            Assert.AreEqual(3, weakVertices.Count);
            Assert.AreEqual(3, weakVertices[0].Value);
            Assert.AreEqual(5, weakVertices[1].Value);
            Assert.AreEqual(7, weakVertices[2].Value);
        }

        [TestMethod]
        public void OnlyStrongVertices()
        {
            SimpleGraph<int> graph = new SimpleGraph<int>(9);

            GraphSeeder.SeedNineVertices(graph);

            graph.AddEdge(3, 5);

            graph.AddEdge(5, 8);

            List<Vertex<int>> weakVertices = graph.WeakVertices();

            Assert.AreEqual(0, weakVertices.Count);
        }

        [TestMethod]
        public void OnlyWeakVertices()
        {
            SimpleGraph<int> graph = new SimpleGraph<int>(9);

            GraphSeeder.SeedNineVertices(graph);

            graph.RemoveEdge(1, 2);

            graph.RemoveEdge(2, 3);

            graph.RemoveEdge(5, 7);

            List<Vertex<int>> weakVertices = graph.WeakVertices();

            Assert.AreEqual(9, weakVertices.Count);
        }

        [TestMethod]
        public void EmptyGraph()
        {
            SimpleGraph<int> graph = new SimpleGraph<int>(0);

            List<Vertex<int>> weakVertices = graph.WeakVertices();

            Assert.AreEqual(0, weakVertices.Count);
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

            graph.AddEdge(6, 6);
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

        public static void SeedNineVertices(SimpleGraph<int> graph)
        {
            for (int i = 1; i <= 9; i++) graph.AddVertex(i);

            graph.AddEdge(0, 1);
            graph.AddEdge(0, 2);

            graph.AddEdge(1, 2);
            graph.AddEdge(1, 3);

            graph.AddEdge(2, 3);
            graph.AddEdge(2, 5);

            graph.AddEdge(3, 4);

            graph.AddEdge(4, 5);

            graph.AddEdge(5, 6);
            graph.AddEdge(5, 7);

            graph.AddEdge(6, 7);

            graph.AddEdge(7, 8);
        }

        public static void SeedNineVerticesSecond(SimpleGraph<int> graph)
        {
            for (int i = 0; i <= 8; i++) graph.AddVertex(i);

            graph.AddEdge(0, 6);
            graph.AddEdge(0, 7);
            graph.AddEdge(0, 8);

            graph.AddEdge(6, 8);
            graph.AddEdge(6, 5);
            graph.AddEdge(6, 4);

            graph.AddEdge(5, 2);

            graph.AddEdge(2, 1);
            graph.AddEdge(2, 4);

            graph.AddEdge(4, 1);

            graph.AddEdge(1, 3);
        }
    }
}