global using FluentValidation;
global using Microsoft.AspNetCore.Http.HttpResults;
global using Microsoft.EntityFrameworkCore;
global using System.Security.Claims;
global using Api.Common;
global using Api.Common.Extensions;
global using Api.Common.Results;
global using Api.Data;
global using Api.Data.Models;
using Api;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting web application");
    var builder = WebApplication.CreateBuilder(args);
    builder.AddServices();
    var app = builder.Build();
    await app.Configure();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}