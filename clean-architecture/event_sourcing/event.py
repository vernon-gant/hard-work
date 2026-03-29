import math
import uuid
from abc import ABC, abstractmethod
from dataclasses import dataclass, field
from datetime import datetime, timezone
from enum import Enum
from functools import singledispatchmethod


class DeviceType(Enum):
    WATER = "water"
    SOAP  = "soap"
    BRUSH = "brush"


@dataclass(frozen=True)
class EventBase(ABC):
    entity_id: str
    id: str = field(default_factory=lambda: str(uuid.uuid4()))
    created_at: datetime = field(default_factory=lambda: datetime.now(timezone.utc))

    @abstractmethod
    def accept(self, visitor: "IEventVisitor") -> None: ...


@dataclass(frozen=True)
class RobotMoved(EventBase):
    distance: int = 0
    def accept(self, visitor): visitor.visit(self)

@dataclass(frozen=True)
class RobotTurned(EventBase):
    angle: int = 0
    def accept(self, visitor): visitor.visit(self)

@dataclass(frozen=True)
class DeviceChanged(EventBase):
    device: DeviceType = DeviceType.WATER
    def accept(self, visitor): visitor.visit(self)

@dataclass(frozen=True)
class CleaningStarted(EventBase):
    def accept(self, visitor): visitor.visit(self)

@dataclass(frozen=True)
class CleaningStopped(EventBase):
    def accept(self, visitor): visitor.visit(self)


class IEventVisitor(ABC):
    @abstractmethod
    def visit(self, event: EventBase) -> None: ...


class RobotStateSnapshot(IEventVisitor):
    def __init__(self) -> None:
        self.x: float = 0.0
        self.y: float = 0.0
        self.angle: float = 0.0
        self.device: DeviceType = DeviceType.WATER
        self.is_running: bool = False

    @singledispatchmethod
    def visit(self, event: EventBase) -> None:
        raise NotImplementedError(f"Unhandled event: {type(event).__name__}")

    @visit.register
    def _(self, event: RobotMoved) -> None:
        rad = math.radians(self.angle)
        self.x += event.distance * math.cos(rad)
        self.y += event.distance * math.sin(rad)

    @visit.register
    def _(self, event: RobotTurned) -> None:
        a = (self.angle + event.angle) % 360
        if a > 180: 
            a -= 360
        if a <= -180: 
            a += 360
        self.angle = a

    @visit.register
    def _(self, event: DeviceChanged) -> None:
        self.device = event.device

    @visit.register
    def _(self, event: CleaningStarted) -> None:
        self.is_running = True

    @visit.register
    def _(self, event: CleaningStopped) -> None:
        self.is_running = False

    def __str__(self) -> str:
        return (f"x={round(self.x)} y={round(self.y)} "
                f"angle={round(self.angle)} device={self.device.value} "
                f"running={self.is_running}")


class TransferVisitor(IEventVisitor):
    def __init__(self, snapshot: RobotStateSnapshot, transfer_fn) -> None:
        self._state = snapshot
        self._transfer_fn = transfer_fn

    def _transfer(self, message: str) -> None:
        self._transfer_fn(message)

    @singledispatchmethod
    def visit(self, event: EventBase) -> None:
        raise NotImplementedError(f"Unhandled event: {type(event).__name__}")

    @visit.register
    def _(self, event: RobotMoved) -> None:
        rad = math.radians(self._state.angle)
        x = round(self._state.x + event.distance * math.cos(rad))
        y = round(self._state.y + event.distance * math.sin(rad))
        self._transfer(f"POS {x},{y}")

    @visit.register
    def _(self, event: RobotTurned) -> None:
        a = (self._state.angle + event.angle) % 360
        if a > 180: a -= 360
        if a <= -180: a += 360
        self._transfer(f"ANGLE {round(a)}")

    @visit.register
    def _(self, event: DeviceChanged) -> None:
        self._transfer(f"STATE {event.device.value}")

    @visit.register
    def _(self, event: CleaningStarted) -> None:
        self._transfer(f"START WITH {self._state.device.value}")

    @visit.register
    def _(self, event: CleaningStopped) -> None:
        self._transfer("STOP")


class IEventStore(ABC):
    @abstractmethod
    def append(self, stream_id: str, events: list[EventBase]) -> None: ...

    @abstractmethod
    def get_events(self, stream_id: str) -> list[EventBase]: ...

    @abstractmethod
    def get_snapshot(self, stream_id: str) -> RobotStateSnapshot: ...

    @abstractmethod
    def get_snapshot_at(self, stream_id: str, version: int) -> RobotStateSnapshot: ...


class InMemoryEventStore(IEventStore):
    def __init__(self) -> None:
        self._streams: dict[str, list[EventBase]] = {}

    def append(self, stream_id: str, events: list[EventBase]) -> None:
        if stream_id not in self._streams:
            self._streams[stream_id] = []
        self._streams[stream_id].extend(events)

    def get_events(self, stream_id: str) -> list[EventBase]:
        return list(self._streams.get(stream_id, []))

    def get_snapshot(self, stream_id: str) -> RobotStateSnapshot:
        return self._rebuild(self.get_events(stream_id))

    def get_snapshot_at(self, stream_id: str, version: int) -> RobotStateSnapshot:
        return self._rebuild(self.get_events(stream_id)[:version])

    def _rebuild(self, events: list[EventBase]) -> RobotStateSnapshot:
        snapshot = RobotStateSnapshot()
        for event in events:
            event.accept(snapshot)
        return snapshot


class EventCoordinator:
    def __init__(self, store: IEventStore, transfer_fn) -> None:
        self._store = store
        self._transfer_fn = transfer_fn

    def apply_event(self, event: EventBase) -> None:
        stream_id = event.entity_id
        snapshot = self._store.get_snapshot(stream_id)

        transfer = TransferVisitor(snapshot, self._transfer_fn)
        event.accept(transfer)

        self._store.append(stream_id, [event])