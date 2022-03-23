import sys
from project3.util import parse_fasta, valid_sequence, parse_cost, write_to_output_approx
from project3.impl.sp_approx import sp_approx
from typing import Any

args = {
    'seq': []
}

def run():
    print('Parsing values...')
    for arg in sys.argv[1:]:
        parse_arg(arg)
    validate_args(args)
    output_specified = 'output' in args.keys()
    print('Computing optimal cost and alignments...')
    cost, alignments = sp_approx(
        F=args['seq'],
        cost=args['cost'],
        gap=args['gapcost']
    )
    if output_specified:
        write_to_output_approx('output/sp_approx_' + args['output'], cost, alignments)
    else:
        print(f'Optimal cost of alignment: {cost}')



def parse_arg(arg: str):
    [id, val] = arg.split('=')
    id = id.strip()
    val = val.strip()
    match id:
        case 'seq' | 's':
            if val.endswith('.fasta'):
                fasta = parse_fasta('fasta/' + val)
                args['seq'].append(fasta)
            else:
                if valid_sequence(val):
                    args['seq'].append(val)
        case 'cost' | 'c':
            if not val.endswith('.cost'):
                raise Exception(f'Invalid cost file: {val}')
            cost = parse_cost('cost_matrices/' + val)
            args['cost'] = cost
        case 'gapcost' | 'g':
            if 'x' not in val:
                raise Exception(f'Expression missing variable x: {val}')
            # test if expression is valid
            try:
                x = 1
                eval(val)
            except:
                raise Exception(f'Invalid expression: {val}')
            args['gapcost'] = lambda x: eval(val)
        case 'output' | 'o':
            args['output'] = val
        case 'optimization' | 'opt':
            if val.lower() not in ['min', 'max']:
                raise Exception(f'Invalid optimization method: {val}')
            args['optimization'] = val.lower()
        case _:
            raise Exception(f'Unknown identifier: {id}')

def validate_args(args: dict[str, Any]):
    if args['seq'] == []:
        raise Exception('Missing argument to program: seq')
    if not 'cost' in args.keys():
        raise Exception('Missing argument to program: cost')
    if not 'gapcost' in args.keys():
        raise Exception('Missing argument to program: gapcost')

if __name__ == '__main__':
    run()