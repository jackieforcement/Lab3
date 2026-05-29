using System.Windows;
using WpfLab3.ViewModels;

namespace WpfLab3.Views
{
    public partial class TaskEditWindow : Window
    {
        public TaskEditWindow()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TaskEditViewModel? oldVm = e.OldValue as TaskEditViewModel;
            if (oldVm != null)
            {
                oldVm.RequestClose -= OnRequestClose;
            }
            TaskEditViewModel? newVm = e.NewValue as TaskEditViewModel;
            if (newVm != null)
            {
                newVm.RequestClose += OnRequestClose;
            }
        }

        private void OnRequestClose(object? sender, bool? result)
        {
            DialogResult = result;
            Close();
        }
    }
}
