from impl import global_pairwise_alignment
from time import perf_counter


def measure_pair(a, b):
    start = perf_counter()
    # global_pairwise_alignment()
    stop = perf_counter()
    time = stop - start
    size = len(a) + len(b)
    return size, time


def get_data(pairs):
    data = []
    for pair in pairs:
        size, time = measure_pair(pair[0], pair[1])
        data.append((size, time))

