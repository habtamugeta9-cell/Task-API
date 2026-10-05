using TaskApi.Domain;
using Xunit;

namespace TaskApi.Tests;

public sealed class TaskItemTests
{
    private static readonly Guid UserId = Guid.NewGuid();

    [Fact]
    public void Complete_ShouldMarkTaskAsCompleted()
    {
        var task = new TaskItem(UserId, "Learn C#", null);

        task.Complete();

        Assert.True(task.IsCompleted);
        Assert.NotNull(task.UpdatedAt);
    }

    [Fact]
    public void Complete_WhenAlreadyCompleted_ShouldDoNothing()
    {
        var task = new TaskItem(UserId, "Learn C#", null);

        task.Complete();
        var firstUpdatedAt = task.UpdatedAt;

        task.Complete();

        Assert.True(task.IsCompleted);
        Assert.Equal(firstUpdatedAt, task.UpdatedAt);
    }

    [Fact]
    public void Reopen_ShouldMarkCompletedTaskAsIncomplete()
    {
        var task = new TaskItem(UserId, "Learn C#", null);

        task.Complete();
        task.Reopen();

        Assert.False(task.IsCompleted);
        Assert.NotNull(task.UpdatedAt);
    }

    [Fact]
    public void Reopen_WhenAlreadyOpen_ShouldDoNothing()
    {
        var task = new TaskItem(UserId, "Learn C#", null);

        task.Reopen();

        Assert.False(task.IsCompleted);
        Assert.Null(task.UpdatedAt);
    }

    [Fact]
    public void Version_ShouldChange_WhenTaskChanges()
    {
        var task = new TaskItem(UserId, "Learn C#", null);
        var originalVersion = task.Version;

        task.Update("Learn ASP.NET Core", "Build the API");

        Assert.NotEqual(originalVersion, task.Version);
        Assert.NotNull(task.UpdatedAt);
    }

    [Fact]
    public void Uncomplete_ShouldMarkTaskAsIncomplete()
    {
        var task = new TaskItem(UserId, "Learn C#", null);
        task.Complete();

        var originalVersion = task.Version;
        task.Uncomplete();

        Assert.False(task.IsCompleted);
        Assert.NotEqual(originalVersion, task.Version);
    }
}
