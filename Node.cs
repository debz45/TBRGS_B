public class Node
{
    public string Name { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public bool Visited { get; set; }
    public Node PreviousNode { get; set; }

    public Node(string name, double x, double y)
    {
        Name = name;
        X = x;
        Y = y;
        Visited = false;
        PreviousNode = null;
    }
}
