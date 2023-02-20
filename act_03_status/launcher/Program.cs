using System.Diagnostics;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var port = Environment.GetEnvironmentVariable("PORT");
var appPort = Environment.GetEnvironmentVariable("APP_PORT");

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddHealthChecks()
  .AddCheck<AppHealthCheck>("Main App");
builder.Services.AddHealthChecksUI(options => {
    options.AddHealthCheckEndpoint("App",$"http://localhost:{port}/api/health");
    options.SetEvaluationTimeInSeconds(5);
  })
  .AddInMemoryStorage();

var app = builder.Build();

app.MapHealthChecks("/api/health", new HealthCheckOptions() {
  ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecksUI(options => {
  options.UIPath = "/health";
  options.AsideMenuOpened = false;
  options.PageTitle = "App Status";
});

RunChildApp(app.Services.GetRequiredService<ILogger<Program>>());

app.Run($"http://127.0.0.1:{port}");

void RunChildApp(ILogger logger) {
  var program = Process.Start(new ProcessStartInfo {
    WorkingDirectory = "../app",
    EnvironmentVariables = {
      ["PORT"] = appPort
    },
    Arguments = "run",
    FileName = "dotnet"
  });

  if (program is null) return;
  
  program.EnableRaisingEvents = true;
  program.Exited += async (sender, eventArgs) => {
    if (program.ExitCode == 0) {
      return;
    }
    logger.LogError("Child app closed. Restarting in 10 seconds");
    await Task.Delay(10000);
    logger.LogInformation("Child app restarting");

    program.Start();
  };
}

public class AppHealthCheck : IHealthCheck {
  private readonly HttpClient _client;
  private readonly string _appPort;
  public AppHealthCheck(HttpClient client) {
    _client = client;
    _appPort = Environment.GetEnvironmentVariable("APP_PORT") ?? "";
  }

  public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken()) {
    var response = await _client.GetAsync($"http://127.0.0.1:{_appPort}", cancellationToken);

    if (!response.IsSuccessStatusCode) {
      return HealthCheckResult.Unhealthy();
    }
    
    return HealthCheckResult.Healthy();
  }
}