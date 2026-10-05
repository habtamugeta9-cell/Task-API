using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using TaskApi.Data;
using TaskApi.Domain;
using TaskApi.DTOs;
using TaskApi.Services;
using TaskApi.Services.Tasks;
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
            NullLogger<TaskService>.Instance,
            new TaskQueryBuilder());
    }

    [Theory]
    [InlineData("createdAt", nameof(Queryable.OrderBy))]
    [InlineData("createdAt_desc", nameof(Queryable.OrderByDescending))]
    public void ApplySorting_ShouldSupportDocumentedCreatedAtValues(
        string sort,
        string expectedMethod)
    {
        var query = new TaskQueryBuilder().ApplySorting(
            Array.Empty<TaskItem>().AsQueryable(),
            sort);

        var orderExpression = Assert.IsAssignableFrom<MethodCallExpression>(query.Expression);

        Assert.Equal(expectedMethod, orderExpression.Method.Name);
    }

    [Fact]
    public void TaskQuery_ShouldRejectInvalidPageAndSortValues()
    {
        var query = new TaskQuery
        {
            Page = 0,
            PageSize = 0,
            Sort = "unknown"
        };

        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(
            query,
            new ValidationContext(query),
            validationResults,
            validateAllProperties: true);

        Assert.False(isValid);
        Assert.Contains(validationResults, result => result.MemberNames.Contains(nameof(TaskQuery.Page)));
        Assert.Contains(validationResults, result => result.MemberNames.Contains(nameof(TaskQuery.PageSize)));
        Assert.Contains(validationResults, result => result.MemberNames.Contains(nameof(TaskQuery.Sort)));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMatchingTaskForOwner()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var aliceId = Guid.NewGuid();
        var createdTask = await service.CreateAsync(aliceId, "Find me", null);

        var result = await service.GetByIdAsync(createdTask.Id, aliceId, false);

        Assert.NotNull(result);
        Assert.Equal(createdTask.Id, result.Id);
        Assert.Equal("Find me", result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNullForMissingTaskOrOtherUsersTask()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var aliceId = Guid.NewGuid();
        var bobId = Guid.NewGuid();

        var aliceTask = await service.CreateAsync(aliceId, "Owned by Alice", null);

        var missing = await service.GetByIdAsync(Guid.NewGuid(), bobId, false);
        var foreign = await service.GetByIdAsync(aliceTask.Id, bobId, false);

        Assert.Null(missing);
        Assert.Null(foreign);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyCurrentUsersTasks()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var aliceId = Guid.NewGuid();
        var bobId = Guid.NewGuid();

        await service.CreateAsync(aliceId, "Alice 1", null);
        await service.CreateAsync(aliceId, "Alice 2", null);
        await service.CreateAsync(bobId, "Bob 1", null);

        var result = await service.GetAllAsync(new TaskQuery
        {
            Sort = "title",
            Page = 1,
            PageSize = 10
        }, aliceId, false);

        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, task => Assert.Equal(aliceId, task.UserId));
    }

    [Fact]
    public async Task GetAllAsync_ShouldIncludeAllTasksForAdmin()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var aliceId = Guid.NewGuid();
        var bobId = Guid.NewGuid();

        await service.CreateAsync(aliceId, "Alice 1", null);
        await service.CreateAsync(bobId, "Bob 1", null);

        var result = await service.GetAllAsync(new TaskQuery
        {
            Page = 1,
            PageSize = 10
        }, aliceId, true);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateAndPersistTaskWithOwner()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var userId = Guid.NewGuid();

        var result = await service.CreateAsync(
            userId,
            "Learn C#",
            "Practice ASP.NET Core");

        var storedTask = await dbContext.Tasks.SingleAsync(
            TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(userId, result.UserId);
        Assert.Equal("Learn C#", result.Title);
        Assert.Equal("Practice ASP.NET Core", result.Description);
        Assert.False(result.IsCompleted);
        Assert.NotEqual(default, result.CreatedAt);
        Assert.Null(result.UpdatedAt);
        Assert.Equal(userId, storedTask.UserId);
    }

    [Fact]
    public async Task CreateAsync_ShouldTrimTitleAndDescription()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var userId = Guid.NewGuid();

        var result = await service.CreateAsync(
            userId,
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
        var userId = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateAsync(userId, "   ", "Description"));

        Assert.Equal("title", exception.ParamName);
        Assert.Equal("Title is required. (Parameter 'title')", exception.Message);
        Assert.Empty(await dbContext.Tasks.ToListAsync(
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task CreateAsync_ShouldRejectTitleLongerThan200Characters()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var userId = Guid.NewGuid();
        var title = new string('A', 201);

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateAsync(userId, title, null));

        Assert.Equal("title", exception.ParamName);
        Assert.Empty(await dbContext.Tasks.ToListAsync(
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task CreateAsync_ShouldRejectDescriptionLongerThan2000Characters()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var userId = Guid.NewGuid();
        var description = new string('A', 2001);

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateAsync(userId, "Valid title", description));

        Assert.Equal("description", exception.ParamName);
        Assert.Empty(await dbContext.Tasks.ToListAsync(
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task CreateAsync_ShouldAcceptMaximumLengthValues()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var userId = Guid.NewGuid();
        var title = new string('A', 200);
        var description = new string('B', 2000);

        var result = await service.CreateAsync(userId, title, description);

        Assert.Equal(200, result.Title.Length);
        Assert.Equal(2000, result.Description?.Length);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateExistingOwnedTask()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var userId = Guid.NewGuid();

        var task = await service.CreateAsync(
            userId,
            "Old title",
            "Old description");

        var result = await service.UpdateAsync(
            task.Id,
            userId,
            false,
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
    public async Task UpdateAsync_ShouldRejectForeignTask()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var aliceId = Guid.NewGuid();
        var bobId = Guid.NewGuid();

        var task = await service.CreateAsync(aliceId, "Owned by Alice", "Description");

        var result = await service.UpdateAsync(
            task.Id,
            bobId,
            false,
            "New title",
            "New description");

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldAllowAdminToUpdateAnotherUsersTask()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var ownerId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var task = await service.CreateAsync(ownerId, "Owned by user", null);

        var updated = await service.UpdateAsync(
            task.Id,
            adminId,
            true,
            "Updated by admin",
            null);

        Assert.True(updated);
        Assert.Equal(
            "Updated by admin",
            (await dbContext.Tasks.SingleAsync(
                TestContext.Current.CancellationToken)).Title);
    }

    [Fact]
    public async Task UpdateAsync_ShouldTrimUpdatedValues()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var userId = Guid.NewGuid();

        var task = await service.CreateAsync(userId, "Old title", "Old");

        var result = await service.UpdateAsync(
            task.Id,
            userId,
            false,
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
        var userId = Guid.NewGuid();

        var result = await service.UpdateAsync(
            Guid.NewGuid(),
            userId,
            false,
            "New title",
            "New description");

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldRejectInvalidTitle()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var userId = Guid.NewGuid();

        var task = await service.CreateAsync(
            userId,
            "Original title",
            "Original description");

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.UpdateAsync(
                task.Id,
                userId,
                false,
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
        var userId = Guid.NewGuid();
        var task = await service.CreateAsync(
            userId,
            "Original title",
            "Original description");

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.UpdateAsync(
                task.Id,
                userId,
                false,
                "Updated title",
                new string('A', 2001)));

        var storedTask = await dbContext.Tasks.SingleAsync(
            TestContext.Current.CancellationToken);

        Assert.Equal("Original title", storedTask.Title);
        Assert.Equal("Original description", storedTask.Description);
        Assert.Null(storedTask.UpdatedAt);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteExistingOwnedTask()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var userId = Guid.NewGuid();

        var task = await service.CreateAsync(
            userId,
            "Delete me",
            null);

        var result = await service.DeleteAsync(task.Id, userId, false);

        Assert.True(result);
        Assert.Empty(await dbContext.Tasks.ToListAsync(
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task DeleteAsync_ShouldRejectForeignTask()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var aliceId = Guid.NewGuid();
        var bobId = Guid.NewGuid();

        var task = await service.CreateAsync(aliceId, "Delete me", null);

        var result = await service.DeleteAsync(task.Id, bobId, false);

        Assert.False(result);
        Assert.Single(await dbContext.Tasks.ToListAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task DeleteAsync_ShouldAllowAdminToDeleteAnotherUsersTask()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var ownerId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var task = await service.CreateAsync(ownerId, "Owned by user", null);

        var deleted = await service.DeleteAsync(task.Id, adminId, true);

        Assert.True(deleted);
        Assert.Empty(await dbContext.Tasks.ToListAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalseForMissingTask()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var userId = Guid.NewGuid();

        var result = await service.DeleteAsync(Guid.NewGuid(), userId, false);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnConflict_WhenVersionIsStale()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var userId = Guid.NewGuid();

        var task = await service.CreateAsync(
            userId,
            "Old title",
            "Original description");

        var staleVersion = task.Version;
        task.Update("Changed elsewhere", "Updated by someone else");
        await dbContext.SaveChangesAsync();

        var result = await service.UpdateAsync(
            task.Id,
            userId,
            false,
            "My update",
            "Latest description",
            staleVersion);

        Assert.Equal(TaskMutationStatus.Conflict, result.Status);
    }

    [Fact]
    public async Task SetCompletionAsync_ShouldChangeCompletionState()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var userId = Guid.NewGuid();

        var task = await service.CreateAsync(
            userId,
            "Write docs",
            null);

        var completed = await service.SetCompletionAsync(
            task.Id,
            userId,
            false,
            true,
            task.Version);

        Assert.Equal(TaskMutationStatus.Success, completed.Status);

        var stored = await dbContext.Tasks.FindAsync(task.Id);
        Assert.NotNull(stored);
        Assert.True(stored.IsCompleted);
    }
}
