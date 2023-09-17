using CreditCardService;
using CreditCardService.Actors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaprClient();
builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<CreditCards>();
    options.Actors.RegisterActor<CreditCard>();
});

var app = builder.Build();

app.MapGet("/", () => "CreditCardService is running 🚀");

app.MapGroup("api").AddRoutes();

app.MapActorsHandlers();

app.Run();