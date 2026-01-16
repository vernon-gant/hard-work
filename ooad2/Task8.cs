// Microsoft Docs Examples - My Comments


// Covariance.
IEnumerable<string> strings = new List<string>();
// In this case string inherits from object and because the IEnumerable container is covariant to the type parameter T
// we can easily assign a list of string to a list of objects, because every string is an object and all operations
// on objects are supported by strings.
IEnumerable<object> objects = strings;

// Contravariance.
static void SetObject(object o) { }
Action<object> actObject = SetObject;
// Action is contravariant to type T
// With contravariance it is a bit trickier as here we have a reversed inheritance chain. It is safe
// to have an instance typed with more general type in a variable which expects more concrete type.
// Why so? Because every string is an object and it is safe to pass to a delegate which expects an object a string
Action<string> actString = actObject;

// And something like this is not safe!
static void SetString(string s) { }
Action<object> actObject = SetString;

// In that case we can not be sure that we pass a string compatible type to the delegate which expects
// an object because we can also pass a Cat for example and signature would allow that!