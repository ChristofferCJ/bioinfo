# Project 5

## How to run
`dotnet` version 6 is required to run this program.\
To run this program, place the phylip file you would like to parse into the `PhylipFiles` folder in the project. Then, refer to the file in `Program.cs` by writing the name of the file on line 4. To specify an output file, write the name of the output file on line 11 of `Program.cs`.
## Introduction
This project is involved with making an efficient implementation of a NJ algorithm, namely the Saitou and Nei algorithm. The project benchmarks this implementation against other NJ algortihms, namely QuickTree and RapidNJ.

## Status of work
The same error that I had from project 4 is still not resolved, namely the Newick file parser that I am using, reports errors on some of the Newick files when trying to parse it into a tree structure. More precisely, the formatter gives an error about a missing ',' in the Newick file, although when I try to parse the same file in this online formatter http://etetoolkit.org/treeview/, the tree is parsed without errors. I suspect that the parser I use have an error, since the library is deprecated, and have not been developed on for a long time.

I have included the RF-distances that I have been able to compute with the Newick parser, and left the rest out - if I manage to get the parser to work, I will update this report wit hthe rest of the RF-distances.

## Optimizations
My first approach to implementing the Saitou and Nei algorithm, was to just write the algorithm out as closely to the way it is described in the slides given, without optimizations. This was to first ensure that my implementation worked, before starting to optimize it. After my implementation worked, the running time of `1849_FG-Gap.phy` was roughly 450 seconds, and once I compared it to the running time of QuickTree and RapidNJ, I saw that a lot of optmization could be done. The first major optimization I made, was that i was computing the entrance `r_i` and `r_j` in every iteration of the double for loop, when computing `N`. I refactored this to compute a list of these values , `R`, ahead of time. This brought the running time of `1849_FG-Gap.phy` down to about 200 seconds, a big improvement. However, I figured that more optimization on `R` could be done, since it is computed in O(n^2) time at every iteration. I brought this down to O(n) time with the following algorithm:

```c#
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
```

Instead of re-computing `R` at every iteration, I found a way to update `R` based on the values of `R` from the previous iteration. More precisely, every entry in the new `R` had to simply remove the factor that taxon `i` and `j` contributed with, add the factor of the new taxon `k`, and make sure the number scaled to the new coeffecient. This brought the running time of `1849_FG-Gap.phy` down to about 110 seconds, another big improvement.

From here, I had some other small optmizations, such as only computing half of `N` (one 'corner', if you will), and then in turn only searching in thie 'corner' when looking for a minimal entry. Other smaller optimizations had also been made, that I will not mention here. All in all, the optimizations brought the running time of `1849_FG-Gap.phy` from 450 seconds to about 35 seconds.

## Experiments
All experiments was run on my desktop pc, with the following specs:\
CPU: Ryzen 5 2600 @ 3.4 GHz\
RAM: 16 GB 2666 MHz\
OS: Manjaro (Linux)\
The Saitou and Nei algorithm was implemented in C#.

|File length|QuickTree (ms) |RapidNJ (ms)|SaitouNei (ms)|QuickTree / SaitouNei|RapidNJ / SaitouNei|QuickTree - RapidNJ RF Distance|RapidNJ - SaitouNei RF Distance|QuickTree - SaitouNei RF Distance|
|----|------|------|-----|-----|---|---|---|---|
|  89|   2.1|   2.9|    4| 1.90|2.11| - | - | 16|
| 214|   9.2|  8.06|   57| 6.20|7.07| - | - | 18|
| 304|  20.0| 15.22|  174| 8.70|11.43| - | - | 22|
| 401|  36.6| 24.75|  352| 9.62|14.22| - | - | 34|
| 494|  45.3| 30.72|  498|11.00|16.21| - | - |260|
| 608|  82.5| 60.33| 1004|12.17|16.64| - | - | 14|
| 777| 168.7| 81.58| 2051|12.16|25.14| - | - |242|
| 877| 258.3|118.28| 3430|13.28|29.00| - | - | 24|
|1347|1170.0|314.94|17126|14.64|54.38| - | - |  2|
|1493|1360.0|362.99|19432|14.29|53.53| - | - | 30|
|1560|1600.0|368.51|21800|13.63|59.16| - | - | 88|
|1689|1840.0|462.16|27225|14.80|58.91| - | - | 42|
|1756|2020.0|525.52|31034|15.36|59.05| - | - | 40|
|1849|2610.0|512.91|37576|14.40|73.26| - | - |116|

As seen from the experiments above, my implementation is not faster than either QuickTree or RapidNJ. Dwelling on why this could be, both the QuickTree and RapidNJ program is implemented in C++, and compiled to machine code and run directly on the CPU. In contrast, C# is compiled into bytecode and interpreted by the dotNET runtime when executing, which could produce an overhead compared to C++.