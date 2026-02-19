import math
from dataclasses import dataclass, field
from enum import Enum

class CleaningDevice(Enum):
    WATER = "water"
    SOAP  = "soap"
    BRUSH = "brush"


@dataclass
class UserInput:
    command_name: str | None = None
    command_arguments: str | None = None

@dataclass
class Robot:
    x: int = 0
    y: int = 0
    angle: int = 0
    cleaning_device: CleaningDevice | None = None


@dataclass
class CommandExecutionContext:
    robot: Robot = field(default_factory=Robot)
    user_input: UserInput = field(default_factory=UserInput)
    argument_error_message: str | None = None
    command_error_message: str | None = None

    int_arg: int = 0
    device_arg: CleaningDevice | None = None


@dataclass
class UserInputContext:
    user_input: UserInput = field(default_factory=UserInput)
    input_error_message: str | None = None


@dataclass
class CommandsHub:
    commands: dict = field(default_factory=dict)
    argument_providers: dict = field(default_factory=dict)


@dataclass
class GlobalContext:
    commands_hub: CommandsHub = field(default_factory=CommandsHub)
    robot: Robot = field(default_factory=Robot)



def move_command(context: CommandExecutionContext) -> int:
    distance = context.int_arg
    radians = context.robot.angle * math.pi / 180.0

    context.robot.x += int(distance * math.cos(radians))
    context.robot.y += int(distance * math.sin(radians))

    print(f"POS {context.robot.x} {context.robot.y}")
    return 0


def turn_command(context: CommandExecutionContext) -> int:
    angle = context.int_arg

    context.robot.angle = (context.robot.angle + angle) % 360

    if context.robot.angle > 180:
        context.robot.angle -= 360

    if context.robot.angle <= -180:
        context.robot.angle += 360

    print(f"ANGLE {context.robot.angle}")
    return 0


def set_command(context: CommandExecutionContext) -> int:
    cleaning_device = context.device_arg

    context.robot.cleaning_device = cleaning_device

    print(f"STATE {cleaning_device.name}")
    return 0


def start_command(context: CommandExecutionContext) -> int:
    if context.robot.cleaning_device is None:
        context.command_error_message = "Cleaning device is not set."
        return -1

    print(f"START WITH {context.robot.cleaning_device.name}")
    return 0


def stop_command(context: CommandExecutionContext) -> int:
    print("STOP")
    return 0


def move_argument_provider(context: CommandExecutionContext) -> int:
    raw = context.user_input.command_arguments.strip()

    if not raw.isdigit():
        context.argument_error_message = "Distance must be a positive integer."
        return -1

    distance = int(raw)

    if distance <= 0:
        context.argument_error_message = "Distance must be a positive integer."
        return -1

    context.int_arg = distance
    return 0


def turn_argument_provider(context: CommandExecutionContext) -> int:
    raw = context.user_input.command_arguments.strip()

    if not raw.lstrip("-").isdigit():
        context.argument_error_message = "Angle must be an integer between -180 and 180, excluding 0."
        return -1

    angle = int(raw)

    if angle == 0 or angle < -180 or angle > 180:
        context.argument_error_message = "Angle must be an integer between -180 and 180, excluding 0."
        return -1

    context.int_arg = angle
    return 0


def set_argument_provider(context: CommandExecutionContext) -> int:
    raw = context.user_input.command_arguments.strip()
    lookup = {d.value.lower(): d for d in CleaningDevice}
    device = lookup.get(raw.lower())

    if device is None:
        names = ", ".join(d.value for d in CleaningDevice)
        context.argument_error_message = f"Cleaning device must be one of: {names}."
        return -1

    context.device_arg = device
    return 0


def start_argument_provider(context: CommandExecutionContext) -> int:
    return 0


def stop_argument_provider(context: CommandExecutionContext) -> int:
    return 0


def configure_commands_hub() -> CommandsHub:
    hub = CommandsHub()

    hub.commands = {
        "move":  move_command,
        "turn":  turn_command,
        "set":   set_command,
        "start": start_command,
        "stop":  stop_command,
    }

    hub.argument_providers = {
        move_command:  move_argument_provider,
        turn_command:  turn_argument_provider,
        set_command:   set_argument_provider,
        start_command: start_argument_provider,
        stop_command:  stop_argument_provider,
    }

    return hub


def read_user_input(context: UserInputContext) -> int:
    raw = input()

    if not raw or not raw.strip():
        context.input_error_message = "Input cannot be empty."
        return -1

    parts = raw.strip().split(maxsplit=1)

    if len(parts) == 0:
        context.input_error_message = "Input cannot be empty."
        return -1

    if not parts[0].strip():
        context.input_error_message = "Command name cannot be empty."
        return -1

    context.user_input.command_name = parts[0]
    context.user_input.command_arguments = parts[1] if len(parts) > 1 else ""

    return 0


def execute_command(global_context: GlobalContext, command_context: CommandExecutionContext) -> int:
    command = global_context.commands_hub.commands.get(command_context.user_input.command_name)

    if command is None:
        command_context.command_error_message = f"Unknown command: {command_context.user_input.command_name}."
        return -1

    argument_provider = global_context.commands_hub.argument_providers[command]
    argument_status = argument_provider(command_context)

    if argument_status != 0:
        return -1

    return command(command_context)


def loop(global_context: GlobalContext) -> None:
    while True:
        user_input_context = UserInputContext()
        user_input_status = read_user_input(user_input_context)

        if user_input_status != 0:
            print(f"ERROR: {user_input_context.input_error_message}")
            continue

        if user_input_context.user_input.command_name == "exit":
            break

        command_execution_context = CommandExecutionContext(robot=global_context.robot,user_input=user_input_context.user_input)
        command_status = execute_command(global_context, command_execution_context)

        if command_status == 0:
            continue

        error_message = command_execution_context.command_error_message or command_execution_context.argument_error_message
        print(f"ERROR: {error_message}")


if __name__ == '__main__':
    global_context = GlobalContext(
        commands_hub=configure_commands_hub(),
        robot=Robot(),
    )

    loop(global_context)