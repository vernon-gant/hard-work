package bloom_filter

/*
* 11. Bloom filter - task 2 - merge filters
* 
* When we merge filters, the amount of n grows and the exponent factor shrinks, so for 10 elements with filter length 32 we get P = 0.6931 ^ 3.2 ~ 30% and doubling
* the amount of elements leads to 55% of false positives.
*/

func Merge(filters ...BloomFilter) BloomFilter {
	merged := filters[0].bit_array
	
	for i := 1; i < len(filters); i++ {
		merged |= filters[i].bit_array
	}
	
	return BloomFilter{filter_len: filters[0].filter_len, bit_array: merged}
}

/*
* 11. Bloom filter - task 3 - remove from Bloom filter
* 
* First idea was to use counter approach, but as I also then saw in the paper - a very good one!!! - https://arxiv.org/pdf/1005.0352 - we can not really avoid breaking the structure when we have deletions. False positives
* can not be avoided, this is just how Bloom Filter works. But we can try to avoid breaking the structure on colliding elments. This paper suggests to reserve `r` regions for collision bits and store there whethe some region has collision or not.
* If it has then during deletion we do not delete that bit and just delete the bit in the no collision region. Thus we will not be able to find an element afterwards, if at least one of its bits is deleted. They also reason
* about probabilities, how to choose r and many other stuff. I wish I had this at the unversity.... But I just made a small example from their work as they showed with 32 bits and 4 regions where the top 4 bits are collision
* bits and the rest is for usage. Pretty nice :)
*/

type DeletableBloomFilter struct {
	bit_array  uint32
}

func (f *DeletableBloomFilter) Hash1(s string) uint32 {
	sum := 0
	for _, char := range s {
		code := int(char)
		sum = ((sum * 17) + code) % 28
	}
	return uint32(sum)
}

func (f *DeletableBloomFilter) Hash2(s string) uint32 {
	sum := 0
	for _, char := range s {
		code := int(char)
		sum = ((sum * 223) + code) % 28
	}
	return uint32(sum)
}

func (f*DeletableBloomFilter) Add(s string) {
	firstHash, secondHash := f.Hash1(s), f.Hash2(s)
	
	if f.bit_array & (1 << firstHash) != 0 {
		region := firstHash / 7
		f.bit_array |= (1 << (28 + region))
	}
	
	if f.bit_array & (1 << secondHash) != 0 {
		region := secondHash / 7
		f.bit_array |= (1 << (28 + region)) 
	}
	
	f.bit_array |= (1 << firstHash)
	f.bit_array |= (1 << secondHash)
}

func (f*DeletableBloomFilter) IsValue(s string) bool {
	firstHash, secondHash := f.Hash1(s), f.Hash2(s)
	return f.bit_array & (1 << firstHash) != 0 && f.bit_array & (1 << secondHash) != 0
}

func (f*DeletableBloomFilter) Remove(s string) {
	firstHash, secondHash := f.Hash1(s), f.Hash2(s)
	firstRegion, secondRegion := firstHash / 7, secondHash / 7
	
	if f.bit_array & (1 << (28 + firstRegion)) == 0 {
		f.bit_array  &^= (1 << firstHash)
	}
	
	if f.bit_array & (1 << (28 + secondRegion)) == 0 {
		f.bit_array  &^= (1 << secondHash)
	}
}

/*
* 11. Bloom filter - task 4 - restore initial element set
* 
* I do not really see a way to solve this problems, because in case of strings we have inifite amount of strings which can be input and thus produce same hash(bit) values. Yes, we have the bitmap and we can
* start brute forcing and trying to recreate the same bitmap, but the outcome will not be THE solution, it will just be a set of strings, which also produced same bit values. Moreover their amount may differ,
* say 5 strings populated 4 bits due to collisions with 2 hash functions and then another 2 strings populated same 4 bits using 2 same hash functions. NP hard problem :)
*/


/*
 * Reflection
 * 
 * 09. Dicitonary - task 5 - dictionary with ordered list
 * 
 * Brilliant idea not to put the value into the correct sorted position of the value list aka same position of the key in the sorted key list, but just to append and point from the key list to the slot where value is stored.
 * For some reason I forgot about pointers which i used in dict implementation taken from C# where they do pretty same thing but for linea probing. I was retrieving the inserted idx from keys and then shifed elements in values
 * to insert the new value into correct position.
 */