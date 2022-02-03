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
        lambda: -5
    )
    f = open('results/simple_pair.txt', 'w')
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
        lambda: -5
    )
    f = open('results/fasta_files.txt', 'w')
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
        lambda: -5
    )
    f = open('results/example_one.txt', 'w')
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
        lambda: -5
    )
    f = open('results/example_two.txt', 'w')
    f.write('Results for example_two test case:\n')
    f.write(f'Optimal cost of alignment: {cost}\n\n')
    f.write(f'Number of alignments: {len(alignments)}\n')
    f.write('Alignments:\n')
    for alignment in alignments:
        f.write(parse_alignment(alignment) + '\n\n')

if __name__ == '__main__':
    simple_pair()
    fasta_files()
    example_one()
    example_two()