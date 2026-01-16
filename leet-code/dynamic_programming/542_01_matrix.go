package dynamic_programming

import "math"

func UpdateMatrix(mat [][]int) [][]int {
	result := baseResultMatrix(len(mat), len(mat[0]))
	shortest := make([][3]int, 0)
	for i, row := range mat {
		for j, value := range row {
			if value == 0 {
				shortest = append(shortest, [3]int{i, j, value})
				result[i][j] = 0
			}
		}
	}
	for len(shortest) > 0 {
		first := shortest[0]
		neighbors := getNeighbors(result, first)
		updateNeighbors(result, neighbors, first[2] + 1)
		shortest = append(shortest[1:], neighbors...)
	}

	return result
}

func baseResultMatrix(m, n int) [][]int {
	result := make([][]int, m)
	for i := 0; i < m; i++ {
		result[i] = make([]int, n)
		for j := 0; j < n; j++ {
			result[i][j] = math.MaxInt
		}
	}
	return result
}

func getNeighbors(mat [][]int, cell [3]int) [][3]int {
	result := make([][3]int, 0)
	if cell[0] != 0 && mat[cell[0] - 1][cell[1]] == math.MaxInt {
		result = append(result, [3]int{cell[0] - 1, cell[1], cell[2] + 1})
	}
	if cell[0] != len(mat)-1 && mat[cell[0] + 1][cell[1]] == math.MaxInt {
		result = append(result, [3]int{cell[0] + 1, cell[1], cell[2] + 1})
	}
	if cell[1] != 0 && mat[cell[0]][cell[1] - 1] == math.MaxInt {
		result = append(result, [3]int{cell[0], cell[1] - 1, cell[2] + 1})
	}
	if cell[1] != len(mat[0])-1 && mat[cell[0]][cell[1] + 1] == math.MaxInt {
		result = append(result, [3]int{cell[0], cell[1] + 1, cell[2] + 1})
	}
	return result
}

func updateNeighbors(mat [][]int, cells [][3]int, newDistance int) {
	for _, cell := range cells {
		cell[2] = newDistance
		mat[cell[0]][cell[1]] = newDistance
	}
}
