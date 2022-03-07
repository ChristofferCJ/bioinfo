def parse_fasta(path: str) -> str:
    try:
        file = open(path)
    except:
        raise Exception(f'Failed to open fasta from path {path}')
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
    return f'>seq1\n{a}\n\n>seq2\n{b}\n{"-" * len(a)}'

def valid_sequence(seq: str) -> bool:
    seq_set = set([s.lower() for s in seq])
    for char in seq_set:
        if char not in ['a', 't', 'g', 'c']:
            raise Exception(f'Invalid character in sequence: {char}')
    return True

def parse_cost(path: str) -> list[list[int]]:
    try:
        file = open(path, 'r')
    except Exception:
        raise Exception(f'Unable to open cost file {path}')
    lines = file.readlines()
    res: list[list[int]] = []
    for line in lines:
        if line.startswith('#'):
            continue
        row = [int(num) for num in line.strip().split(' ')]
        res.append(row)
    return res

def write_to_output(path: str, cost: int, alignments: list[dict[str, str]]):
    try:
        file = open(path, 'w')
    except:
        raise Exception(f'Unable to open file from path {path}')
    file.write(f'Optimal cost of alignments: {cost}\n\n')
    file.write(f'Number of alignments: {len(alignments)}\n\n')
    file.write('Alignments:\n')
    for alignment in alignments:
        text = parse_alignment(alignment)
        file.write(text + '\n\n')
    