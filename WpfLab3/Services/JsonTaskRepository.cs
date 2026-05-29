using System.IO;
using System.Text.Json;
using WpfLab3.Models;

namespace WpfLab3.Services;

public class JsonTaskRepository : ITaskRepository
{
    private readonly string _filePath;
    private static readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    public JsonTaskRepository(string? filePath = null)
    {
        if (filePath is null)
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WpfLab3");
            Directory.CreateDirectory(dir);
            _filePath = Path.Combine(dir, "tasks.json");
        }
        else
        {
            _filePath = filePath;
            var parent = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(parent))
                Directory.CreateDirectory(parent);
        }
    }

    public async Task<IReadOnlyList<TodoTask>> LoadAsync()
    {
        await SimulateLatencyAsync();
        if (!File.Exists(_filePath))
            return Array.Empty<TodoTask>();

        try
        {
            await using var stream = File.OpenRead(_filePath);
            var list = await JsonSerializer.DeserializeAsync<List<TodoTask>>(stream, _options);
            return list ?? new List<TodoTask>();
        }
        catch (JsonException)
        {
            return Array.Empty<TodoTask>();
        }
    }

    public async Task SaveAsync(IEnumerable<TodoTask> tasks)
    {
        await SimulateLatencyAsync();
        var tmp = _filePath + ".tmp";
        await using (var stream = File.Create(tmp))
        {
            await JsonSerializer.SerializeAsync(stream, tasks.ToList(), _options);
        }
        File.Move(tmp, _filePath, overwrite: true);
    }

    private static Task SimulateLatencyAsync() =>
        Task.Delay(Random.Shared.Next(500, 1001));
}
