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
            MainViewModel = new MainViewModel("devenv.exe");
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
                    case NodeViewModel.ElementType.Root:
                        if (dialog.ShowDialog().GetValueOrDefault())
                            MainViewModel.SelectedElementNode.Name = dialog.ElementName;
                        break;
                    case NodeViewModel.ElementType.Container:
                        if (dialog.ShowDialog().GetValueOrDefault())
                            MainViewModel.SelectedElementNode.Name = dialog.ElementName;
                        break;
                    case NodeViewModel.ElementType.Element:
                        if (dialog.ShowDialog().GetValueOrDefault())
                            MainViewModel.SelectedElementNode.Content.Title = dialog.ElementName;
                        break;
                }

                dialog.Close();
            };
            NodeContentDisplay.Visibility = Visibility.Hidden;
        }

        private void MainViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(MainViewModel.SelectedElementNode))
            {
                NodeContentDisplay.Visibility = MainViewModel.SelectedElementNode.Type == NodeViewModel.ElementType.Element 
                    ? Visibility.Visible 
                    : Visibility.Hidden;
            }
        }

        private void OnNewSessionClicked(object sender, RoutedEventArgs e)
        {
            MainViewModel = new MainViewModel("new.exe");
            DataContext = MainViewModel;
        }
    }
}
