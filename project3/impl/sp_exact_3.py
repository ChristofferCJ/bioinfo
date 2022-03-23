from typing import Callable, Optional
from copy import deepcopy

def sp_exact_3(
    A:          str,
    B:          str,
    C:          str,
    cost:       list[list[float]],
    alphabet:   str,
    gap:        Callable[[int], float],
):
    # check if cost is a square matrix
    if len(cost[0]) != len(cost):
        raise Exception(f'Cost matrix with dimensions {len(cost[0])}x{len(cost)} is not square.')

    # check if alphabet matches square matrix
    if len(alphabet) != len(cost):
        raise Exception(f'Alphabet size ({len(alphabet)}) does not match length ({len(cost)}) of cost matrix.')

    # convert to lower case for simplicity
    A = A.lower()
    B = B.lower()
    C = C.lower()
    alphabet = alphabet.lower()

    # check if alphabet has duplicates
    if len(set(alphabet)) != len(alphabet):
        raise Exception(f'Alphabet \'{alphabet}\' has duplicates.')

    # prepend whitespace to sequences to avoid index errors
    A = ' ' + A
    B = ' ' + B
    C = ' ' + C

    # create dynamic programming table
    dp: list[list[list[Optional[float]]]] = []
    for _ in range(len(A)):
        entry: list[list[Optional[float]]] = []
        for _ in range(len(B)):
            row: list[Optional[float]] = []
            for _ in range(len(C)):
                row.append(None)
            entry.append(row)
        dp.append(entry)

    # helper function to convert symbols to indices for cost matrix
    def convert(sym: str) -> int:
        if sym not in alphabet:
            raise Exception(f'Unexpected symbol in sequence (not part of alphabet): {sym}')
        return alphabet.index(sym)

    def sum_of_pairs(a: str, b: str, c: str) -> float:
        # gap + gap
        if (a == '-' and b == '-') or (a == '-' and c == '-') or (b == '-' and c == '-'):
            return gap(1) + gap (1)
        # gap + gap + cost(b, c)
        if a == '-':
            b_idx = convert(b)
            c_idx = convert(c)
            return gap(1) + gap(1) + cost[b_idx][c_idx]
        # gap + gap + cost(a, b)
        if c == '-':
            a_idx = convert(a)
            b_idx = convert(b)
            return gap(1) + gap(1) + cost[a_idx][b_idx]
        # gap + gap + cost(a, c)
        if b == '-':
            a_idx = convert(a)
            c_idx = convert(c)
            return gap(1) + gap(1) + cost[a_idx][c_idx]
        # cost(a, b) + cost(a, c) + cost(b, c)
        a_idx = convert(a)
        b_idx = convert(b)
        c_idx = convert(c)
        return cost[a_idx][b_idx] + cost[a_idx][b_idx] + cost[b_idx][c_idx]
    
    def compute(i: int, j: int, k: int) -> float:
        # check dynamic programming table, and return if value is found
        v = dp[i][j][k]
        if v is not None:
            return v
        vals: list[float] = []
        a = A[i]
        b = B[j]
        c = C[k]
        # cases
        if i == 0 and j == 0 and k == 0:
            vals.append(0.0)
        if i > 0 and j > 0 and k > 0:
            val = compute(i-1, j-1, k-1) + sum_of_pairs(a, b, c)
            vals.append(val)
        if i > 0 and j > 0 and k >= 0:
            val = compute(i-1, j-1, k) + sum_of_pairs(a, b, '-')
            vals.append(val)
        if i > 0 and j >= 0 and k > 0:
            val = compute(i-1, k, k-1) + sum_of_pairs(a, '-', c)
            vals.append(val)
        if i >= 0 and j > 0 and k > 0:
            val = compute(i, j-1, k-1) + sum_of_pairs('-', b, c)
            vals.append(val)
        if i > 0 and j >= 0 and k >= 0:
            val = compute(i-1, j, k) + sum_of_pairs(a, '-', '-')
            vals.append(val)
        if i >= 0 and j > 0 and k >= 0:
            val = compute(i, j-1, k) + sum_of_pairs('-', b, '-')
            vals.append(val)
        if i >= 0 and j >= 0 and k > 0:
            val = compute(i, j, k-1) + sum_of_pairs('-', '-', c)
            vals.append(val)
        optimum = min(vals)
        dp[i][j][k] = optimum
        return optimum

    def backtrack(i: int, j: int, k: int, acc: dict[str, str], alignments: list[dict[str, str]]) -> list[dict[str, str]]:
        # check if dp table is filled out
        for matrix in dp:
            for row in matrix:
                for item in row:
                    assert item is not None
        # base case
        if i == 0 and j == 0 and k == 0:
            return alignments + [acc]
        # composites
        res: list[dict[str, str]] = []
        if i > 0 and j > 0 and k > 0 and dp[i][j][k] == (dp[i-1][j-1][k-1] + sum_of_pairs(A[i], B[j], C[k])):
            copy = deepcopy(acc)
            copy['A'] += A[i]
            copy['B'] += B[j]
            copy['C'] += C[k]
            res += backtrack(i - 1, j - 1, k - 1, copy, alignments)
        if i > 0 and j > 0 and k >= 0 and dp[i][j][k] == (dp[i-1][j-1][k] + sum_of_pairs(A[i], B[j], '-')):
            copy = deepcopy(acc)
            copy['A'] += A[i]
            copy['B'] += B[j]
            copy['C'] += '-'
            res += backtrack(i - 1, j - 1, k, copy, alignments)
        if i > 0 and j >= 0 and k > 0 and dp[i][j][k] == (dp[i-1][j][k-1] + sum_of_pairs(A[i], '-', C[k])):
            copy = deepcopy(acc)
            copy['A'] += A[i]
            copy['B'] += '-'
            copy['C'] += C[k]
            res += backtrack(i - 1, j, k - 1, copy, alignments)
        if i >= 0 and j > 0 and k > 0 and dp[i][j][k] == (dp[i][j-1][k-1] + sum_of_pairs('-', B[j], C[k])):
            copy = deepcopy(acc)
            copy['A'] += '-'
            copy['B'] += B[j]
            copy['C'] += C[k]
            res += backtrack(i, j - 1, k - 1, copy, alignments)
        if i > 0 and j >= 0 and k >= 0 and dp[i][j][k] == (dp[i-1][j][k] + sum_of_pairs(A[i], '-', '-')):
            copy = deepcopy(acc)
            copy['A'] += A[i]
            copy['B'] += '-'
            copy['C'] += '-'
            res += backtrack(i - 1, j, k, copy, alignments)
        if i >= 0 and j > 0 and k >= 0 and dp[i][j][k] == (dp[i][j-1][k] + sum_of_pairs('-', B[j], '-')):
            copy = deepcopy(acc)
            copy['A'] += '-'
            copy['B'] += B[j]
            copy['C'] += '-'
            res += backtrack(i, j - 1, k, copy, alignments)
        if i >= 0 and j >= 0 and k > 0 and dp[i][j][k] == (dp[i][j][k-1] + sum_of_pairs('-', '-', C[k])):
            copy = deepcopy(acc)
            copy['A'] += '-'
            copy['B'] += B[j]
            copy['C'] += '-'
            res += backtrack(i, j - 1, k, copy, alignments)

        return res
    
    # compute optimal score and fill out dynamic programming table
    compute(len(A) - 1, len(B) - 1, len(C) - 1)