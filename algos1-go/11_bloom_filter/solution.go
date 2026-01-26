package main

import "os"

type BloomFilter struct {
	filter_len int
	bit_array uint32
}

func Init() BloomFilter {
	return BloomFilter{
		filter_len: 32,
		bit_array:  0,
	}
}

func (f*BloomFilter) Hash1(s string) int {
	sum := 0
	for _, char := range s {
		code := int(char)
		sum = ((sum * 17) + code) % f.filter_len
	}
	return sum
}

func (f*BloomFilter) Hash2(s string) int {
	sum := 0
	for _, char := range s {
		code := int(char)
		sum = ((sum * 223) + code) % f.filter_len
	}
	return sum
}

func (f*BloomFilter) Add(s string) {
	firstHash, secondHash := f.Hash1(s), f.Hash2(s)
	f.bit_array |= (1 << firstHash)
	f.bit_array |= (1 << secondHash)
}

func (f*BloomFilter) IsValue(s string) bool {
	firstHash, secondHash := f.Hash1(s), f.Hash2(s)
	return f.bit_array & (1 << firstHash) != 0 && f.bit_array & (1 << secondHash) != 0
}
