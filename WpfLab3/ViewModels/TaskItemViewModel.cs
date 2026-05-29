using System;
using WpfLab3.Models;
using WpfLab3.Mvvm;

namespace WpfLab3.ViewModels
{
    public class TaskItemViewModel : ObservableObject
    {
        private readonly TodoTask _model;
        private string _title;
        private string? _description;
        private bool _isCompleted;
        private bool _isSelected;

        public TaskItemViewModel(TodoTask model)
        {
            _model = model;
            _title = model.Title;
            _description = model.Description;
            _isCompleted = model.IsCompleted;
            _isSelected = false;
        }

        public TodoTask Model
        {
            get
            {
                return _model;
            }
        }

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
                    _model.Title = value;
                }
            }
        }

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
                    _model.Description = value;
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
                if (SetProperty(ref _isCompleted, value, "IsCompleted"))
                {
                    _model.IsCompleted = value;
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                SetProperty(ref _isSelected, value, "IsSelected");
            }
        }

        public DateTime CreatedAt
        {
            get
            {
                return _model.CreatedAt;
            }
        }

        public Guid Id
        {
            get
            {
                return _model.Id;
            }
        }
    }
}
