using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfLab3.Models;

namespace WpfLab3.ViewModels;

public partial class TaskEditViewModel : ObservableValidator
{
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    [Required(ErrorMessage = "Название обязательно")]
    [MaxLength(100, ErrorMessage = "Максимум 100 символов")]
    private string _title = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [MaxLength(500, ErrorMessage = "Максимум 500 символов")]
    private string? _description;

    [ObservableProperty] private bool _isCompleted;

    public bool? DialogResult { get; private set; }
    public string WindowTitle { get; }

    public event EventHandler<bool?>? RequestClose;

    public TaskEditViewModel(TodoTask? source = null)
    {
        if (source is null)
        {
            WindowTitle = "Новая задача";
        }
        else
        {
            WindowTitle = "Редактирование задачи";
            _title = source.Title;
            _description = source.Description;
            _isCompleted = source.IsCompleted;
        }
        ValidateAllProperties();
    }

    public TodoTask ToModel(TodoTask? existing = null)
    {
        var model = existing ?? new TodoTask();
        model.Title = Title.Trim();
        model.Description = string.IsNullOrWhiteSpace(Description) ? null : Description!.Trim();
        model.IsCompleted = IsCompleted;
        return model;
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private void Save()
    {
        ValidateAllProperties();
        if (HasErrors) return;
        DialogResult = true;
        RequestClose?.Invoke(this, true);
    }

    private bool CanSave() => !HasErrors && !string.IsNullOrWhiteSpace(Title);

    [RelayCommand]
    private void Cancel()
    {
        DialogResult = false;
        RequestClose?.Invoke(this, false);
    }
}
