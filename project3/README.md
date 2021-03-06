# Project 3

## Usage
The program requires `python3.10` or above. \
To use the program, start by cloning this git repository to your computer:
```
git clone https://github.com/ChristofferCJ/bioinfo.git
```
From here, navigate to the `project3` folder in a terminal, and run the following command:
```
# for sp_exact_3
python sp_exact_3.py [ARGS]

# for sp_approx
python sp_approx.py [ARGS]
```
All arguments to the program is of the format `[IDENTIFIER]=[VALUE]`, and all possible identifiers and values are explained below. \
Example usage:
```
python run.py s1=seq1.fasta s2=seq2.fasta c=cases.cost g=5*x opt=min o=results.txt
```
### Identifiers
- `cost` or `c` (required): \
Used to specify the cost matrix. Excepts files that end with '.cost', which are located in the `/cost_matrices` folder. For more information about how to specify .cost files, check out `example.cost` in `/cost_matrices`.\
Example usage:
```
cost=example.cost
```
- `gapcost` or `g` (required): \
Used to specify the gap cost function used for global pairwise alignment. The function should be specified as a python expression, including the variable `x` for linear and affine gap cost functions. \
Example usage:
```
# constant
gapcost=5

# linear
gapcost=5*x

# affine
gapcost=5+5*x
```
- `optimization` or `opt` (required): \
Used to specify whether the global pairwise alignment should minimize or maximise the optimal cost. Accepted values are `min` or `max`. \
- `output` or `o` (optional): \
Used to specify an output file, if the optimal alignments are desired. The output file, with optimal cost as well as alignments, will be placed in the `/output` folder. \
Example usage:
```
output=output.txt
```
#### sp_exact_3
- `seq1` or `s1` (required): \
Used to specify the first sequence to use for global pairwise alignment. The sequence can either be specified as a .fasta file, or given directly as an argument. To specify a .fasta file, put the .fasta file in the `/fasta` folder in the project, and write the name of the fasta file as the value. \
Example usage:
```
# for specifying fasta files
seq1=some_fasta_file.fasta

# for specfying sequences directly
seq1=atcgtgca
```
- `seq2` or `s2` (required): \
Used to specify the second sequence to sue for global pairwise alignemnt. Usage is the same as for `seq1`.
- `seq3` or `s3` (required): \
Used to specify the second sequence to sue for global pairwise alignemnt. Usage is the same as for `seq1`.

#### sp_approx
- `seq` or `s` (required): \
Used to specify the first sequence to use for global pairwise alignment. The sequence can either be specified as a .fasta file, or given directly as an argument. To specify a .fasta file, put the .fasta file in the `/fasta` folder in the project, and write the name of the fasta file as the value. \
Example usage:
```
# for specifying fasta files
seq1=some_fasta_file.fasta

# for specfying sequences directly
seq1=atcgtgca
```
This argument needs to be given once for each sequence to run the program with.

## Introduction
The goal of this assignment is to implement `sp_exact_3` and `sp_approx`, to compute optimal alignments of 3 sequences, and any number of sequences respectively. `sp_aprox`, as the name suggests, is an approximation algorithm, which ensures a score of alignment that is at most twice as much as the optimal score of alignment. My implementations of these algorithms work as expected, although they are quite slow. Since the complexity of the algortihms are getting somewhat high, it would make more sense to write to algorithms in a compiled language, like Rust, for performance gains. When testing the approximation ratio between `sp_exact_3` and `sp_approx` for 3 sequences, I got slightly different results than I expected.

## Methods
### sp_exact_3
My implementation of `sp_exact_3` follows the outlined algorithm in the slides <a href="https://brightspace.au.dk/content/enforced/53951-LR8255/AiB_F2022_Slides/MSA.pdf">Multiple alignments</a>, pretty much to a T. Testing the algorithm with the input given in `testdata_short.txt` and `testdata_long.txt` gives the expected output. The output from these tests can be seen <a href="https://github.com/ChristofferCJ/bioinfo/tree/main/project3/testdata_output">here</a>
### sp_approx
My implementation of `sp_approx` follows the outlined algorithm in the slides <a href="https://brightspace.au.dk/content/enforced/53951-LR8255/AiB_F2022_Slides/SP-MSA-Approx.pdf">Approximating an optimal Sum-of-Pairs multiple alignment</a>. I use my implementation of `global_pairwise_alignment` from Project 1, to calculate the pairwise alignment and cost of 2 sequences in the `sp_approx` implementation. To test the implementation, I have run the same three sequences as in `testdata_long.txt`, and the output can be seen <a href="https://github.com/ChristofferCJ/bioinfo/blob/main/project3/output/sp_approx_testdatalong.txt">here</a>
## Experiments
### Experiment 1
The results of running `sp_exact_3` on the first three sequences in `brca1-testseqs.fasta` can be seen <a href="https://github.com/ChristofferCJ/bioinfo/blob/main/project3/output/sp_approx_testdatalong.txt">here</a>

### Experiment 2
The results of running `sp_approx` on the first five sequences in `brca1-testseqs.fasta` can be seen <a href="https://github.com/ChristofferCJ/bioinfo/blob/main/project3/experiments/sp_approx_experiment2.txt">here</a>

### Experiment 3
The individual results of running `sp_exact_3` and `sp_approx` on the sequences in `testseqs.zip` can be seen <a href="https://github.com/ChristofferCJ/bioinfo/tree/main/project3/experiments/experiment3">here</a>. \
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

From the above table, we would expect an approximation ratio of 4/3, i.e. 1.33. The approximation ratios in the table are quite larger however, roughly around 1.66. This is a bit higher than expected.

### Presentation
To compute the results seen below, I have used my implementation of `sp_approx` for all the different cases. I decided to use `sp_approx` for all cases, even though the first could be handled by `sp_exact_3` because I find it more interesting to compare results for different input to the same algorithm. \
To test the running time and memory consumption of `sp_approx` I have used the `perf_counter` and `psutil` libraries respectively. I measure the currrent time and memory usage at the start of `sp_approx`, and measure it again at the end, to get the running time and memory consumption of the algorithm. \
Running the test cases in the presentation gave the following results: \
| Testcase|Score|    Running time (sec)|Memory consumption (KB)|
|---------|-----|----------------------|-----------------------|
| seqs 1-3|  402|    1.5598689159960486|              10715.136|
| seqs 1-4|  858|    2.9263971250038594|              15859.712|
| seqs 1-5|  897|    4.660215500014601 |              17022.976|
| seqs 1-6| 1357|    6.821916417015018 |              21692.416|
|full seqs|41663|13714.005886792002    |            2376466.432|