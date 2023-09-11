using PRS.Collector.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region BackgroundService

builder.Services.AddSingleton<CollectorBackgroundService>();
builder.Services.AddHostedService<CollectorBackgroundService>(provider => provider.GetService<CollectorBackgroundService>());

#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
