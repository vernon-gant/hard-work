from pure_robot import *

class Robot:
    def __init__(self):
        self._state = RobotState(0, 0, 0, WATER)
        self._transfer = transfer_to_cleaner

    def move(self, distance):
        self._state = move(self._transfer, distance, self._state)

    def turn(self, angle):
        self._state = turn(self._transfer, angle, self._state)

    def set_device(self, device):
        self._state = set_state(self._transfer, device, self._state)

    def start(self):
        self._state = start(self._transfer, self._state)

    def stop(self):
        self._state = stop(self._transfer, self._state)