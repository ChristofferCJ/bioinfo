namespace Implementation;

using System.Collections.Generic;
using Bio.Phylogenetics;
using System.Linq;
using System.Diagnostics;

public class SaitouNeiNew
{
    private List<List<float>> DistanceMatrix;

    private List<string> Names;

    private HashSet<int> Taxa;

    private List<List<float>> N;

    private List<float?> R;

    private Dictionary<int, Node> DanglingBranches;

    private int K;

    private long TotalRunningTime = 0;

    public SaitouNeiNew(List<string> names, List<List<float>> distanceMatrix)
    {
        var validDistanceMatrix = ValidateDistanceMatrix(distanceMatrix);
        if (!validDistanceMatrix)
            throw new System.ArgumentException("Distance matrix is not valid");
        
        DistanceMatrix = distanceMatrix;
        Names = names;
        Taxa = CreateTaxa();
        DanglingBranches = new();
        K = DistanceMatrix.Count;
        R = InitializeR();
    }

    public Tree ToTree(bool logRunningTime = false)
    {
        Stopwatch stopwatch = new();
        int iteration = 1;
        while (Taxa.Count > 3)
        {
            if(logRunningTime)
            {
                stopwatch.Start();
                System.Console.WriteLine($"Iteration: {iteration}");
                System.Console.WriteLine("--------------------------------------");
            }
            N = ComputeN();
            if(logRunningTime)
            {
                var time = stopwatch.ElapsedMilliseconds;
                LogRunningTime("ComputeN", time);
                stopwatch.Reset();
                stopwatch.Start();
            }
            var (minI, minJ) = FindMinimalEntryInN();
            if(logRunningTime)
            {
                var time = stopwatch.ElapsedMilliseconds;
                LogRunningTime("FindMinimalEntryInN", time);
                stopwatch.Reset();
                stopwatch.Start();
            }
            AddNodeKAndEdges(minI, minJ);
            if(logRunningTime)
            {
                var time = stopwatch.ElapsedMilliseconds;
                LogRunningTime("AddNodeKAndEdges", time);
                stopwatch.Reset();
                stopwatch.Start();
            }
            UpdateDistanceMatrix(minI, minJ);
            if(logRunningTime)
            {
                var time = stopwatch.ElapsedMilliseconds;
                LogRunningTime("UpdateDistanceMatrix", time);
                stopwatch.Reset();
                stopwatch.Start();
            }
            UpdateTaxaSet(minI, minJ);
            if(logRunningTime)
            {
                var time = stopwatch.ElapsedMilliseconds;
                LogRunningTime("UpdateTaxaSet", time);
                stopwatch.Reset();
                System.Console.WriteLine();
            }
            K++;
            iteration++;
            R = ResetR();
        }
        if(logRunningTime)
            stopwatch.Start();
        var root = AddRemainingTaxa();
        if(logRunningTime)
        {
            var time = stopwatch.ElapsedMilliseconds;
            LogRunningTime("AddRemainingTaxa", time);
            stopwatch.Reset();
            System.Console.WriteLine();
        }
        var tree = ConstructTree(root);
        if(logRunningTime)
        {
            var time = stopwatch.ElapsedMilliseconds;
            LogRunningTime("ConstructTree", time);
            stopwatch.Reset();
            System.Console.WriteLine();
        }

        if(logRunningTime)
        {
            if(TotalRunningTime < 20000)
                System.Console.WriteLine($"Total running time: {TotalRunningTime} ms.");
            else
            {
                var timeInSeconds = TotalRunningTime / 1000;
                System.Console.WriteLine($"Total running time: {timeInSeconds.ToString("0.00")} seconds.");
            }
        }
        return tree;
    }

    private HashSet<int> CreateTaxa()
    {
        var taxa = new HashSet<int>();
        for (int taxon = 0; taxon < DistanceMatrix.Count; taxon++)
        {
            taxa.Add(taxon);
        }
        return taxa;
    }

    private bool ValidateDistanceMatrix(List<List<float>> distanceMatrix)
    {
        if (distanceMatrix.Count < 1)
            return false;
        var currentRowCounter = distanceMatrix[0].Count;
        for (int i = 1; i < distanceMatrix.Count; i++)
        {
            var iterRowCounter = distanceMatrix[i].Count;
            if (currentRowCounter != iterRowCounter)
                return false;
        }
        return distanceMatrix.Count == currentRowCounter;
    }

    private List<List<float>> ComputeN()
    {
        List<List<float>> N = CreateN();

        var taxaCoefficient = ComputeTaxaCoefficient();
        for (int i = 0; i < DistanceMatrix.Count; i++)
        {
            if (!Taxa.Contains(i))
                continue;
            var rI = ComputeR(taxaCoefficient, i);
            for (int j = 0; j < DistanceMatrix.Count; j++)
            {
                if(!Taxa.Contains(j) || i == j)
                    continue;
                var rJ = ComputeR(taxaCoefficient, j);
                var nIJ = DistanceMatrix[i][j] - (rI + rJ);
                N[i][j] = nIJ;
            }
        }
        return N;
    }

    private float ComputeR(float taxaCoefficient, int index)
    {
        if(R[index] != null)
            return (float) R[index];
        var sum = 0f;
        foreach(var taxon in Taxa)
        {
            sum +=  DistanceMatrix[index][taxon];
        }
        var res = (float) taxaCoefficient * sum;
        R[index] = res;
        return res;
    }
    
    private float ComputeTaxaCoefficient()
    {
        var taxaSize = Taxa.Count;
        return (float) (1f / (taxaSize - 2));
    }

    private (int, int) FindMinimalEntryInN()
    {
        float best = float.MaxValue;
        int bestI = 0;
        int bestJ = 0;

        for (int i = 0; i < N.Count; i++)
        {
            if (!Taxa.Contains(i))
                continue;
            for (int j = 0; j < N.Count; j++)
            {
                if(!Taxa.Contains(j) || i == j)
                    continue;
                
                if (N[i][j] < best)
                {
                    best = N[i][j];
                    bestI = i;
                    bestJ = j;
                }
            }
        }
        return (bestI, bestJ);
    }

    private void AddNodeKAndEdges(int i, int j)
    {
        var kNode = CreateNode(K);
        var iNode = GetNode(i);
        var jNode = GetNode(j);
        var (iWeight, jWeight) = ComputeWeightForEdge(i, j);
        var iEdge = CreateEdge(iWeight);
        var jEdge = CreateEdge(jWeight);
        kNode.Children.Add(iNode, iEdge);
        kNode.Children.Add(jNode, jEdge);
        DanglingBranches.Add(K, kNode);
    }

    private (float, float) ComputeWeightForEdge(int i, int j)
    {
        var taxaCoefficient = ComputeTaxaCoefficient();
        var rI = ComputeR(taxaCoefficient, i);
        var rJ = ComputeR(taxaCoefficient, j);
        var iWeight = (float) .5f * (DistanceMatrix[i][j] + rI - rJ);
        var jWeight = (float) .5f * (DistanceMatrix[i][j] + rJ - rI);
        return (iWeight, jWeight);
    }

    private Edge CreateEdge(float distance)
    {
        var edge = new Edge();
        edge.Distance = distance;
        return edge;
    }

    private Node GetNode(int index)
    {
        if (DanglingBranches.ContainsKey(index))
            return DanglingBranches[index];
        else
            return CreateNode(index);
    }

    private Node CreateNode(int nameIndex)
    {
        var node = new Node();
        if (Names.Count > nameIndex)
        {
            node.Name = Names[nameIndex];
        }
        return node;
    }

    private void UpdateDistanceMatrix(int i, int j)
    {
       var newRow = new List<float>();
       for (int idx = 0; idx < DistanceMatrix.Count; idx++)
       {
           var entry = (float) .5f * (DistanceMatrix[i][idx] + DistanceMatrix[j][idx] - DistanceMatrix[i][j]);
           newRow.Add(entry);
           DistanceMatrix[idx].Add(entry);
       }
       newRow.Add(0f); // Diagonal
       DistanceMatrix.Add(newRow);
    }

    private void UpdateTaxaSet(int i, int j)
    {
        Taxa.Remove(i);
        Taxa.Remove(j);
        Taxa.Add(K);
    }

    private Node AddRemainingTaxa()
    {
        var taxaList = Taxa.ToList();
        var i = taxaList[0];
        var j = taxaList[1];
        var m = taxaList[2];

        var (iWeight, jWeight, mWeight) = ComputeWeightForRemainingTaxa(i, j, m);
        
        var vNode = CreateNode(int.MaxValue);

        var iNode = GetNode(i);
        var jNode = GetNode(j);
        var mNode = GetNode(m);

        var iEdge = CreateEdge(iWeight);
        var jEdge = CreateEdge(jWeight);
        var mEdge = CreateEdge(mWeight);

        vNode.Children.Add(iNode, iEdge);
        vNode.Children.Add(jNode, jEdge);
        vNode.Children.Add(mNode, mEdge);

        return vNode;
    }

    private (float, float, float) ComputeWeightForRemainingTaxa(int i, int j, int m)
    {
        var iWeight = (float) (DistanceMatrix[i][j] + DistanceMatrix[i][m] - DistanceMatrix[j][m]) / 2f;
        var jWeight = (float) (DistanceMatrix[i][j] + DistanceMatrix[j][m] - DistanceMatrix[i][m]) / 2f;
        var mWeight = (float) (DistanceMatrix[i][m] + DistanceMatrix[j][m] - DistanceMatrix[i][j]) / 2f;
        return (iWeight, jWeight, mWeight);
    }

    private Tree ConstructTree(Node root)
    {
        var tree = new Tree();
        tree.Root = root;
        return tree;
    }

    private void LogRunningTime(string method, long timeInMs)
    {
        var runningTimeInSeconds = (float) (timeInMs) / 1000f;
        if(timeInMs < 10000)
            System.Console.WriteLine($"Method: {method} ran in {timeInMs} ms.");
        else
            System.Console.WriteLine($"Method: {method} ran in {runningTimeInSeconds.ToString("0.00")} seconds.");

        TotalRunningTime += timeInMs;
    }

    private List<float?> InitializeR()
    {
        return ResetR();
    }

    private List<float?> ResetR()
    {
        var R = new List<float?>();
        for (int _ = 0; _ < K; _++)
        {
            R.Add(null);
        }
        return R;
    }

    private List<List<float>> CreateN()
    {
        List<List<float>> N = new();
        for(int i = 0; i < K; i++)
        {
            List<float> row = new();
            for(int j = 0; j < K; j++)
            {
                row.Add(default);
            }
            N.Add(row);
        }
        return N;
    }
}