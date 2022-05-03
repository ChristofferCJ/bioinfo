namespace Implementation;

using System.Collections.Generic;

public class Node
{
    public string Name;

    public Node Parent;

    public List<Node> Children;
    
    public float Weight;

    public Node(string name = "")
    {
        Name = name;
        Parent = null;
        Children = new();
        Weight = default;
    }

    public void AddParent(Node parent)
    {
        Parent = parent;
        parent.AddChild(this);
    }

    public void AddChild(Node child)
    {
        Children.Add(child);
        child.Parent = this;
    }

    public void Print()
    {
        if (Parent != null)
        {
            System.Console.WriteLine($"{Parent.Name} -- {Weight.ToString("0.00")} -- {Name}");
        }
        foreach(var child in Children)
        {
            child.Print();
        }
    }
}