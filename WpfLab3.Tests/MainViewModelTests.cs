using System.Linq;
using NUnit.Framework;
using WpfLab3.Models;
using WpfLab3.ViewModels;

namespace WpfLab3.Tests;

[TestFixture, Apartment(ApartmentState.STA)]
public class MainViewModelTests
{
    private FakeTaskRepository _repo = null!;
    private FakeDialogService _dialogs = null!;
    private MainViewModel _vm = null!;

    [SetUp]
    public void Setup()
    {
        _repo = new FakeTaskRepository();
        _dialogs = new FakeDialogService();
        _vm = new MainViewModel(_repo, _dialogs);
    }

    [Test]
    public async Task Load_PopulatesTasksFromRepository()
    {
        _repo.Store.Add(new TodoTask { Title = "A" });
        _repo.Store.Add(new TodoTask { Title = "B" });

        await _vm.LoadCommand.ExecuteAsync(null);

        Assert.That(_vm.Tasks, Has.Count.EqualTo(2));
        Assert.That(_repo.LoadCalls, Is.EqualTo(1));
    }

    [Test]
    public async Task Add_WhenDialogAccepts_AddsTaskAndSaves()
    {
        _dialogs.EditorHandler = editor =>
        {
            editor.Title = "New task";
            editor.Description = "Some description";
            return true;
        };

        await _vm.AddCommand.ExecuteAsync(null);

        Assert.That(_vm.Tasks, Has.Count.EqualTo(1));
        Assert.That(_vm.Tasks[0].Title, Is.EqualTo("New task"));
        Assert.That(_repo.SaveCalls, Is.EqualTo(1));
        Assert.That(_repo.Store, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task Add_WhenDialogCancels_DoesNothing()
    {
        _dialogs.EditorHandler = _ => false;

        await _vm.AddCommand.ExecuteAsync(null);

        Assert.That(_vm.Tasks, Is.Empty);
        Assert.That(_repo.SaveCalls, Is.Zero);
    }

    [Test]
    public async Task SetFilter_Active_FilteredViewShowsOnlyIncomplete()
    {
        _repo.Store.Add(new TodoTask { Title = "active1" });
        _repo.Store.Add(new TodoTask { Title = "active2" });
        _repo.Store.Add(new TodoTask { Title = "done", IsCompleted = true });
        await _vm.LoadCommand.ExecuteAsync(null);

        _vm.SetFilterCommand.Execute(TaskFilter.Active);

        var visible = _vm.FilteredTasks.Cast<TaskItemViewModel>().ToList();
        Assert.That(visible, Has.Count.EqualTo(2));
        Assert.That(visible.All(t => !t.IsCompleted), Is.True);
    }

    [Test]
    public async Task SetFilter_Completed_FilteredViewShowsOnlyCompleted()
    {
        _repo.Store.Add(new TodoTask { Title = "a" });
        _repo.Store.Add(new TodoTask { Title = "b", IsCompleted = true });
        await _vm.LoadCommand.ExecuteAsync(null);

        _vm.SetFilterCommand.Execute(TaskFilter.Completed);

        var visible = _vm.FilteredTasks.Cast<TaskItemViewModel>().ToList();
        Assert.That(visible, Has.Count.EqualTo(1));
        Assert.That(visible[0].Title, Is.EqualTo("b"));
    }

    [Test]
    public async Task DeleteSelected_RemovesOnlyMarkedTasks()
    {
        _repo.Store.Add(new TodoTask { Title = "keep" });
        _repo.Store.Add(new TodoTask { Title = "drop1" });
        _repo.Store.Add(new TodoTask { Title = "drop2" });
        await _vm.LoadCommand.ExecuteAsync(null);

        foreach (var t in _vm.Tasks.Where(t => t.Title.StartsWith("drop")))
            t.IsSelected = true;

        await _vm.DeleteSelectedAsync();

        Assert.That(_vm.Tasks, Has.Count.EqualTo(1));
        Assert.That(_vm.Tasks[0].Title, Is.EqualTo("keep"));
        Assert.That(_repo.Store, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task Delete_WhenConfirmed_RemovesTask()
    {
        _repo.Store.Add(new TodoTask { Title = "to remove" });
        await _vm.LoadCommand.ExecuteAsync(null);
        _dialogs.ConfirmResult = true;

        await _vm.DeleteCommand.ExecuteAsync(_vm.Tasks[0]);

        Assert.That(_vm.Tasks, Is.Empty);
        Assert.That(_repo.Store, Is.Empty);
    }

    [Test]
    public async Task Delete_WhenCancelled_KeepsTask()
    {
        _repo.Store.Add(new TodoTask { Title = "stays" });
        await _vm.LoadCommand.ExecuteAsync(null);
        _dialogs.ConfirmResult = false;

        await _vm.DeleteCommand.ExecuteAsync(_vm.Tasks[0]);

        Assert.That(_vm.Tasks, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task ToggleComplete_FlipsIsCompletedAndSaves()
    {
        _repo.Store.Add(new TodoTask { Title = "t", IsCompleted = false });
        await _vm.LoadCommand.ExecuteAsync(null);
        var before = _repo.SaveCalls;

        await _vm.ToggleCompleteAsync(_vm.Tasks[0]);

        Assert.That(_vm.Tasks[0].IsCompleted, Is.True);
        Assert.That(_repo.SaveCalls, Is.GreaterThan(before));
    }
}
