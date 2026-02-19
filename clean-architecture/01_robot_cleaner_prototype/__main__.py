from mediatr import Mediator

from value_objects import MovingEntity, CleaningDevice
from commands import (
    MoveHandler, TurnHandler, SetHandler, StartHandler, StopHandler,
)
from command_parameters import (
    MoveParameterFactory, TurnParameterFactory, SetParameterFactory,
)
from infrastructure import (
    LoopExecutor, BatchInputReader, ConsoleInputReader,
    MoveCommand, TurnCommand, SetCommand, StartCommand, StopCommand,
)

def build() -> LoopExecutor:
    robot = MovingEntity()
    device = CleaningDevice()

    handler_map = {
        MoveHandler: lambda: MoveHandler(robot),
        TurnHandler: lambda: TurnHandler(robot),
        SetHandler: lambda: SetHandler(device),
        StartHandler: lambda: StartHandler(device),
        StopHandler: lambda: StopHandler(device),
    }
    mediator = Mediator(
        handler_class_manager=lambda cls, is_behavior=False: handler_map[cls]()
    )

    return LoopExecutor([
        MoveCommand(mediator, MoveParameterFactory()),
        TurnCommand(mediator, TurnParameterFactory()),
        SetCommand(mediator, SetParameterFactory()),
        StartCommand(mediator),
        StopCommand(mediator),
    ])


if __name__ == "__main__":
    program = [
        "move 100",
        "turn -90",
        "set soap",
        "start",
        "move 50",
        "stop",
    ]
    build().run(BatchInputReader(program))
    # build().run(ConsoleInputReader())
