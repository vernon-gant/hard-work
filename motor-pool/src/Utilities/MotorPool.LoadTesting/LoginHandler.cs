using System.Text;
using System.Text.Json;
using MotorPool.LoadTesting.Configuration;

namespace MotorPool.LoadTesting;

public class LoginHandler
{
    private readonly AppSettings _appSettings;
    private readonly HttpClient _httpClient;

    public LoginHandler(AppSettings appSettings)
    {
        _appSettings = appSettings;
        _httpClient = new HttpClient
                      {
                          BaseAddress = new Uri(_appSettings.BaseUrl)
                      };
    }

    public async Task<string> GetTokenAsync()
    {
        var loginDto = new
                       {
                           email = _appSettings.ManagerLogin,
                           password = _appSettings.ManagerPassword
                       };

        var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/auth/login", content);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var responseData = JsonSerializer.Deserialize<LoginResponse>(responseString, JsonSettings.SerializerOptions);

        if (responseData == null || string.IsNullOrEmpty(responseData.Token))
            throw new Exception("Failed to retrieve the token");

        return responseData.Token;
    }

    private class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}