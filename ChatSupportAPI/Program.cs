using ChatSupport.Persistence;
using ChatSupport.Application;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog;
using Serilog.Extensions.Logging;
using ChatSupport.WebAPI.Middleware;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{  
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key Authentication \r\n\r\n" +
                      "Example: \"XXLvx0dE0ufqNLNv\"",
        In = ParameterLocation.Header,
        Name = "x-api-key",
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
                },
                new string[] {}
        }
    });    
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ChatSupport v1",
        Description = "API to manage a Chat",        
        Contact = new OpenApiContact
        {
            Name = "Raydel Daniel Abreu Maceo",
            Url = new Uri("https://github.com/bryanrd87")
        }
    });
});

builder.Services.AddPersistence();
builder.Services.AddApplication(builder.Configuration);

builder.Services.AddHttpClient();

builder.Services.AddLogging();

builder.Services.AddSingleton<ILoggerFactory>(serviceProvider =>
{
    var loggerConfiguration = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}",
        theme: AnsiConsoleTheme.Code);

    var logger = loggerConfiguration.CreateLogger();

    return new SerilogLoggerFactory(logger, dispose: true);
});

builder.Services.AddScoped<ExceptionMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("all");

app.UseMiddleware<ApiKeyMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

InitialSeed.Initialize(app);

app.Run();
