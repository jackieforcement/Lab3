using WpfLab3.Services;
using WpfLab3.ViewModels;

namespace WpfLab3.Tests;

public class FakeDialogService : IDialogService
{
    public Func<TaskEditViewModel, bool>? EditorHandler { get; set; }
    public bool ConfirmResult { get; set; } = true;

    public bool ShowTaskEditor(TaskEditViewModel viewModel)
        => EditorHandler?.Invoke(viewModel) ?? false;

    public bool Confirm(string message, string title = "Подтверждение") => ConfirmResult;
}
