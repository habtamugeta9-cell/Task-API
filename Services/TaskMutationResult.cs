namespace TaskApi.Services;

public enum TaskMutationStatus
{
    Success,
    NotFound,
    Conflict
}

public readonly record struct TaskMutationResult(
    TaskMutationStatus Status);
