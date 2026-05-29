using System.Collections.Generic;
using System.Threading.Tasks;
using WpfLab3.Models;
using WpfLab3.Services;

namespace WpfLab3.Tests
{
    public class FakeTaskRepository : ITaskRepository
    {
        private readonly List<TodoTask> _store;
        private int _loadCalls;
        private int _saveCalls;

        public FakeTaskRepository()
        {
            _store = new List<TodoTask>();
            _loadCalls = 0;
            _saveCalls = 0;
        }

        public List<TodoTask> Store
        {
            get
            {
                return _store;
            }
        }

        public int LoadCalls
        {
            get
            {
                return _loadCalls;
            }
        }

        public int SaveCalls
        {
            get
            {
                return _saveCalls;
            }
        }

        public Task<IReadOnlyList<TodoTask>> LoadAsync()
        {
            _loadCalls++;
            List<TodoTask> snapshot = new List<TodoTask>(_store);
            IReadOnlyList<TodoTask> readonlySnapshot = snapshot;
            return Task.FromResult(readonlySnapshot);
        }

        public Task SaveAsync(IEnumerable<TodoTask> tasks)
        {
            _saveCalls++;
            _store.Clear();
            foreach (TodoTask task in tasks)
            {
                _store.Add(task);
            }
            return Task.CompletedTask;
        }
    }
}
