package doubly_linked_list

/*
* 1. Linked lists - task number 9 - reverse linked list
*
* ! For all tasks below I will use the LinkedList2 as a normal linked list without the prev pointer !
*
* Solved this task already some time ago on leet code, but wanted to do now using recursion, all solution with recusion are damn elegant. In essence we just need
* to pass the prev pointer to the recursive function and then reorganize them. The time complexity is still O(n). Space complexity is again arguable - if we count
* stack allocations then O(n) but without any loops this looks more elegant :)
*/

func (list *LinkedList2) Reverse() {
	if list.head == nil || list.head == list.tail {
		return
	}

	next := list.head.next
	list.tail = list.head
	list.tail.next = nil
	list.head = Reverse(list.head, next)
}

func Reverse(prev, head * Node) *Node {
	if head == nil {
		return prev
	}

	next := head.next
	head.next = prev
	prev = head
	return Reverse(prev, next)
}

/*
* 2. Doubly Linked lists - task number 10  - detect cycles in the linked list
*
* Okay here the whole task boils down to one loop and is really more intuitive in imperative version. Recursive variant required more checks. But here just a simple
* turtle hare algorithm where we keep slow and fast pointers and when they meet then there is a cycle. Like in real life :) The complexity is still O(n).
*/

func (list *LinkedList2) IsCyclic() bool {
	slow, fast := list.head, list.head
	for fast != nil && fast.next != nil {
		slow = slow.next
		fast = fast.next.next
		if slow == fast {
			return true
		}
	}

	return false
}

/*
* 2. Doubly Linked lists - task number 11 + 12  - sort linked list and merge two sorted lists
*
* I also remember I did this task on leet code a long time ago and actually remembered the solution. Not surprisignly that you added the previous task for this one :) because we actually
* do the merge sort here and in order to split the linked list into two halves we can also use the hare tortoise algorithm. Without `temp` variable, however, we would not be able to split
* linked list with 2 elements. In this case the doubly linked list would be great :) So we keep splitting the list until there are either no elements or just 1 and then merge them together
* into a sorted list. Also recursively! I liked this idea that we just take from both lists depending on their heads value the correct head and then just set the `head.next` in another recursive
* call to this function. Amazing... The complexity of this solution is O(n * log(n))
*/
func (list *LinkedList2) Sort() {
	list.head = SortRec(list.head)

	temp := list.head

	for ; temp != nil && temp.next != nil; temp = temp.next {}

	list.tail = temp
}

func SortRec(head *Node) *Node {
	if head == nil || head.next == nil {
		return head
	}

	var slow, fast, temp *Node = head, head, nil

	for fast != nil && fast.next != nil {
		temp = slow
		slow = slow.next
		fast = fast.next.next
	}

	temp.next = nil

	left := SortRec(head)
	right := SortRec(slow)

	return mergeLists(left, right)
}

func mergeLists(list1, list2 *Node) *Node {
	if list1 == nil {
		return list2
	}

	if list2 == nil {
		return list1
	}

	if list1.value <= list2.value {
		list1.next = mergeLists(list1.next, list2)
		return list1
	}

	list2.next = mergeLists(list1, list2.next)
	return list2
}

/*
* 2. Doubly Linked lists - task number 13 - dummy nodes
*
* In go we do everything differently as I see and because there is no direct inheritance. So we just keep two sentinel nodes head and tail which carry no data. And the actual list is in the range
* head.next and tail.prev. As said this brings us to the state where each node has both neighbors :) The only thing I do not like is that we do not have in Go the normal constructor
* and for this reason have to alway call init to make sure the invariant for this technique holds. But the rest is pretty straightforware and we have way less if checks.
*/

func (l *LinkedList2) init() {
	if l.head != nil { return }
	l.head, l.tail = &Node{}, &Node{}
	l.head.next, l.tail.prev = l.tail, l.head
}

func (l *LinkedList2) AddInTail2(item Node) {
	l.init()
	p := l.tail.prev
	item.prev, item.next = p, l.tail
	p.next = &item
	l.tail.prev = &item
	l.count++
}

func (l *LinkedList2) InsertFirst2(first Node) {
	l.init()
	n := l.head.next
	first.prev, first.next = l.head, n
	l.head.next = &first
	n.prev = &first
	l.count++
}

func (l *LinkedList2) Insert2(after *Node, add Node) {
	l.init()
	if after == nil {
		n := l.head.next
		add.prev, add.next = l.head, n
		l.head.next = &add
		n.prev = &add
		l.count++
		return
	}
	// assume 'after' is in the list per spec
	n := after.next
	add.prev, add.next = after, n
	after.next = &add
	n.prev = &add
	l.count++
}

func (l *LinkedList2) Delete2(val int, all bool) {
	l.init()
	for n := l.head.next; n != l.tail; {
		if n.value == val {
			next := n.next
			// unlink n
			n.prev.next = n.next
			n.next.prev = n.prev
			n.prev, n.next = nil, nil
			l.count--
			if !all { return }
			n = next
			continue
		}
		n = n.next
	}
}