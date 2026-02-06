package cache

/*
 * Reflection
 *
 * 10. Set - task 4 - cartesian product
 *
 * Recursion comes as always as the most elegant solution. I solved it imperatively where we just iterate in 2 for loops and put the ordered pair into the resulting set.
 * But this approach is obviously more elegant and native to math :)
 *
 * 10. Set - task 6 - multi bag
 *
 * There was not that much what could go wrong, but in the BagEntry I also used the counter and incremented it when we added the element which already existed and decremented it
 * in the removal. Of course when counter gets 0, then we remove the entry completely.
 *
 * 11. Bloom Filter - task 2 - merge filters
 *
 * Correctly implemented using bitwise OR. Also correcly identified that the probability of false positives increases because merged filters have more set bits.
 *
 * 11. Bloom Filter - task 3 - bloom filter with removal
 *
 * Read an article about this region approach to removing where we do not use counters, but have n regions and split the whole set of rest bits into exactly n regions.
 * Each region also occupies one bit in the bit array to makr collision. When we have a collision in some region, we do not delete from this region in the delete operation.
 * Counting filters are also interesting, but in this case we do not save us from deleting false positives which will make the structure of the filter bad.
 *
 * 11. Bloom Filter - task 4 - recover bloom filter
 *
 * I did not actually think that we could limit the potential input range to some more less limited set of values based on the current domain. Metrics of data similarity and statistical method
 * is a cool solution. If we can not solve something directly - use heuristics aka statistics, I should remember that. My version was that it is impossible for an infinite input set
 * and that is is an NP hard problem :)
 */
