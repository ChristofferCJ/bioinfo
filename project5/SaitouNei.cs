namespace Implementation;

using System;
using System.Collections.Generic;
using Bio.Phylogenetics;
using System.Linq;
using System.Diagnostics;

public class SaitouNei
{
    private List<List<double>> DistanceMatrix;

    private List<string> Names;

    private HashSet<int> Taxa;

    private List<List<double>> N;

    private List<double> R;

    private Dictionary<int, Node> DanglingBranches;

    private int K;

    private long TotalRunningTimeInMs = 0;

    private Dictionary<string, List<long>> RunningTimes;

    public SaitouNei(List<string> names, List<List<double>> distanceMatrix)
    {
        var validDistanceMatrix = ValidateDistanceMatrix(distanceMatrix);
        if (!validDistanceMatrix)
            throw new System.ArgumentException("Distance matrix is not valid");
        
        DistanceMatrix = distanceMatrix;
        Names = names;
        Taxa = CreateTaxa();
        DanglingBranches = new();
        K = DistanceMatrix.Count;
        R = InitializeR(K);
        N = CreateN();
    }

    public Tree ToTree(bool logRunningTime = false)
    {
        Stopwatch stopwatch = new();
        if (logRunningTime)
            RunningTimes = InitializeRunningTimes();

        R = ComputeRFirstTime(K);

        while (Taxa.Count > 3)
        {
            if(logRunningTime)
                stopwatch.Start();
            
            N = ComputeN();
            if(logRunningTime)
            {
                var time = stopwatch.ElapsedTicks;
                var timeInMs = stopwatch.ElapsedMilliseconds;
                LogRunningTime("ComputeN", time, timeInMs);
                stopwatch.Reset();
                stopwatch.Start();
            }
            var (minI, minJ) = FindMinimalEntryInN();
            if(logRunningTime)
            {
                var time = stopwatch.ElapsedTicks;
                var timeInMs = stopwatch.ElapsedMilliseconds;
                LogRunningTime("FindMinimalEntryInN", time, timeInMs);
                stopwatch.Reset();
                stopwatch.Start();
            }
            AddNodeKAndEdges(minI, minJ);
            if(logRunningTime)
            {
                var time = stopwatch.ElapsedTicks;
                var timeInMs = stopwatch.ElapsedMilliseconds;
                LogRunningTime("AddNodeKAndEdges", time, timeInMs);
                stopwatch.Reset();
                stopwatch.Start();
            }
            UpdateDistanceMatrix(minI, minJ);
            if(logRunningTime)
            {
                var time = stopwatch.ElapsedTicks;
                var timeInMs = stopwatch.ElapsedMilliseconds;
                LogRunningTime("UpdateDistanceMatrix", time, timeInMs);
                stopwatch.Reset();
                stopwatch.Start();
            }
            UpdateTaxaSet(minI, minJ);
            if(logRunningTime)
            {
                var time = stopwatch.ElapsedTicks;
                var timeInMs = stopwatch.ElapsedMilliseconds;
                LogRunningTime("UpdateTaxaSet", time, timeInMs);
                stopwatch.Reset();
                stopwatch.Start();
            }

            R = UpdateR(minI, minJ, K);
            if(logRunningTime)
            {
                var time = stopwatch.ElapsedTicks;
                var timeInMs = stopwatch.ElapsedMilliseconds;
                LogRunningTime("UpdateR", time, timeInMs);
                stopwatch.Reset();
            }
            K++;
        }
        if(logRunningTime)
            stopwatch.Start();
        var root = AddRemainingTaxa();
        if(logRunningTime)
        {
            var time = stopwatch.ElapsedTicks;
            var timeInMs = stopwatch.ElapsedMilliseconds;
            LogRunningTime("AddRemainingTaxa", time, timeInMs);
            stopwatch.Reset();
            stopwatch.Start();
        }
        var tree = ConstructTree(root);
        if(logRunningTime)
        {
            var time = stopwatch.ElapsedTicks;
            var timeInMs = stopwatch.ElapsedMilliseconds;
            LogRunningTime("ConstructTree", time, timeInMs);
            stopwatch.Reset();
        }

        if(logRunningTime)
            PrintRunningTimes();

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

    private bool ValidateDistanceMatrix(List<List<double>> distanceMatrix)
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

    private List<List<double>> ComputeN()
    {
        List<List<double>> N = ExtendN();

        for (int i = 0; i < DistanceMatrix.Count; i++)
        {
            if (!Taxa.Contains(i))
                continue;
            var rI = R[i];
            for (int j = 0; j < i; j++)
            {
                if(!Taxa.Contains(j))
                    continue;
                var rJ = R[j];
                var nIJ = DistanceMatrix[i][j] - (rI + rJ);
                N[i][j] = nIJ;
            }
        }
        return N;
    }

    private List<double> ComputeRFirstTime(int k)
    {
        var taxaCoefficient = ComputeTaxaCoefficient(Taxa.Count);
        List<double> R = InitializeR(k);

        foreach(var taxon in Taxa)
            R[taxon] = ComputeR(taxaCoefficient, taxon);

        return R;
    }

    private double ComputeR(double taxaCoefficient, int index)
    {
        double sum = 0;
        foreach(var taxon in Taxa)
        {
            sum +=  DistanceMatrix[index][taxon];
        }
        var res = (double) taxaCoefficient * sum;
        return res;
    }

    private List<double> UpdateR(int removedI, int removedJ, int k)
    {
        List<double> updatedR = InitializeR(k + 1);

        var oldTaxaSize = Taxa.Count + 1;
        var oldTaxaCoefficient = ComputeTaxaCoefficient(oldTaxaSize);
        var newTaxaCoefficient = ComputeTaxaCoefficient(Taxa.Count);

        // Update all existing rows in R
        foreach(var taxon in Taxa)
        {
            if (taxon == k) // Compute k row anew after
                continue;
            var oldR = R[taxon];
            var iFactorToRemove = DistanceMatrix[removedI][taxon] * oldTaxaCoefficient;
            oldR -= iFactorToRemove;
            var jFactorToRemove = DistanceMatrix[removedJ][taxon] * oldTaxaCoefficient;
            oldR -= jFactorToRemove;
            var kFactorToAdd = DistanceMatrix[K][taxon] * oldTaxaCoefficient;
            oldR += kFactorToAdd;
            var updatedRScaled = (double) ((oldR * (oldTaxaSize - 2)) * newTaxaCoefficient);
            updatedR[taxon] = updatedRScaled;
        }
        
        // Add new row corresponding to taxon K
        var kRow = ComputeR(newTaxaCoefficient, k);
        updatedR[k] = kRow;

        return updatedR;
    }
    
    private double ComputeTaxaCoefficient(int taxaSize)
    {
        return (double) (1f / (taxaSize - 2));
    }

    private (int, int) FindMinimalEntryInN()
    {
        double best = double.MaxValue;
        int bestI = 0;
        int bestJ = 0;

        for (int i = 0; i < N.Count; i++)
        {
            if (!Taxa.Contains(i))
                continue;
            for (int j = 0; j < i; j++)
            {
                if(!Taxa.Contains(j))
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

    private (double, double) ComputeWeightForEdge(int i, int j)
    {
        var taxaCoefficient = ComputeTaxaCoefficient(Taxa.Count);
        var rI = R[i];
        var rJ = R[j];
        var iWeight = (double) .5f * (DistanceMatrix[i][j] + rI - rJ);
        var jWeight = (double) .5f * (DistanceMatrix[i][j] + rJ - rI);
        return (iWeight, jWeight);
    }

    private Edge CreateEdge(double distance)
    {
        var edge = new Edge();
        var roundedDistance = Math.Round(distance, 2);
        edge.Distance = (double) roundedDistance;
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
       var newRow = new List<double>();
       for (int idx = 0; idx < DistanceMatrix.Count; idx++)
       {
           var entry = (double) .5f * (DistanceMatrix[i][idx] + DistanceMatrix[j][idx] - DistanceMatrix[i][j]);
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

    private (double, double, double) ComputeWeightForRemainingTaxa(int i, int j, int m)
    {
        var iWeight = (double) (DistanceMatrix[i][j] + DistanceMatrix[i][m] - DistanceMatrix[j][m]) / 2f;
        var jWeight = (double) (DistanceMatrix[i][j] + DistanceMatrix[j][m] - DistanceMatrix[i][m]) / 2f;
        var mWeight = (double) (DistanceMatrix[i][m] + DistanceMatrix[j][m] - DistanceMatrix[i][j]) / 2f;
        return (iWeight, jWeight, mWeight);
    }

    private Tree ConstructTree(Node root)
    {
        var tree = new Tree();
        tree.Root = root;
        return tree;
    }

    private List<double> InitializeR(int k)
    {
        var R = new List<double>();
        for (int _ = 0; _ < k; _++)
        {
            R.Add(default);
        }
        return R;
    }

    private List<List<double>> ExtendN()
    {
        var row = new List<double>();
        for(int i = 0; i < K; i++)
        {
            row.Add(default);
        }
        N.Add(row);
        return N;
    }

    private List<List<double>> CreateN()
    {
        List<List<double>> N = new();
        for (int i = 0; i < K; i++)
        {
            List<double> row = new();
            for (int j = 0; j <= i; j++)
            {
                row.Add(default);
            }
            N.Add(row);
        }

        return N;
    }

    private Dictionary<string, List<long>> InitializeRunningTimes()
    {
        Dictionary<string, List<long>> runningTimes = new();

        runningTimes.Add("ComputeN", new());
        runningTimes.Add("FindMinimalEntryInN", new());
        runningTimes.Add("AddNodeKAndEdges", new());
        runningTimes.Add("UpdateDistanceMatrix", new());
        runningTimes.Add("UpdateTaxaSet", new());
        runningTimes.Add("UpdateR", new());
        runningTimes.Add("AddRemainingTaxa", new());
        runningTimes.Add("ConstructTree", new());

        return runningTimes;
    }

    private void LogRunningTime(string method, long timeInTicks, long timeInMs)
    {
        RunningTimes[method].Add(timeInTicks);
        TotalRunningTimeInMs += timeInMs;
    }

    private void PrintRunningTimes()
    {
        foreach(var (method, runningTimes) in RunningTimes)
        {
            var minTime = runningTimes.Min();
            var maxTime = runningTimes.Max();
            var avgTime = (long) runningTimes.Average();
            System.Console.WriteLine($"Performance of method {method}:");
            System.Console.WriteLine("------------------------------");
            System.Console.WriteLine($"Minimum runnning time: {minTime} ticks");
            System.Console.WriteLine($"Maximum runnning time: {maxTime} ticks");
            System.Console.WriteLine($"Average runnning time: {avgTime} ticks");
            System.Console.WriteLine();
        }
        if (TotalRunningTimeInMs < 10000)
        {
            System.Console.WriteLine($"Total running time: {TotalRunningTimeInMs.ToString()} ms.");;
        }
        else
        {
            var timeInSeconds = TotalRunningTimeInMs / 1000f;
            System.Console.WriteLine($"Total running time: {timeInSeconds.ToString("0.000")} seconds.");
        }
    }
}