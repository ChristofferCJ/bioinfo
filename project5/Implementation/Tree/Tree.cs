namespace Implementation;
using BioPhylogenetics = Bio.Phylogenetics;


public class Tree
{
    public Node Root;

    public Tree(Node root)
    {
        Root = root;
    }

    public void Print()
    {
        Root.Print();
    }

    public BioPhylogenetics.Tree ToBioTree()
    {
        var bioTree = new BioPhylogenetics.Tree();
        var root = new BioPhylogenetics.Node();

        

        return bioTree;
    }
}