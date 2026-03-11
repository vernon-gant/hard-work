import pure_robot

class RobotApi:

    def setup(self, execute, transfer):
        self.execute = execute
        self.transfer = transfer

    def make(self, command):
        if not hasattr(self, 'cleaner_state'):
            self.cleaner_state = pure_robot.RobotState(0.0, 0.0, 0, pure_robot.WATER)

        self.cleaner_state = self.execute(self.transfer, command, self.cleaner_state)

        return self.cleaner_state

    def __call__(self, command):
        return self.make(command)

def transfer_to_cleaner(message):
    print(message)

def execute(transfer, command, state):
    cmd = command.split(' ')
    if cmd[0] == 'move':
        return pure_robot.move(transfer, int(cmd[1]), state)
    elif cmd[0] == 'turn':
        return pure_robot.turn(transfer, int(cmd[1]), state)
    elif cmd[0] == 'set':
        return pure_robot.set_state(transfer, cmd[1], state)
    elif cmd[0] == 'start':
        return pure_robot.start(transfer, state)
    elif cmd[0] == 'stop':
        return pure_robot.stop(transfer, state)
    return state

api = RobotApi()
api.setup(execute, transfer_to_cleaner)