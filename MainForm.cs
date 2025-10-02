using System;
using System.Collections.Generic;
using System.Windows.Forms;

public class MainForm : Form
{
    private TextBox originBox, destBox;
    private Button findRouteBtn;
    private ListBox resultBox;

    public MainForm()
    {
        this.Text = "TBRGS Route Finder";
        this.Width = 600;
        this.Height = 400;

        originBox = new TextBox() { Top = 20, Left = 20, Width = 100 };
        destBox = new TextBox() { Top = 20, Left = 140, Width = 100 };
        findRouteBtn = new Button() { Text = "Find Route", Top = 20, Left = 260 };
        resultBox = new ListBox() { Top = 60, Left = 20, Width = 540, Height = 280 };

        findRouteBtn.Click += FindRouteBtn_Click;

        this.Controls.Add(originBox);
        this.Controls.Add(destBox);
        this.Controls.Add(findRouteBtn);
        this.Controls.Add(resultBox);
    }

    private void FindRouteBtn_Click(object sender, EventArgs e)
    {
        string origin = originBox.Text;
        string dest = destBox.Text;

        var (graph, _, _) = GraphLoader.LoadGraphFromFile("Datasets/map.json");
        JSONLoader.UpdateGraphCosts(graph, "Datasets/PredictedTravelTimes.json");

        SearchAlgorithms searcher = new SearchAlgorithms();
        var (path, nodesExplored) = searcher.DFS(graph, origin, new List<string> { dest });

        resultBox.Items.Clear();
        if (path.Count > 0)
            resultBox.Items.Add($"Path: {string.Join(" -> ", path)} | Nodes explored: {nodesExplored}");
        else
            resultBox.Items.Add("No path found.");
    }
}
