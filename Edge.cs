public class Edge
{
    public Node From { get; set; }
    public Node To { get; set; }
    public double Cost { get; set; }

    public Edge(Node from, Node to, double cost)
    {
        From = from;
        To = to;
        Cost = cost;
    }
}
