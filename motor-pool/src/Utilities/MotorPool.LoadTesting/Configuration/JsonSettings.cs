using System.Text.Json;

namespace MotorPool.LoadTesting.Configuration;

public static class JsonSettings
{
    public static JsonSerializerOptions SerializerOptions => new()
                                                             {
                                                                 PropertyNameCaseInsensitive = true,
                                                                 PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                                                             };
}