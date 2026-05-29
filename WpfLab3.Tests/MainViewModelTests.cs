using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using WpfLab3.Models;
using WpfLab3.ViewModels;

namespace WpfLab3.Tests
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class MainViewModelTests
    {
        private FakeTaskRepository _repo;
        private FakeDialogService _dialogs;
        private MainViewModel _vm;

        public MainViewModelTests()
        {
            _repo = new FakeTaskRepository();
            _dialogs = new FakeDialogService();
            _vm = new MainViewModel(_repo, _dialogs);
        }

        [SetUp]
        public void Setup()
        {
            _repo = new FakeTaskRepository();
            _dialogs = new FakeDialogService();
            _vm = new MainViewModel(_repo, _dialogs);
        }

        private static TodoTask NewTask(string title)
        {
            TodoTask task = new TodoTask();
            task.Title = title;
            return task;
        }

        private static TodoTask NewTask(string title, bool isCompleted)
        {
            TodoTask task = new TodoTask();
            task.Title = title;
            task.IsCompleted = isCompleted;
            return task;
        }

        private static List<TaskItemViewModel> SnapshotFiltered(MainViewModel vm)
        {
            List<TaskItemViewModel> visible = new List<TaskItemViewModel>();
            foreach (object? obj in vm.FilteredTasks)
            {
                TaskItemViewModel? item = obj as TaskItemViewModel;
                if (item != null)
                {
                    visible.Add(item);
                }
            }
            return visible;
        }

        [Test]
        public async Task Load_PopulatesTasksFromRepository()
        {
            _repo.Store.Add(NewTask("A"));
            _repo.Store.Add(NewTask("B"));

            await _vm.LoadCommand.ExecuteAsync(null);

            Assert.That(_vm.Tasks, Has.Count.EqualTo(2));
            Assert.That(_repo.LoadCalls, Is.EqualTo(1));
        }

        [Test]
        public async Task Add_WhenDialogAccepts_AddsTaskAndSaves()
        {
            bool AcceptHandler(TaskEditViewModel editor)
            {
                editor.Title = "New task";
                editor.Description = "Some description";
                return true;
            }
            _dialogs.EditorHandler = AcceptHandler;

            await _vm.AddCommand.ExecuteAsync(null);

            Assert.That(_vm.Tasks, Has.Count.EqualTo(1));
            Assert.That(_vm.Tasks[0].Title, Is.EqualTo("New task"));
            Assert.That(_repo.SaveCalls, Is.EqualTo(1));
            Assert.That(_repo.Store, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task Add_WhenDialogCancels_DoesNothing()
        {
            bool RejectHandler(TaskEditViewModel editor)
            {
                return false;
            }
            _dialogs.EditorHandler = RejectHandler;

            await _vm.AddCommand.ExecuteAsync(null);

            Assert.That(_vm.Tasks, Is.Empty);
            Assert.That(_repo.SaveCalls, Is.Zero);
        }

        [Test]
        public async Task SetFilter_Active_FilteredViewShowsOnlyIncomplete()
        {
            _repo.Store.Add(NewTask("active1"));
            _repo.Store.Add(NewTask("active2"));
            _repo.Store.Add(NewTask("done", true));
            await _vm.LoadCommand.ExecuteAsync(null);

            _vm.SetFilterCommand.Execute(TaskFilter.Active);

            List<TaskItemViewModel> visible = SnapshotFiltered(_vm);
            Assert.That(visible, Has.Count.EqualTo(2));
            bool allIncomplete = true;
            foreach (TaskItemViewModel item in visible)
            {
                if (item.IsCompleted)
                {
                    allIncomplete = false;
                    break;
                }
            }
            Assert.That(allIncomplete, Is.True);
        }

        [Test]
        public async Task SetFilter_Completed_FilteredViewShowsOnlyCompleted()
        {
            _repo.Store.Add(NewTask("a"));
            _repo.Store.Add(NewTask("b", true));
            await _vm.LoadCommand.ExecuteAsync(null);

            _vm.SetFilterCommand.Execute(TaskFilter.Completed);

            List<TaskItemViewModel> visible = SnapshotFiltered(_vm);
            Assert.That(visible, Has.Count.EqualTo(1));
            Assert.That(visible[0].Title, Is.EqualTo("b"));
        }

        [Test]
        public async Task DeleteSelected_RemovesOnlyMarkedTasks()
        {
            _repo.Store.Add(NewTask("keep"));
            _repo.Store.Add(NewTask("drop1"));
            _repo.Store.Add(NewTask("drop2"));
            await _vm.LoadCommand.ExecuteAsync(null);

            foreach (TaskItemViewModel t in _vm.Tasks)
            {
                if (t.Title.StartsWith("drop"))
                {
                    t.IsSelected = true;
                }
            }

            await _vm.DeleteSelectedAsync();

            Assert.That(_vm.Tasks, Has.Count.EqualTo(1));
            Assert.That(_vm.Tasks[0].Title, Is.EqualTo("keep"));
            Assert.That(_repo.Store, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task Delete_WhenConfirmed_RemovesTask()
        {
            _repo.Store.Add(NewTask("to remove"));
            await _vm.LoadCommand.ExecuteAsync(null);
            _dialogs.ConfirmResult = true;

            await _vm.DeleteCommand.ExecuteAsync(_vm.Tasks[0]);

            Assert.That(_vm.Tasks, Is.Empty);
            Assert.That(_repo.Store, Is.Empty);
        }

        [Test]
        public async Task Delete_WhenCancelled_KeepsTask()
        {
            _repo.Store.Add(NewTask("stays"));
            await _vm.LoadCommand.ExecuteAsync(null);
            _dialogs.ConfirmResult = false;

            await _vm.DeleteCommand.ExecuteAsync(_vm.Tasks[0]);

            Assert.That(_vm.Tasks, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task ToggleComplete_FlipsIsCompletedAndSaves()
        {
            _repo.Store.Add(NewTask("t", false));
            await _vm.LoadCommand.ExecuteAsync(null);
            int before = _repo.SaveCalls;

            await _vm.ToggleCompleteAsync(_vm.Tasks[0]);

            Assert.That(_vm.Tasks[0].IsCompleted, Is.True);
            Assert.That(_repo.SaveCalls, Is.GreaterThan(before));
        }
    }
}
