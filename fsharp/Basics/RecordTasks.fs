module RecordTasks

type TimeOfDay = { hours: int; minutes: int; f: string }

let (.>.) x y =
    let convert time =
        match time.f with
        | "AM" -> (time.hours, time.minutes)
        | _ -> (time.hours + 12, time.minutes)
    convert x > convert y