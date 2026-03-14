import pure_robot

def transfer(message):
    print(message)


COMMANDS = {'move', 'turn', 'set', 'start', 'stop'}
NO_ARG_COMMANDS = {'start', 'stop'}


def execute(stream, state):
    tokens = stream.split()
    stack = []

    for token in tokens:
        if token not in COMMANDS:
            stack.append(token)
            continue

        if token in NO_ARG_COMMANDS:
            if token == 'start':
                state = pure_robot.start(transfer, state)
            elif token == 'stop':
                state = pure_robot.stop(transfer, state)
        else:
            arg = stack.pop()
            if token == 'move':
                state = pure_robot.move(transfer, int(arg), state)
            elif token == 'turn':
                state = pure_robot.turn(transfer, int(arg), state)
            elif token == 'set':
                state = pure_robot.set_state(transfer, arg, state)

    return state