package binary_tree

// My first inefficient solution with a real tree implementation
type EmployeeNode struct {
    InformTime int
    Subordinates []*EmployeeNode
}

func NumOfMinutes(n int, headID int, manager []int, informTime []int) int {
    employeeNodes := make([]*EmployeeNode, n)
    for i := 0; i < n; i++ {
        employeeNodes[i] = &EmployeeNode{Subordinates: []*EmployeeNode{}}
    }
    for idx,managerId := range manager {
        if managerId == -1 {
            continue
        }
        currentManager := employeeNodes[managerId]
        currentManager.InformTime = informTime[managerId]
        currentManager.Subordinates = append(employeeNodes[managerId].Subordinates, employeeNodes[idx])
    }
	return deepestEmployeeCall(employeeNodes[headID], 0)
}

func deepestEmployeeCall(root * EmployeeNode, currentTime int) int {
    if len(root.Subordinates) == 0 {
        return currentTime
    }
    var deepestCall int
    for _, subordinate := range root.Subordinates {
        deepestCall = max(deepestCall, deepestEmployeeCall(subordinate, currentTime + root.InformTime))
    }
    return deepestCall
}

