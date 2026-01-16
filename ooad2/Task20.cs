// Functional variation inheritance

public class Vehicle
{
    // without virtual keyword functional variation inheritance does not work in C#
    public virtual void Move()
    {
        Console.WriteLine("The vehicle moves forward.");
    }
}

public class Airplane : Vehicle
{
    public override void Move()
    {
        Console.WriteLine("The airplane flies through the sky.");
    }
}

// Type variation inheritance
// Simple example with calculator where we say want to add in the derived class a new method add for doubles.

public class Calculator
{
    public int add(int a, int b) => a + b;
}

public class AdvancedCalculator
{
    public double add(double a, double b) => a + b;
}

// Reification inheritance

// Example taken from FluentValidationLibrary. Here we have an abstract ComparisonValidator which is partially implemented. It leaves IsValid and Comparison for subclasses.
// An implementation example is a LessThanValidator which reifies the validator into an implementable entity.

public abstract class AbstractComparisonValidator<T, TProperty> : PropertyValidator<T,TProperty>, IComparisonValidator where TProperty : IComparable<TProperty>, IComparable {
	private readonly Func<T, (bool HasValue, TProperty Value)> _valueToCompareFuncForNullables;
	private readonly Func<T, TProperty> _valueToCompareFunc;
	private readonly string _comparisonMemberDisplayName;

    ...

	public (bool HasValue, TProperty Value) GetComparisonValue(ValidationContext<T> context) {
		if(_valueToCompareFunc != null) {
			var value = _valueToCompareFunc(context.InstanceToValidate);
			return (value != null, value);
		}
		if (_valueToCompareFuncForNullables != null) {
			return _valueToCompareFuncForNullables(context.InstanceToValidate);
		}

		return (ValueToCompare != null, ValueToCompare);
	}

	public abstract bool IsValid(TProperty value, TProperty valueToCompare);

	public abstract Comparison Comparison { get; }

	public MemberInfo MemberToCompare { get; }

	public TProperty ValueToCompare { get; }
}

public class LessThanValidator<T, TProperty> : AbstractComparisonValidator<T, TProperty> where TProperty : IComparable<TProperty>, IComparable {
	public override string Name => "LessThanValidator";

	public override bool IsValid(TProperty value, TProperty valueToCompare) {
		if (valueToCompare == null)
			return false;

		return value.CompareTo(valueToCompare) < 0;
	}

	public override Comparison Comparison => Comparison.LessThan;

	protected override string GetDefaultMessageTemplate(string errorCode) {
		return Localized(errorCode, Name);
	}
}

// Structural inheritance

// Example again taken from the same library. Very representative for our case - we have an AssemblyScanner with one maina abstraction and purprose for scannig current Assembly for probably
// validators. But we also want it to be enumerable so that we can iterate over it with foreach. For that we add a qualitetively new abstraction to this class and make it also implement
// IEnumerable.

public class AssemblyScanner : IEnumerable<AssemblyScanner.AssemblyScanResult> {
	readonly IEnumerable<Type> _types;

	public AssemblyScanner(IEnumerable<Type> types) {
		_types = types;
	}

	public static AssemblyScanner FindValidatorsInAssembly(Assembly assembly, bool includeInternalTypes = false) {
		return new AssemblyScanner(includeInternalTypes ? assembly.GetTypes() : assembly.GetExportedTypes());
	}

	public static AssemblyScanner FindValidatorsInAssemblies(IEnumerable<Assembly> assemblies, bool includeInternalTypes = false) {
		var types = assemblies.SelectMany(x => includeInternalTypes ? x.GetTypes() : x.GetExportedTypes()).Distinct();
		return new AssemblyScanner(types);
	}

	public static AssemblyScanner FindValidatorsInAssemblyContaining<T>() {
		return FindValidatorsInAssembly(typeof(T).Assembly);
	}

	public static AssemblyScanner FindValidatorsInAssemblyContaining(Type type) {
		return FindValidatorsInAssembly(type.Assembly);
	}

	private IEnumerable<AssemblyScanResult> Execute() {
		var openGenericType = typeof(IValidator<>);

		var query = from type in _types
			where !type.IsAbstract && !type.IsGenericTypeDefinition
			let interfaces = type.GetInterfaces()
			let genericInterfaces = interfaces.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericType)
			let matchingInterface = genericInterfaces.FirstOrDefault()
			where matchingInterface != null
			select new AssemblyScanResult(matchingInterface, type);

		return query;
	}

	public void ForEach(Action<AssemblyScanResult> action) {
		foreach (var result in this) {
			action(result);
		}
	}

	public IEnumerator<AssemblyScanResult> GetEnumerator() {
		return Execute().GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}
}