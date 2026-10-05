using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskApi.Data;
using TaskApi.DTOs;


public sealed class TaskApiWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextOptions = services
                .Where(descriptor => descriptor.ServiceType == typeof(DbContextOptions<AppDbContext>))
                .ToList();
            foreach (var descriptor in dbContextOptions)
            {
                services.Remove(descriptor);
            }

            var dbContextOptionsConfiguration = services
                .Where(descriptor => descriptor.ServiceType == typeof(IDbContextOptionsConfiguration<AppDbContext>))
                .ToList();
            foreach (var descriptor in dbContextOptionsConfiguration)
            {
                services.Remove(descriptor);
            }

            services.RemoveAll<AppDbContext>();

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("task-api-integration-tests"));
        });
    }
}

public sealed class TasksApiIntegrationTests(TaskApiWebApplicationFactory factory) : IClassFixture<TaskApiWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        BaseAddress = new Uri("https://localhost")
    });

    private void ResetDatabase()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyCollection_WhenDatabaseIsEmpty()
    {
        ResetDatabase();

        var response = await _client.GetAsync("/api/tasks", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<PagedTaskResponse>(
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(body);
        Assert.Empty(body.Items);
        Assert.Equal(1, body.Page);
        Assert.Equal(20, body.PageSize);
        Assert.Equal(0, body.TotalCount);
        Assert.Equal(0, body.TotalPages);
    }

    [Fact]
    public async Task GetAll_ShouldHonorCreatedAtSortAlias()
    {
        ResetDatabase();

        await _client.PostAsJsonAsync("/api/tasks", new CreateTaskRequest
        {
            Title = "Zebra",
            Description = "Latest task"
        }, TestContext.Current.CancellationToken);

        await _client.PostAsJsonAsync("/api/tasks", new CreateTaskRequest
        {
            Title = "Alpha",
            Description = "Earliest task"
        }, TestContext.Current.CancellationToken);

        var response = await _client.GetAsync("/api/tasks?sort=createdAt", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<PagedTaskResponse>(
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(body);
        Assert.Equal(2, body.Items.Count);
        Assert.Contains(body.Items, item => item.Title == "Alpha");
        Assert.Contains(body.Items, item => item.Title == "Zebra");
    }

    [Fact]
    public async Task Post_ShouldCreateTaskAndReturn201Created()
    {
        ResetDatabase();

        var request = new CreateTaskRequest
        {
            Title = "Write integration tests",
            Description = "Cover the task API end-to-end"
        };

        var response = await _client.PostAsJsonAsync("/api/tasks", request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var location = response.Headers.Location;
        Assert.NotNull(location);
        Assert.Contains("/api/", location!.ToString(), StringComparison.OrdinalIgnoreCase);

        var body = await response.Content.ReadFromJsonAsync<TaskResponse>(
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(body);
        Assert.Equal("Write integration tests", body.Title);
        Assert.Equal("Cover the task API end-to-end", body.Description);
        Assert.False(body.IsCompleted);
    }

    [Fact]
    public async Task Post_ShouldReturnProblemDetails_WhenTaskTitleIsInvalid()
    {
        ResetDatabase();

        var request = new CreateTaskRequest
        {
            Title = " ",
            Description = "This should not be accepted"
        };

        var response = await _client.PostAsJsonAsync("/api/tasks", request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(problem);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
        Assert.Equal("One or more validation errors occurred.", problem.Title);
        Assert.Contains("errors", problem.Extensions.Keys);
        Assert.Contains("traceId", problem.Extensions.Keys);
    }

    [Fact]
    public async Task Put_ShouldUpdateExistingTask()
    {
        ResetDatabase();

        var createResponse = await _client.PostAsJsonAsync("/api/tasks", new CreateTaskRequest
        {
            Title = "Original title",
            Description = "Original description"
        }, TestContext.Current.CancellationToken);

        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskResponse>(
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(createdTask);
        Assert.NotEqual(Guid.Empty, createdTask.Id);

        var updateRequest = new { Title = "Updated title", Description = "Updated description" };
        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/tasks/{createdTask.Id}",
            updateRequest,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var fetchResponse = await _client.GetAsync($"/api/tasks/{createdTask.Id}", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, fetchResponse.StatusCode);

        var updatedTask = await fetchResponse.Content.ReadFromJsonAsync<TaskResponse>(
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(updatedTask);
        Assert.Equal("Updated title", updatedTask.Title);
        Assert.Equal("Updated description", updatedTask.Description);
    }

    [Fact]
    public async Task Delete_ShouldRemoveExistingTask()
    {
        ResetDatabase();

        var createResponse = await _client.PostAsJsonAsync("/api/tasks", new CreateTaskRequest
        {
            Title = "Delete me",
            Description = "Temporary record"
        }, TestContext.Current.CancellationToken);

        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskResponse>(
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(createdTask);
        Assert.NotEqual(Guid.Empty, createdTask.Id);

        var deleteResponse = await _client.DeleteAsync($"/api/tasks/{createdTask.Id}", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var fetchResponse = await _client.GetAsync($"/api/tasks/{createdTask.Id}", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, fetchResponse.StatusCode);
    }
}
