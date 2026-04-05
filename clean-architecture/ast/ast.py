import math
from dataclasses import dataclass
from collections import namedtuple
from enum import Enum

RobotState = namedtuple("RobotState", "x y angle state")

WATER = 1
SOAP = 2
BRUSH = 3


class MoveResponse(Enum):
    OK = "MOVE_OK"
    BARRIER = "HIT_BARRIER"


class SetStateResponse(Enum):
    OK = "STATE_OK"
    NO_WATER = "OUT_OF_WATER"
    NO_SOAP = "OUT_OF_SOAP"


@dataclass(frozen=True)
class MoveInstr:
    distance: float


@dataclass(frozen=True)
class TurnInstr:
    angle: float


@dataclass(frozen=True)
class SetStateInstr:
    mode: int


@dataclass(frozen=True)
class StartInstr:
    pass


@dataclass(frozen=True)
class StopInstr:
    pass


@dataclass
class Node:
    instruction: object
    next: object


@dataclass
class Terminal:
    pass


def check_position(x, y):
    cx = max(0, min(100, x))
    cy = max(0, min(100, y))
    if x == cx and y == cy:
        return cx, cy, MoveResponse.OK
    return cx, cy, MoveResponse.BARRIER


def check_resources(mode):
    if mode == WATER:
        return SetStateResponse.NO_WATER
    if mode == SOAP:
        return SetStateResponse.NO_SOAP
    return SetStateResponse.OK


def apply_move(state, instr):
    rads = state.angle * (math.pi / 180.0)
    cx, cy, result = check_position(
        state.x + instr.distance * math.cos(rads),
        state.y + instr.distance * math.sin(rads),
    )
    new_state = RobotState(cx, cy, state.angle, state.state)
    return new_state, result, f"{result.value} POS({int(cx)},{int(cy)})"


def apply_turn(state, instr):
    new_state = RobotState(state.x, state.y, state.angle + instr.angle, state.state)
    return new_state, None, f"ANGLE {new_state.angle}"


def apply_set_state(state, instr):
    result = check_resources(instr.mode)
    if result == SetStateResponse.OK:
        new_state = RobotState(state.x, state.y, state.angle, instr.mode)
        return new_state, result, f"STATE {instr.mode}"
    return state, result, result.value


def apply_start(state, instr):
    return state, None, f"START WITH {state.state}"


def apply_stop(state, instr):
    return state, None, "STOP"


HANDLERS = {
    MoveInstr: apply_move,
    TurnInstr: apply_turn,
    SetStateInstr: apply_set_state,
    StartInstr: apply_start,
    StopInstr: apply_stop,
}


def interpret(state, node):
    log = []

    while not isinstance(node, Terminal):
        handler = HANDLERS[type(node.instruction)]
        state, result, message = handler(state, node.instruction)
        log.append(message)
        node = node.next(result)

    return state, log


def on_set_state_result(result):
    if result != SetStateResponse.OK:
        return Node(SetStateInstr(BRUSH), lambda _:
        Node(StartInstr(), lambda _:
        Node(StopInstr(), lambda _:
        Terminal())))
    return Node(StartInstr(), lambda _:
    Node(MoveInstr(50), lambda _:
    Node(StopInstr(), lambda _:
    Terminal())))


def on_move_result(result):
    if result == MoveResponse.BARRIER:
        return Node(TurnInstr(180), lambda _:
        Node(MoveInstr(50), lambda _:
        Node(StopInstr(), lambda _:
        Terminal())))
    return Node(SetStateInstr(SOAP), on_set_state_result)


program = Node(MoveInstr(100), lambda _:
Node(TurnInstr(-90), lambda _:
Node(SetStateInstr(SOAP), lambda _:
Node(StartInstr(), lambda _:
Node(MoveInstr(50), lambda _:
Node(StopInstr(), lambda _:
Terminal()))))))
