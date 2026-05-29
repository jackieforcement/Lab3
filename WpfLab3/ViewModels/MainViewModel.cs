using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfLab3.Models;
using WpfLab3.Services;

namespace WpfLab3.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ITaskRepository _repository;
    private readonly IDialogService _dialogs;

    public ObservableCollection<TaskItemViewModel> Tasks { get; } = new();
    public ICollectionView FilteredTasks { get; }

    [ObservableProperty] private TaskFilter _currentFilter = TaskFilter.All;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _statusMessage = string.Empty;

    public MainViewModel(ITaskRepository repository, IDialogService dialogs)
    {
        _repository = repository;
        _dialogs = dialogs;
        FilteredTasks = CollectionViewSource.GetDefaultView(Tasks);
        FilteredTasks.Filter = FilterPredicate;
        FilteredTasks.SortDescriptions.Add(new SortDescription(nameof(TaskItemViewModel.CreatedAt), ListSortDirection.Descending));
    }

    partial void OnCurrentFilterChanged(TaskFilter value) => FilteredTasks.Refresh();

    private bool FilterPredicate(object obj)
    {
        if (obj is not TaskItemViewModel t) return false;
        return CurrentFilter switch
        {
            TaskFilter.Active => !t.IsCompleted,
            TaskFilter.Completed => t.IsCompleted,
            _ => true,
        };
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        await RunBusyAsync(async () =>
        {
            var list = await _repository.LoadAsync();
            Tasks.Clear();
            foreach (var t in list)
                Tasks.Add(WrapTask(t));
            StatusMessage = $"Загружено задач: {list.Count}";
        });
    }

    [RelayCommand]
    private async Task AddAsync()
    {
        var editVm = new TaskEditViewModel();
        if (!_dialogs.ShowTaskEditor(editVm)) return;

        var model = editVm.ToModel();
        var wrapped = WrapTask(model);
        Tasks.Add(wrapped);
        await RunBusyAsync(() => _repository.SaveAsync(Tasks.Select(x => x.Model)));
        StatusMessage = $"Добавлена задача: {model.Title}";
    }

    [RelayCommand]
    private async Task EditAsync(TaskItemViewModel? item)
    {
        if (item is null) return;
        var editVm = new TaskEditViewModel(item.Model);
        if (!_dialogs.ShowTaskEditor(editVm)) return;

        editVm.ToModel(item.Model);
        item.Title = item.Model.Title;
        item.Description = item.Model.Description;
        item.IsCompleted = item.Model.IsCompleted;
        FilteredTasks.Refresh();
        await RunBusyAsync(() => _repository.SaveAsync(Tasks.Select(x => x.Model)));
        StatusMessage = $"Сохранена задача: {item.Title}";
    }

    [RelayCommand]
    private async Task DeleteAsync(TaskItemViewModel? item)
    {
        if (item is null) return;
        if (!_dialogs.Confirm($"Удалить задачу «{item.Title}»?")) return;

        Tasks.Remove(item);
        await RunBusyAsync(() => _repository.SaveAsync(Tasks.Select(x => x.Model)));
        StatusMessage = "Задача удалена";
    }

    [RelayCommand]
    public async Task DeleteSelectedAsync()
    {
        var selected = Tasks.Where(t => t.IsSelected).ToList();
        if (selected.Count == 0) return;
        if (!_dialogs.Confirm($"Удалить выбранные задачи ({selected.Count})?")) return;

        foreach (var t in selected)
            Tasks.Remove(t);
        await RunBusyAsync(() => _repository.SaveAsync(Tasks.Select(x => x.Model)));
        StatusMessage = $"Удалено задач: {selected.Count}";
    }

    [RelayCommand]
    public async Task ToggleCompleteAsync(TaskItemViewModel? item)
    {
        if (item is null) return;
        item.IsCompleted = !item.IsCompleted;
        FilteredTasks.Refresh();
        await RunBusyAsync(() => _repository.SaveAsync(Tasks.Select(x => x.Model)));
    }

    [RelayCommand]
    private void SetFilter(TaskFilter filter) => CurrentFilter = filter;

    private TaskItemViewModel WrapTask(TodoTask model)
    {
        var vm = new TaskItemViewModel(model);
        vm.PropertyChanged += OnItemPropertyChanged;
        return vm;
    }

    private async void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TaskItemViewModel.IsCompleted))
        {
            FilteredTasks.Refresh();
            await RunBusyAsync(() => _repository.SaveAsync(Tasks.Select(x => x.Model)));
        }
    }

    private async Task RunBusyAsync(Func<Task> action)
    {
        try
        {
            IsBusy = true;
            await action();
        }
        catch (Exception ex)
        {
            StatusMessage = "Ошибка: " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
