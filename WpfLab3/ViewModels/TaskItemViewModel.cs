using CommunityToolkit.Mvvm.ComponentModel;
using WpfLab3.Models;

namespace WpfLab3.ViewModels;

public partial class TaskItemViewModel : ObservableObject
{
    public TodoTask Model { get; }

    [ObservableProperty] private string _title;
    [ObservableProperty] private string? _description;
    [ObservableProperty] private bool _isCompleted;
    [ObservableProperty] private bool _isSelected;

    public DateTime CreatedAt => Model.CreatedAt;
    public Guid Id => Model.Id;

    public TaskItemViewModel(TodoTask model)
    {
        Model = model;
        _title = model.Title;
        _description = model.Description;
        _isCompleted = model.IsCompleted;
    }

    partial void OnTitleChanged(string value) => Model.Title = value;
    partial void OnDescriptionChanged(string? value) => Model.Description = value;
    partial void OnIsCompletedChanged(bool value) => Model.IsCompleted = value;
}
