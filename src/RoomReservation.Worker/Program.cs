using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Repositories;
using RoomReservation.Core.Services;
using RoomReservation.Worker;
using RoomReservation.Worker.Jobs;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IJobHandler, SendEmailHandler>();
builder.Services.AddScoped<JobDispatcher>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
