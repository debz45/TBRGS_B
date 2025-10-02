using System;
using System.Collections.Generic;
using System.Linq;

public class SearchAlgorithms
{
    private List<(double, Node)> frontier = new List<(double, Node)>();

    // Depth First Search (DFS)
    public (List<string>, int) DFS(Graph graph, string origin, List<string> destinations)
    {
        foreach (Node n in graph.Nodes.Values)
        {
            n.Visited = false;
            n.PreviousNode = null;
        }

        int nodesExplored = 1;
        List<string> path = new List<string>();

        if (!graph.Nodes.ContainsKey(origin))
            return (path, nodesExplored);

        Node currentNode = graph.Nodes[origin];
        currentNode.Visited = true;

        while (!destinations.Contains(currentNode.Name))
        {
            Edge nextEdge = FindNode(graph, currentNode, origin);
            if (nextEdge.To == nextEdge.From)
                return (path, nodesExplored);

            currentNode = visitNode(nextEdge);
            nodesExplored++;
        }

        return (FindPath(origin, currentNode), nodesExplored);
    }

    // Breadth-First Search (BFS)
    public (List<string>, double, int) BFS(Graph graph, string origin, List<string> destinations)
    {
        List<string> path = new List<string>();
        double totalCost = 0.0;
        int nodesExplored = 0;

        foreach (Node n in graph.Nodes.Values)
        {
            n.Visited = false;
            n.PreviousNode = null;
        }

        if (!graph.Nodes.ContainsKey(origin) || destinations.Count == 0)
            return (path, totalCost, nodesExplored);

        List<string> frontier = new List<string>();
        int frontIndex = 0;
        graph.Nodes[origin].Visited = true;
        frontier.Add(origin);

        string? goalFound = null;

        while (frontIndex < frontier.Count)
        {
            string current = frontier[frontIndex++];
            nodesExplored++;

            if (destinations.Contains(current)) { goalFound = current; break; }

            if (!graph.AdjacencyList.TryGetValue(current, out List<Edge>? edges)) continue;

            List<string> neighbors = edges.Select(e => e.To.Name).ToList();
            neighbors.Sort(StringComparer.Ordinal);

            foreach (string neighbor in neighbors)
            {
                Node nn = graph.Nodes[neighbor];
                if (nn.Visited) continue;
                nn.Visited = true;
                nn.PreviousNode = graph.Nodes[current];
                frontier.Add(neighbor);
            }
        }

        if (goalFound == null) return (new List<string>(), 0.0, nodesExplored);

        Node cur = graph.Nodes[goalFound];
        path.Add(cur.Name);
        while (cur.Name != origin && cur.PreviousNode != null)
        {
            cur = cur.PreviousNode;
            path.Add(cur.Name);
        }
        path.Reverse();

        for (int i = 0; i < path.Count - 1; i++)
        {
            string from = path[i], to = path[i + 1];
            if (graph.AdjacencyList.TryGetValue(from, out List<Edge>? outEdges))
            {
                foreach (var edge in outEdges)
                {
                    if (edge.To.Name == to) { totalCost += edge.Cost; break; }
                }
            }
        }

        return (path, totalCost, nodesExplored);
    }

    // Dijkstra (DSA)
    public (List<string>, double, int) DSA(Graph graph, string origin, List<string> destinations)
    {
        foreach (Node n in graph.Nodes.Values)
        {
            n.Visited = false;
            n.PreviousNode = null;
        }
        frontier.Clear();
        int nodesExplored = 1;

        if (!graph.Nodes.ContainsKey(origin)) return (new List<string>(), 0.0, nodesExplored);

        Node currentNode = graph.Nodes[origin];
        currentNode.Visited = true;
        Enqueue(0.0, currentNode);

        while (!destinations.Contains(currentNode.Name))
        {
            UpdateDistances(graph, currentNode);
            if (frontier.Count == 0) return (new List<string>(), 0.0, nodesExplored);
            currentNode = frontier[0].Item2;
            nodesExplored++;
            currentNode.Visited = true;
        }

        var path = FindPath(origin, currentNode);
        double totalCost = 0.0;
        for (int i = 0; i < path.Count - 1; i++)
        {
            string from = path[i], to = path[i + 1];
            if (graph.AdjacencyList.TryGetValue(from, out List<Edge>? outEdges))
            {
                foreach (var edge in outEdges)
                    if (edge.To.Name == to) { totalCost += edge.Cost; break; }
            }
        }

        return (path, totalCost, nodesExplored);
    }

    // K-shortest paths (simplified)
    public List<(List<string> path, double cost)> FindKShortestPaths(Graph graph, string origin, string destination, int k)
    {
        List<(List<string> path, double cost)> paths = new List<(List<string>, double)>();

        var (firstPath, firstCost, _) = DSA(graph, origin, new List<string> { destination });
        if (firstPath.Count == 0) return paths;
        paths.Add((firstPath, firstCost));

        for (int i = 1; i < k; i++)
        {
            foreach (var edge in graph.Edges)
            {
                if (firstPath.Contains(edge.From.Name) && firstPath.Contains(edge.To.Name))
                    edge.Cost *= 1.5;
            }

            var (newPath, newCost, _) = DSA(graph, origin, new List<string> { destination });
            if (newPath.Count == 0 || paths.Any(p => p.path.SequenceEqual(newPath)))
                break;

            paths.Add((newPath, newCost));
        }

        return paths;
    }

    // Helper methods (Enqueue, Dequeue, UpdateDistances, FindPath, FindNode, visitNode)
    private int IndexOf(Node node) => frontier.FindIndex(f => f.Item2 == node);

    private void Enqueue(double distance, Node node)
    {
        frontier.Add((distance, node));
        frontier.Sort((a, b) => a.Item1.CompareTo(b.Item1));
    }

    private void Dequeue() => frontier.RemoveAt(0);

    private void UpdateDistances(Graph graph, Node thisNode)
    {
        foreach (Edge edge in graph.AdjacencyList[thisNode.Name])
        {
            if (IndexOf(edge.To) < 0 && !edge.To.Visited)
            {
                edge.To.PreviousNode = edge.From;
                Enqueue(edge.Cost + frontier[0].Item1, edge.To);
            }
            else if (!edge.To.Visited && frontier[IndexOf(edge.To)].Item1 > (edge.Cost + frontier[0].Item1))
            {
                edge.To.PreviousNode = edge.From;
                frontier[IndexOf(edge.To)] = (edge.Cost + frontier[0].Item1, edge.To);
            }
        }
        Dequeue();
    }

    private List<string> FindPath(string origin, Node endNode)
    {
        List<string> path = new List<string>();
        Node currentNode = endNode;
        while (origin != currentNode.Name)
        {
            path.Add(currentNode.Name);
            currentNode = currentNode.PreviousNode;
        }
        path.Add(origin);
        path.Reverse();
        return path;
    }

    private Edge FindNode(Graph graph, Node thisNode, string origin)
    {
        foreach (var edges in graph.AdjacencyList)
        {
            if (edges.Key == thisNode.Name)
            {
                foreach (var edge in edges.Value)
                    if (!edge.To.Visited) return edge;

                if (thisNode.Name == origin) return new Edge(thisNode, thisNode, 0.0);
                return FindNode(graph, thisNode.PreviousNode, origin);
            }
        }
        return null!;
    }

    private Node visitNode(Edge newEdge)
    {
        newEdge.To.PreviousNode = newEdge.From;
        newEdge.To.Visited = true;
        return newEdge.To;
    }
}
