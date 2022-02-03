from typing import Callable, Optional
from copy import deepcopy

def global_pairwise_alignment(
    a:      str,
    b:      str,
    c:      list[list[int]],
    gap:    Callable[[], int]
    ):
    # prepend whitespace to both symbols to prevent index errors
    a = ' ' + a
    b = ' ' + b
    # dynamic programming table
    dp: list[list[Optional[int]]] = [[None for _ in b] for _ in a]
    # helper function to convert symbols to indices
    def convert(sym: str) -> int:
        sym = sym.lower()
        lookup = {
            'a' : 0,
            'c' : 1,
            'g' : 2,
            't' : 3
        }
        return lookup[sym]

    def compute(i: int, j: int) -> int:
        v = dp[i][j]
        if v is not None:
            return v
        vals: list[Optional[int]] = [None for _ in range(4)]
        if i > 0 and j > 0:
            a_idx = convert(a[i])
            b_idx = convert(b[j])
            vals[0] = compute(i - 1, j - 1) + c[a_idx][b_idx]
        if i > 0 and j >= 0:
            vals[1] = compute(i - 1, j) + gap()
        if i >= 0 and j > 0:
            vals[2] = compute(i, j - 1) + gap()
        if i == 0 and j == 0:
            vals[3] = 0
        opt = max([v for v in vals if v is not None])
        dp[i][j] = opt
        return opt

    # recursion is hard, i opt for the easy way out
    alignments: list[dict[str, str]] = []
    def backtrack(i: int, j: int, acc: dict[str, str]):
        a_idx = convert(a[i]) if i > 0 else -1
        b_idx = convert(b[j]) if j > 0 else -1
        curr = dp[i][j]
        diag = dp[i-1][j-1]
        left = dp[i][j-1]
        up = dp[i-1][j]
        if curr is None or diag is None or left is None or up is None:
            return
        # base case
        if i == 0 and j == 0:
            alignments.append(acc)
            return
        # composites
        if i > 0 and j > 0 and curr == (diag + c[a_idx][b_idx]):
            copy = deepcopy(acc)
            copy['a'] += a[i]
            copy['b'] += b[j]
            backtrack(i - 1, j - 1, copy)
        if i >= 0 and j > 0 and curr == (left + gap()):
            copy = deepcopy(acc)
            copy['a'] += '-'
            copy['b'] += b[j]
            backtrack(i, j - 1, copy)
        if i > 0 and j >= 0 and curr == (up + gap()):
            copy = deepcopy(acc)
            copy['a'] += a[i]
            copy['b'] += '-'
            backtrack(i - 1, j, copy)
        
    # call cost to get optimal cost of alignment
    cost = compute(len(a) - 1, len(b) - 1)
    # find all possible optimal allignments
    backtrack(len(a) - 1, len(b) - 1, {'a' : '', 'b' : ''})
    return cost, alignments
