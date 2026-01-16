package subsets

func CanCompleteCircuit(gas []int, cost []int) int {
    balance := 0
    for i := 0; i < len(gas); i++ {
        margin := gas[i] - cost[i]
        balance += margin
        gas[i] = margin
    }
    if balance < 0 {
        return -1
    }
	var i int
    for i = 0; i < len(gas); i++ {
        if gas[i] < 0 {
            continue
        }
        balance := gas[i]
		var j int
        for j = i + 1; j < len(gas) && balance > 0; j++ {
            balance += gas[j]
        }
        if j == len(gas) {
            break
        }
        i = j - 1
    }
	return i
}

func canCompleteCircuit(gas []int, cost []int) int {
    result, total, sum := 0, 0, 0
    for i := range gas {
        total += gas[i] - cost[i]
        sum += gas[i] - cost[i]
        if sum < 0 {
            sum = 0
            result = i + 1
        }
    }
    
    if total < 0 {
        return -1
    }
    
    return result
}