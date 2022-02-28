from impl import global_pairwise_alignment
from time import perf_counter
from typing import Callable, Tuple
from util import parse_fasta

def measure_performance(
    a               : str,
    b               : str,
    c               : list[list[int]],
    gap             : Callable[[int], int],
    opt_method      : str) -> Tuple[float, int, int]:
    start = perf_counter()
    _, _ = global_pairwise_alignment(a, b, c, gap, opt_method, False)
    stop = perf_counter()
    time = stop - start
    return (time, len(a), len(b))

def case_one():
    cost_matrix = [
        [0, 5, 2, 5],
        [5, 0, 5, 2],
        [2, 5, 0, 5],
        [5, 2, 5, 0]
    ]
    a = 'acgtgtcaacgt'
    b = 'acgtcgtagcta'
    time, len_a, len_b = measure_performance(
        a,
        b,
        cost_matrix,
        lambda x: 5*x,
        'min'
    )
    f = open('performance_results/case_one.txt', 'w')
    f.write('Performance for case_one test case with gap cost g(k) = 5*k:\n')
    f.write(f'Running time: {time}\nn: {len_a}\nm: {len_b}')

def case_two():
    cost_matrix = [
        [0, 5, 2, 5],
        [5, 0, 5, 2],
        [2, 5, 0, 5],
        [5, 2, 5, 0]
    ]
    a = 'aataat'
    b = 'aagg'
    time, len_a, len_b = measure_performance(
        a,
        b,
        cost_matrix,
        lambda x: 5*x,
        'min'
    )
    f = open('performance_results/case_two.txt', 'w')
    f.write('Performance for case_two test case with gap cost g(k) = 5*k:\n')
    f.write(f'Running time: {time}\nn: {len_a}\nm: {len_b}')

def case_three():
    cost_matrix = [
        [0, 5, 2, 5],
        [5, 0, 5, 2],
        [2, 5, 0, 5],
        [5, 2, 5, 0]
    ]
    a = 'tccagaga'
    b = 'tcgat'
    time, len_a, len_b = measure_performance(
        a,
        b,
        cost_matrix,
        lambda x: 5*x,
        'min'
    )
    f = open('performance_results/case_three.txt', 'w')
    f.write('Performance for case_three test case with gap cost g(k) = 5*k:\n')
    f.write(f'Running time: {time}\nn: {len_a}\nm: {len_b}')


def case_four():
    cost_matrix = [
        [0, 5, 2, 5],
        [5, 0, 5, 2],
        [2, 5, 0, 5],
        [5, 2, 5, 0]
    ]
    a = parse_fasta('fasta/case_four_seq1.fasta')
    b = parse_fasta('fasta/case_four_seq2.fasta')
    time, len_a, len_b = measure_performance(
        a,
        b,
        cost_matrix,
        lambda x: 5*x,
        'min'
    )
    f = open('performance_results/case_four.txt', 'w')
    f.write('Performance for case_four test case with gap cost g(k) = 5*k:\n')
    f.write(f'Running time: {time}\nn: {len_a}\nm: {len_b}')



if __name__ == '__main__':
    case_one()
    case_two()
    case_three()
    case_four()