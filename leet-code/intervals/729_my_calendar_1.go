package intervals

type BookingEntry struct {
    Start, End int
    Left       *BookingEntry
    Right      *BookingEntry
}
type MyCalendar struct {
    Root *BookingEntry
}

func Constructor() MyCalendar {
    return MyCalendar{Root: nil}
}

func (this *MyCalendar) Book(start int, end int) bool {
    return dfs(start, end, &this.Root)
}

func dfs(start, end int, root **BookingEntry) bool {
    if *root == nil {
        *root = &BookingEntry{Start: start, End: end}
        return true
    }
    if end <= (*root).Start {
        return dfs(start, end, &(*root).Left)
    }
    if start >= (*root).End {
        return dfs(start, end, &(*root).Right)
    }
    return false
}