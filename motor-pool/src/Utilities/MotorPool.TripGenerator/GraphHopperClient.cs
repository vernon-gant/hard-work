using System.Net.Http.Json;
using System.Text.Json;
using OneOf;
using OneOf.Types;
using Polly;
using Polly.Retry;

namespace MotorPool.TripGenerator;

public class GraphHopperClient
{
    private const string _baseUrl = "https://graphhopper.com/api/1/route";
    private static readonly HttpClient _httpClient = new();
    private readonly string _apiKey;

    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public GraphHopperClient(string apiKey)
    {
        _apiKey = apiKey;

        _retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(response => response.StatusCode == (System.Net.HttpStatusCode)429)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (_, timespan, retryCount, _) =>
                {
                    Console.WriteLine($"Retry {retryCount} for 429 Too Many Requests. Waiting {timespan.TotalSeconds} seconds...");
                });
    }

    public async Task<OneOf<List<Point>, Error>> GetRouteAsync(Point p1, Point p2)
    {
        try
        {
            string url = $"{_baseUrl}?point={p1}&point={p2}&profile=car&key={_apiKey}&calc_points=true&instructions=false&points_encoded=false";

            HttpResponseMessage response = await _retryPolicy.ExecuteAsync(() => _httpClient.GetAsync(url));

            if (!response.IsSuccessStatusCode) return new Error();

            GraphHopperResponse responseBody = await response.Content.ReadFromJsonAsync<GraphHopperResponse>(_jsonSerializerOptions);

            return responseBody.Paths[0].Points.Coordinates
                .Select(point => new Point(point[1], point[0]))
                .ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred while getting the route: {e.Message}");
            return new Error();
        }
    }
}

public struct GraphHopperResponse
{
    public List<Path> Paths { get; set; }
}

public struct Path
{
    public double Distance { get; set; }
    public PointsInfo Points { get; set; }
}

public struct PointsInfo
{
    public List<List<double>> Coordinates { get; set; }
}