package k_way_merge

func merge(nums1 []int, m int, nums2 []int, n int)  {
	nums1Pointer, nums2Pointer := m-1, n-1
	for insertIdx := m + n - 1; nums2Pointer >= 0; insertIdx-- {
		if nums1Pointer >= 0 && nums1[nums1Pointer] > nums2[nums2Pointer] {
			nums1[insertIdx] = nums1[nums1Pointer]
			nums1Pointer--
		} else {
			nums1[insertIdx] = nums2[nums2Pointer]
			nums2Pointer--
		}
	}
}