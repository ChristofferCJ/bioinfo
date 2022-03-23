from typing import Callable
from global_pairwise_alignment import global_pairwise_alignment

def sp_approx(
    F:          list[str],
    cost:       list[list[float]],
    alphabet:   str,
    gap:        Callable[[int], float],
):
    # check if cost is a square matrix
    if len(cost[0]) != len(cost):
        raise Exception(f'Cost matrix with dimensions {len(cost[0])}x{len(cost)} is not square.')

    # check if alphabet has duplicates
    if len(set(alphabet)) != len(alphabet):
        raise Exception(f'Alphabet \'{alphabet}\' has duplicates.')

    # check if alphabet matches square matrix
    if len(alphabet) != len(cost):
        raise Exception(f'Alphabet size ({len(alphabet)}) does not match length ({len(cost)}) of cost matrix.')
    
    # method to find center string
    def find_center() -> tuple[int, str]:
        opt_idx = -1
        opt = -1.0
        for idx, candidate in enumerate(F):
            sum = 0.0
            for other_idx, other in enumerate(F):
                if idx == other_idx: # skip checking against itself, since sum is 0
                    continue
                score, alignments = global_pairwise_alignment(
                    candidate,
                    other,
                    cost,
                    gap,
                    'max', # change when parent method is parameterized with opt method
                    False
                )
                sum += score
            if sum < opt or opt_idx == -1:
                opt_idx = idx
                opt = sum
        return opt_idx, F[opt_idx]
    
    center_idx, center = find_center()

    # method to find all alignments from center sequence to all other sequences
    def find_alignments() -> list[dict[str, str]]:
        res = []
        for idx, seq in enumerate(F):
            if idx == center_idx: # skip alignment with itself
                continue
            _, alignments = global_pairwise_alignment(
                    center,
                    seq,
                    cost,
                    gap,
                    'max', # change when parent method is parameterized with opt method
                    True
                )
            res.append(alignments[0])
        return res
    
    alignments = find_alignments()
    
    M = [center]

    def extend(M: list[str], seq: dict[str, str]) -> list[str]:
        i = 0
        j = 0
