var port = Environment.GetEnvironmentVariable("PORT");

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();
app.UseFileServer();

app.MapGet("/crash", () => Environment.Exit(1));

app.Run($"http://127.0.0.1:{port}");