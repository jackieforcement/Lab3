using System.Windows;
using WpfLab3.Services;
using WpfLab3.ViewModels;

namespace WpfLab3.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            JsonTaskRepository repository = new JsonTaskRepository();
            DialogService dialogs = new DialogService();
            _viewModel = new MainViewModel(repository, dialogs);
            DataContext = _viewModel;
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadCommand.ExecuteAsync(null);
        }
    }
}
