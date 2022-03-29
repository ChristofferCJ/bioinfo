# Project 3

## Introduction
The goal of this assignment is to implement `sp_exact_3` and `sp_approx`, to compute optimal alignments of 3 sequences, and any number of sequences respectively. `sp_aprox`, as the name suggests, is an approximation algorithm, which ensures a score of alignment that is at most twice as much as the optimal score of alignment. My implementations of these algorithms work as expected, although they are quite slow. Since the complexity of the algortihms are getting somewhat high, it would make more sense to write to algorithms in a compiled language, like Rust, for performance gains.

## Methods
### sp_exact_3
My implementation of `sp_exact_3` follows the outlined algorithm in the slides <a href="https://brightspace.au.dk/content/enforced/53951-LR8255/AiB_F2022_Slides/MSA.pdf">Multiple alignments</a>, pretty much to a T. Testing the algorithm with the input given in `testdata_short.txt` and `testdata_long.txt` gives the expected output. The output from these tests can be seen `here`
### sp_approx
My implementation of `sp_approx` follows the outlined algorithm in the slides <a href="https://brightspace.au.dk/content/enforced/53951-LR8255/AiB_F2022_Slides/SP-MSA-Approx.pdf">Approximating an optimal Sum-of-Pairs multiple alignment</a>. I use my implementation of `global_pairwise_alignment` from Project 1, to calculate the pairwise alignment and cost of 2 sequences in the `sp_approx` implementation. To test the implementation, I have run the same three sequences as in `testdata_long.txt`, and the output can be seen `here`
## Experiments
### Experiment 1
The results of running `sp_exact_3` on the first three sequences in `brca1-testseqs.fasta` can be seen `here`

### Experiment 2
The results of running `sp_approx` on the first five sequences in `brca1-testseqs.fasta` can be seen `here`

### Experiment 3
The individual results of running `sp_exact_3` and `sp_approx` on the sequences in `testseqs.zip` can be seen `here`. \
The approximation ratios for each of these results can be seen in the table below: \
|  Sequence length|Approximation ratio|
|-----------------|-------------------|
|               10| 1.8918918918918919|
|               20| 1.5340909090909092|
|               30| 1.7906976744186047|
|               40| 1.6914893617021276|
|               50| 1.6812227074235808|
|               60| 1.6541353383458646|
|               70| 1.5876923076923077|
|               80| 1.6136986301369862|
|               90| 1.5858585858585859|
|              100| 1.6435406698564594|
|              110| 1.668141592920354 |
|              120| 1.623246492985972 |
|              130| 1.6213768115942029|
|              140| 1.6528497409326426|
|              150| 1.6161137440758293|
|              160| 1.6265060240963856|
|              170| 1.6246575342465754|
|              180| 1.6241234221598877|
|              190| 1.6273062730627306|
|              200| 1.6554621848739495|

From the above table, we would expect an approximation ratio of 4/3, i.e. 1.33. The approximation ratios in the table are quite larger however, roughly around 1.66. 
