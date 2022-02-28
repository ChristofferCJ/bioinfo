from util import parse_fasta
from impl import global_pairwise_alignment

def main():
    cost_matrix = [ # specify your cost matrix here
        [0, 0, 0, 0],
        [0, 0, 0, 0],
        [0, 0, 0, 0],
        [0, 0, 0, 0]
    ]

    # un-comment these lines if you want to write a sequence here
    #a = ''
    #b = ''

    # un-comment these lines if you want to read from a fasta file (reads from the fasta folder)
    #a = parse_fasta('fasta/[YOUR_FASTA_FILE_HERE]')
    #b = parse_fasta('fasta/[YOUR_FASTA_FILE_HERE]')

    def gap_cost(x):
        return 0 # write your gap cost function here

    perform_backtrack = False # change this to return alignments as well

    output_file = 'output.txt' # change this to specify where to 

    cost, alignments = global_pairwise_alignment(
        a,
        b,
        cost_matrix,
        gap_cost,
        perform_backtrack
    )

if __name__ == '__main__':
    main()