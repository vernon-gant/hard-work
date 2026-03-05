import math
from collections import namedtuple

RobotState = namedtuple("RobotState", "x y angle state")

WATER = 1
SOAP  = 2
BRUSH = 3

def transfer_to_cleaner(message):
    print (message)

def move(transfer,dist,state):
    angle_rads = state.angle * (math.pi/180.0)
    new_state = RobotState(
        state.x + dist * math.cos(angle_rads),
        state.y + dist * math.sin(angle_rads),
        state.angle,
        state.state)
    transfer(('POS(',new_state.x,',',new_state.y,')'))
    return new_state

def turn(transfer,turn_angle,state):
    new_state = RobotState(
        state.x,
        state.y,
        state.angle + turn_angle,
        state.state)
    transfer(('ANGLE',state.angle))
    return new_state

def set_state(transfer,new_internal_state,state):
    if new_internal_state=='water':
        self_state = WATER
    elif new_internal_state=='soap':
        self_state = SOAP
    elif new_internal_state=='brush':
        self_state = BRUSH
    else:
        return state
    new_state = RobotState(
        state.x,
        state.y,
        state.angle,
        self_state)
    transfer(('STATE',self_state))
    return new_state

def start(transfer,state):
    transfer(('START WITH',state.state))
    return state

def stop(transfer,state):
    transfer(('STOP',))
    return state

def make(transfer,code,state):
    for command in code:
        cmd = command.split(' ')
        if cmd[0]=='move':
            state = move(transfer,int(cmd[1]),state)
        elif cmd[0]=='turn':
            state = turn(transfer,int(cmd[1]),state)
        elif cmd[0]=='set':
            state = set_state(transfer,cmd[1],state)
        elif cmd[0]=='start':
            state = start(transfer,state)
        elif cmd[0]=='stop':
            state = stop(transfer,state)
    return state