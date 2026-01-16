using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

            return m_adjacency[v1, v2] == 1 && m_adjacency[v2, v1] == 1;
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
            m_adjacency[v2, v1] = 0;
        }

        private bool InvalidIndex(int v)
        {
            return v < 0 || v >= max_vertex;
        }

        private bool InvalidIndicesOperation(int v1, int v2)
        {
            return InvalidIndex(v1) || InvalidIndex(v2) || vertex[v1] == null || vertex[v2] == null;
        }

        public List<Vertex<T>> WeakVertices()
        {
            List<Vertex<T>> weakVertices = new List<Vertex<T>>();

            for (int i = 0; i < max_vertex; i++)
            {
                bool isStrongVertex = false;

                List<int> realNeighbours = GetRealNeighbours(i);

                foreach (int firstNeighbour in realNeighbours)
                {
                    foreach (int secondNeighbour in realNeighbours)
                    {
                        if (firstNeighbour != secondNeighbour && IsEdge(firstNeighbour, secondNeighbour))
                        {
                            isStrongVertex = true;
                            break;
                        }
                    }

                    if (isStrongVertex) break;
                }

                if (!isStrongVertex) weakVertices.Add(vertex[i]);
            }

            return weakVertices;
        }

        private List<int> GetRealNeighbours(int vertexIdx)
        {
            List<int> realNeighbours = new List<int>();

            foreach (int neighbour in Enumerable.Range(0, max_vertex))
            {
                if (IsEdge(vertexIdx, neighbour) && neighbour != vertexIdx) realNeighbours.Add(neighbour);
            }

            return realNeighbours;
        }
    }
}