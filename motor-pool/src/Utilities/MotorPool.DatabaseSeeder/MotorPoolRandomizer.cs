using System.Text;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;

namespace MotorPool.DatabaseSeeder;

public interface MotorPoolRandomizer
{
    int FromRange(int min, int max);

    string MotorVIN();

    List<T> GetSample<T>(List<T> collection);

    List<T> GetSample<T>(List<T> collection, double successProbability);
}

public class PseudoMotorPoolRandomizer : MotorPoolRandomizer
{
    public int FromRange(int min, int max) => min + Random.Shared.Next() % (max - min + 1);

    public string MotorVIN()
    {
        const string possibleChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        ContinuousUniform possibleCharsUniform = new(0, possibleChars.Length - 1);

        StringBuilder stringBuilder = new();

        for (int i = 0; i < 17; i++) stringBuilder.Append(possibleChars[(int)possibleCharsUniform.Sample()]);

        return stringBuilder.ToString();
    }

    public List<T> GetSample<T>(List<T> collection)
    {
        int sampleSize = collection.Count < 1000 ? FromRange(1, collection.Count - 1) : FromRange(1, 1000);
        bool[] combination = Combinatorics.GenerateCombination(collection.Count, sampleSize);

        return combination.Select((result, index) => (result, index))
                          .Where(resultIndexTuple => resultIndexTuple.result)
                          .Select(resultIndexTuple => collection[resultIndexTuple.index])
                          .ToList();
    }

    public List<T> GetSample<T>(List<T> collection, double successProbability)
    {
        Bernoulli bernoulli = new(successProbability);

        return bernoulli.Samples()
                        .Take(collection.Count)
                        .Select((result, index) => (result, index))
                        .Where(resultIndexTuple => resultIndexTuple.result == 1)
                        .Select(resultIndexTuple => collection[resultIndexTuple.index])
                        .ToList();
    }
}