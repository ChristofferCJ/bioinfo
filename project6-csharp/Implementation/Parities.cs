namespace Parities;

using System.Collections.Generic;

public enum Parity
{
    Even,
    Odd,
    None
}

public enum Direction
{
    None,
    Left,
    Right
}


public class Parities
{
    private List<Parity> List;

    public Parities(string hp)
    {
        List = GenerateParitiesList(hp);
    }

    public List<Parity> GetList()
    {
        return List;
    }

    private List<Parity> GenerateParitiesList(string hp)
    {
        var list = new List<Parity>();
        for (int idx = 0; idx < hp.Length; idx++)
        {
            var chr = hp[idx];
            if (chr == 'p')
            {
                list.Add(Parity.None);
                continue;
            }
            if (idx % 2 == 0)
                list.Add(Parity.Even);
            else
                list.Add(Parity.Odd);
        }
        return list;
    }
}