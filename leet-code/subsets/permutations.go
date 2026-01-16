package subsets

func Permute(nums []int) [][]int {
    return permuteRec(nums,0, [][]int{})
}

func permuteRec(currentPermutation []int, swapIdx int, result [][]int) [][]int {
    if swapIdx == len(currentPermutation) - 1 {
        result = append(result, currentPermutation)
        return result
    }
	if swapIdx == len(currentPermutation) - 2 {
		result = append(result, currentPermutation)
		result = append(result, swap(currentPermutation, swapIdx, swapIdx + 1))
		return result
	}
    for i := swapIdx; i < len(currentPermutation); i++ {
        result = permuteRec(swap(currentPermutation, swapIdx, i), swapIdx + 1, result)
    }
    return result
}

func swap(list []int, idx1, idx2 int) []int {
    newList := make([]int, len(list))
    copy(newList, list)
    newList[idx1], newList[idx2] = newList[idx2], newList[idx1] 
    return newList
}