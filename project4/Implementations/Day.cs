using System;
using System.Collections.Generic;
using Bio.Phylogenetics;
using System.Linq;
namespace project4.Implementations
{
    public class Day : IImplementation
    {

        private Dictionary<string, int> Ordering;

        public Day()
        {
            Ordering = new();
        }

        public int RFDistance(Tree a, Tree b)
        {
            // Step 1: Make sure that both trees are rooted at the same leaf
            a = RootTreeAtLeaf(a);
            b = ChangeRoot(b, a.Root);

            // Step 2 and 3: Create ordering of leaves from tree a
            CreateOrderingOfLeaves(a);

            // Step 4: Create intervals for both trees
            var aIntervals = FindIntervals(a.Root);
            var bIntervals = FindIntervals(b.Root);

            // Step 5: Compare intervals and count doublets
            var doublets = CompareIntervals(aIntervals, bIntervals);
            var rf = ComputeRF(doublets, aIntervals, bIntervals);
            return rf;
        }

        private int ComputeRF(int doublets, HashSet<List<int>> aIntervals, HashSet<List<int>> bIntervals)
        {
            var aSplits = aIntervals.Count;
            var bSplits = bIntervals.Count;
            return (aSplits + bSplits) - 2 * doublets;
        }

        private void PrintInterval(List<int> interval)
        {
            var stringInterval = interval.Select(v => v.ToString());
            var str = String.Join(',', stringInterval);
            System.Console.WriteLine(str);
        }

        private int CompareIntervals(HashSet<List<int>> a, HashSet<List<int>> b)
        {
            List<string> aStr = new();
            foreach(var interval in a)
            {
                interval.Sort();
                var str = String.Join(',', interval.Select(v => v.ToString()));
                aStr.Add(str);
            }
            List<string> bStr = new();
            foreach(var interval in b)
            {
                interval.Sort();
                var str = String.Join(',', interval.Select(v => v.ToString()));
                bStr.Add(str);
            }

            int doublets = 0;
            foreach(var interval in aStr)
            {
                if(bStr.Contains(interval))
                    doublets++;
            }

            return doublets;
        }

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

        private (bool, List<Node>) GetBranchForNewRoot(Node curr, Node newRoot)
        {
            // Base case
            if (curr.Name == newRoot.Name)
            {
                return (true, new() { curr });
            }

            // Composite case
            foreach (var (node, _) in curr.Children)
            {
                var branch = new List<Node> { curr };
                var (found, tmp) = GetBranchForNewRoot(node, newRoot);
                branch.AddRange(tmp);
                if (found)
                    return (true, branch);
            }
            return (false, new() { curr });
        }

        private HashSet<List<int>> FindIntervals(Node curr)
        {
            var intervals = new HashSet<List<int>>();
            foreach(var (child, _) in curr.Children ?? new Dictionary<Node, Edge>())
            {
                // Internal node
                if(String.IsNullOrEmpty(child.Name))
                {
                    // Add interval for current internal node
                    intervals.Add(FindIntervalForInternalNode(child));
                    intervals.UnionWith(FindIntervals(child));                    
                }
                // Leaf
                else continue;
            }
            return intervals;
        }

        private List<int> FindIntervalForInternalNode(Node node)
        {
            var interval = new List<int>();
            foreach(var (child, _) in node.Children ?? new Dictionary<Node, Edge>())
            {
                // Internal sub-node
                if(String.IsNullOrEmpty(child.Name))
                {
                    interval.AddRange(FindIntervalForInternalNode(child));
                }
                // Leaf
                else
                {
                    var trimmedName = child.Name.Replace("'", "").Trim();
                    var order = Ordering[trimmedName];
                    interval.Add(order);
                }
            }
            return interval;
        }
    }
}
