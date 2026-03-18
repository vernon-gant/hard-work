import math
from dataclasses import dataclass
from enum import Enum
from functools import reduce

class DeviceType(Enum):
    WATER = "water"
    SOAP  = "soap"
    BRUSH = "brush"


@dataclass(frozen=True)
class RobotState:
    x: float = 0.0
    y: float = 0.0
    angle: float = 0.0
    device: DeviceType = DeviceType.WATER


def move(state, dist):
    rad = math.radians(state.angle)
    new = RobotState(
        state.x + dist * math.cos(rad),
        state.y + dist * math.sin(rad),
        state.angle,
        state.device,
    )
    return new, f"POS {round(new.x)},{round(new.y)}"


def turn(state, angle):
    a = (state.angle + angle) % 360
    if a > 180: a -= 360
    if a <= -180: a += 360
    new = RobotState(state.x, state.y, a, state.device)
    return new, f"ANGLE {round(a)}"


def set_device(state, name):
    new = RobotState(state.x, state.y, state.angle, DeviceType(name))
    return new, f"STATE {name}"


def start(state, _):
    return state, f"START WITH {state.device.value}"


def stop(state, _):
    return state, "STOP"

COMMANDS = {
    "move":  lambda arg: lambda s: move(s, int(arg)),
    "turn":  lambda arg: lambda s: turn(s, int(arg)),
    "set":   lambda arg: lambda s: set_device(s, arg),
    "start": lambda arg: lambda s: start(s, arg),
    "stop":  lambda arg: lambda s: stop(s, arg),
}


def parse(line):
    parts = line.strip().split(maxsplit=1)
    cmd, arg = parts[0], parts[1] if len(parts) > 1 else ""
    return COMMANDS[cmd](arg)


def step(acc, command):
    state, logs = acc
    new_state, log = command(state)
    return new_state, logs + [log]


def execute_batch(program, initial_state=RobotState()):
    commands = [parse(line) for line in program]
    final_state, logs = reduce(step, commands, (initial_state, []))
    for log in logs:
        print(log)
    return final_state


result = execute_batch([
    "move 100",
    "turn -90",
    "set soap",
    "start",
    "move 50",
    "stop",
])

print(f"\nFinal: x={round(result.x)} y={round(result.y)} angle={round(result.angle)} device={result.device.value}")