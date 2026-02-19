from __future__ import annotations

from dataclasses import dataclass

from mediatr import Mediator

from value_objects import (
    UInt, Angle, DeviceType, MovingEntity, CleaningDevice,
)


@dataclass(frozen=True)
class MoveRequest:
    distance: UInt


@dataclass(frozen=True)
class TurnRequest:
    angle: Angle


@dataclass(frozen=True)
class SetRequest:
    device: DeviceType


@dataclass(frozen=True)
class StartRequest:
    pass


@dataclass(frozen=True)
class StopRequest:
    pass


@Mediator.handler
class MoveHandler:
    def __init__(self, robot: MovingEntity) -> None:
        self._r = robot

    def handle(self, req: MoveRequest) -> None:
        self._r.move_forward(req.distance)
        print(f"POS {self._r.position}")


@Mediator.handler
class TurnHandler:
    def __init__(self, robot: MovingEntity) -> None:
        self._r = robot

    def handle(self, req: TurnRequest) -> None:
        self._r.rotate(req.angle)
        print(f"ANGLE {self._r.angle}")


@Mediator.handler
class SetHandler:
    def __init__(self, device: CleaningDevice) -> None:
        self._d = device

    def handle(self, req: SetRequest) -> None:
        self._d.set(req.device)
        print(f"STATE {req.device.value}")


@Mediator.handler
class StartHandler:
    def __init__(self, device: CleaningDevice) -> None:
        self._d = device

    def handle(self, _: StartRequest) -> None:
        self._d.start()
        print(f"START WITH {self._d.current.value}")


@Mediator.handler
class StopHandler:
    def __init__(self, device: CleaningDevice) -> None:
        self._d = device

    def handle(self, _: StopRequest) -> None:
        self._d.stop()
        print("STOP")
