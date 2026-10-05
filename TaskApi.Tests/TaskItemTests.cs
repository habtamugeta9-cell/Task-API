using TaskApi.Domain;
using Xunit;

namespace TaskApi.Tests;

public sealed class TaskItemTests
{
    [Fact]
    public void Complete_ShouldMarkTaskAsCompleted()
    {
        var task = new TaskItem("Learn C#");

        task.Complete();

        Assert.True(task.IsCompleted);
        Assert.NotNull(task.UpdatedAt);
    }

    [Fact]
    public void Complete_WhenAlreadyCompleted_ShouldDoNothing()
    {
        var task = new TaskItem("Learn C#");

        task.Complete();
        var firstUpdatedAt = task.UpdatedAt;

        task.Complete();

        Assert.True(task.IsCompleted);
        Assert.Equal(firstUpdatedAt, task.UpdatedAt);
    }

    [Fact]
    public void Reopen_ShouldMarkCompletedTaskAsIncomplete()
    {
        var task = new TaskItem("Learn C#");

        task.Complete();
        task.Reopen();

        Assert.False(task.IsCompleted);
        Assert.NotNull(task.UpdatedAt);
    }

    [Fact]
    public void Reopen_WhenAlreadyOpen_ShouldDoNothing()
    {
        var task = new TaskItem("Learn C#");

        task.Reopen();

        Assert.False(task.IsCompleted);
        Assert.Null(task.UpdatedAt);
    }
}
