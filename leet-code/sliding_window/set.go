package sliding_window

type Set struct {
	hashMap map[interface{}]bool
}

func NewSet() *Set {
	s := new(Set)
	s.hashMap = make(map[interface{}]bool)
	return s
}

func (s *Set) Add(value interface{}) {
	s.hashMap[value] = true
}


func (s *Set) Delete(value interface{}) {
	delete(s.hashMap, value)
}

func (s *Set) Exists(value interface{}) bool {
	_, ok := s.hashMap[value]
	return ok
}