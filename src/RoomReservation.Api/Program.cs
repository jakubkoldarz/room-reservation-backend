using Microsoft.EntityFrameworkCore;
using RoomReservation.Api.Extensions;
using RoomReservation.Core.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCore(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
        Console.WriteLine("--> Migration executed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"--> Migration error: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Room Reservation API";
        options.Theme = ScalarTheme.Default;
        options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.Run();

