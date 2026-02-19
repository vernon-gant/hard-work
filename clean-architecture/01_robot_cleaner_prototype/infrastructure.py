from __future__ import annotations

from abc import ABC, abstractmethod
from dataclasses import dataclass
from typing import Generic, TypeVar

from mediatr import Mediator
from result import Result, Ok, Err

from value_objects import NonEmptyString
from commands import (
    MoveRequest, TurnRequest, SetRequest, StartRequest, StopRequest,
)
from command_parameters import (
    ParameterFactory, NoParameterFactory,
    MoveParameterFactory, TurnParameterFactory, SetParameterFactory,
)

TRequest = TypeVar("TRequest")


class IRobotCommand(ABC):
    @abstractmethod
    def works_with(self, command: NonEmptyString) -> bool: ...

    @abstractmethod
    def execute(self, parameter: str) -> Result[None, str]: ...


class GenericCommand(IRobotCommand, Generic[TRequest]):
    def __init__(self,mediator: Mediator,parameter_factory: ParameterFactory[TRequest],command_name: NonEmptyString,) -> None:
        self._mediator = mediator
        self._factory = parameter_factory
        self._name = command_name

    def works_with(self, command: NonEmptyString) -> bool:
        return command.value.casefold() == self._name.value.casefold()

    def execute(self, parameter: str) -> Result[None, str]:
        return (
            self._factory
            .create(parameter)
            .map(lambda req: self._mediator.send(req))
        )


class MoveCommand(GenericCommand[MoveRequest]):
    def __init__(self, m: Mediator, f: MoveParameterFactory) -> None:
        super().__init__(m, f, NonEmptyString("move"))


class TurnCommand(GenericCommand[TurnRequest]):
    def __init__(self, m: Mediator, f: TurnParameterFactory) -> None:
        super().__init__(m, f, NonEmptyString("turn"))


class SetCommand(GenericCommand[SetRequest]):
    def __init__(self, m: Mediator, f: SetParameterFactory) -> None:
        super().__init__(m, f, NonEmptyString("set"))


class StartCommand(GenericCommand[StartRequest]):
    def __init__(self, m: Mediator) -> None:
        super().__init__(m, NoParameterFactory(StartRequest()), NonEmptyString("start"))


class StopCommand(GenericCommand[StopRequest]):
    def __init__(self, m: Mediator) -> None:
        super().__init__(m, NoParameterFactory(StopRequest()), NonEmptyString("stop"))



@dataclass(frozen=True)
class UserInput:
    command: NonEmptyString
    parameter: str


class InputReader(ABC):
    @abstractmethod
    def read(self) -> Result[UserInput, str] | None:
        """Returns Ok(UserInput), Err(message), or None to signal exit."""
        ...


class ConsoleInputReader(InputReader):
    def read(self) -> Result[UserInput, str] | None:
        try:
            line = input("Command: ").strip()
        except (EOFError, KeyboardInterrupt):
            return None

        if line in ("exit", "quit"):
            return None

        if not line:
            return Err("Input cannot be empty.")

        parts = line.split(maxsplit=1)
        param = parts[1] if len(parts) > 1 else ""

        return NonEmptyString.create(parts[0]).map(lambda name: UserInput(name, param))


class BatchInputReader(InputReader):
    def __init__(self, program: list[str]) -> None:
        self._iter = iter(program)

    def read(self) -> Result[UserInput, str] | None:
        line = next(self._iter, None)
        if line is None:
            return None

        parts = line.strip().split(maxsplit=1)
        param = parts[1] if len(parts) > 1 else ""

        return NonEmptyString.create(parts[0]).map(lambda name: UserInput(name, param))



class LoopExecutor:
    def __init__(self, commands: list[IRobotCommand]) -> None:
        self._commands = commands

    def run(self, reader: InputReader) -> None:
        while (user_input := reader.read()) is not None:
            (
                user_input
                .and_then(self._execute)
                .map_err(lambda e: print(f"Error: {e}"))
            )

    def _find_command(self, name: NonEmptyString) -> Result[IRobotCommand, str]:
        return next(
            (Ok(c) for c in self._commands if c.works_with(name)),
            Err(f"Command '{name}' not found."),
        )

    def _execute(self, inp: UserInput) -> Result[None, str]:
        return (
            self._find_command(inp.command)
            .and_then(lambda cmd: cmd.execute(inp.parameter))
        )
