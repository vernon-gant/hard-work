package intervals

import (
	"math"
	"sort"
	"strconv"
)

type Interval struct {
	Start int
	End   int
}

func (i *Interval) IntervalInit(start int, end int) {
	i.Start = start
	i.End = end
}

func (i *Interval) Str() string {
	out := "(" + strconv.Itoa(i.Start) + ", " + strconv.Itoa(i.End) + ")"
	return out
}

func EmployeeFreeTime(schedule [][]*Interval) []*Interval {
	var allIntervals []*Interval
	for _, singleSchedule := range schedule {
		allIntervals = append(allIntervals, singleSchedule...)
	}
	if len(allIntervals) == 1 || len(allIntervals) == 0 {
		return allIntervals
	}
	sort.Slice(allIntervals, func(i, j int) bool {
		if allIntervals[i].Start == allIntervals[j].Start {
			return allIntervals[i].End < allIntervals[j].End
		}
		return allIntervals[i].Start < allIntervals[j].Start
	})
	var result []*Interval
	currentIntervalEnd := math.MinInt
	for i := 1; i < len(allIntervals); i++ {
		if allIntervals[i].Start <= allIntervals[i-1].End {
			currentIntervalEnd = max(allIntervals[i-1].End, allIntervals[i].End)
			continue
		}
		result = append(result, &Interval{
			Start: currentIntervalEnd,
			End:   allIntervals[i].Start,
		})
		currentIntervalEnd = allIntervals[i].End
	}
	return result
}
