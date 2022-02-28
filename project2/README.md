# Project 2

## Introduction
The scope of this project is to implement different types of gap costs for global alignment. I decided to implement a catch-all function, `global_pairwise_alignment`, which is capable of handling all different types of gap costs, including linear, affine and constant. I have implemented the function recursively, both for finding the optimal cost, as well as finding all possible alignments. The function works as intended, but I have not optimized finding the optimal cost to using linear space, using Hirschberg's idea. Additionally, since I have opted for finding all possible optimal alignments, instead of just one, the backtracking part of the function is rather slow.

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
<img src="graphics/affine_from_slides.png" width="50%" height="50%"/> \
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
To test my programs, I have used the examples provided in Project 1 and Project 2. The main reason for using these exampels to verify my program, is that I can compare the output of these examples to the answers given in the examples, to verify the correctness. The way I test these examples can be seen in <a href="https://github.com/ChristofferCJ/bioinfo/blob/main/project2/test.py">here</a>. The test cases provided in *project2_examples.txt* works as expected. The results for all the test cases can be seen <a href="https://github.com/ChristofferCJ/bioinfo/blob/main/project2/test.py">here</a>.