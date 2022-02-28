from impl import global_pairwise_alignment

def parse_fasta(path: str) -> str:
    try:
        file = open(path)
    except Exception as e:
        print(f'Failed to open fasta from path {path}: {e}')
        return ''
    res = ''
    lines = file.readlines()
    for line in lines:
        if line.startswith('>') or line.startswith(';'):
            continue
        res += line.strip().replace(' ', '')
    return res.strip()

def parse_alignment(d: dict[str, str]) -> str:
    a = d['a'][::-1]
    b = d['b'][::-1]
    return f'{a}\n{b}'
    

def case_one():
    cost_matrix = [
        [0, 5, 2, 5],
        [5, 0, 5, 2],
        [2, 5, 0, 5],
        [5, 2, 5, 0]
    ]
    a = 'acgtgtcaacgt'
    b = 'acgtcgtagcta'
    cost, alignments = global_pairwise_alignment(
        a,
        b,
        cost_matrix,
        lambda x: 5*x,
        'min'
    )
    f = open('case_results/case_one.txt', 'w')
    f.write('Results for case_one test case with gap cost g(k) = 5*k:\n')
    f.write(f'Optimal cost of alignment: {cost}\n\n')
    f.write(f'Number of alignments: {len(alignments)}\n')
    f.write('Alignments:\n')
    for alignment in alignments:
        f.write(parse_alignment(alignment) + '\n\n')
    f.write('\n\n')

    cost, alignments = global_pairwise_alignment(
    a,
    b,
    cost_matrix,
    lambda x: 5+5*x,
    'min'
    )
    f.write('Results for case_one test case with gap cost g(k) = 5+5*k:\n')
    f.write(f'Optimal cost of alignment: {cost}\n\n')
    f.write(f'Number of alignments: {len(alignments)}\n')
    f.write('Alignments:\n')
    for alignment in alignments:
        f.write(parse_alignment(alignment) + '\n\n')

def case_two():
    cost_matrix = [
        [0, 5, 2, 5],
        [5, 0, 5, 2],
        [2, 5, 0, 5],
        [5, 2, 5, 0]
    ]
    a = 'aataat'
    b = 'aagg'
    cost, alignments = global_pairwise_alignment(
        a,
        b,
        cost_matrix,
        lambda x: 5*x,
        'min'
    )
    f = open('case_results/case_two.txt', 'w')
    f.write('Results for case_two test case with gap cost g(k) = 5*k:\n')
    f.write(f'Optimal cost of alignment: {cost}\n\n')
    f.write(f'Number of alignments: {len(alignments)}\n')
    f.write('Alignments:\n')
    for alignment in alignments:
        f.write(parse_alignment(alignment) + '\n\n')
    f.write('\n\n')

    cost, alignments = global_pairwise_alignment(
    a,
    b,
    cost_matrix,
    lambda x: 5+5*x,
    'min'
    )
    f.write('Results for case_two test case with gap cost g(k) = 5+5*k:\n')
    f.write(f'Optimal cost of alignment: {cost}\n\n')
    f.write(f'Number of alignments: {len(alignments)}\n')
    f.write('Alignments:\n')
    for alignment in alignments:
        f.write(parse_alignment(alignment) + '\n\n')

def case_three():
    cost_matrix = [
        [0, 5, 2, 5],
        [5, 0, 5, 2],
        [2, 5, 0, 5],
        [5, 2, 5, 0]
    ]
    a = 'tccagaga'
    b = 'tcgat'
    cost, alignments = global_pairwise_alignment(
        a,
        b,
        cost_matrix,
        lambda x: 5*x,
        'min'
    )
    f = open('case_results/case_three.txt', 'w')
    f.write('Results for case_three test case with gap cost g(k) = 5*k:\n')
    f.write(f'Optimal cost of alignment: {cost}\n\n')
    f.write(f'Number of alignments: {len(alignments)}\n')
    f.write('Alignments:\n')
    for alignment in alignments:
        f.write(parse_alignment(alignment) + '\n\n')
    f.write('\n\n')

    cost, alignments = global_pairwise_alignment(
    a,
    b,
    cost_matrix,
    lambda x: 5+5*x,
    'min'
    )
    f.write('Results for case_three test case with gap cost g(k) = 5+5*k:\n')
    f.write(f'Optimal cost of alignment: {cost}\n\n')
    f.write(f'Number of alignments: {len(alignments)}\n')
    f.write('Alignments:\n')
    for alignment in alignments:
        f.write(parse_alignment(alignment) + '\n\n')


def case_four():
    cost_matrix = [
        [0, 5, 2, 5],
        [5, 0, 5, 2],
        [2, 5, 0, 5],
        [5, 2, 5, 0]
    ]
    a = parse_fasta('fasta/case_four_seq1.fasta')
    b = parse_fasta('fasta/case_four_seq2.fasta')
    cost, alignments = global_pairwise_alignment(
        a,
        b,
        cost_matrix,
        lambda x: 5*x,
        'min'
    )
    f = open('case_results/case_four.txt', 'w')
    f.write('Results for case_four test case with gap cost g(k) = 5*k:\n')
    f.write(f'Optimal cost of alignment: {cost}\n\n')
    f.write(f'Number of alignments: {len(alignments)}\n')
    f.write('Alignments:\n')
    for alignment in alignments:
        f.write(parse_alignment(alignment) + '\n\n')
    f.write('\n\n')

    cost, alignments = global_pairwise_alignment(
    a,
    b,
    cost_matrix,
    lambda x: 5+5*x,
    'min'
    )
    f.write('Results for case_four test case with gap cost g(k) = 5+5*k:\n')
    f.write(f'Optimal cost of alignment: {cost}\n\n')
    f.write(f'Number of alignments: {len(alignments)}\n')
    f.write('Alignments:\n')
    for alignment in alignments:
        f.write(parse_alignment(alignment) + '\n\n')

def simple_pair():
    cost_matrix = [
        [10, 2, 5, 2],
        [2, 10, 2, 5],
        [5, 2, 10, 2],
        [2, 5, 2, 10]
    ]
    a = 'AATAAT'
    b = 'AAGG'
    cost, alignments = global_pairwise_alignment(
        a,
        b,
        cost_matrix,
        lambda x: -5*x,
        'max'
    )
    f = open('test_results/simple_pair.txt', 'w')
    f.write('Results for simple_pair test case:\n')
    f.write(f'Optimal cost of alignment: {cost}\n\n')
    f.write(f'Number of alignments: {len(alignments)}\n')
    f.write('Alignments:\n')
    for alignment in alignments:
        f.write(parse_alignment(alignment) + '\n\n')

def fasta_files():
    cost_matrix = [
        [10, 2, 5, 2],
        [2, 10, 2, 5],
        [5, 2, 10, 2],
        [2, 5, 2, 10]
    ]
    a = parse_fasta('fasta/seq1.fasta')
    b = parse_fasta('fasta/seq2.fasta')
    cost, alignments = global_pairwise_alignment(
        a,
        b,
        cost_matrix,
        lambda x: -5*x,
        'max'
    )
    f = open('test_results/fasta_files.txt', 'w')
    f.write('Results for fasta_files test case:\n')
    f.write(f'Optimal cost of alignment: {cost}\n\n')
    f.write(f'Number of alignments: {len(alignments)}\n')
    f.write('Alignments:\n')
    for alignment in alignments:
        f.write(parse_alignment(alignment) + '\n\n')

def example_one():
    cost_matrix = [
        [10, 2, 5, 2],
        [2, 10, 2, 5],
        [5, 2, 10, 2],
        [2, 5, 2, 10]
    ]
    a = 'TCCAGAGA'
    b = 'TCGAT'
    cost, alignments = global_pairwise_alignment(
        a,
        b,
        cost_matrix,
        lambda x: -5*x,
        'max'
    )
    f = open('test_results/example_one.txt', 'w')
    f.write('Results for example_one test case:\n')
    f.write(f'Optimal cost of alignment: {cost}\n\n')
    f.write(f'Number of alignments: {len(alignments)}\n')
    f.write('Alignments:\n')
    for alignment in alignments:
        f.write(parse_alignment(alignment) + '\n\n')

def example_two():
    cost_matrix = [
        [10, 2, 5, 2],
        [2, 10, 2, 5],
        [5, 2, 10, 2],
        [2, 5, 2, 10]
    ]
    a = 'CGTGTCAAGTCT'
    b = 'ACGTCGTAGCTAGG'
    cost, alignments = global_pairwise_alignment(
        a,
        b,
        cost_matrix,
        lambda x: -5*x,
        'max'
    )
    f = open('test_results/example_two.txt', 'w')
    f.write('Results for example_two test case:\n')
    f.write(f'Optimal cost of alignment: {cost}\n\n')
    f.write(f'Number of alignments: {len(alignments)}\n')
    f.write('Alignments:\n')
    for alignment in alignments:
        f.write(parse_alignment(alignment) + '\n\n')

def example_three():
    cost_matrix = [
        [0, 5, 2, 5],
        [5, 0, 5, 2],
        [2, 5, 0, 5],
        [5, 2, 5, 0]
    ]
    a = 'acgtgtcaacgt'
    b = 'acgtcgtagcta'
    cost, alignments = global_pairwise_alignment(
        a,
        b,
        cost_matrix,
        lambda x: 5*x,
        'min'
    )
    f = open('test_results/example_three.txt', 'w')
    f.write('Results for example_two test case:\n')
    f.write(f'Optimal cost of alignment: {cost}\n\n')
    f.write(f'Number of alignments: {len(alignments)}\n')
    f.write('Alignments:\n')
    for alignment in alignments:
        f.write(parse_alignment(alignment) + '\n\n')

def example_four():
    cost_matrix = [
        [0, 5, 2, 5],
        [5, 0, 5, 2],
        [2, 5, 0, 5],
        [5, 2, 5, 0]
    ]
    a = 'acgtgtcaacgt'
    b = 'acgtcgtagcta'
    cost, alignments = global_pairwise_alignment(
        a,
        b,
        cost_matrix,
        lambda x: 5+5*x,
        'min'
    )
    f = open('test_results/example_four.txt', 'w')
    f.write('Results for example_two test case:\n')
    f.write(f'Optimal cost of alignment: {cost}\n\n')
    f.write(f'Number of alignments: {len(alignments)}\n')
    f.write('Alignments:\n')
    for alignment in alignments:
        f.write(parse_alignment(alignment) + '\n\n')

if __name__ == '__main__':
    # simple_pair()
    # fasta_files()
    # example_one()
    # example_two()
    # example_three()
    # example_four()
    case_one()
    case_two()
    case_three()
    case_four()