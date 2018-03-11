using System.ComponentModel;
using System.Linq;
using System.Windows;
using TouchbarMaker.Tools;
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
            CreateNewSession();
        }

        private void MainViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(MainViewModel.SelectedElementNode))
            {
                NodeContentDisplay.Visibility = MainViewModel.SelectedElementNode.Type == NodeViewModel.NodeType.Element 
                    ? Visibility.Visible 
                    : Visibility.Hidden;
            }
        }

        private void OnNewSessionClicked(object sender, RoutedEventArgs e)
        {
            CreateNewSession();
        }

        private void CreateNewSession(string appName = "app.exe")
        {
            MainViewModel = new MainViewModel(appName);
            DataContext = MainViewModel;
            MainViewModel.PropertyChanged += MainViewModelOnPropertyChanged;

            TreeView.SelectedItemChanged += (sender, args) =>
            {
                MainViewModel.SelectedElementNode = TreeView.SelectedItem as NodeViewModel;
            };

            TreeView.MouseDoubleClick += (sender, args) =>
            {
                var dialog = new ChangeNameDialog(MainViewModel.SelectedElementNode.Name);

                switch (MainViewModel.SelectedElementNode.Type)
                {
                    case NodeViewModel.NodeType.Container:
                        if (dialog.ShowDialog().GetValueOrDefault())
                            MainViewModel.SelectedElementNode.Name = dialog.ElementName;
                        break;
                    case NodeViewModel.NodeType.Element:
                        if (dialog.ShowDialog().GetValueOrDefault())
                            MainViewModel.SelectedElementNode.ElementContent.Title = dialog.ElementName;
                        break;
                }

                dialog.Close();
            };
            NodeContentDisplay.Visibility = Visibility.Hidden;
        }

        private void OnExportClicked(object sender, RoutedEventArgs e)
        {
            var toucharDefinition = MainViewModel.TreeElements.ToList().ConvertFromNodes();
        }
    }
}
