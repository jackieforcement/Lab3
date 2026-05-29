using System.Windows;
using WpfLab3.Services;
using WpfLab3.ViewModels;

namespace WpfLab3.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var vm = new MainViewModel(new JsonTaskRepository(), new DialogService());
        DataContext = vm;
        Loaded += async (_, _) => await vm.LoadCommand.ExecuteAsync(null);
    }
}
