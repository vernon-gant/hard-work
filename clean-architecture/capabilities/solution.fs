// This is the final leeson and i was so impressed with the power of functinal approach, so I decided to implement the capabilities using F#? Now I start feeling your point about
// the power hidden in the type system of functional languages, because we can literally encode the robot's mobility and cleaning state as types, and the compiler will catch
// any invalid operations at compile time. This is what I did in the RobotProtocol with indexed type Robot where we encode the robot's mobility and cleaning state as index types. 
// Compiler then enforces the type safety of the robot's operations. Some functions may return that robot is Robot<StillMovable, m'> say after turning and then we can only apply
// operations that are valid for that state. This is what was desired for the capability, as I assume. Of course this is my first version, I can't still think on that abstract level.
// But what was mind blowing is how we actually encode the robot state in types and then make the compiler enforce the type safety of the robot's operations. In OOP we achieve the same
// using interfaces, I think we could do the same with type constraints in C# with interfaces, but the syntax will be more verbose. I was really impressed how elegant it can be here.
// And this is even considering the fact that I do not know F# and was using the pocket jarvis. I think with computed expressions and other F# features we can do really mad stuff...
// 
// Okay, as I see I got the idea a bit incorrectly, I though that literally returning the available funcitons was just the naive approach. But it was not... But I still liked
// practicing with F# and capabilities over types!

open System

type CleaningMode =
    | Water
    | Soap
    | Brush

type RobotData =
    { x: float
      y: float
      angle: float
      mode: CleaningMode }

type MoveError = | HitBarrier

type SetModeError =
    | OutOfWater
    | OutOfSoap

type CanMove = CanMove
type Blocked = Blocked

type CleaningOn = CleaningOn
type CleaningOff = CleaningOff

// Opaque typed robot handle
type Robot<'mobility, 'cleaning> = private Robot of RobotData

module RobotHandle =
    let inline unwrap (Robot data) = data
    let inline wrap data = Robot data

module RobotDomain =

    type MoveResult =
        | MoveOk of RobotData * distanceMoved: float
        | MoveFailed of RobotData * MoveError

    type SetModeResult =
        | SetModeOk of RobotData
        | SetModeFailed of RobotData * SetModeError

    let initialData =
        { x = 0.0
          y = 0.0
          angle = 0.0
          selectedMode = Water
          isCleaning = false }

    let private degToRad angle = angle * Math.PI / 180.0

    let private constrainPosition (x: float) (y: float) =
        let cx = max 0.0 (min 100.0 x)
        let cy = max 0.0 (min 100.0 y)
        let wasConstrained = (cx <> x) || (cy <> y)
        cx, cy, wasConstrained

    let move distance (state: RobotData) : MoveResult =
        let radians = degToRad state.angle
        let newX = state.x + distance * cos radians
        let newY = state.y + distance * sin radians
        let cx, cy, constrained = constrainPosition newX newY

        let updated = { state with x = cx; y = cy }

        if constrained then
            MoveFailed(updated, HitBarrier)
        else
            MoveOk(updated, distance)

    let turn angle (state: RobotData) : RobotData =
        { state with
            angle = state.angle + angle }

    let private checkResources newMode =
        match newMode with
        | Water -> Ok()
        | Soap -> Error OutOfSoap // demo failure rule
        | Brush -> Ok()

    let setMode newMode (state: RobotData) : SetModeResult =
        match checkResources newMode with
        | Error err -> SetModeFailed(state, err)
        | Ok() ->
            let updated = { state with selectedMode = newMode }
            SetModeOk updated

module RobotProtocol =

    open RobotDomain
    open RobotHandle

    type MoveOutcome<'cleaning> =
        | StillMovable of Robot<CanMove, 'cleaning> * distanceMoved: float
        | BecameBlocked of Robot<Blocked, 'cleaning> * MoveError

    type SetModeOutcome<'mobility> =
        | ModeChanged of Robot<'mobility, CleaningOff>
        | ModeRejected of Robot<'mobility, CleaningOff> * SetModeError

    let create () : Robot<CanMove, CleaningOff> = wrap initialData

    // Available only when cleaning is OFF
    let startCleaning (robot: Robot<'m, CleaningOff>) : Robot<'m, CleaningOn> =
        let data = unwrap robot
        RobotDomain.startCleaning data |> wrap

    // Available only when cleaning is ON
    let stopCleaning (robot: Robot<'m, CleaningOn>) : Robot<'m, CleaningOff> =
        let data = unwrap robot
        RobotDomain.stopCleaning data |> wrap

    // Available for movable robots, regardless of cleaning status
    let move distance (robot: Robot<CanMove, 'c>) : MoveOutcome<'c> =
        let data = unwrap robot

        match RobotDomain.move distance data with
        | MoveOk(updated, dist) -> StillMovable(wrap updated, dist)
        | MoveFailed(updated, err) -> BecameBlocked(wrap updated, err)

    // Turning while movable keeps robot movable
    let turn angle (robot: Robot<CanMove, 'c>) : Robot<CanMove, 'c> =
        let data = unwrap robot
        RobotDomain.turn angle data |> wrap

    // Turning while blocked unblocks the robot
    let turnBlocked angle (robot: Robot<Blocked, 'c>) : Robot<CanMove, 'c> =
        let data = unwrap robot
        RobotDomain.turn angle data |> wrap

    // setMode available only when cleaning is OFF
    let setMode newMode (robot: Robot<'m, CleaningOff>) : SetModeOutcome<'m> =
        let data = unwrap robot

        match RobotDomain.setMode newMode data with
        | SetModeOk updated -> ModeChanged(wrap updated)
        | SetModeFailed(sameData, err) -> ModeRejected(wrap sameData, err)

module PureExample =

    open RobotProtocol
    open RobotHandle

    let demo () =
        let r0 = create ()
        let r1 = startCleaning r0

        let r2 =
            match move 100.0 r1 with
            | StillMovable(r, _) -> r
            | BecameBlocked(b, _) -> turnBlocked 45.0 b

        let r3 =
            match setMode Soap r2 with
            | ModeChanged r -> r
            | ModeRejected(r, _) -> r

        let r4 = stopCleaning r3
        unwrap r4