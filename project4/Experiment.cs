using System;
using System.IO;
using project4.Implementations;
using project4;
using Bio.Phylogenetics;

public class Experiment
{
    public static void Run()
    {
        // Normal files
        RunNormal();
        RunPermuted();
        RunBoth();
    }

    private static void RunNormal()
    {
        System.Console.WriteLine();
        System.Console.WriteLine("Normal files:");
        System.Console.WriteLine("---------------------------------------------");
        // Normal files
        var fileNames = Directory.GetFiles("Data/normal");
        for (int i = 0; i < fileNames.Length; i++)
        {
            fileNames[i] = fileNames[i].Replace("Data/normal/", "");
        }

        for (int i = 0; i < fileNames.Length; i++)
        {
            for (int j = 0; j < fileNames.Length; j++)
            {
                if (i == j) continue;
                Tree treeOne;
                Tree treeTwo;
                try
                {
                    treeOne = NewickParser.FromPath($"Data/normal/{fileNames[i]}");
                }
                catch
                {
                    System.Console.WriteLine($"Failed to parse {fileNames[i]}, skipping.");
                    continue;
                }
                try
                {
                    treeTwo = NewickParser.FromPath($"Data/normal/{fileNames[j]}");
                }
                catch
                {
                    System.Console.WriteLine($"Failed to parse {fileNames[j]}, skipping.");
                    continue;
                }

                var day = new Day();
                var RFDistance = day.RFDistance(treeOne, treeTwo);
                System.Console.WriteLine($"RF Distance for {fileNames[i]} and {fileNames[j]}: {RFDistance}.");
            }
        }
    }

    private static void RunPermuted()
    {
        System.Console.WriteLine();
        System.Console.WriteLine("Permuted files:");
        System.Console.WriteLine("---------------------------------------------");
        // Permuted files
        var fileNames = Directory.GetFiles("Data/permuted");
        for (int i = 0; i < fileNames.Length; i++)
        {
            fileNames[i] = fileNames[i].Replace("Data/permuted/", "");
        }

        for (int i = 0; i < fileNames.Length; i++)
        {
            for (int j = 0; j < fileNames.Length; j++)
            {
                if (i == j) continue;
                Tree treeOne;
                Tree treeTwo;
                try
                {
                    treeOne = NewickParser.FromPath($"Data/permuted/{fileNames[i]}");
                }
                catch
                {
                    System.Console.WriteLine($"Failed to parse {fileNames[i]}, skipping.");
                    continue;
                }
                try
                {
                    treeTwo = NewickParser.FromPath($"Data/permuted/{fileNames[j]}");
                }
                catch
                {
                    System.Console.WriteLine($"Failed to parse {fileNames[j]}, skipping.");
                    continue;
                }

                var day = new Day();
                var RFDistance = day.RFDistance(treeOne, treeTwo);
                System.Console.WriteLine($"RF Distance for {fileNames[i]} and {fileNames[j]}: {RFDistance}.");
            }
        }
    }

    private static void RunBoth()
    {
        System.Console.WriteLine();
        System.Console.WriteLine("Both files:");
        System.Console.WriteLine("---------------------------------------------");
        // Both files
        var fileNames = Directory.GetFiles("Data/permuted");
        for (int i = 0; i < fileNames.Length; i++)
        {
            fileNames[i] = fileNames[i].Replace("Data/permuted/", "");
        }

        for (int i = 0; i < fileNames.Length; i++)
        {
            Tree treeOne;
            Tree treeTwo;
            try
            {
                treeOne = NewickParser.FromPath($"Data/normal/{fileNames[i]}");
            }
            catch
            {
                System.Console.WriteLine($"Failed to parse {fileNames[i]}, skipping.");
                continue;
            }
            try
            {
                treeTwo = NewickParser.FromPath($"Data/permuted/{fileNames[i]}");
            }
            catch
            {
                System.Console.WriteLine($"Failed to parse {fileNames[i]}, skipping.");
                continue;
            }

            var day = new Day();
            var RFDistance = day.RFDistance(treeOne, treeTwo);
            System.Console.WriteLine($"RF Distance for {fileNames[i]} and {fileNames[i]}: {RFDistance}.");
        }
    }
}