using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;
using OutreachFlow.Api.Endpoints;
using OutreachFlow.Application.DependencyInjection;
using OutreachFlow.Infrastructure.DependencyInjection;
using OutreachFlow.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OutreachFlowDbContext>();
    await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}

app.MapGet("/api/v1/health", () =>
    Results.Ok(new
    {
        status = "Healthy",
        utcNow = DateTimeOffset.UtcNow
    }))
    .WithName("GetHealth")
    .WithOpenApi();

app.MapOrganizationEndpoints();
app.MapContactEndpoints();
app.MapTagEndpoints();
app.MapSenderProfileEndpoints();
app.MapEmailTemplateEndpoints();

app.Run();

public partial class Program;
