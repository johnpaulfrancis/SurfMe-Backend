using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SurfMe.Data;
using SurfMe.Models;
using SurfMe.Service;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Configure Serilog to log to a file
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/errors.txt", rollingInterval: RollingInterval.Day) // daily log files
    .CreateLogger();
builder.Host.UseSerilog();

// Add CORS service
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularLocalhost",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "https://surfmeappservic-cqg5hdbegfdqb2f0.centralindia-01.azurewebsites.net", "https://yellow-meadow-070a22d00.1.azurestaticapps.net") // Angular dev URL or production url
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(builder =>
{
    builder.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SurfMe API",
        Version = "v1"
    });
});

//JWT
builder.Services.AddSingleton<JWTService>();
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    };
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Register the EncryptDecryptService with configuration
builder.Services.Configure<EncryptDecryptServiceModel>(builder.Configuration.GetSection("EncryptionSettings"));
// Register the EncryptDecrypt as a singleton service
builder.Services.AddSingleton<EncryptDecryptService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(builder =>
    {
        builder.SwaggerEndpoint("/swagger/v1/swagger.json", "SurfMe API V1");
        builder.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
//}

app.UseMiddleware<ErrorHandlingMiddleware>(); // register our middleware
app.UseMiddleware<APILoggerService>(); //to log API calls

app.UseCors("AllowAngularLocalhost");
app.UseHttpsRedirection();

// Add auth middleware
app.UseAuthentication();
app.UseAuthorization();

// Apply migrations on startup -- for publishing in azure
//if (!app.Environment.IsDevelopment())
//{
//    using (var scope = app.Services.CreateScope())
//    {
//        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//        db.Database.Migrate(); // This applies all pending migrations
//    }
//}
app.UseRouting();
app.MapControllers();


app.Run();
