using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.Services;


var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddControllers();
builder
    .Services
    .AddOpenApi(options =>
    {
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Info ??= new();
            document.Info.Title = "Task API";
            document.Info.Version = "v1";
            return Task.CompletedTask;
        });
    });
builder
    .Services
    .AddDbContext<AppDbContext>(
        options =>
        {
            options.UseNpgsql(
                builder
                    .Configuration
                    .GetConnectionString("TaskApiDatabase")
                );
        });

builder
    .Services
    .AddScoped<ITaskService, TaskService>();
builder
    .Services
    .AddScoped<ITaskQueryBuilder, TaskQueryBuilder>();

builder
    .Services
    .AddProblemDetails(
        options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context
                    .ProblemDetails
                    .Extensions["traceId"] = context
                    .HttpContext
                    .TraceIdentifier;
            };
            
        });

builder
    .Services
    .AddExceptionHandler<TaskApi.Errors.GlobalExceptionHandler>();



var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();

public partial class Program { }

