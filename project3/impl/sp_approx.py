from typing import Callable
from impl.global_pairwise_alignment import global_pairwise_alignment
import psutil
from time import perf_counter

def sp_approx(
    F:          list[str],
    cost:       list[list[float]],
    gap:        Callable[[int], float],
    alphabet:   str = 'ACGT'
) -> tuple[float, list[str]]:
    mem_before = psutil.Process().memory_info().rss
    time_before = perf_counter()
    # check if cost is a square matrix
    if len(cost[0]) != len(cost):
        raise Exception(f'Cost matrix with dimensions {len(cost[0])}x{len(cost)} is not square.')

    # check if alphabet has duplicates
    if len(set(alphabet)) != len(alphabet):
        raise Exception(f'Alphabet \'{alphabet}\' has duplicates.')

    # # check if alphabet matches square matrix
    # if len(alphabet) != len(cost):
    #     raise Exception(f'Alphabet size ({len(alphabet)}) does not match length ({len(cost)}) of cost matrix.')
    
    # method to find center string
    print('Finding center')
    def find_center() -> tuple[int, str]:
        opt_idx = -1
        opt = -1.0
        for idx, candidate in enumerate(F):
            sum = 0.0
            for other_idx, other in enumerate(F):
                if idx == other_idx: # skip checking against itself, since sum is 0
                    continue
                score, _ = global_pairwise_alignment(
                    a=candidate,
                    b=other,
                    c=cost,
                    gap=gap
                )
                sum += score
                print(f'Finished running global_pairwise_alignemnt for {idx} and {other_idx}')
            if sum < opt or opt_idx == -1:
                opt_idx = idx
                opt = sum
        return opt_idx, F[opt_idx]
    
    center_idx, center = find_center()

    print('Finding alignments with center string')
    # method to find all alignments from center sequence to all other sequences
    def find_alignments_and_cost() -> tuple[float, list[dict[str, str]]]:
        alignments = []
        total_cost = 0.0
        for idx, seq in enumerate(F):
            if idx == center_idx: # skip alignment with itself
                continue
            c, a = global_pairwise_alignment(
                    center,
                    seq,
                    cost,
                    gap,
                )
            print(f'Finished finding alignment for {idx} with center string')
            alignments.append(a[0])
            total_cost += c
        return total_cost, alignments
    
    optimum_cost, alignments = find_alignments_and_cost()
    
    M = [center]
    print('Extending M')
    def extend(M: list[str], seq: dict[str, str]) -> list[str]:
        MA = ['' for _ in range(len(M) + 1)]
        i = 0
        j = 0
        i_max = len(M[0])
        j_max = len(seq['b'])
        while i < i_max and j < j_max:
            c = M[0][i]
            # print(seq)
            # print(j)
            # print(j_max)
            a = seq['a'][j]
            b = seq['b'][j]
            # case 1
            if c == '-' and a == '-':
                for idx, s in enumerate(M):
                    MA[idx] += s[i]
                MA[-1] += b
                i += 1
                j += 1
            # case 2
            elif c == '-' and a != '-':
                for idx, s in enumerate(M):
                    MA[idx] += s[i]
                MA[-1] += '-'
                i += 1
            # case 3
            elif c != '-' and a == '-':
                for idx, _ in enumerate(M):
                    MA[idx] += '-'
                MA[-1] += b
                j += 1
            # case 4
            elif c != '-' and a != '-':
                for idx, s in enumerate(M):
                    MA[idx] += s[i]
                MA[-1] += b
                i += 1
                j += 1

        return MA

    for idx, seq in enumerate(alignments):
        M = extend(M, seq)
        print(f'Finished finding for {idx}')
    mem_after = psutil.Process().memory_info().rss
    time_after = perf_counter()
    mem = mem_after - mem_before
    time = time_after - time_before
    print(f"""
        Running time (sec): {time}
        Memory consumption (bytes): {mem}
    """)
    return optimum_cost, M
