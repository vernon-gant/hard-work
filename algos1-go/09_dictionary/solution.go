package main

import (
	"os"
	"strconv"
	"errors"
)

type NativeDictionary[T any] struct {
	size     int
	slots    []string
	values   []T
	occupied []bool
}

func Init[T any](sz int) NativeDictionary[T] {
	nd := NativeDictionary[T]{size: sz}
	nd.slots = make([]string, sz)
	nd.values = make([]T, sz)
	nd.occupied = make([]bool, sz)
	return nd
}

func (nd *NativeDictionary[T]) HashFun(value string) int {
	result := 0
	for i := 0; i < len(value); i++ {
		pow := 1
		for j := 0; j < len(value)-i; j++ {
			pow *= 26
		}
		result += (int(value[i]) * pow) % len(nd.slots)
	}
	return result % len(nd.slots)
}

func (nd *NativeDictionary[T]) IsKey(key string) bool {
	startIdx := nd.HashFun(key)
	idx := startIdx

	for {
		if !nd.occupied[idx] {
			return false
		}

		if nd.slots[idx] == key {
			return true
		}

		idx = (idx + 1) % len(nd.slots)

		if idx == startIdx {
			return false
		}
	}
}

func (nd *NativeDictionary[T]) Get(key string) (T, error) {
	startIdx := nd.HashFun(key)
	idx := startIdx
	var result T

	for {
		if !nd.occupied[idx] {
			return result, errors.New("key not found")
		}

		if nd.slots[idx] == key {
			return nd.values[idx], nil
		}

		idx = (idx + 1) % len(nd.slots)

		if idx == startIdx {
			return result, errors.New("key not found")
		}
	}
}

func (nd *NativeDictionary[T]) Put(key string, value T) {
	startIdx := nd.HashFun(key)
	idx := startIdx

	for nd.occupied[idx] && nd.slots[idx] != key {
		idx = (idx + 1) % len(nd.slots)

		if idx == startIdx {
			return
		}
	}

	nd.slots[idx] = key
	nd.values[idx] = value
	nd.occupied[idx] = true
}
