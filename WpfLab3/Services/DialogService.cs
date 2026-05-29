using System.Windows;
using WpfLab3.ViewModels;
using WpfLab3.Views;

namespace WpfLab3.Services;

public class DialogService : IDialogService
{
    public bool ShowTaskEditor(TaskEditViewModel viewModel)
    {
        var window = new TaskEditWindow
        {
            DataContext = viewModel,
            Owner = Application.Current?.MainWindow
        };
        return window.ShowDialog() == true;
    }

    public bool Confirm(string message, string title = "Подтверждение")
    {
        var result = MessageBox.Show(
            Application.Current?.MainWindow!,
            message,
            title,
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);
        return result == MessageBoxResult.Yes;
    }
}
