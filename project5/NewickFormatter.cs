using System.IO;
using Bio.Phylogenetics;
using BioNewick = Bio.IO.Newick;

static class NewickFormatter
{
    public static void ToFile(string filePath, Tree tree)
    {
        var formatter = new BioNewick.NewickFormatter();
        formatter.Format(tree, "NewickFiles/" + filePath);
    }
}