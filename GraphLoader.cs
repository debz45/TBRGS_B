using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public static class GraphLoader
{
    public static (Graph graph, string origin, List<string> destinations) LoadGraphFromFile(string filePath)
    {
        Graph graph = new Graph();
        string origin = "NodeA"; // default origin
        List<string> destinations = new List<string> { "NodeB" }; // default destination

        if (!File.Exists(filePath))
        {
            Console.WriteLine("Graph JSON file not found!");
            return (graph, origin, destinations);
        }

        string json = File.ReadAllText(filePath);
        dynamic data = JsonConvert.DeserializeObject(json);

        // Load nodes
        foreach (var node in data.Nodes)
        {
            graph.AddNode(new Node((string)node.Name, (double)node.X, (double)node.Y));
        }

        // Load edges
        foreach (var edge in data.Edges)
        {
            graph.AddEdge((string)edge.From, (string)edge.To, (double)edge.Cost);
        }

        origin = data.Origin;
        destinations = new List<string>();
        foreach (var dest in data.Destinations)
            destinations.Add((string)dest);

        return (graph, origin, destinations);
    }
}
