from typing import Optional
from impl import global_pairwise_alignment
from util import parse_fasta
from util import parse_alignment

def evaluate():
    c = [
    [0, 5, 2, 5],
    [5, 0, 5, 2],
    [2, 5, 0, 5],
    [5, 2, 5, 0]
    ]

    question3_costs: list[list[Optional[int]]] = [[None for _ in range(6)] for _ in range(6)]
    question3_alignments: list[list[Optional[dict[str, str]]]] = [[None for _ in range(6)] for _ in range(6)]
    question4_costs: list[list[Optional[int]]] = [[None for _ in range(6)] for _ in range(6)]
    question4_alignments: list[list[Optional[dict[str, str]]]] = [[None for _ in range(6)] for _ in range(6)]

    for i in range(1, 5+1):
        for j in range(1, 5+1):
            a = parse_fasta(f'fasta/eval_seq{i}.fasta')
            b = parse_fasta(f'fasta/eval_seq{j}.fasta')
            # question 3
            cost, alignments = global_pairwise_alignment(
                a,
                b,
                c,
                lambda x: eval('5*x'),
                'min'
            )
            question3_costs[i][j] = cost
            question3_alignments[i][j] = alignments[0]

            # question 4
            cost, alignments = global_pairwise_alignment(
                a,
                b,
                c,
                lambda x: eval('5+5*x'),
                'min'
            )
            question4_costs[i][j] = cost
            question4_alignments[i][j] = alignments[0]

    write_question(question3_costs, question3_alignments, 'eval/question3.txt')
    write_question(question4_costs, question4_alignments, 'eval/question4.txt')


def write_question(costs: list[list[Optional[int]]], alignments: list[list[Optional[dict[str, str]]]], path: str):
    cost_str = 'Optimal cost for alignments:\n'
    for idx, row in enumerate(costs):
        for i, cost in enumerate(row):
            if idx == 0 or i == 0:
                continue
            if cost is None:
                cost_str += '0 '
            else:
                cost_str += f'{cost} '
        cost_str += '\n'
    alignments_str = ''
    for i, row in enumerate(alignments):
        for j, alignment in enumerate(row):
            if alignment is None:
                continue
            alignments_str += f'Alignment for seq{i} and seq{j}:\n'
            alignments_str += parse_alignment(alignment)
            alignments_str += '\n\n'
    try:
        file = open(path, 'w')
    except:
        raise Exception(f'Unable to open path: {path}')
    
    file.write(cost_str)
    file.write('\n')
    file.write(alignments_str)
    file.close()

if __name__ == '__main__':
    evaluate()