using System.Linq;

using AlgorithmsDataStructures2;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphTests
{
    [TestClass]
    public class AddVertex
    {
        private readonly SimpleGraph _graph = new (5);

        [TestMethod]
        public void Empty()
        {
            _graph.AddVertex(1);
            Assert.AreEqual(1, _graph.count);
            Assert.AreEqual(1, _graph.vertex[0].Value);
        }

        [TestMethod]
        public void FullGraph()
        {
            _graph.SeedFiveVertices();
            Assert.AreEqual(5, _graph.count);
            Assert.AreEqual(1, _graph.vertex[0].Value);
            Assert.AreEqual(2, _graph.vertex[1].Value);
            Assert.AreEqual(3, _graph.vertex[2].Value);
            Assert.AreEqual(4, _graph.vertex[3].Value);
            Assert.AreEqual(5, _graph.vertex[4].Value);
            _graph.AddVertex(6);
            Assert.AreEqual(5, _graph.count);
            Assert.AreEqual(1, _graph.vertex[0].Value);
            Assert.AreEqual(2, _graph.vertex[1].Value);
            Assert.AreEqual(3, _graph.vertex[2].Value);
            Assert.AreEqual(4, _graph.vertex[3].Value);
            Assert.AreEqual(5, _graph.vertex[4].Value);
        }

        [TestMethod]
        public void InTheMiddle()
        {
            _graph.SeedFiveVertices();
            _graph.RemoveVertex(2);
            Assert.AreEqual(4, _graph.count);
            Assert.AreEqual(1, _graph.vertex[0].Value);
            Assert.AreEqual(2, _graph.vertex[1].Value);
            Assert.AreEqual(null, _graph.vertex[2]);
            Assert.AreEqual(4, _graph.vertex[3].Value);
            Assert.AreEqual(5, _graph.vertex[4].Value);
            _graph.AddVertex(6);
            Assert.AreEqual(6, _graph.vertex[2].Value);
            Assert.IsFalse(Enumerable.Range(0, 5).All(i => _graph.IsEdge(2, i)));
            Assert.IsFalse(Enumerable.Range(0, 5).All(i => _graph.IsEdge(i, 2) == (i != 2)));
        }
    }

    [TestClass]
    public class AddEdge
    {
        private readonly SimpleGraph _graph = new (5);

        [DataTestMethod]
        [DataRow(-1, 0)]
        [DataRow(0, -1)]
        [DataRow(2, 2)]
        [DataRow(5, 5)]
        public void UnexistingVertices(int v1, int v2)
        {
            _graph.AddEdge(v1, v2);
            Assert.AreEqual(0, _graph.m_adjacency[0, 1]);
            Assert.AreEqual(0, _graph.m_adjacency[1, 0]);
        }

        [TestMethod]
        public void Empty()
        {
            _graph.AddVertex(1);
            _graph.AddVertex(2);
            Assert.AreEqual(0, _graph.m_adjacency[0, 1]);
            _graph.AddEdge(0, 1);
            Assert.AreEqual(1, _graph.m_adjacency[0, 1]);
            Assert.AreEqual(0, _graph.m_adjacency[1, 0]);
        }
    }

    [TestClass]
    public class RemoveVertex
    {
        private readonly SimpleGraph _graph = new (5);

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(5)]
        [DataRow(6)]
        public void InvalidIndices(int idx)
        {
            _graph.RemoveVertex(idx);
            Assert.AreEqual(0, _graph.count);
        }

        [TestMethod]
        public void AtTheBeginning()
        {
            _graph.SeedFiveVertices();
            Assert.AreEqual(5, _graph.count);
            Assert.AreEqual(0, _graph.m_adjacency[0, 0]);
            Assert.AreEqual(0, _graph.m_adjacency[1, 0]);
            Assert.AreEqual(1, _graph.m_adjacency[2, 0]);
            Assert.AreEqual(0, _graph.m_adjacency[3, 0]);
            Assert.AreEqual(1, _graph.m_adjacency[4, 0]);
            _graph.RemoveVertex(0);
            Assert.IsNull(_graph.vertex[0]);
            Assert.IsTrue(Enumerable.Range(0, 5).All(i => _graph.m_adjacency[0, i] == 0));
            Assert.AreEqual(4, _graph.count);
            Assert.AreEqual(0, _graph.m_adjacency[0, 0]);
            Assert.AreEqual(0, _graph.m_adjacency[1, 0]);
            Assert.AreEqual(0, _graph.m_adjacency[2, 0]);
            Assert.AreEqual(0, _graph.m_adjacency[3, 0]);
            Assert.AreEqual(0, _graph.m_adjacency[4, 0]);
        }

        [TestMethod]
        public void RemoveTwice()
        {
            _graph.SeedFiveVertices();
            _graph.RemoveVertex(0);
            _graph.RemoveVertex(0);
            Assert.AreEqual(4, _graph.count);
            Assert.AreEqual(0, _graph.m_adjacency[0, 0]);
            Assert.AreEqual(0, _graph.m_adjacency[1, 0]);
            Assert.AreEqual(0, _graph.m_adjacency[2, 0]);
            Assert.AreEqual(0, _graph.m_adjacency[3, 0]);
            Assert.AreEqual(0, _graph.m_adjacency[4, 0]);
        }
    }

    [TestClass]
    public class RemoveEdge
    {
        private readonly SimpleGraph _graph = new (5);

        [DataTestMethod]
        [DataRow(-1, 0)]
        [DataRow(0, -1)]
        [DataRow(2, 2)]
        [DataRow(5, 5)]
        public void UnexistingVertices(int v1, int v2)
        {
            _graph.RemoveEdge(v1, v2);
            Assert.AreEqual(0, _graph.m_adjacency[0, 1]);
            Assert.AreEqual(0, _graph.m_adjacency[1, 0]);
        }

        [TestMethod]
        public void Existing()
        {
            _graph.AddVertex(1);
            _graph.AddVertex(2);
            _graph.AddEdge(0, 1);
            Assert.AreEqual(1, _graph.m_adjacency[0, 1]);
            Assert.AreEqual(0, _graph.m_adjacency[1, 0]);
            _graph.RemoveEdge(0, 1);
            Assert.AreEqual(0, _graph.m_adjacency[0, 1]);
            Assert.AreEqual(0, _graph.m_adjacency[1, 0]);
        }
    }

    [TestClass]
    public class CheckIsEdge
    {
        private readonly SimpleGraph _graph = new (5);

        [DataTestMethod]
        [DataRow(-1, 0)]
        [DataRow(0, -1)]
        [DataRow(2, 2)]
        [DataRow(5, 5)]
        public void UnexistingVertices(int v1, int v2)
        {
            Assert.IsFalse(_graph.IsEdge(v1, v2));
        }

        [TestMethod]
        public void Existing()
        {
            _graph.AddVertex(1);
            _graph.AddVertex(2);
            _graph.AddEdge(0, 1);
            Assert.IsTrue(_graph.IsEdge(0, 1));
            Assert.IsFalse(_graph.IsEdge(1, 0));
        }
    }


    public static class GraphSeeder
    {
        public static void SeedFiveVertices(this SimpleGraph graph)
        {
            graph.AddVertex(1);
            graph.AddVertex(2);
            graph.AddVertex(3);
            graph.AddVertex(4);
            graph.AddVertex(5);

            // Add edges randomly but make graph is dense enough
            graph.AddEdge(0, 1);
            graph.AddEdge(0, 2);
            graph.AddEdge(0, 3);

            graph.AddEdge(1, 2);
            graph.AddEdge(1, 3);
            graph.AddEdge(1, 4);

            graph.AddEdge(2, 0);
            graph.AddEdge(2, 1);
            graph.AddEdge(2, 3);
            graph.AddEdge(2, 4);

            graph.AddEdge(3, 4);

            graph.AddEdge(4, 0);
            graph.AddEdge(4, 1);
            graph.AddEdge(4, 2);
            graph.AddEdge(4, 3);
        }
    }
}