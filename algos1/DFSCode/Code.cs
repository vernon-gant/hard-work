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

        public List<Vertex<T>> DepthFirstSearch(int VFrom, int VTo)
        {
            if (InvalidIndicesOperation(VFrom, VTo)) return new List<Vertex<T>>();
            if (VFrom == VTo && m_adjacency[VFrom, VTo] == 1) return StackToPath(new Stack<int>(new[] { VFrom }));

            ResetVisited();
            Stack<int> currentPath = new Stack<int>();
            int currentVertexIdx = VFrom;
            vertex[currentVertexIdx].Hit = true;
            currentPath.Push(currentVertexIdx);

            for (; currentPath.Count > 0;)
            {
                List<int> unvisitedNeighbours = GetUnvisitedNeighbours(currentVertexIdx);

                if (unvisitedNeighbours.Count == 0)
                {
                    currentPath.Pop();
                    currentVertexIdx = currentPath.Count > 0 ? currentPath.Peek() : -1;
                    continue;
                }

                int destinationIdx = unvisitedNeighbours.BinarySearch(VTo);

                if (destinationIdx >= 0)
                {
                    currentPath.Push(VTo);
                    break;
                }

                currentVertexIdx = unvisitedNeighbours[0];
                vertex[currentVertexIdx].Hit = true;
                currentPath.Push(currentVertexIdx);
            }

            return StackToPath(currentPath);
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

        private List<Vertex<T>> StackToPath(Stack<int> path)
        {
            Stack<int> tempStack = new ();

            for (; path.Count > 0; path.Pop())
            {
                int tempVertexIdx = path.Peek();
                tempStack.Push(tempVertexIdx);
            }

            List<Vertex<T>> resultPath = new ();

            foreach (int tempVertexIdx in tempStack) resultPath.Add(vertex[tempVertexIdx]);

            return resultPath;
        }

        private void ResetVisited()
        {
            for (int i = 0; i < max_vertex; i++)
            {
                if (vertex[i] != null) vertex[i].Hit = false;
            }
        }
    }
}