using Microsoft.Extensions.Caching.Memory;
using TaskApi.Domain;

namespace TaskApi.Services;

public sealed class TaskCacheService(
    IMemoryCache memoryCache)
{
    private static string GetKey(
        Guid userId,
        Guid taskId)
    {
        return $"task:{userId}:{taskId}";
    }

    public bool TryGet(
        Guid userId,
        Guid taskId,
        out TaskItem? task)
    {
        return memoryCache.TryGetValue(
            GetKey(userId, taskId),
            out task);
    }

    public void Set(
        Guid userId,
        TaskItem task)
    {
        memoryCache.Set(
            GetKey(userId, task.Id),
            task,
            TimeSpan.FromSeconds(15));
    }

    public void Remove(
        Guid userId,
        Guid taskId)
    {
        memoryCache.Remove(
            GetKey(userId, taskId));
    }
}
