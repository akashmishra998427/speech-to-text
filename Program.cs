using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using speect_to_text_Api.Helpers;
using speect_to_text_Api.Services;
using speect_to_text_Api.Validators;
using System.Data;
using System.Text;
using Npgsql;


Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        $"LogManager/log_{DateTime.Now:yyyyMMdd_HHmmss}.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// validation regrastration

builder.Services.AddValidatorsFromAssemblyContaining<UserRegValidator>();

// connection string setup 
builder.Services.AddScoped<IDbConnection>(sp =>
{
    var connectionString =
        builder.Configuration.GetConnectionString("DefaultConnection");

    return new NpgsqlConnection(connectionString);
});


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
    };
});

// services
builder.Services.AddScoped<Iuser, User>();

// Helpers
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<CloudinaryHelper>();

// client URL

builder.Services.AddHttpClient("SpeechToText", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["SpeechToText:BaseUrl"]!
    );

    client.Timeout = TimeSpan.FromMinutes(5);
});

//CORS (Cross-Origin Resource Sharing)

builder.Services.AddCors(option =>
{
    option.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyHeader()
         .AllowAnyOrigin()
         .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
//default route for testing
app.MapGet("/", () => "Speech to text API is running successfully!");

app.Run();
