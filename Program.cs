using WebApi.DataAcesss;
using WebApi.Middlewares;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApi.OptionsSetup;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Middlewares
builder.Services.AddTransient<ErrorHandlingMiddleware>();

// Add services to the container.
builder.Services.AddSingleton<Database>();
builder.Services.AddSingleton<UserDao>();
builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<LoginService>();
builder.Services.AddSingleton<JwtService>();


builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseAuthorization();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

app.Run();
