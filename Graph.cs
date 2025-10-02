using System.Collections.Generic;

public class Graph
{
    public Dictionary<string, Node> Nodes { get; set; } = new Dictionary<string, Node>();
    public Dictionary<string, List<Edge>> AdjacencyList { get; set; } = new Dictionary<string, List<Edge>>();

    public void AddNode(Node node)
    {
        Nodes[node.Name] = node;
        if (!AdjacencyList.ContainsKey(node.Name))
            AdjacencyList[node.Name] = new List<Edge>();
    }

    public void AddEdge(string from, string to, double cost)
    {
        Node fNode = Nodes[from];
        Node tNode = Nodes[to];
        Edge edge = new Edge(fNode, tNode, cost);
        AdjacencyList[from].Add(edge);
    }
}
