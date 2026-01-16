using System;
using System.Collections;
using System.Collections.Generic;

namespace AlgorithmsDataStructures2
{
    public class Vertex<T>
    {
        public bool Hit;
        public T Value;
        public Vertex(T val)
        {
            Value = val;
            Hit = false;
        }
    }

    public class SimpleGraph<T>
    {
        public Vertex<T>[] vertex;
        public int[,] m_adjacency;
        public int max_vertex;
        public int count;

        public SimpleGraph(int size)
        {
            max_vertex = size;
            m_adjacency = new int [size, size];
            vertex = new Vertex<T> [size];
            count = 0;
        }

        public void AddVertex(T value)
        {
            if (count == max_vertex) return;

            Vertex<T> newVertex = new Vertex<T>(value);

            for (int i = 0; i < max_vertex; i++)
            {
                if (vertex[i] == null)
                {
                    vertex[i] = newVertex;
                    count++;

                    break;
                }
            }
        }

        public void RemoveVertex(int v)
        {
            if (InvalidIndex(v)) return;

            vertex[v] = null;

            for (int i = 0; i < max_vertex; i++)
            {
                if (i == v) ClearVertexRow(v);
                else m_adjacency[i, v] = 0;
            }

            count--;
        }

        private void ClearVertexRow(int v)
        {
            for (int i = 0; i < max_vertex; i++)
            {
                m_adjacency[v, i] = 0;
            }
        }

        public bool IsEdge(int v1, int v2)
        {
            if (InvalidIndicesOperation(v1, v2)) return false;

            return m_adjacency[v1, v2] == 1;
        }

        public void AddEdge(int v1, int v2)
        {
            if (InvalidIndicesOperation(v1, v2)) return;

            m_adjacency[v1, v2] = 1;
            m_adjacency[v2, v1] = 1;
        }

        public void RemoveEdge(int v1, int v2)
        {
            if (InvalidIndicesOperation(v1, v2)) return;

            m_adjacency[v1, v2] = 0;
        }

        private bool InvalidIndex(int v)
        {
            return v < 0 || v >= max_vertex;
        }

        private bool InvalidIndicesOperation(int v1, int v2)
        {
            return InvalidIndex(v1) || InvalidIndex(v2) || vertex[v1] == null || vertex[v2] == null;
        }

        public List<Vertex<T>> BreadthFirstSearch(int VFrom, int VTo)
        {
            if (InvalidIndicesOperation(VFrom, VTo)) return new List<Vertex<T>>();
            if (VFrom == VTo && m_adjacency[VFrom, VTo] == 1) return new List<Vertex<T>> {vertex[VFrom]};

            ResetVisited();
            Queue<int> currentVertexIndices = new ();
            Dictionary<int, int> vertexWithParent = new ();
            int currentVertexIdx = VFrom;
            vertex[currentVertexIdx].Hit = true;
            currentVertexIndices.Enqueue(currentVertexIdx);

            for (;currentVertexIndices.Count > 0;)
            {
                currentVertexIdx = currentVertexIndices.Dequeue();

                // We need to check also whether is is an unexisting cyclic path which would result in enqueuing the starting vertex
                // and then retrieving it from the queue and returning it as a path
                // If the cyclic path does exist it is checked in the beginning of the method
                if (currentVertexIdx == VTo && currentVertexIdx != VFrom) return BuildPathFromVerticesWithParents(vertexWithParent, VFrom, VTo);

                List<int> unvisitedNeighbours = GetUnvisitedNeighbours(currentVertexIdx);

                foreach (int unvisitedNeighbour in unvisitedNeighbours)
                {
                    vertex[unvisitedNeighbour].Hit = true;
                    vertexWithParent.Add(unvisitedNeighbour, currentVertexIdx);
                    currentVertexIndices.Enqueue(unvisitedNeighbour);
                }
            }

            return new List<Vertex<T>>();
        }

        private List<int> GetUnvisitedNeighbours(int vertexIdx)
        {
            List<int> unvisitedNeighbours = new ();

            for (int i = 0; i < max_vertex; i++)
            {
                if (m_adjacency[vertexIdx, i] != 0 && !vertex[i].Hit)
                    unvisitedNeighbours.Add(i);
            }

            return unvisitedNeighbours;
        }

        private List<Vertex<T>> BuildPathFromVerticesWithParents(Dictionary<int, int> vertexWithParent, int VFrom, int VTo)
        {
            List<Vertex<T>> path = new ();

            int currentVertexIdx = VTo;

            for (; currentVertexIdx != VFrom; currentVertexIdx = vertexWithParent[currentVertexIdx]) path.Add(vertex[currentVertexIdx]);

            path.Add(vertex[VFrom]);
            path.Reverse();

            return path;
        }

        private void ResetVisited()
        {
            for (int i = 0; i < max_vertex; i++) if (vertex[i] != null) vertex[i].Hit = false;
        }
    }
}