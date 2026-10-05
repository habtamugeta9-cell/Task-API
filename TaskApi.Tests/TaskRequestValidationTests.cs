using System.ComponentModel.DataAnnotations;
using TaskApi.Requests;
using Xunit;

namespace TaskApi.Tests;

public sealed class TaskRequestValidationTests
{
    [Fact]
    public void CreateTaskRequest_ShouldRequireTitle()
    {
        var request = new CreateTaskRequest { Title = " " };

        Assert.False(IsValid(request));
    }

    [Fact]
    public void UpdateTaskRequest_ShouldRequireTitle()
    {
        var request = new UpdateTaskRequest { Title = string.Empty };

        Assert.False(IsValid(request));
    }

    [Fact]
    public void Requests_ShouldRejectValuesBeyondMaximumLengths()
    {
        var createRequest = new CreateTaskRequest
        {
            Title = new string('A', 201),
            Description = new string('B', 2001)
        };
        var updateRequest = new UpdateTaskRequest
        {
            Title = new string('A', 201),
            Description = new string('B', 2001)
        };

        Assert.False(IsValid(createRequest));
        Assert.False(IsValid(updateRequest));
    }

    [Fact]
    public void Requests_ShouldAcceptMaximumAllowedLengths()
    {
        var createRequest = new CreateTaskRequest
        {
            Title = new string('A', 200),
            Description = new string('B', 2000)
        };
        var updateRequest = new UpdateTaskRequest
        {
            Title = new string('A', 200),
            Description = new string('B', 2000)
        };

        Assert.True(IsValid(createRequest));
        Assert.True(IsValid(updateRequest));
    }

    private static bool IsValid(object request)
    {
        var results = new List<ValidationResult>();
        return Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            results,
            validateAllProperties: true);
    }
}