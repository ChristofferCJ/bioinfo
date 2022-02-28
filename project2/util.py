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