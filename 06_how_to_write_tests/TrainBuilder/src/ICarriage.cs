namespace TrainBuilder;

/// <summary>
/// Carriage is added to the <see cref="ITrain"/>. Each carriage has a different marker, normally it is
/// the first letter of the carriage type. Capacity is readonly and is set on the creation.
/// </summary>
public interface ICarriage
{
    // Queries

    int Capacity { get; }

    char Marker { get; }
}