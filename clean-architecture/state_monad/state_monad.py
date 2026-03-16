from pymonad.state import State
from pure_robot import *

def lift(command_fn, transfer, *args):
    return State(lambda s: (None, command_fn(transfer, *args, s)))

def interpret(transfer, cmd_str):
    parts = cmd_str.split(' ')
    if parts[0] == 'move':    return lift(move, transfer, int(parts[1]))
    elif parts[0] == 'turn':  return lift(turn, transfer, int(parts[1]))
    elif parts[0] == 'set':   return lift(set_state, transfer, parts[1])
    elif parts[0] == 'start': return lift(start, transfer)
    elif parts[0] == 'stop':  return lift(stop, transfer)
    else:                     return State.insert(None)

def make(transfer, commands):
    pipeline = State.insert(None)
    for cmd in commands:
        pipeline = pipeline.bind(lambda _, c=cmd: interpret(transfer, c))
    return pipeline

### Run

initial = RobotState(x=0.0, y=0.0, angle=0.0, state=WATER)

program = make(transfer_to_cleaner, [
    'start',
    'move 100',
    'turn -90',
    'set soap',
    'move 50',
    'stop'
])

result, final_state = program.run(initial)