# Project 3

## Introduction
The goal of this assignment is to implement `sp_exact_3` and `sp_approx`, to compute optimal alignments of 3 sequences, and any number of sequences respectively. `sp_aprox`, as the name suggests, is an approximation algorithm, which ensures a score of alignment that is at most twice as much as the optimal score of alignment. My implementations of these algorithms work as expected, although they are quite slow. Since the complexity of the algortihms are getting somewhat high, it would make more sense to write to algorithms in a compiled language, like Rust, for performance gains.

## Methods
### sp_exact_3
My implementation of `sp_exact_3` follows the outlined algorithm in the slides <a href="https://brightspace.au.dk/content/enforced/53951-LR8255/AiB_F2022_Slides/MSA.pdf">Multiple alignments</a>, pretty much to a T. Testing the algorithm with the input given in `testdata_short.txt` and `testdata_long.txt` gives the expected output. The output from these tests can be seen `here`
### sp_approx
My implementation of `sp_approx` follows the outlined algorithm in the slides <a href="https://brightspace.au.dk/content/enforced/53951-LR8255/AiB_F2022_Slides/SP-MSA-Approx.pdf">Approximating an optimal Sum-of-Pairs multiple alignment</a>. I use my implementation of `global_pairwise_alignment` from Project 2, to calculate the pairwise alignment and cost of 2 sequences in the `sp_approx` implementation.
## Experiments
