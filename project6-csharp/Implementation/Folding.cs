namespace Folding;

using System;
using System.Text;
using Matches;
using Parities;

public class Folding
{
    private string HP;

    private Parities Parities;

    private Matches Matches;

    public string Fold;

    
    public Folding(string hp)
    {
        hp = ValidateHPString(hp);
        HP = hp;
        Parities = new Parities(hp);
        Matches = new Matches(Parities.GetList());
        Fold = CreateFold();
    }

    private string ValidateHPString(string hp)
    {
        hp = hp.ToLower();
        foreach(var chr in hp)
        {
            if (!(chr == 'h' || chr == 'p'))
                throw new ArgumentException($"Invalid character in HP String: {chr}");
        }
        return hp;
    }

    private string CreateFold()
    {
        var matches = Matches.GetListForFolding();
        var fold = new StringBuilder();

        var hasTurned = false;
        var first = true;
        var prevMatch = 0;
        foreach (var match in matches)
        {
            if (first)
            {
                first = false;
                var add = new string('e', match);
                fold.Append(add);
                prevMatch = match;
                continue;
            }
            var size = match - prevMatch;

            if (size % 2 != 0) // Turn
            {
                hasTurned = true;
                var half = size / 2;

                var left = new string('e', half);
                var turn = "s";
                var right = new string('w', half);
                fold.Append(left);
                fold.Append(turn);
                fold.Append(right);
            }

            if (size % 2 == 0)
            {
                if (hasTurned) // Going left
                {
                    var height = (size / 2) - 1;
                    var down = new string('n', height);
                    var turn = "w";
                    var up = new string('s', height);
                    var end = "w";
                    fold.Append(up);
                    fold.Append(turn);
                    fold.Append(down);
                    fold.Append(end);
                }

                if (!hasTurned) // Going right
                {
                    var height = (size / 2) - 1;
                    var up = new string('s', height);
                    var turn = "e";
                    var down = new string('n', height);
                    var end = "e";
                    fold.Append(down);
                    fold.Append(turn);
                    fold.Append(up);
                    fold.Append(end);
                }
            }

            prevMatch = match;
        }
        var lastFoldsCount = HP.Length - matches[matches.Count - 1] - 1;
        var lastFold = new string('w', lastFoldsCount);
        fold.Append(lastFold);
        var res = fold.ToString();

        return res;
    }
}