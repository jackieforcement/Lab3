using WpfLab3.Models;

namespace WpfLab3.Services;

public interface ITaskRepository
{
    Task<IReadOnlyList<TodoTask>> LoadAsync();
    Task SaveAsync(IEnumerable<TodoTask> tasks);
}
