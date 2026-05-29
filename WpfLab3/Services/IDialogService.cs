using WpfLab3.ViewModels;

namespace WpfLab3.Services;

public interface IDialogService
{
    bool ShowTaskEditor(TaskEditViewModel viewModel);
    bool Confirm(string message, string title = "Подтверждение");
}
