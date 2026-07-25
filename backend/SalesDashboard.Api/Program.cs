using Microsoft.EntityFrameworkCore;
using SalesDashboard.Application.Messaging;
using SalesDashboard.Infrastructure.Data;
using SalesDashboard.Infrastructure.Messaging;
using SalesDashboard.Api.Hubs;
using SalesDashboard.Api.Realtime;
using SalesDashboard.Application.Realtime;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SalesDashboardDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddControllers();

builder.Services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
builder.Services.AddHostedService<OrderCreatedConsumer>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "SalesDashboard:";
});
builder.Services.AddSignalR();
builder.Services.AddSingleton<IRealtimeNotifier, SignalRRealtimeNotifier>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SalesDashboard API V1");
    });
}

app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");
app.MapControllers();
app.MapHub<SalesDashboardHub>("/hubs/sales-dashboard");
app.Run();