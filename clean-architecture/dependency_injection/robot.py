from abc import ABC, abstractmethod


class Robot(ABC):

    @abstractmethod
    def move(self, dist): ...

    @abstractmethod
    def turn(self, angle): ...

    @abstractmethod
    def set_state(self, new_state): ...

    @abstractmethod
    def start(self): ...

    @abstractmethod
    def stop(self): ...