from typing import Callable, Optional, Tuple
from copy import deepcopy

def global_pairwise_alignment(
    a:                  str,
    b:                  str,
    c:                  list[list[float]],
    gap:                Callable[[int], float],
    opt_method:         str,
    perform_backtrack:  bool = True
    ) -> tuple[float, list[dict[str, str]]]:
    # check if opt_method is valid
    opt_method = opt_method.lower()
    if opt_method not in ['max', 'min']:
        raise Exception(f'Invalid argument to opt_method: {opt_method}')
    # prepend whitespace to both sequences to prevent index errors
    a = ' ' + a
    b = ' ' + b
    # dynamic programming tables
    dp:     list[list[Optional[float]]] = [[None for _ in b] for _ in a]
    dp_del: list[list[Optional[Tuple[float, int]]]] = [[None for _ in b] for _ in a]
    dp_ins: list[list[Optional[Tuple[float, int]]]] = [[None for _ in b] for _ in a]
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

    def compute(i: int, j: int) -> float:
        v = dp[i][j]
        if v is not None:
            return v
        vals: list[Optional[float]] = [None for _ in range(4)]
        if i > 0 and j > 0:
            a_idx = convert(a[i])
            b_idx = convert(b[j])
            vals[0] = compute(i - 1, j - 1) + c[a_idx][b_idx]
        if i > 0 and j >= 0:
            vals[1] = delete(i, j)
        if i >= 0 and j > 0:
            vals[2] = insert(i, j)
        if i == 0 and j == 0:
            vals[3] = 0
        opt = max([v for v in vals if v is not None]) if opt_method == 'max' else min([v for v in vals if v is not None])
        dp[i][j] = opt
        return opt

    def insert(i: int, j: int, len: int = 1) -> float:
        v = dp_ins[i][j]
        if v is not None:
            v, v_len = v
            return v - gap(v_len) + gap(len)
        vals: list[Optional[float]] = [None for _ in range(2)]
        if i >= 0 and j > 0:
            vals[0] = compute(i, j - 1) + gap(len)
        if i >= 0 and j > 1:
            vals[1] = insert(i, j - 1, len + 1)
        opt = max([v for v in vals if v is not None]) if opt_method == 'max' else min([v for v in vals if v is not None])
        dp_ins[i][j] = (opt, len)
        return opt

    def delete(i: int, j: int, len: int = 1) -> float:
        v = dp_del[i][j]
        if v is not None:
            v, v_len = v
            return v - gap(v_len) + gap(len)
        vals: list[Optional[float]] = [None for _ in range(2)]
        if i > 0 and j >= 0:
            vals[0] = compute(i - 1, j) + gap(len)
        if i > 1 and j >= 0:
            vals[1] = delete(i - 1, j, len + 1)
        opt = max([v for v in vals if v is not None]) if opt_method == 'max' else min([v for v in vals if v is not None])
        dp_del[i][j] = (opt, len)
        return opt

    def backtrack(i: int, j: int, k: int, acc: dict[str, str], alignments: list[dict[str, str]], checking: str = 'None') -> list[dict[str, str]]:
        a_idx = convert(a[i]) if i > 0 else -1
        b_idx = convert(b[j]) if j > 0 else -1
        curr = dp[i][j]
        diag = dp[i-1][j-1]
        left = dp[i][j-k]
        up = dp[i-k][j]
        if curr is None or diag is None or left is None or up is None:
            raise Exception('Dynamic programming table is not filled before calling backtrack')
        # base case
        if i == 0 and j == 0:
            return alignments + [acc]
        # composites
        res: list[dict[str, str]] = []
        if i > 0 and j > 0 and k == 1 and curr == (diag + c[a_idx][b_idx]) and checking == 'None':
            copy = deepcopy(acc)
            copy['a'] += a[i]
            copy['b'] += b[j]
            res += (backtrack(i - 1, j - 1, 1, copy, alignments))
        if i >= 0 and j > 0 and curr == (left + gap(k)) and checking != 'Up':
            copy = deepcopy(acc)
            copy['a'] += '-' * k
            copy['b'] += b[j-k+1:j+1]
            res += backtrack(i, j - k, 1, copy, alignments)
        elif j > k and checking != 'Up':
            res += backtrack(i, j, k + 1, acc, alignments, 'Left')
        if i > 0 and j >= 0 and curr == (up + gap(k)) and checking != 'Left':
            copy = deepcopy(acc)
            copy['a'] += a[i-k+1:i+1]
            copy['b'] += '-'
            res += backtrack(i - k, j, 1, copy, alignments)
        elif i > k and checking != 'Left':
            res += backtrack(i, j, k + 1, acc, alignments, 'Up')
        
        return res

    # call cost to get optimal cost of alignment
    cost = compute(len(a) - 1, len(b) - 1)
    # find all possible optimal allignments
    alignments = []
    if perform_backtrack:
        alignments = backtrack(len(a) - 1, len(b) - 1, 1, {'a' : '', 'b' : ''}, [])
    return cost, alignments
