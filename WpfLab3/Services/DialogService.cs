using System.Windows;
using WpfLab3.ViewModels;
using WpfLab3.Views;

namespace WpfLab3.Services
{
    public class DialogService : IDialogService
    {
        public bool ShowTaskEditor(TaskEditViewModel viewModel)
        {
            TaskEditWindow window = new TaskEditWindow();
            window.DataContext = viewModel;
            if (Application.Current != null)
            {
                window.Owner = Application.Current.MainWindow;
            }
            bool? result = window.ShowDialog();
            if (result.HasValue && result.Value)
            {
                return true;
            }
            return false;
        }

        public bool Confirm(string message, string title = "Подтверждение")
        {
            Window? owner = null;
            if (Application.Current != null)
            {
                owner = Application.Current.MainWindow;
            }
            MessageBoxResult result;
            if (owner != null)
            {
                result = MessageBox.Show(
                    owner,
                    message,
                    title,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
            }
            else
            {
                result = MessageBox.Show(
                    message,
                    title,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
            }
            return result == MessageBoxResult.Yes;
        }
    }
}
