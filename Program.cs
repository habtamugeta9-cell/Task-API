using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder
            .Configuration
            .GetConnectionString("TaskApiDatabase"));
});
builder.Services.AddScoped<ITaskService, TaskService>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
} 

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

