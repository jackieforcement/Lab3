using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using WpfLab3.Models;
using WpfLab3.Mvvm;
using WpfLab3.Services;

namespace WpfLab3.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly ITaskRepository _repository;
        private readonly IDialogService _dialogs;
        private readonly ObservableCollection<TaskItemViewModel> _tasks;
        private readonly ICollectionView _filteredTasks;

        private TaskFilter _currentFilter;
        private bool _isBusy;
        private string _statusMessage;

        private readonly AsyncRelayCommand _loadCommand;
        private readonly AsyncRelayCommand _addCommand;
        private readonly AsyncRelayCommand<TaskItemViewModel> _editCommand;
        private readonly AsyncRelayCommand<TaskItemViewModel> _deleteCommand;
        private readonly AsyncRelayCommand _deleteSelectedCommand;
        private readonly AsyncRelayCommand<TaskItemViewModel> _toggleCompleteCommand;
        private readonly RelayCommand<TaskFilter> _setFilterCommand;

        public MainViewModel(ITaskRepository repository, IDialogService dialogs)
        {
            _repository = repository;
            _dialogs = dialogs;
            _tasks = new ObservableCollection<TaskItemViewModel>();
            _currentFilter = TaskFilter.All;
            _isBusy = false;
            _statusMessage = string.Empty;

            _filteredTasks = CollectionViewSource.GetDefaultView(_tasks);
            _filteredTasks.Filter = FilterPredicate;
            _filteredTasks.SortDescriptions.Add(
                new SortDescription("CreatedAt", ListSortDirection.Descending));

            _loadCommand = new AsyncRelayCommand(LoadAsync);
            _addCommand = new AsyncRelayCommand(AddAsync);
            _editCommand = new AsyncRelayCommand<TaskItemViewModel>(EditAsync);
            _deleteCommand = new AsyncRelayCommand<TaskItemViewModel>(DeleteAsync);
            _deleteSelectedCommand = new AsyncRelayCommand(DeleteSelectedAsync);
            _toggleCompleteCommand = new AsyncRelayCommand<TaskItemViewModel>(ToggleCompleteAsync);
            _setFilterCommand = new RelayCommand<TaskFilter>(SetFilter);
        }

        public ObservableCollection<TaskItemViewModel> Tasks
        {
            get
            {
                return _tasks;
            }
        }

        public ICollectionView FilteredTasks
        {
            get
            {
                return _filteredTasks;
            }
        }

        public TaskFilter CurrentFilter
        {
            get
            {
                return _currentFilter;
            }
            set
            {
                if (SetProperty(ref _currentFilter, value, "CurrentFilter"))
                {
                    _filteredTasks.Refresh();
                }
            }
        }

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                SetProperty(ref _isBusy, value, "IsBusy");
            }
        }

        public string StatusMessage
        {
            get
            {
                return _statusMessage;
            }
            set
            {
                SetProperty(ref _statusMessage, value, "StatusMessage");
            }
        }

        public AsyncRelayCommand LoadCommand
        {
            get
            {
                return _loadCommand;
            }
        }

        public AsyncRelayCommand AddCommand
        {
            get
            {
                return _addCommand;
            }
        }

        public AsyncRelayCommand<TaskItemViewModel> EditCommand
        {
            get
            {
                return _editCommand;
            }
        }

        public AsyncRelayCommand<TaskItemViewModel> DeleteCommand
        {
            get
            {
                return _deleteCommand;
            }
        }

        public AsyncRelayCommand DeleteSelectedCommand
        {
            get
            {
                return _deleteSelectedCommand;
            }
        }

        public AsyncRelayCommand<TaskItemViewModel> ToggleCompleteCommand
        {
            get
            {
                return _toggleCompleteCommand;
            }
        }

        public RelayCommand<TaskFilter> SetFilterCommand
        {
            get
            {
                return _setFilterCommand;
            }
        }

        public async Task LoadAsync()
        {
            await RunBusyAsync(LoadCoreAsync);
        }

        public async Task AddAsync()
        {
            TaskEditViewModel editVm = new TaskEditViewModel();
            if (!_dialogs.ShowTaskEditor(editVm))
            {
                return;
            }
            TodoTask model = editVm.ToModel();
            TaskItemViewModel wrapped = WrapTask(model);
            _tasks.Add(wrapped);
            await RunBusyAsync(SaveAllAsync);
            StatusMessage = string.Concat("Добавлена задача: ", model.Title);
        }

        public async Task EditAsync(TaskItemViewModel? item)
        {
            if (item == null)
            {
                return;
            }
            TaskEditViewModel editVm = new TaskEditViewModel(item.Model);
            if (!_dialogs.ShowTaskEditor(editVm))
            {
                return;
            }
            editVm.ToModel(item.Model);
            item.Title = item.Model.Title;
            item.Description = item.Model.Description;
            item.IsCompleted = item.Model.IsCompleted;
            _filteredTasks.Refresh();
            await RunBusyAsync(SaveAllAsync);
            StatusMessage = string.Concat("Сохранена задача: ", item.Title);
        }

        public async Task DeleteAsync(TaskItemViewModel? item)
        {
            if (item == null)
            {
                return;
            }
            string prompt = string.Concat("Удалить задачу «", item.Title, "»?");
            if (!_dialogs.Confirm(prompt))
            {
                return;
            }
            _tasks.Remove(item);
            await RunBusyAsync(SaveAllAsync);
            StatusMessage = "Задача удалена";
        }

        public async Task DeleteSelectedAsync()
        {
            List<TaskItemViewModel> selected = new List<TaskItemViewModel>();
            foreach (TaskItemViewModel t in _tasks)
            {
                if (t.IsSelected)
                {
                    selected.Add(t);
                }
            }
            if (selected.Count == 0)
            {
                return;
            }
            string prompt = string.Concat("Удалить выбранные задачи (", selected.Count.ToString(), ")?");
            if (!_dialogs.Confirm(prompt))
            {
                return;
            }
            foreach (TaskItemViewModel t in selected)
            {
                _tasks.Remove(t);
            }
            await RunBusyAsync(SaveAllAsync);
            StatusMessage = string.Concat("Удалено задач: ", selected.Count.ToString());
        }

        public async Task ToggleCompleteAsync(TaskItemViewModel? item)
        {
            if (item == null)
            {
                return;
            }
            item.IsCompleted = !item.IsCompleted;
            _filteredTasks.Refresh();
            await RunBusyAsync(SaveAllAsync);
        }

        public void SetFilter(TaskFilter filter)
        {
            CurrentFilter = filter;
        }

        private bool FilterPredicate(object obj)
        {
            TaskItemViewModel? item = obj as TaskItemViewModel;
            if (item == null)
            {
                return false;
            }
            if (_currentFilter == TaskFilter.Active)
            {
                return !item.IsCompleted;
            }
            if (_currentFilter == TaskFilter.Completed)
            {
                return item.IsCompleted;
            }
            return true;
        }

        private async Task LoadCoreAsync()
        {
            IReadOnlyList<TodoTask> list = await _repository.LoadAsync();
            _tasks.Clear();
            foreach (TodoTask t in list)
            {
                _tasks.Add(WrapTask(t));
            }
            StatusMessage = string.Concat("Загружено задач: ", list.Count.ToString());
        }

        private Task SaveAllAsync()
        {
            List<TodoTask> snapshot = new List<TodoTask>();
            foreach (TaskItemViewModel item in _tasks)
            {
                snapshot.Add(item.Model);
            }
            return _repository.SaveAsync(snapshot);
        }

        private TaskItemViewModel WrapTask(TodoTask model)
        {
            TaskItemViewModel vm = new TaskItemViewModel(model);
            vm.PropertyChanged += OnItemPropertyChanged;
            return vm;
        }

        private async void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsCompleted")
            {
                _filteredTasks.Refresh();
                await RunBusyAsync(SaveAllAsync);
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
                StatusMessage = string.Concat("Ошибка: ", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
