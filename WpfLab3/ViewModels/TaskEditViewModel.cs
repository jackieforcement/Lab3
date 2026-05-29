using System;
using System.ComponentModel.DataAnnotations;
using WpfLab3.Models;
using WpfLab3.Mvvm;

namespace WpfLab3.ViewModels
{
    public class TaskEditViewModel : ObservableValidator
    {
        private string _title;
        private string? _description;
        private bool _isCompleted;
        private bool? _dialogResult;
        private readonly string _windowTitle;
        private readonly RelayCommand _saveCommand;
        private readonly RelayCommand _cancelCommand;

        public TaskEditViewModel() : this(null)
        {
        }

        public TaskEditViewModel(TodoTask? source)
        {
            if (source == null)
            {
                _windowTitle = "Новая задача";
                _title = string.Empty;
                _description = null;
                _isCompleted = false;
            }
            else
            {
                _windowTitle = "Редактирование задачи";
                _title = source.Title;
                _description = source.Description;
                _isCompleted = source.IsCompleted;
            }
            _saveCommand = new RelayCommand(Save, CanSave);
            _cancelCommand = new RelayCommand(Cancel);
            ValidateAllProperties();
        }

        public event EventHandler<bool?>? RequestClose;

        [Required(ErrorMessage = "Название обязательно")]
        [MaxLength(100, ErrorMessage = "Максимум 100 символов")]
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (SetProperty(ref _title, value, "Title"))
                {
                    ValidateProperty(value, "Title");
                    _saveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        [MaxLength(500, ErrorMessage = "Максимум 500 символов")]
        public string? Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (SetProperty(ref _description, value, "Description"))
                {
                    ValidateProperty(value, "Description");
                    _saveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsCompleted
        {
            get
            {
                return _isCompleted;
            }
            set
            {
                SetProperty(ref _isCompleted, value, "IsCompleted");
            }
        }

        public bool? DialogResult
        {
            get
            {
                return _dialogResult;
            }
            private set
            {
                SetProperty(ref _dialogResult, value, "DialogResult");
            }
        }

        public string WindowTitle
        {
            get
            {
                return _windowTitle;
            }
        }

        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand;
            }
        }

        public RelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand;
            }
        }

        public TodoTask ToModel()
        {
            return ToModel(null);
        }

        public TodoTask ToModel(TodoTask? existing)
        {
            TodoTask model;
            if (existing == null)
            {
                model = new TodoTask();
            }
            else
            {
                model = existing;
            }
            model.Title = _title.Trim();
            if (string.IsNullOrWhiteSpace(_description))
            {
                model.Description = null;
            }
            else
            {
                model.Description = _description.Trim();
            }
            model.IsCompleted = _isCompleted;
            return model;
        }

        private void Save()
        {
            ValidateAllProperties();
            if (HasErrors)
            {
                return;
            }
            DialogResult = true;
            EventHandler<bool?>? handler = RequestClose;
            if (handler != null)
            {
                handler(this, true);
            }
        }

        private bool CanSave()
        {
            if (HasErrors)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(_title))
            {
                return false;
            }
            return true;
        }

        private void Cancel()
        {
            DialogResult = false;
            EventHandler<bool?>? handler = RequestClose;
            if (handler != null)
            {
                handler(this, false);
            }
        }
    }
}
