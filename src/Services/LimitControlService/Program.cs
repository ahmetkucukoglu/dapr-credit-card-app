using LimitControlService;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDaprClient();

var app = builder.Build();

app.MapGet("/", () => "LimitControlService is running 🚀");

app.MapGroup("api").AddRoutes();

app.Run();