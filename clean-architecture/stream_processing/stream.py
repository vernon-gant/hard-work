import math
import uuid
import threading
import queue
from abc import ABC, abstractmethod
from dataclasses import dataclass, field
from datetime import datetime, timezone
from enum import Enum
from functools import singledispatchmethod
from typing import Callable


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

@dataclass(frozen=True)
class PositionUpdated(EventBase):
    x: float = 0.0
    y: float = 0.0
    def accept(self, visitor): visitor.visit(self)

@dataclass(frozen=True)
class AngleUpdated(EventBase):
    angle: float = 0.0
    def accept(self, visitor): visitor.visit(self)

@dataclass(frozen=True)
class CleaningReport(EventBase):
    total_distance: float = 0.0
    total_turns: int = 0
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
        pass

    @visit.register
    def _(self, event: RobotMoved) -> None:
        rad = math.radians(self.angle)
        self.x += event.distance * math.cos(rad)
        self.y += event.distance * math.sin(rad)

    @visit.register
    def _(self, event: RobotTurned) -> None:
        a = (self.angle + event.angle) % 360
        if a > 180: a -= 360
        if a <= -180: a += 360
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

    @visit.register
    def _(self, event: PositionUpdated) -> None:
        pass

    @visit.register
    def _(self, event: AngleUpdated) -> None:
        pass

    @visit.register
    def _(self, event: CleaningReport) -> None:
        pass

    def __str__(self) -> str:
        return (f"x={round(self.x)} y={round(self.y)} "
                f"angle={round(self.angle)} device={self.device.value} "
                f"running={self.is_running}")


class EventStore:
    def __init__(self) -> None:
        self._streams: dict[str, list[EventBase]] = {}
        self._subscribers: list[Callable[[EventBase], None]] = []
        self._lock = threading.Lock()

    def subscribe(self, handler: Callable[[EventBase], None]) -> None:
        self._subscribers.append(handler)

    def append(self, event: EventBase) -> None:
        with self._lock:
            if event.entity_id not in self._streams:
                self._streams[event.entity_id] = []
            self._streams[event.entity_id].append(event)
        for subscriber in self._subscribers:
            subscriber(event)

    def get_events(self, stream_id: str) -> list[EventBase]:
        with self._lock:
            return list(self._streams.get(stream_id, []))

    def get_snapshot(self, stream_id: str) -> RobotStateSnapshot:
        snapshot = RobotStateSnapshot()
        for event in self.get_events(stream_id):
            event.accept(snapshot)
        return snapshot


class EventProcessor(ABC):
    def __init__(self, store: EventStore) -> None:
        self._store = store
        self._queue: queue.Queue = queue.Queue()
        store.subscribe(self._enqueue)
        self._thread = threading.Thread(target=self._run, daemon=True)
        self._thread.start()

    def _enqueue(self, event: EventBase) -> None:
        self._queue.put(event)

    def _run(self) -> None:
        while True:
            event = self._queue.get()
            new_events = self.process(event)
            for new_event in new_events:
                self._store.append(new_event)
            self._queue.task_done()

    @abstractmethod
    def process(self, event: EventBase) -> list[EventBase]: ...

    def wait_idle(self) -> None:
        self._queue.join()


class PositionProcessor(EventProcessor):
    @singledispatchmethod
    def process(self, event: EventBase) -> list[EventBase]:
        return []

    @process.register
    def _(self, event: RobotMoved) -> list[EventBase]:
        snapshot = self._store.get_snapshot(event.entity_id)
        return [PositionUpdated(
                    entity_id=event.entity_id,
                    x=round(snapshot.x, 2),
                    y=round(snapshot.y, 2),
                )]

    @process.register
    def _(self, event: RobotTurned) -> list[EventBase]:
        snapshot = self._store.get_snapshot(event.entity_id)
        return [AngleUpdated(
                    entity_id=event.entity_id,
                    angle=round(snapshot.angle, 2),
                )]


class TransferProcessor(EventProcessor):
    def __init__(self, store: EventStore, transfer_fn: Callable[[str], None]) -> None:
        self._transfer_fn = transfer_fn
        super().__init__(store)

    @singledispatchmethod
    def process(self, event: EventBase) -> list[EventBase]:
        return []

    @process.register
    def _(self, event: PositionUpdated) -> list[EventBase]:
        self._transfer_fn(f"POS {round(event.x)},{round(event.y)}")
        return []

    @process.register
    def _(self, event: AngleUpdated) -> list[EventBase]:
        self._transfer_fn(f"ANGLE {round(event.angle)}")
        return []

    @process.register
    def _(self, event: DeviceChanged) -> list[EventBase]:
        self._transfer_fn(f"STATE {event.device.value}")
        return []

    @process.register
    def _(self, event: CleaningStarted) -> list[EventBase]:
        snapshot = self._store.get_snapshot(event.entity_id)
        self._transfer_fn(f"START WITH {snapshot.device.value}")
        return []

    @process.register
    def _(self, event: CleaningStopped) -> list[EventBase]:
        self._transfer_fn("STOP")
        return []


class StatsProcessor(EventProcessor):
    def __init__(self, store: EventStore) -> None:
        self._totals: dict[str, dict] = {}
        super().__init__(store)

    @singledispatchmethod
    def process(self, event: EventBase) -> list[EventBase]:
        return []

    @process.register
    def _(self, event: RobotMoved) -> list[EventBase]:
        if event.entity_id not in self._totals:
            self._totals[event.entity_id] = {"distance": 0.0, "turns": 0}
        self._totals[event.entity_id]["distance"] += event.distance
        return []

    @process.register
    def _(self, event: RobotTurned) -> list[EventBase]:
        if event.entity_id not in self._totals:
            self._totals[event.entity_id] = {"distance": 0.0, "turns": 0}
        self._totals[event.entity_id]["turns"] += 1
        return []

    @process.register
    def _(self, event: CleaningStopped) -> list[EventBase]:
        totals = self._totals.get(event.entity_id, {"distance": 0.0, "turns": 0})
        return [CleaningReport(
                    entity_id=event.entity_id,
                    total_distance=totals["distance"],
                    total_turns=totals["turns"],
                )]