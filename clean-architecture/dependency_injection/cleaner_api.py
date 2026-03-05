from robot import Robot
from robot_state_reader import RobotStateReader


class CleanerApi:

    def __init__(self, robot: Robot, state_reader: RobotStateReader):
        self._robot = robot
        self._reader = state_reader

    def activate_cleaner(self, code):
        for command in code:
            cmd = command.split(' ')
            if cmd[0] == 'move':
                self._robot.move(int(cmd[1]))
            elif cmd[0] == 'turn':
                self._robot.turn(int(cmd[1]))
            elif cmd[0] == 'set':
                self._robot.set_state(cmd[1])
            elif cmd[0] == 'start':
                self._robot.start()
            elif cmd[0] == 'stop':
                self._robot.stop()

    def get_x(self):
        return self._reader.get_x()

    def get_y(self):
        return self._reader.get_y()

    def get_angle(self):
        return self._reader.get_angle()

    def get_state(self):
        return self._reader.get_state()