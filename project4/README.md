# Project 4

## How to run
``dotnet`` version 4 or higher is required to run this. To run the project, place your Newick files in the `Data` folder, an refer to them from `Program.cs` on lines 10 and 11.

## Status of work
The implementation works fine, however, the parser library that I have found for parsing Newick files are, unfortunately, rather bad. Sometimes it does not parse a Newick file, throwing an error of a missing comma in the Newick file. I have found the source code and tried to fix the parser myself, but to no avail.

## Implementation
For my implementation, I decided to implement the Day algorithm. My implementation follows the algortihm pretty much to a T, with some small exceptions. When creating the ordering of the leaves in the first tree, I store the ordering as a dictionary:
```c#
private void CreateOrderingOfLeaves(Tree tree)
{
    var ord = 1;
    Inner(tree.Root);
    void Inner(Node curr)
    {
        if(!String.IsNullOrEmpty(curr.Name))
        {
            var trimmedName = curr.Name.Replace("'", "").Trim();
            Ordering.Add(trimmedName, ord++);
        }
        foreach(var node in curr.Nodes ?? new List<Node>())
            Inner(node);
    }
}
```
Doing it this way saves passing over the second tree to also assign orderings, since an invariant to the two trees are that they both should contain the same leaf names.

The parser I use for the Newick files roots the trees at an arbitrary internal node, so I also had to write functions for rooting the first tree at a leaf, and then root the second tree at that same leaf:
```c#
private Tree RootTreeAtLeaf(Tree tree)
{
    // Find first leaf
    var leaf = tree.Root;
    while (!leaf.IsLeaf)
        leaf = leaf.Nodes[0];
    var leafRootedTree = ChangeRoot(tree, leaf);
    return leafRootedTree;
}

private Tree ChangeRoot(Tree tree, Node newRoot)
{
    var (_, branch) = GetBranchForNewRoot(tree.Root, newRoot);
    tree.Root.IsRoot = false;

    for (int idx = 1; idx < branch.Count; idx++)
    {
        var prev = branch[idx - 1];
        var curr = branch[idx];
        var edge = prev.Children[curr];
        prev.Children.Remove(curr);
        curr.Children.Add(prev, edge);
    }
    
    tree.Root = branch[branch.Count - 1];
    tree.Root.IsRoot = true;
    return tree;
}
```

## Experiment

Legend:\
QT = QuickTree\
RN = RapidNJ\
c = Clustal Omega\
k = K-Align\
m = Muscle

Normal files:
|QT c|QT k|QT m|RN c|RN k|RN m|
|---|---|---|---|---|---|
|  0|206|198|259|323| - |
|206|  0|264|313|261| - |
|198|264|  0|309|339| - |
|271|317|309|  0|274| - |
|274|259|323|274|  0| - |
| - | - | - | - | - |  0|

Permuted files:
|QT c|QT k|QT m|RN c|RN k|RN m|
|---|---|---|---|---|---|
|  0|198|158|269| - | - |
|206|  0|224|319| - | - |
|198|224|  0|301| - | - |
|271|323|305|  0| - | - |
| - | - | - | - |  0| - |
| - | - | - | - | - |  0|

Both files:
|QT c|QT k|QT m|RN c|RN k|RN m|
|---|---|---|---|---|---|
| 68|172|158|160| - | - |
