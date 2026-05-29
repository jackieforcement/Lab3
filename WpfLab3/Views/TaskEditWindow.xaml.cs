using System.ComponentModel;
using System.Windows;
using WpfLab3.ViewModels;

namespace WpfLab3.Views;

public partial class TaskEditWindow : Window
{
    public TaskEditWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is TaskEditViewModel oldVm)
            oldVm.RequestClose -= OnRequestClose;
        if (e.NewValue is TaskEditViewModel newVm)
            newVm.RequestClose += OnRequestClose;
    }

    private void OnRequestClose(object? sender, bool? result)
    {
        DialogResult = result;
        Close();
    }
}
