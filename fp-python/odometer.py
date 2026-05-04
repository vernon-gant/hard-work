def odometer(oksana):
    speeds = oksana[0::2]
    times = oksana[1::2]
    durations = (b - a for a, b in zip([0] + times[:-1], times))
    return sum(s * d for s, d in zip(speeds, durations))    