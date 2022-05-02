namespace Implementation;

using System;
using System.Collections.Generic;
using System.Linq;

// https://brightspace.au.dk/content/enforced/53951-LR8255/AiB_F2022_Slides/UPGMA_NJ.pdf

static class SaitouNei
{
    public static Dictionary<int, Node> AvailableNodes = new();

    private static int kCounter = 0;

    public static Tree ToNewickFormat(List<List<float>> distanceMatrix)
    {
        var validDistanceMatrix = ValidateDistanceMatrix(distanceMatrix);
        if (!validDistanceMatrix)
            throw new ArgumentException($"{nameof(distanceMatrix)} have to be a square matrix.");
        
        var n = distanceMatrix.Count;
        kCounter = n;
        if (n < 3)
            throw new ArgumentException($"{nameof(distanceMatrix)} has to be a square matrix of at least size 3.");
        var taxaSet = CreateTaxaSet(n);

        while (taxaSet.Count > 3)
        {
            var N = ComputeN(distanceMatrix, taxaSet);
            var (minI, minJ) = FindMinimalEntryInN(N, taxaSet);
            ComputeWeightsAndAddNodes(minI, minJ, distanceMatrix, taxaSet);
            distanceMatrix = UpdateDistanceMatrix(minI, minJ, distanceMatrix);
            taxaSet = UpdateTaxaSet(taxaSet, minI, minJ, kCounter);
            kCounter++;
        }
        var taxaList = taxaSet.ToList();
        if (taxaList.Count != 3)
            throw new Exception($"Remaining number of items in taxa is not 3 at termination stage.");
        var iTaxa = taxaList[0];
        var jTaxa = taxaList[1];
        var mTaxa = taxaList[2];
        var root = AddRemainingTaxasToTree(distanceMatrix, iTaxa, jTaxa, mTaxa);

        var tree = new Tree(root);

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
        var vNode = new Node(kCounter.ToString());
        Node iNode;
        if (AvailableNodes.ContainsKey(iTaxa))
            iNode = AvailableNodes[iTaxa];
        else
            iNode = new Node(iTaxa.ToString());

        Node jNode;
        if (AvailableNodes.ContainsKey(jTaxa))
            jNode = AvailableNodes[jTaxa];
        else
            jNode = new Node(jTaxa.ToString());

        Node mNode;
        if (AvailableNodes.ContainsKey(mTaxa))
            mNode = AvailableNodes[mTaxa];
        else
            mNode = new Node(mTaxa.ToString());
        iNode.Weight = iWeight;
        jNode.Weight = jWeight;
        mNode.Weight = mWeight;
        vNode.AddChild(iNode);
        vNode.AddChild(jNode);
        vNode.AddChild(mNode);

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
        var kNode = new Node(kCounter.ToString());
        Node iNode;
        if(AvailableNodes.ContainsKey(minI))
            iNode = AvailableNodes[minI];
        else
            iNode = new Node(minI.ToString());

        Node jNode;
        if (AvailableNodes.ContainsKey(minJ))
            jNode = AvailableNodes[minJ];
        else
            jNode = new Node(minJ.ToString());
        
        kNode.AddChild(iNode);
        kNode.AddChild(jNode);
        var rI = CalculateR(distanceMatrix, taxaSet, minI);
        var rJ = CalculateR(distanceMatrix, taxaSet, minJ);
        var iWeight = (float) (.5 * (distanceMatrix[minI][minJ] + rI - rJ));
        var jWeight = (float) (.5 * (distanceMatrix[minI][minJ] + rJ - rI));
        iNode.Weight = iWeight;
        jNode.Weight = jWeight;
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

    private static List<List<float>> ComputeN(List<List<float>> distanceMatrix, HashSet<int> taxaSet)
    {
        var n = distanceMatrix.Count;
        var N = InitializeN(n);
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (i == j)
                    continue;
                var dIJ = distanceMatrix[i][j];
                var rI = CalculateR(distanceMatrix, taxaSet, i);
                var rJ = CalculateR(distanceMatrix, taxaSet, j);
                var nIJ = dIJ - (rI + rJ);
                N[i][j] = nIJ;
            }
        }
        return N;
    }

    private static float CalculateR(List<List<float>> distanceMatrix, HashSet<int> taxaSet, int idx)
    {
        var fractionCoefficient = CalculateFractionCoefficient(taxaSet);
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
}