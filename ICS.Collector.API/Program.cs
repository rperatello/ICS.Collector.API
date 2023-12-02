using AutoMapper;
using ICS.Collector.API.Mapper;
using ICS.Collector.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region BackgroundService

builder.Services.AddSingleton<CollectorBackgroundService>();
builder.Services.AddHostedService<CollectorBackgroundService>(provider => provider.GetService<CollectorBackgroundService>());
#endregion

#region Mappers

builder.Services.AddSingleton(new MapperConfiguration(config =>
    config.ToIpcaDto()
).CreateMapper());

#endregion

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

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

// Ativa o sistema de roteamento
app.UseRouting();

//app.UseHttpsRedirection();

// Ativa a política CORS configurada em ConfigureServices
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
