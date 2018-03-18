using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using TouchbarMaker.Tools;
using TouchbarMaker.ViewModels;

namespace TouchbarMaker.Views
{
    public partial class MainWindow : MetroWindow
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

        private async void OnNewSessionClicked(object sender, RoutedEventArgs e)
        {
            var settings = new MetroDialogSettings {DefaultText = MainViewModel.SelectedElementNode?.Name ?? "myApp" };
            var result = await this.ShowInputAsync("Create new Project", "App executable name\n'make sure to mach the *.exe name'", settings);
            TreeView.MouseDoubleClick -= OnTreeViewDoubleClick;
            MainViewModel.PropertyChanged -= MainViewModelOnPropertyChanged;
            CreateNewSession(result);
        }

        private void OnLoadClicked(object sender, RoutedEventArgs e)
        {
            
        }

        private async void OnExportClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                var toucharDefinition = MainViewModel.TreeElements.ToList().ConvertFromNodes(MainViewModel.ApplicationName);
                var xml = toucharDefinition.ToXmlAsString();
                File.WriteAllText(Core.Tools.GetPathToStore(MainViewModel.ApplicationName), xml);
                await this.ShowMessageAsync("Export result", "Exported successfully.");
            }
            catch (DirectoryNotFoundException)
            {
                await this.ShowMessageAsync("Export result", "Directory not found!\nAre you running from inside Parallels?", MessageDialogStyle.Affirmative, new MetroDialogSettings{ColorScheme = MetroDialogColorScheme.Inverted});
            }
            catch (IOException)
            {
                await this.ShowMessageAsync("Export result", "Could not access file system.", MessageDialogStyle.Affirmative, new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Inverted });
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("Export result", "Export failed.\nReason: " + ex, MessageDialogStyle.Affirmative, new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Inverted });
            }
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

            TreeView.MouseDoubleClick += OnTreeViewDoubleClick;

            MainViewModel.SelectedElementNode = MainViewModel.TreeElements.FirstOrDefault();
            NodeContentDisplay.Visibility = Visibility.Hidden;
        }

        private async void OnTreeViewDoubleClick(object sender, MouseButtonEventArgs args)
        {
            var hit = VisualTreeHelper.HitTest(TreeView, args.GetPosition(TreeView));
            if (!(hit.VisualHit is TextBlock)) return;

            var settings = new MetroDialogSettings();
            switch (MainViewModel.SelectedElementNode.Type)
            {
                case NodeViewModel.NodeType.Root:
                    settings.DefaultText = "myApp";
                    MainViewModel.ApplicationName = await this.ShowInputAsync("Change name", "App executable name\n'make sure to mach the *.exe name'", settings) ?? MainViewModel.ApplicationName;
                    MainViewModel.TreeElements.First().Name = MainViewModel.ApplicationName;
                    break;
                case NodeViewModel.NodeType.Container:
                    settings.DefaultText = MainViewModel.SelectedElementNode.Name;
                    MainViewModel.SelectedElementNode.Name = await this.ShowInputAsync("Change node name", "Container name", settings) ?? MainViewModel.SelectedElementNode.Name;
                    break;
                case NodeViewModel.NodeType.Element:
                    settings.DefaultText = MainViewModel.SelectedElementNode.Name;
                    MainViewModel.SelectedElementNode.Name = await this.ShowInputAsync("Change node name", "Element name", settings) ?? MainViewModel.SelectedElementNode.Name;
                    break;
            }
        }
    }
}
