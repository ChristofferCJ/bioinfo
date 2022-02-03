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

def print_alignment(d: dict[str, str]):
    a = d['a'][::-1]
    b = d['b'][::-1]
    print(f'{a}\n{b}\n')
    

def simple_pair():
    print('Simple pair:')
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
        lambda: -5
    )
    print(f'Optimal cost of alignment: {cost}')
    print(f'Number of alignments: {len(alignments)}')
    print('Alignments:')
    for alignment in alignments:
        print_alignment(alignment)

def fasta_files():
    print('Fasta files:')
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
        lambda: -5
    )
    print(f'Optimal cost of alignment: {cost}')
    print(f'Number of alignments: {len(alignments)}')

def example_one():
    print('Example one:')
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
        lambda: -5
    )
    print(f'Optimal cost of alignment: {cost}')
    print(f'Number of alignments: {len(alignments)}')
    print('Alignments:')
    for alignment in alignments:
        print_alignment(alignment)

def example_two():
    print('Example two:')
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
        lambda: -5
    )
    print(f'Optimal cost of alignment: {cost}')
    print(f'Number of alignments: {len(alignments)}')
    print('Alignments:')
    for alignment in alignments:
        print_alignment(alignment)

if __name__ == '__main__':
    simple_pair()
    fasta_files()
    example_one()
    example_two()