import pure_robot
from robot import Robot
from robot_state_reader import RobotStateReader


class PureRobotImplementation(Robot, RobotStateReader):

    def __init__(self, transfer):
        self._transfer = transfer
        self._state = pure_robot.RobotState(0.0, 0.0, 0, pure_robot.WATER)

    def move(self, dist):
        self._state = pure_robot.move(self._transfer, dist, self._state)

    def turn(self, angle):
        self._state = pure_robot.turn(self._transfer, angle, self._state)

    def set_state(self, new_state):
        self._state = pure_robot.set_state(self._transfer, new_state, self._state)

    def start(self):
        self._state = pure_robot.start(self._transfer, self._state)

    def stop(self):
        self._state = pure_robot.stop(self._transfer, self._state)

    def get_x(self):
        return self._state.x

    def get_y(self):
        return self._state.y

    def get_angle(self):
        return self._state.angle

    def get_state(self):
        return self._state.state