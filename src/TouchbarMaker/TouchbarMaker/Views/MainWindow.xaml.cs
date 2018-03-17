using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
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
            var dialog = new ChangeNameDialog(MainViewModel.SelectedElementNode.Name);
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                var appName = dialog.ElementName ?? "myApp";
                CreateNewSession(appName);
            }
        }

        private void OnLoadClicked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnExportClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                var toucharDefinition = MainViewModel.TreeElements.ToList().ConvertFromNodes();
                var xml = toucharDefinition.ToXmlAsString();
                File.WriteAllText(Core.Tools.GetPathToStore(MainViewModel.ApplicationName), xml);
                MessageBox.Show("Exported successfully!.");
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("Directory not found!\nAre you running from inside Parallels?", "Export failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException)
            {
                MessageBox.Show("Could not access file system.", "Export failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Export failed.\nReason: " + ex, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnExitClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnKeyPressedInTree(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && MainViewModel.RemoveElementCommand.CanExecute(TreeView))
                MainViewModel.RemoveElementCommand.Execute(TreeView);
        }

        private void CreateNewSession(string appName = "myApp")
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
                    case NodeViewModel.NodeType.Root:
                        if (dialog.ShowDialog().GetValueOrDefault())
                        {
                            MainViewModel.ApplicationName = dialog.ElementName;
                            MainViewModel.TreeElements.First().Name = MainViewModel.ApplicationName;
                        }
                        break;
                    case NodeViewModel.NodeType.Container:
                        if (dialog.ShowDialog().GetValueOrDefault())
                            MainViewModel.SelectedElementNode.Name = dialog.ElementName;
                        break;
                    case NodeViewModel.NodeType.Element:
                        if (dialog.ShowDialog().GetValueOrDefault())
                            MainViewModel.SelectedElementNode.Name = dialog.ElementName;
                        break;
                }

                dialog.Close();
            };
            NodeContentDisplay.Visibility = Visibility.Hidden;
        }
    }
}
