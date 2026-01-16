package recursion

import (
	"fmt"
	"log"
	"math"
	"os"
	"path/filepath"
)

// 1.
func MyPow2(x float64, n int) float64 {
	if n == 0 {
		return 1
	}
	if n < 0 {
		return (1 / x) * MyPow2(x, n+1)
	}
	n--
	return x * MyPow2(x, n)
}

// 2. + Assume n is positive
func SumOfDigits(n int) int {
	if n == 0 {
		return 0
	}
	lastDigit := n % 10
	n /= 10
	return lastDigit + SumOfDigits(n)
}

// 3.
func ListLength(numbers *[]int) int {
	if len(*numbers) == 0 {
		return 0
	}
	*numbers = (*numbers)[1:]
	return 1 + ListLength(numbers)
}

// 4. + Assume that string must not be trimmed or fine tuned and that all characters are low
func IsPalindrome(testString string) bool {
	if len(testString) == 1 || len(testString) == 0 {
		return true
	}
	if testString[0] != testString[len(testString)-1] {
		return false
	}
	nextString := testString[1 : len(testString)-1]
	return IsPalindrome(nextString)
}

// 5.
func PrintEvenNumbers(numbers []int) {
	printEvenNumbers(numbers,0)
}

func printEvenNumbers(numbers []int, index int) {
	if len(numbers) == index {
		return
	}
	currentElement := numbers[index]
	if currentElement% 2 == 0 {
		fmt.Println(currentElement)
	}
	index++
	printEvenNumbers(numbers,index)
}

// 6.
func PrintEvenIndexNumbers(nums []int) {
	printEvenIndexNumbers(&nums, 0)
}

func printEvenIndexNumbers(numbers * []int, idx int) {
	if idx == len(*numbers) {
		return
	}
	if idx % 2 == 0 {
		fmt.Println((*numbers)[idx])
	}
	idx++
	printEvenIndexNumbers(numbers, idx)
}

// 7.
func FindSecondLargestNumber(numbers []int) int {
	return findSecondLargestNumber(numbers, numbers[0], math.MinInt64, 0)
}

func findSecondLargestNumber(numbers []int, maxValue, secondMax, currentIndex int) int {
	if currentIndex == len(numbers) {
		return secondMax
	}
	currentNumber := numbers[currentIndex]
	if currentNumber > maxValue {
		secondMax = maxValue
		maxValue = currentNumber
	} else {
		secondMax = max(currentNumber, secondMax)
	}
	currentIndex++
	return findSecondLargestNumber(numbers, maxValue, secondMax, currentIndex)
}

// 8.
func RecursiveFileSearch(dirName string) []os.DirEntry {
	return recursiveFileSearch(dirName)
}

func recursiveFileSearch(dirName string) []os.DirEntry {
	var folderEntries []os.DirEntry
	entries, err := os.ReadDir(dirName)
	if err != nil {
		log.Println("Failed to read directory:", err)
	}
	for _, entry := range entries {
		if !entry.IsDir() {
			folderEntries = append(folderEntries,entry)
			continue
		}
		path := filepath.Join(dirName, entry.Name())
		folderEntries = append(folderEntries,recursiveFileSearch(path)...)
	}
	return folderEntries
}


