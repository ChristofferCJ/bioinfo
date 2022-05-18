namespace Matches;

using System.Collections.Generic;
using Parities;

public class Matches
{
    List<(int, int)> List;

    public Matches(List<Parity> parities)
    {
        List = GenerateMatches(parities);
    }

    public List<int> GetListForFolding()
    {
        var clone = new List<(int, int)>(List);
        var res = new List<int>();
        foreach(var (l, _) in clone)
        {
            res.Add(l);
        }
        clone.Reverse();
        foreach(var (_, r) in clone)
        {
            res.Add(r);
        }
        return res;
    }

    private List<(int, int)> GenerateMatches(List<Parity> lst)
    {
        var evenToOdd = new List<(int, int)>();
        var l = 0;
        var r = lst.Count - 1;
        while (l < r) // Even to Odd
        {
            if (lst[l] == Parity.None || lst[l] == Parity.Odd)
            {
                l++;
                continue;
            }
            if (lst[r] == Parity.None || lst[r] == Parity.Even)
            {
                r--;
                continue;
            }
            evenToOdd.Add((l, r));
            l++;
            r--;
        }

        var oddToEven = new List<(int, int)>();
        l = 0;
        r = lst.Count - 1;
        while (l < r) // Odd to Even
        {
            if (lst[l] == Parity.None || lst[l] == Parity.Even)
            {
                l++;
                continue;
            }
            if (lst[r] == Parity.None || lst[r] == Parity.Odd)
            {
                r--;
                continue;
            }
            oddToEven.Add((l, r));
            l++;
            r--;
        }
        if (evenToOdd.Count > oddToEven.Count)
            return evenToOdd;
        else
            return oddToEven;
    }
}