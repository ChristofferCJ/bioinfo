# Project 2

## Introduction
The scope of this project is to implement different types of gap costs for global alignment. I decided to implement a catch-all function, `global_pairwise_alignment`, which is capable of handling all different types of gap costs, including linear, affine and constant. I have implemented the function recursively, both for finding the optimal cost, as well as finding all possible alignments. The function works as intended, but I have not optimized finding the optimal cost to using linear space, using Hirschberg's idea. Additionally, since I have opted for finding all possible optimal alignments, instead of just one, the backtracking part of the function is rather slow.

## How to use
The program requires `python3.10` or above. \
To use the program, start by cloning this git repository to your computer:
```
git clone https://github.com/ChristofferCJ/bioinfo.git
```
From here, navigate to the `project2` folder in a terminal, and run the following command:
```
python run.py [ARGS]
```
All arguments to the program is of the format `[IDENTIFIER]=[VALUE]`, and all possible identifiers and values are explained below. \
Example usage:
```
python run.py s1=seq1.fasta s2=seq2.fasta c=cases.cost g=5*x opt=min o=results.txt
```
### Identifiers
- `seq1` or `s1` (required): \
Used to specify the first sequence to use for global pairwise alignment. The sequence can either be specified as a .fasta file, or given directly as an argument. To specify a .fasta file, put the .fasta file in the `/fasta` folder in the project, and write the name of the fasta file as the value. \
Example usage:
```
# for specifying fasta files
seq1=some_fast_file.fasta

# for specfying sequences directly
seq1=atcgtgca
```
- `seq2` or `s2` (required): \
Used to specify the second sequence to sue for global pairwise alignemnt. Usage is the same as for `seq1`.
- `cost` or `c` (required): \
Used to specify the cost matrix. Excepts files that end with '.cost', which are located in the `/cost_matrices` folder. \
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
## Method
The implementation of `global_pairwise_alignment` can be seen
<a href="https://github.com/ChristofferCJ/bioinfo/blob/main/project2/impl.py">here</a>, which includes the inner functions `compute`, `insert`, `delete` and `backtrack` mentioned throughout this section.
The goal with my implementation was to make it as generic as possible, such that it can be used for all possible cost matrices and gap costs. To allow this, the signature of the function is defined as follows:
```py
def global_pairwise_alignment(
    a:          str,
    b:          str,
    c:          list[list[int]],
    gap:        Callable[[int], int],
    opt_method: str
    ):
```
The interesting part here is `c` and `gap`, which defines the cost matrix and gap cost respective. `c` is defined as a 2D array, allowing for all possible cost matrices. `gap` is defined as a function, taking one parameter of type `int` and returning an `int`. This allows specifying any kind of function of this signature, including constant, linear and affine gap costs.

### Computing optimal cost of alignment
The inner function, `compute`, is used to calculate the optimal cost of alignment, as well as fill out the dynamic programming table for later use. This function is based on the recursion that is presented in the slides <a href="https://brightspace.au.dk/content/enforced/53951-LR8255/AiB_F2022_Slides/AffineGapcost.pdf">Global alignment with general affine gapcost</a>. Keeping the goal of genericness in mind, it is important to notice that handling affine gap costs is a more general problem than handling linear or constant gap costs, implying that the function works for these gap costs as well (in terms of complexity theory, we can say that linear and constant gap cost problems can be reduced to affine gap cost problems). \
To accompany this idea, my implementation differs slightly from the presented solution in the slides. More specifically, the slides states the recursion for insertion and deletion as follows:
<img src="graphics/affine_from_slides.png" width="80%" height="80%"/> \
Here, alpha and beta denotes the scalar and constant for affine gap costs respectively, which is of the form 'alpha * k + beta', where k denotes the length of the gap. What is worth noting here, is that k is implictly incremented by 1 in the last case of both definitions. My implementation expresses this explicitly, to account for different types of gap costs:
```python
if i >= 0 and j > 0:
    vals[0] = compute(i, j - 1) + gap(len)
if i >= 0 and j > 1:
    vals[1] = insert(i, j - 1, len + 1)
```
The inner functions `insert` and `delete` are parameterized with a `len`, which denotes k. When these functions recursively check a step ahead (the last if-statement), len is incremented by 1, following the same idea as the recursion in the slides. An important thing to notice, is that the gap cost in my implementation is only computed in the first if-statement, when calling back to `compute`. If the gap cost was calculated in both if-statements, it would break the algorithm for affine gap costs since the constant beta would be used more than once.

### Backtracking to find optimal alignments
My goal with backtracking was to not only find an optimal alignment, but return all possible optimal alignments for a specific problem. To do this, I followed the idea behind backtracking in the same slides as before, with some slight modifications. Firstly, my implementation of backtracking is recursive, rather than iterative as in the slides. I decided to solve it recursively, because it made it easier for me to find all possible optimal alignments, instead of just an optimal alignment. As such, the first modification was to translate the iterative while loops in the slides to recursive calls to the `backtrack` function. Translating the while loop responsible for checking for deletion- and insertion blocks proved rather cumbersome, as there were a lot of small things to take into account. A snippet of the translation can be seen here:
```python
if i >= 0 and j > 0 and curr == (left + gap(k)) and checking != 'Up':
    copy = deepcopy(acc)
    copy['a'] += '-' * k
    copy['b'] += b[j-k+1:j+1]
    res += backtrack(i, j - k, 1, copy, alignments)
elif j > k and checking != 'Up':
    res += backtrack(i, j, k + 1, acc, alignments, 'Left')
```
The snippet shows how I handled checking for deletions recursively (checking for insertions follows the same idea). The first if-statement checks if the k'th left cell of the current cell is a potential candidate for backtracking (in other words, if the k'th left cell + gap cost of length k is equal to the current cell). If this is the case, I add characters appropriately to our accumulator, and call `backtrack` recursively at the k'th left cell to continue. In the else if-statement, I recursively check the k+1'th left cell for the same property described above, includin a guard to ensure I do not check outside of the dynamic programming table. \
An important thing to notice is the `checking` variable. I use this variable to denote when recursive calls are checking the k+1'th left or upper cell for deletions and insertions respectively. This is done to avoid these recursive calls to also check in the other direction (e.g. recursive left calls also checking up and vice versa). In the code snippet, this is used if the else if-statement, only allowing futher recursive calls to the k+1'th left cell, if the recursive call is not currently checking the k'th upper cell.

## Test
To test my programs, I have used the examples provided in Project 1 and Project 2. The main reason for using these exampels to verify my program, is that I can compare the output of these examples to the answers given in the examples, to verify the correctness. The way I test these examples can be seen in <a href="https://github.com/ChristofferCJ/bioinfo/blob/main/project2/test.py">here</a>. The test cases provided in *project2_examples.txt* works as expected, where I have checked my answers against the ones provided in the text file to verify. The results for all the test cases in *project2_examples.txt* can be seen <a href="https://github.com/ChristofferCJ/bioinfo/tree/main/project2/case_results">here</a>.

## Experiments
To evaluate the running time of my algorithm, in order to compare it to the theoretical running time, I have created a small script, `performance.py`. The script simply runs the algorithm with a given set of sequences, cost matrix and gap cost, and measures the running time of the algortihm, in seconds. I have performed this evaluation on the four different cases given in *project2_examples.txt*. The resulsts can be seen <a href="https://github.com/ChristofferCJ/bioinfo/tree/main/project2/performance_results">here</a>. Since my implementation handles all different types of gap costs, I have created a single graph over the performance resulsts of all four test cases, to check the evaluated running time against the theoretical running time. The theoretical running time of my implementation is O(nm), for inputs of size n and m. \
|        |  n|  m| Running time (ms) |
|--------|---|---|-------------------|
| Case 1 | 12| 12|             0.8625|
| Case 2 |  6|  4|             0.2218|
| Case 3 |  8|  5|             0.2486|
| Case 4 |198|198|           214.0946|

The above table shows the input sizes and running times of the four cases provided in *project2_examples.txt*. From the table, we can extrapolate that the empirical average running time  over the four test cases is f(n, m) = 0.006727 * n * m. This means that the empricial running times is in line with the expected theoretical running times.

## Note
My table is not a graph, which was expected of my. Initally, I wanted to do regression over me experiments, to see how well the empirical running times fit the theoretical ones, but i was not sure how to plot it in a graph, since the running time depends on both `n` and `m` (which would result in a 3D graph, when running time is also added).
