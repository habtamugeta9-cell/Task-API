using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddConnections();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<ITaskService, TaskService>();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql
        (builder.
            Configuration.
            GetConnectionString("TaskApiDatabase")
        );
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();