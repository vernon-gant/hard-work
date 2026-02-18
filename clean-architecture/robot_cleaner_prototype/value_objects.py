from __future__ import annotations

import math
from dataclasses import dataclass
from enum import Enum

from result import Result, Ok, Err


@dataclass(frozen=True)
class NonEmptyString:
    value: str

    @staticmethod
    def create(value: str) -> Result[NonEmptyString, str]:
        if not value or not value.strip():
            return Err("Value cannot be null or empty.")
        return Ok(NonEmptyString(value))

    def __str__(self) -> str:
        return self.value


@dataclass(frozen=True)
class UInt:
    value: int

    @staticmethod
    def create(value: int) -> Result[UInt, str]:
        if value < 0:
            return Err(f"Value must be non-negative, got {value}")
        return Ok(UInt(value))

    def __int__(self) -> int:
        return self.value


@dataclass(frozen=True)
class Angle:
    value: int

    @staticmethod
    def create(value: int) -> Result[Angle, str]:
        if not -180 <= value <= 180:
            return Err(f"Angle must be between -180 and 180, got {value}")
        return Ok(Angle(value))


@dataclass(frozen=True)
class Point:
    x: float = 0.0
    y: float = 0.0

    def __str__(self) -> str:
        return f"{round(self.x)},{round(self.y)}"


class DeviceType(Enum):
    WATER = "water"
    SOAP = "soap"
    BRUSH = "brush"

    @staticmethod
    def from_str(raw: str) -> Result[DeviceType, str]:
        _lookup = {d.value: d for d in DeviceType}
        match _lookup.get(raw.lower()):
            case None:
                return Err(f"Unknown device '{raw}'. Use: water, soap, brush.")
            case found:
                return Ok(found)


class MovingEntity:
    def __init__(self) -> None:
        self._angle = 0.0
        self._pos = Point()

    def rotate(self, angle: Angle) -> None:
        a = (self._angle + angle.value) % 360
        if a > 180:
            a -= 360
        if a <= -180:
            a += 360
        self._angle = a

    def move_forward(self, distance: UInt) -> None:
        rad = math.radians(self._angle)
        self._pos = Point(
            self._pos.x + distance.value * math.cos(rad),
            self._pos.y + distance.value * math.sin(rad),
        )

    @property
    def angle(self) -> int:
        return round(self._angle)

    @property
    def position(self) -> Point:
        return self._pos


class CleaningDevice:
    def __init__(self) -> None:
        self.current = DeviceType.WATER
        self.is_running = False

    def set(self, d: DeviceType) -> None:
        self.current = d

    def start(self) -> None:
        self.is_running = True

    def stop(self) -> None:
        self.is_running = False
