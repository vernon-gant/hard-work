from __future__ import annotations

from abc import ABC, abstractmethod
from typing import Generic, TypeVar

from result import Result, Ok

from value_objects import NonEmptyString, UInt, Angle, DeviceType
from commands import MoveRequest, TurnRequest, SetRequest
from utils import parse_int


TRequest = TypeVar("TRequest")


class ParameterFactory(ABC, Generic[TRequest]):
    @abstractmethod
    def create(self, raw: str) -> Result[TRequest, str]: ...


class NoParameterFactory(ParameterFactory[TRequest]):
    def __init__(self, instance: TRequest) -> None:
        self._instance = instance

    def create(self, _: str) -> Result[TRequest, str]:
        return Ok(self._instance)


class MoveParameterFactory(ParameterFactory[MoveRequest]):
    def create(self, raw: str) -> Result[MoveRequest, str]:
        return (
            NonEmptyString.create(raw)
            .and_then(parse_int)
            .and_then(UInt.create)
            .map(MoveRequest)
        )


class TurnParameterFactory(ParameterFactory[TurnRequest]):
    def create(self, raw: str) -> Result[TurnRequest, str]:
        return (
            NonEmptyString.create(raw)
            .and_then(parse_int)
            .and_then(Angle.create)
            .map(TurnRequest)
        )


class SetParameterFactory(ParameterFactory[SetRequest]):
    def create(self, raw: str) -> Result[SetRequest, str]:
        return (
            NonEmptyString.create(raw)
            .and_then(lambda s: DeviceType.from_str(s.value))
            .map(SetRequest)
        )