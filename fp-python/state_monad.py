from dataclasses import dataclass, field, replace
from typing import Callable, Dict, Tuple

from pymonad.state import State

# The only interesting task I could get from claude. I know, probably we could add here the Either or Maybe to avoid this ugly @instruction decorator, but still works quite nice.
# I have never even tried to implement my own interpreter, but this micro vesion makes me fell not that nervous when thinking about how it works :)


@dataclass(frozen=True)
class MachineState:
    stack: Tuple[int, ...] = ()
    registers: Dict[str, int] = field(default_factory=dict)
    pc: int = 0
    trace: Tuple[Tuple[int, str], ...] = ()


@dataclass(frozen=True)
class Continue:
    pass


@dataclass(frozen=True)
class Error:
    message: str
    pc: int


@dataclass(frozen=True)
class Halted:
    pass


ExecutionStatus = Continue | Error | Halted


def instruction(body: Callable[[MachineState], Tuple[ExecutionStatus, MachineState]]):
    def outer(status: ExecutionStatus) -> State:
        def state_fn(machine: MachineState):
            if not isinstance(status, Continue):
                return status, machine
            return body(machine)

        return State(state_fn)

    return outer


def push(n: int):
    @instruction
    def body(machine: MachineState):
        new_trace = machine.trace + ((machine.pc, "PUSH"),)
        new_pc = machine.pc + 1
        return Continue(), replace(
            machine, pc=new_pc, stack=machine.stack + (n,), trace=new_trace
        )

    return body


def pop():
    @instruction
    def body(machine: MachineState):
        if len(machine.stack) == 0:
            return Error("Empty stack", machine.pc), machine

        new_trace = machine.trace + ((machine.pc, "POP"),)
        new_pc = machine.pc + 1
        return Continue(), replace(
            machine, pc=new_pc, stack=machine.stack[:-1], trace=new_trace
        )

    return body


def add():
    @instruction
    def body(machine: MachineState):
        if len(machine.stack) < 2:
            return Error("Not enough elements", machine.pc), machine

        top = machine.stack[-1]
        second = machine.stack[-2]
        new_trace = machine.trace + ((machine.pc, "ADD"),)
        new_pc = machine.pc + 1
        return Continue(), replace(
            machine,
            pc=new_pc,
            stack=machine.stack[:-2] + (second + top,),
            trace=new_trace,
        )

    return body


def start() -> State:
    return State.insert(Continue())


initial = MachineState()

pipeline = (
    start()
    .then(push(3))
    .then(push(4))
    .then(add())
    .then(push(2))
    .then(pop())
    .then(add())
    .then(push(99))
)
