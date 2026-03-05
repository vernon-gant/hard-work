from abc import ABC, abstractmethod

class RobotStateReader(ABC):

    @abstractmethod
    def get_x(self): ...

    @abstractmethod
    def get_y(self): ...

    @abstractmethod
    def get_angle(self): ...

    @abstractmethod
    def get_state(self): ...