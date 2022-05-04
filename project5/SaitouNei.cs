namespace Implementation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Bio.Phylogenetics;

// https://brightspace.au.dk/content/enforced/53951-LR8255/AiB_F2022_Slides/UPGMA_NJ.pdf

static class SaitouNei
{
    public static Dictionary<int, Node> AvailableNodes = new();

    private static int kCounter = 0;

    private static List<string> Names = new();

    public static Tree ToNewickFormat(List<string> names, List<List<float>> distanceMatrix)
    {
        Names = names;
        var validDistanceMatrix = ValidateDistanceMatrix(distanceMatrix);
        if (!validDistanceMatrix)
            throw new ArgumentException($"{nameof(distanceMatrix)} have to be a square matrix.");
        
        var n = distanceMatrix.Count;
        kCounter = n;
        if (n < 3)
            throw new ArgumentException($"{nameof(distanceMatrix)} has to be a square matrix of at least size 3.");
        var taxaSet = CreateTaxaSet(n);

        var iter = 1;
        var stopwatch = new Stopwatch();
        while (taxaSet.Count > 3)
        {
            stopwatch.Start();  
            System.Console.WriteLine($"Iteration {iter}");
            var preTaxaSetSize = taxaSet.Count;
            var N = ComputeN(distanceMatrix, taxaSet);
            var computeNTime = stopwatch.ElapsedMilliseconds;
            System.Console.WriteLine($"Computed matrix N from distance matrix, took {computeNTime} ms");
            var (minI, minJ) = FindMinimalEntryInN(N, taxaSet);
            var findMinimalEntryInNTime = stopwatch.ElapsedMilliseconds;
            System.Console.WriteLine($"Found minimal entry in N, took {findMinimalEntryInNTime - computeNTime} ms");
            ComputeWeightsAndAddNodes(minI, minJ, distanceMatrix, taxaSet);
            var computeWeightsAndAddNodesTime = stopwatch.ElapsedMilliseconds;
            System.Console.WriteLine($"Computed weights and added nodes, took {computeWeightsAndAddNodesTime - findMinimalEntryInNTime} ms");
            distanceMatrix = UpdateDistanceMatrix(minI, minJ, distanceMatrix);
            var updateDistanceMatrixTime = stopwatch.ElapsedMilliseconds;
            System.Console.WriteLine($"Updated distance matrix, took {updateDistanceMatrixTime - computeWeightsAndAddNodesTime} ms");
            taxaSet = UpdateTaxaSet(taxaSet, minI, minJ, kCounter);
            var updateTaxaSetTime = stopwatch.ElapsedMilliseconds;
            System.Console.WriteLine($"Updated taxa set, took {updateTaxaSetTime - updateDistanceMatrixTime} ms");
            var postTaxaSetSize = taxaSet.Count;
            if (preTaxaSetSize >= postTaxaSetSize)
                System.Console.WriteLine($"preTaxaSetSize: {preTaxaSetSize}, postTaxaSetSize: {postTaxaSetSize}");
            else
                System.Console.WriteLine($"postTaxaSetSize: {postTaxaSetSize}");
            kCounter++;
            iter++;
            stopwatch.Reset();
        }
        var taxaList = taxaSet.ToList();
        if (taxaList.Count != 3)
            throw new Exception($"Remaining number of items in taxa is not 3 at termination stage.");
        var iTaxa = taxaList[0];
        var jTaxa = taxaList[1];
        var mTaxa = taxaList[2];
        var root = AddRemainingTaxasToTree(distanceMatrix, iTaxa, jTaxa, mTaxa);

        var tree = new Tree();
        tree.Root = root;
        Reset();
        return tree;
    }

    private static Node AddRemainingTaxasToTree(
        List<List<float>> distanceMatrix,
        int iTaxa,
        int jTaxa,
        int mTaxa)
    {
        var iWeight = (float) ((distanceMatrix[iTaxa][jTaxa] + distanceMatrix[iTaxa][mTaxa] - distanceMatrix[jTaxa][mTaxa]) / 2);
        var jWeight = (float) ((distanceMatrix[iTaxa][jTaxa] + distanceMatrix[jTaxa][mTaxa] - distanceMatrix[iTaxa][mTaxa]) / 2);
        var mWeight = (float) ((distanceMatrix[iTaxa][mTaxa] + distanceMatrix[jTaxa][mTaxa] - distanceMatrix[iTaxa][jTaxa]) / 2);
        var vNode = new Node();
        vNode.Name = "";
        Node iNode;
        if (AvailableNodes.ContainsKey(iTaxa))
            iNode = AvailableNodes[iTaxa];
        else
        {
            iNode = new Node();
            if(iTaxa < Names.Count())
                iNode.Name = Names[iTaxa];
            else iNode.Name = "";
        }

        Node jNode;
        if (AvailableNodes.ContainsKey(jTaxa))
            jNode = AvailableNodes[jTaxa];
        else
        {
            jNode = new Node();
            if(jTaxa < Names.Count())
                jNode.Name = Names[jTaxa];
            else jNode.Name = "";
        }

        Node mNode;
        if (AvailableNodes.ContainsKey(mTaxa))
            mNode = AvailableNodes[mTaxa];
        else
        {
            mNode = new Node();
            if (mTaxa < Names.Count())
                mNode.Name = Names[mTaxa];
            mNode.Name = "";
        }
        var iEdge = new Edge();
        iEdge.Distance = iWeight;
        var jEdge = new Edge();
        jEdge.Distance = jWeight;
        var mEdge = new Edge();
        mEdge.Distance = mWeight;
        vNode.Children.Add(iNode, iEdge);
        vNode.Children.Add(jNode, jEdge);
        vNode.Children.Add(mNode, mEdge);
        return vNode;
    }

    private static HashSet<int> UpdateTaxaSet(
        HashSet<int> taxaSet,
        int minI,
        int minJ,
        int kCounter)
    {
        var didRemoveI = taxaSet.Remove(minI);
        var didRemoveJ = taxaSet.Remove(minJ);
        taxaSet.Add(kCounter);

        return taxaSet;
    }

    private static List<List<float>> UpdateDistanceMatrix(
        int minI,
        int minJ,
        List<List<float>> distanceMatrix)
    {
        List<float> kRow = new();
        for (int m = 0; m < distanceMatrix.Count; m++)
        {
            var entry = (float) (.5f * (distanceMatrix[minI][m] + distanceMatrix[minJ][m] - distanceMatrix[minI][minJ]));
            distanceMatrix[m].Add(entry);
            kRow.Add(entry);
        }
        kRow.Add(0f); // Diagonal
        distanceMatrix.Add(kRow);

        return distanceMatrix;
    }

    private static void ComputeWeightsAndAddNodes(
        int minI,
        int minJ,
        List<List<float>> distanceMatrix,
        HashSet<int> taxaSet)
    {
        var kNode = new Node();
        kNode.Name = "";
        Node iNode;
        if(AvailableNodes.ContainsKey(minI))
            iNode = AvailableNodes[minI];
        else
        {
            iNode = new Node();
            if (minI < Names.Count())
                iNode.Name = Names[minI];
            else iNode.Name = "";

        }

        Node jNode;
        if (AvailableNodes.ContainsKey(minJ))
            jNode = AvailableNodes[minJ];
        else
        {
            jNode = new Node();
            if (minJ < Names.Count())
                jNode.Name = Names[minJ];
            else jNode.Name = "";
        }
        var fractionCoefficient = CalculateFractionCoefficient(taxaSet);
        var rI = CalculateR(distanceMatrix, taxaSet, minI, fractionCoefficient);
        var rJ = CalculateR(distanceMatrix, taxaSet, minJ, fractionCoefficient);
        var iWeight = (float) (.5 * (distanceMatrix[minI][minJ] + rI - rJ));
        var jWeight = (float) (.5 * (distanceMatrix[minI][minJ] + rJ - rI));
        var iEdge = new Edge();
        var jEdge = new Edge();
        iEdge.Distance = iWeight;
        jEdge.Distance = jWeight;
        kNode.Children.Add(iNode, iEdge);
        kNode.Children.Add(jNode, jEdge);
        AvailableNodes.Add(kCounter, kNode);
    }

    private static bool ValidateDistanceMatrix(List<List<float>> distanceMatrix)
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

    private static HashSet<int> CreateTaxaSet(int n)
    {
        HashSet<int> set = new();
        for (int i = 0; i < n; i++)
        {
            set.Add(i);
        }
        return set;
    }

    private static List<List<float>>   ComputeN(List<List<float>> distanceMatrix, HashSet<int> taxaSet)
    {
        var n = distanceMatrix.Count;
        var fractionCoefficient = CalculateFractionCoefficient(taxaSet);
        var N = InitializeN(n);
        for (int i = 0; i < n; i++)
        {
            var rI = CalculateR(distanceMatrix, taxaSet, i, fractionCoefficient);
            for (int j = 0; j < n; j++)
            {
                if (i == j)
                    continue;
                var dIJ = distanceMatrix[i][j];
                var rJ = CalculateR(distanceMatrix, taxaSet, j, fractionCoefficient);
                var nIJ = dIJ - (rI + rJ);
                N[i][j] = nIJ;
            }
        }
        return N;
    }

    private static float CalculateR(
        List<List<float>> distanceMatrix,
        HashSet<int> taxaSet,
        int idx,
        float fractionCoefficient)
    {
        float taxaSetSum = 0f;
        foreach(var taxa in taxaSet)
        {
            taxaSetSum += distanceMatrix[idx][taxa];
        }
        return (float) (fractionCoefficient * taxaSetSum);
    }

    private static List<List<float>> InitializeN(int n)
    {
        List<List<float>> N = new();
        for (int i = 0; i < n; i++)
        {
            var row = new List<float>();
            for (int j = 0; j < n; j++)
            {
                row.Add(0f);
            }
            N.Add(row);
        }
        return N;
    }

    private static float CalculateFractionCoefficient(HashSet<int> taxaSet)
    {
        var taxaSetSize = taxaSet.Count;
        return (float) (1 / (float)(taxaSetSize - 2));
    }

    private static (int, int) FindMinimalEntryInN(List<List<float>> N, HashSet<int> taxaSet)
    {
        float minEntry = float.MaxValue;
        int minI = 0;
        int minJ = 0;
        for (int i = 0; i < N.Count; i++)
        {
            if (!taxaSet.Contains(i))
                continue;
            for (int j = 0; j < N.Count; j++)
            {
                if (i == j)
                    continue;
                if (!taxaSet.Contains(j))
                    continue;
                if (N[i][j] < minEntry)
                {
                    minEntry = N[i][j];
                    minI = i;
                    minJ = j;
                }
            }
        }
        return (minI, minJ);
    }
    private static void Reset()
    {
        kCounter = 0;
        AvailableNodes = new();
        Names = new();
    }
}