using WpfLab3.Models;
using WpfLab3.Services;

namespace WpfLab3.Tests;

public class FakeTaskRepository : ITaskRepository
{
    public List<TodoTask> Store { get; } = new();
    public int LoadCalls { get; private set; }
    public int SaveCalls { get; private set; }

    public Task<IReadOnlyList<TodoTask>> LoadAsync()
    {
        LoadCalls++;
        return Task.FromResult<IReadOnlyList<TodoTask>>(Store.ToList());
    }

    public Task SaveAsync(IEnumerable<TodoTask> tasks)
    {
        SaveCalls++;
        Store.Clear();
        Store.AddRange(tasks);
        return Task.CompletedTask;
    }
}
