using System;
using System.Collections.Generic;
using System.Windows.Forms;

public class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            // Run GUI if no arguments
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            return;
        }

        if (args.Length != 2)
        {
            Console.WriteLine("Usage: route_finder <map_file_path> <search_method>");
            return;
        }

        string filePath = args[0];
        string searchMethod = args[1];

        var (graph, origin, destinations) = GraphLoader.LoadGraphFromFile(filePath);
        if (graph == null || string.IsNullOrEmpty(origin) || destinations.Count == 0)
        {
            Console.WriteLine("Failed to load map data or essential components.");
            return;
        }

        // Load ML-predicted travel times
        JSONLoader.UpdateGraphCosts(graph, "Datasets/PredictedTravelTimes.json");

        List<string> path = null;
        double cost = 0;
        int nodesExplored = 0;

        SearchAlgorithms searcher = new SearchAlgorithms();

        switch (searchMethod.ToUpper())
        {
            case "DFS":
                (path, nodesExplored) = searcher.DFS(graph, origin, destinations);
                cost = 0;
                break;
            case "BFS":
                (path, cost, nodesExplored) = searcher.BFS(graph, origin, destinations);
                break;
            case "GBFS":
                (path, nodesExplored) = searcher.GBFS(graph, origin, destinations);
                cost = 0;
                break;
            case "ASTAR":
                (path, cost, nodesExplored) = searcher.AStar(graph, origin, destinations);
                break;
            case "DSA":
                (path, nodesExplored) = searcher.DSA(graph, origin, destinations);
                cost = 0;
                break;
            case "IDASTAR":
                (path, cost, nodesExplored) = searcher.IDAStar(graph, origin, destinations);
                break;
            default:
                Console.WriteLine($"Unknown search method: {searchMethod}");
                return;
        }

        if (path != null && path.Count > 0)
        {
            Console.WriteLine($"Path: {string.Join(" -> ", path)} | Nodes explored: {nodesExplored} | Cost: {cost:F2}");
        }
        else
        {
            Console.WriteLine("No path found.");
        }
    }
}
