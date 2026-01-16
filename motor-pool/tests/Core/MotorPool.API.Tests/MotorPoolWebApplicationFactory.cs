using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace MotorPool.API.Tests;

public class MotorPoolWebApplicationFactory(string connectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
                                                                             {
                                                                                 { "ConnectionStrings:DefaultConnection", connectionString },
                                                                                 { "JWTConfig:Key", AuthHelper.SecretKey },
                                                                                 { "JWTConfig:Issuer", AuthHelper.Issuer },
                                                                                 { "JWTConfig:Audience", AuthHelper.Audience }
                                                                             })
                                                      .Build();

        builder.UseConfiguration(configuration);

        builder.ConfigureLogging(logging => { logging.AddSerilog(new LoggerConfiguration().WriteTo.Console().MinimumLevel.Debug().CreateLogger()); });
    }
}