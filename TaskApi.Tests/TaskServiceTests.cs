using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using TaskApi.Data;
using TaskApi.Queries;
using TaskApi.Services;
using Xunit;

namespace TaskApi.Tests;

public sealed class TaskServiceTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static TaskService CreateService(AppDbContext dbContext)
    {
        return new TaskService(
            dbContext,
            NullLogger<ITaskService>.Instance);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMatchingTask()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var createdTask = await service.CreateAsync("Find me", null);

        var result = await service.GetByIdAsync(createdTask.Id);

        Assert.NotNull(result);
        Assert.Equal(createdTask.Id, result.Id);
        Assert.Equal("Find me", result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNullForMissingTask()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);

        var result = await service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnSortedPageAndPaginationMetadata()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        await service.CreateAsync("Bravo", null);
        await service.CreateAsync("Alpha", null);
        await service.CreateAsync("Charlie", null);

        var result = await service.GetAllAsync(new TaskQuery
        {
            Sort = "title",
            Page = 2,
            PageSize = 2
        });

        Assert.Collection(
            result.Items,
            task => Assert.Equal("Charlie", task.Title));
        Assert.Equal(2, result.Page);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
        Assert.True(result.HasPreviousPage);
        Assert.False(result.HasNextPage);
    }

    [Fact]
    public async Task GetAllAsync_ShouldFilterCompletedTasksAndClampPageParameters()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var completedTask = await service.CreateAsync("Completed", null);
        await service.CreateAsync("Still open", null);
        completedTask.Complete();
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await service.GetAllAsync(new TaskQuery
        {
            Completed = true,
            Page = 0,
            PageSize = 500
        });

        var task = Assert.Single(result.Items);
        Assert.Equal("Completed", task.Title);
        Assert.True(task.IsCompleted);
        Assert.Equal(1, result.Page);
        Assert.Equal(100, result.PageSize);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.TotalPages);
        Assert.False(result.HasPreviousPage);
        Assert.False(result.HasNextPage);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateAndPersistTask()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);

        var result = await service.CreateAsync(
            "Learn C#",
            "Practice ASP.NET Core");

        var storedTask = await dbContext.Tasks.SingleAsync(
            TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Learn C#", result.Title);
        Assert.Equal("Practice ASP.NET Core", result.Description);
        Assert.False(result.IsCompleted);
        Assert.NotEqual(default, result.CreatedAt);
        Assert.Null(result.UpdatedAt);

        Assert.Equal(result.Id, storedTask.Id);
        Assert.Equal("Learn C#", storedTask.Title);
    }

    [Fact]
    public async Task CreateAsync_ShouldTrimTitleAndDescription()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);

        var result = await service.CreateAsync(
            "  Learn C#  ",
            "  Practice APIs  ");

        Assert.Equal("Learn C#", result.Title);
        Assert.Equal("Practice APIs", result.Description);
    }

    [Fact]
    public async Task CreateAsync_ShouldRejectBlankTitle()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateAsync("   ", "Description"));

        Assert.Equal("title", exception.ParamName);
        Assert.Equal("Task title is required. (Parameter 'title')", exception.Message);

        Assert.Empty(await dbContext.Tasks.ToListAsync(
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task CreateAsync_ShouldRejectTitleLongerThan200Characters()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);

        var title = new string('A', 201);

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateAsync(title, null));

        Assert.Equal("title", exception.ParamName);

        Assert.Empty(await dbContext.Tasks.ToListAsync(
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task CreateAsync_ShouldRejectDescriptionLongerThan2000Characters()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);

        var description = new string('A', 2001);

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateAsync("Valid title", description));

        Assert.Equal("description", exception.ParamName);

        Assert.Empty(await dbContext.Tasks.ToListAsync(
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task CreateAsync_ShouldAcceptMaximumLengthValues()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var title = new string('A', 200);
        var description = new string('B', 2000);

        var result = await service.CreateAsync(title, description);

        Assert.Equal(200, result.Title.Length);
        Assert.Equal(2000, result.Description?.Length);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateExistingTask()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);

        var task = await service.CreateAsync(
            "Old title",
            "Old description");

        var result = await service.UpdateAsync(
            task.Id,
            "New title",
            "New description");

        var updatedTask = await dbContext.Tasks
            .SingleAsync(TestContext.Current.CancellationToken);

        Assert.True(result);
        Assert.Equal("New title", updatedTask.Title);
        Assert.Equal("New description", updatedTask.Description);
        Assert.NotNull(updatedTask.UpdatedAt);
        Assert.True(updatedTask.UpdatedAt >= updatedTask.CreatedAt);
    }

    [Fact]
    public async Task UpdateAsync_ShouldTrimUpdatedValues()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);

        var task = await service.CreateAsync("Old title", "Old");

        var result = await service.UpdateAsync(
            task.Id,
            "  New title  ",
            "  New description  ");

        var updatedTask = await dbContext.Tasks
            .SingleAsync(TestContext.Current.CancellationToken);

        Assert.True(result);
        Assert.Equal("New title", updatedTask.Title);
        Assert.Equal("New description", updatedTask.Description);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalseForMissingTask()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);

        var result = await service.UpdateAsync(
            Guid.NewGuid(),
            "New title",
            "New description");

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldRejectInvalidTitle()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);

        var task = await service.CreateAsync(
            "Original title",
            "Original description");

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.UpdateAsync(
                task.Id,
                "",
                "Updated description"));

        var storedTask = await dbContext.Tasks
            .SingleAsync(TestContext.Current.CancellationToken);

        Assert.Equal("Original title", storedTask.Title);
        Assert.Equal("Original description", storedTask.Description);
    }

    [Fact]
    public async Task UpdateAsync_ShouldRejectInvalidDescriptionWithoutChangingTask()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var task = await service.CreateAsync(
            "Original title",
            "Original description");

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.UpdateAsync(
                task.Id,
                "Updated title",
                new string('A', 2001)));

        var storedTask = await dbContext.Tasks.SingleAsync(
            TestContext.Current.CancellationToken);

        Assert.Equal("Original title", storedTask.Title);
        Assert.Equal("Original description", storedTask.Description);
        Assert.Null(storedTask.UpdatedAt);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteExistingTask()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);

        var task = await service.CreateAsync(
            "Delete me",
            null);

        var result = await service.DeleteAsync(task.Id);

        Assert.True(result);
        Assert.Empty(await dbContext.Tasks.ToListAsync(
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalseForMissingTask()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);

        var result = await service.DeleteAsync(Guid.NewGuid());

        Assert.False(result);
    }
}
