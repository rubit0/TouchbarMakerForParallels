using System.ComponentModel;
using System.Windows;
using TouchbarMaker.ViewModels;

namespace TouchbarMaker.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel MainViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            MainViewModel = new MainViewModel {ApplicationName = "devenv.exe"};
            DataContext = MainViewModel;
            MainViewModel.PropertyChanged += MainViewModelOnPropertyChanged;
            TreeView.SelectedItemChanged += (sender, args) =>
            {
                MainViewModel.SelectedElementNode = TreeView.SelectedItem as NodeViewModel;
            };
            TreeView.MouseDoubleClick += (sender, args) =>
            {
                var dialog = new ChangeNameDialog(MainViewModel.SelectedElementNode.Name);
                if (dialog.ShowDialog().GetValueOrDefault())
                    MainViewModel.SelectedElementNode.Name = dialog.ElementName;

                dialog.Close();
            };
            NodeContentDisplay.Visibility = Visibility.Hidden;
        }

        private void MainViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(MainViewModel.SelectedElementNode))
            {
                NodeContentDisplay.Visibility = MainViewModel.SelectedElementNode.Type == NodeViewModel.ElementType.Element 
                    ? Visibility.Visible 
                    : Visibility.Hidden;
            }
        }
    }
}
