using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using wep_api_food.Services.Implementations;
using wep_api_food_delivery.Data;
using wep_api_food_delivery.Factories;
using wep_api_food_delivery.Services.Implementations;
using wep_api_food_delivery.Services.Intefaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["GrpcAuth"],
            ValidAudience = builder.Configuration["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SecretKey"]))
        };
    });
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    );

builder.Services.AddHostedService<MessageBusSub>();
builder.Services.AddSingleton<EventProcessor>();
builder.Services.AddScoped<IAuthDataClient, AuthDataClient>();
builder.Services.AddHttpClient<HttpClientSender>();
builder.Services.AddSingleton<IDbContextFactory, DbContextFactory>(provider =>
{
    var options = provider.GetRequiredService<DbContextOptions<ApplicationDbContext>>();
    return new DbContextFactory(options);
});

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var context = app.Services.GetRequiredService<ApplicationDbContext>();
context.Database.Migrate();
SeedData.Initialize(context);

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
Task.Delay(10000).Wait();
app.Run();
