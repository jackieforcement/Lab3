using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using WpfLab3.Models;

namespace WpfLab3.Services
{
    public class JsonTaskRepository : ITaskRepository
    {
        private readonly string _filePath;
        private static readonly JsonSerializerOptions _options;

        static JsonTaskRepository()
        {
            _options = new JsonSerializerOptions();
            _options.WriteIndented = true;
        }

        public JsonTaskRepository() : this(null)
        {
        }

        public JsonTaskRepository(string? filePath)
        {
            if (filePath == null)
            {
                string baseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string dir = Path.Combine(baseDir, "WpfLab3");
                Directory.CreateDirectory(dir);
                _filePath = Path.Combine(dir, "tasks.json");
            }
            else
            {
                _filePath = filePath;
                string? parent = Path.GetDirectoryName(_filePath);
                if (parent != null && parent.Length > 0)
                {
                    Directory.CreateDirectory(parent);
                }
            }
        }

        public async Task<IReadOnlyList<TodoTask>> LoadAsync()
        {
            await SimulateLatencyAsync();
            if (!File.Exists(_filePath))
            {
                return Array.Empty<TodoTask>();
            }

            FileStream? stream = null;
            try
            {
                stream = File.OpenRead(_filePath);
                List<TodoTask>? list = await JsonSerializer.DeserializeAsync<List<TodoTask>>(stream, _options);
                if (list == null)
                {
                    return new List<TodoTask>();
                }
                return list;
            }
            catch (JsonException)
            {
                return Array.Empty<TodoTask>();
            }
            finally
            {
                if (stream != null)
                {
                    await stream.DisposeAsync();
                }
            }
        }

        public async Task SaveAsync(IEnumerable<TodoTask> tasks)
        {
            await SimulateLatencyAsync();
            string tmp = string.Concat(_filePath, ".tmp");

            List<TodoTask> snapshot = new List<TodoTask>();
            foreach (TodoTask task in tasks)
            {
                snapshot.Add(task);
            }

            FileStream? stream = null;
            try
            {
                stream = File.Create(tmp);
                await JsonSerializer.SerializeAsync(stream, snapshot, _options);
            }
            finally
            {
                if (stream != null)
                {
                    await stream.DisposeAsync();
                }
            }
            File.Move(tmp, _filePath, true);
        }

        private static Task SimulateLatencyAsync()
        {
            return Task.Delay(Random.Shared.Next(500, 1001));
        }
    }
}
