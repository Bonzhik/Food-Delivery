using Microsoft.OpenApi.Models;
using web_api_food_auth.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddGrpc();

var app = builder.Build();

app.UseRouting();
app.MapGrpcService<GrpcTokenService>();
app.MapControllers();
app.Run();
