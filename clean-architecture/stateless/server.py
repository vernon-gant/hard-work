import pure_robot

def process_request(state_dict, commands):
    state = pure_robot.RobotState(
        state_dict["x"],
        state_dict["y"],
        state_dict["angle"],
        state_dict["state"],
    )

    log = []

    def transfer(message):
        log.append(message)

    new_state = pure_robot.make(transfer, commands, state)

    return {
        "x": new_state.x,
        "y": new_state.y,
        "angle": new_state.angle,
        "state": new_state.state,
        "log": log,
    }