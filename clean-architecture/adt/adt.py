import math
from abc import ABC, abstractmethod
from enum import Enum
from typing import Tuple, List, Type
from functools import reduce
from dataclasses import dataclass

class CleaningMode(Enum):
    WATER = 1
    SOAP  = 2
    BRUSH = 3

    @staticmethod
    def from_string(s: str) -> 'CleaningMode':
        return CleaningMode[s.upper()]

class IMovable(ABC):
    @abstractmethod
    def move(self, distance: float) -> 'IMovable': ...
    @abstractmethod
    def turn(self, angle: float) -> 'IMovable': ...
    @abstractmethod
    def get_position(self) -> Tuple[float, float]: ...
    @abstractmethod
    def get_angle(self) -> float: ...


class Mover(IMovable):
    def __init__(self, x: float = 0.0, y: float = 0.0, angle: float = 0.0):
        self.__x = x
        self.__y = y
        self.__angle = angle

    def move(self, distance: float) -> 'Mover':
        rads = self.__angle * (math.pi / 180.0)
        return Mover(
            self.__x + distance * math.cos(rads),
            self.__y + distance * math.sin(rads),
            self.__angle
        )

    def turn(self, angle: float) -> 'Mover':
        return Mover(self.__x, self.__y, self.__angle + angle)

    def get_position(self) -> Tuple[float, float]:
        return (self.__x, self.__y)

    def get_angle(self) -> float:
        return self.__angle

class ICleanable(ABC):
    @abstractmethod
    def set_mode(self, mode: CleaningMode) -> 'ICleanable': ...
    @abstractmethod
    def start(self) -> 'ICleanable': ...
    @abstractmethod
    def stop(self) -> 'ICleanable': ...
    @abstractmethod
    def get_mode(self) -> CleaningMode: ...


class Cleaner(ICleanable):
    def __init__(self, mode: CleaningMode = CleaningMode.WATER):
        self.__mode = mode

    def set_mode(self, mode: CleaningMode) -> 'Cleaner':
        return Cleaner(mode)

    def start(self) -> 'Cleaner':
        return Cleaner(self.__mode)

    def stop(self) -> 'Cleaner':
        return Cleaner(self.__mode)

    def get_mode(self) -> CleaningMode:
        return self.__mode


@dataclass(frozen=True)
class Logged:
    mover: IMovable
    cleaner: ICleanable
    messages: Tuple[str, ...]

    @staticmethod
    def of(mover: IMovable, cleaner: ICleanable, message: str) -> 'Logged':
        return Logged(mover, cleaner, (message,))

    def then(self, other: 'Logged') -> 'Logged':
        return Logged(other.mover, other.cleaner, self.messages + other.messages)


class Command(ABC):
    _registry: dict[str, Type['Command']] = {}

    def __init_subclass__(cls, keyword: str = '', **kwargs):
        super().__init_subclass__(**kwargs)
        if keyword:
            Command._registry[keyword] = cls

    @classmethod
    def from_parts(cls, parts: List[str]) -> 'Command':
        return cls()

    @abstractmethod
    def _transform(self, mover: IMovable, cleaner: ICleanable) -> Tuple[IMovable, ICleanable]: ...

    @abstractmethod
    def _format(self, mover: IMovable, cleaner: ICleanable) -> str: ...

    def apply(self, mover: IMovable, cleaner: ICleanable) -> Logged:
        new_mover, new_cleaner = self._transform(mover, cleaner)
        return Logged.of(new_mover, new_cleaner, self._format(new_mover, new_cleaner))

    @staticmethod
    def parse(line: str) -> 'Command':
        parts = line.split(' ')
        return Command._registry[parts[0]].from_parts(parts)


@dataclass(frozen=True)
class MoveCmd(Command, keyword='move'):
    distance: float

    @classmethod
    def from_parts(cls, parts: List[str]) -> 'MoveCmd':
        return cls(float(parts[1]))

    def _transform(self, mover, cleaner):
        return mover.move(self.distance), cleaner

    def _format(self, mover, cleaner):
        x, y = mover.get_position()
        return f'POS({x},{y})'


@dataclass(frozen=True)
class TurnCmd(Command, keyword='turn'):
    angle: float

    @classmethod
    def from_parts(cls, parts: List[str]) -> 'TurnCmd':
        return cls(float(parts[1]))

    def _transform(self, mover, cleaner):
        return mover.turn(self.angle), cleaner

    def _format(self, mover, cleaner):
        return f'ANGLE {mover.get_angle()}'


@dataclass(frozen=True)
class SetModeCmd(Command, keyword='set'):
    mode: CleaningMode

    @classmethod
    def from_parts(cls, parts: List[str]) -> 'SetModeCmd':
        return cls(CleaningMode.from_string(parts[1]))

    def _transform(self, mover, cleaner):
        return mover, cleaner.set_mode(self.mode)

    def _format(self, mover, cleaner):
        return f'STATE {self.mode.name.lower()}'


@dataclass(frozen=True)
class StartCmd(Command, keyword='start'):

    def _transform(self, mover, cleaner):
        return mover, cleaner.start()

    def _format(self, mover, cleaner):
        return f'START WITH {cleaner.get_mode().name.lower()}'


@dataclass(frozen=True)
class StopCmd(Command, keyword='stop'):

    def _transform(self, mover, cleaner):
        return mover, cleaner.stop()

    def _format(self, mover, cleaner):
        return 'STOP'

def interpret(mover: IMovable, cleaner: ICleanable, code: List[str]) -> Logged:
    commands = [Command.parse(line) for line in code]

    def step(acc: Logged, cmd: Command) -> Logged:
        return acc.then(cmd.apply(acc.mover, acc.cleaner))

    return reduce(step, commands, Logged(mover, cleaner, ()))


if __name__ == '__main__':

    result = interpret(Mover(), Cleaner(), [
        'move 100',
        'turn -90',
        'set soap',
        'start',
        'move 50',
        'stop'
    ])

    for msg in result.messages:
        print(msg)