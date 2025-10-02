using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class EdgeCost
{
    public string From { get; set; }
    public string To { get; set; }
    public double TravelTime { get; set; }
}

public static class JSONLoader
{
    public static void UpdateGraphCosts(Graph graph, string jsonPath)
    {
        if (!File.Exists(jsonPath))
        {
            Console.WriteLine($"JSON file not found: {jsonPath}");
            return;
        }

        string json = File.ReadAllText(jsonPath);
        List<EdgeCost> costs = JsonConvert.DeserializeObject<List<EdgeCost>>(json);

        foreach (EdgeCost cost in costs)
        {
            if (graph.AdjacencyList.TryGetValue(cost.From, out List<Edge> edges))
            {
                foreach (Edge e in edges)
                {
                    if (e.To.Name == cost.To)
                    {
                        e.Cost = cost.TravelTime;
                    }
                }
            }
        }
    }
}
