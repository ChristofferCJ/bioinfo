using System.Diagnostics;
using System.Collections.Generic;

var hpStrings = new List<string>()
{
    "hhppppphhppphppphp",
    "hphphhhppphhhhpphh",
    "phpphphhhphhphhhhh",
    "hphpphhphpphphhpphph",
    "hhhpphphphpphphphpph",
    "hhpphpphpphpphpphpphpphh",
    "pphpphhpppphhpppphhpppphh",
    "ppphhpphhppppphhhhhhhpphhpppphhpphpp",
    "pphpphhpphhppppphhhhhhhhhhpppppphhpphhpphpphhhhh",
    "hhphphphphhhhphppphppphpppphppphppphphhhhphphphphh",
    "pphhhphhhhhhhhppphhhhhhhhhhphppphhhhhhhhhhhhpppphhhhhhphhphp",
    "hhhhhhhhhhhhphphpphhpphhpphpphhpphhpphpphhpphhpphphphhhhhhhhhhhh",
    "hhhhpppphhhhhhhhhhhhpppppphhhhhhhhhhhhppphhhhhhhhhhhhppphhhhhhhhhhhhppphpphhpphhpphph",
    "pppppphphhppppphhhphhhhhphhpppphhpphhphhhhhphhhhhhhhhhphhphhhhhhhppppppppppphhhhhhhpphphhhpppppphphh",
    "ppphhpphhhhpphhhphhphhphhhhpppppppphhhhhhpphhhhhhppppppppphphhphhhhhhhhhhhpphhhphhphpphphhhpppppphhh"
};

var sw = new Stopwatch();
for (int idx = 0; idx < hpStrings.Count; idx++)
{
    var hpString = hpStrings[idx];

    sw.Start();
    var fold = new Folding.Folding(hpString);
    sw.Stop();
    var time = sw.ElapsedTicks;
    sw.Reset();

    System.Console.WriteLine($"Folding {idx + 1}: {fold.Fold}");
    System.Console.WriteLine($"Running time: {time} ticks.");
}