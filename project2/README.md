# Project 2

## Introduction
The scope of this project is to implement different types of gap costs for global alignment. I decided to implement a catch-all function, `global_pairwise_alignment`, which is capable of handling all different types of gap costs, including linear, affine and constant. I have implemented the function recursively, both for finding the optimal cost, as well as finding all possible alignments. The function works as intended, but I have not optimized finding the optimal cost to using linear space, using Hirschberg's idea. Additionally, since I have opted for finding all possible optimal alignments, instead of just one, the backtracking part of the function is rather slow.

## Method
The implementation of `global_pairwise_alignment` can be seen
<a href="https://en.wikipedia.org/wiki/Hobbit#Lifestyle" title="Hobbit lifestyles">here</a>