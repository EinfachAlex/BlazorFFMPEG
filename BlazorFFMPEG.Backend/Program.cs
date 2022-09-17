using System.Text.Json.Serialization;
using BlazorFFMPEG.Backend.Database;
using BlazorFFMPEG.Backend.Modules.Jobs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
);

builder.Services.AddDbContext<databaseContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("blazorFFMPEG");
    options.UseNpgsql(connectionString);
    options.UseLazyLoadingProxies();
    QueueScannerJob.connectionString = connectionString;
    
    JobManager.getInstance().startJobThreads();
});

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

app.UseWebSockets();

app.Run();