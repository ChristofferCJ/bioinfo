from ast import parse
import sys
from util import parse_multiple_fasta, write_to_output, write_to_output_approx
from impl.sp_approx import sp_approx
from impl.sp_exact_3 import sp_exact_3

cost_matrix = [
    [0.0, 5.0, 2.0, 5.0],
    [5.0, 0.0, 5.0, 2.0],
    [2.0, 5.0, 0.0, 5.0],
    [5.0, 2.0, 5.0, 0.0]
]
gap_cost = lambda x: 5.0

def main():
    # fuck recursion
    sys.setrecursionlimit(1000000)

    # exact_costs = []
    # approx_costs = []
    seqs = parse_multiple_fasta('fasta/brca1-full.fasta')
    # exact_cost, exact_alignments = sp_exact_3(
    #     A = seqs[0],
    #     B = seqs[1],
    #     C = seqs[2],
    #     cost = cost_matrix,
    #     gap = gap_cost
    # )
    # exact_costs.append(exact_cost)
    #write_to_output(f'fasta/exact_{file}', exact_cost, exact_alignments, idfk=False)

    approx_cost, approx_alignments = sp_approx(
        F = seqs,
        cost = cost_matrix,
        gap = gap_cost
    )
    # approx_costs.append(approx_cost)
    #write_to_output_approx(f'fasta/approx_{file}', approx_cost, approx_alignments)

    # calculate approximation ratio for each
    # approximation_ratios = []
    # for exact, approx in zip(exact_costs, approx_costs):
    #     approx = exact / approx
    #     approximation_ratios.append(approx)
    
    # for idx, approx in enumerate(approximation_ratios):
    #     print(f'Approximation ratio for seq {10 * idx + 10} is {approx}')
    print(approx_cost)
if __name__ == '__main__':
    main()