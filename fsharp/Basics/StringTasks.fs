// 17.1
let rec pow text n =
    match n with
    | n when n <= 0 -> ""
    | 1 -> text
    | n -> text + pow text (n - 1)


// 17.2
let rec isIthChar (text: string) position letter =
    text.[position] = letter

// 17.3
let rec occFromIth (text: string) position letter =
    let length = String.length text
    let rec occFromIthRec currentPosition acc =
        match currentPosition with
        | n when n >= length -> acc
        | n when text.[n] = letter -> occFromIthRec (currentPosition + 1) (acc + 1)
        | _ -> occFromIthRec (currentPosition + 1) acc
    occFromIthRec position 0