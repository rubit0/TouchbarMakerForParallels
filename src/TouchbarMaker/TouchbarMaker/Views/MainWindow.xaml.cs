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
                if (MainViewModel.SelectedElementNode == null)
                {
                    NodeContentDisplay.Visibility = Visibility.Hidden;
                }
                else
                {
                    //Check if is element
                    if (MainViewModel.SelectedElementNode.Type == NodeViewModel.NodeType.Element)
                    {
                        NodeContentDisplay.Visibility = Visibility.Visible;

                        if ((int) MainViewModel.SelectedElementNode.ElementContent.Type < 200)
                        {
                            DefaultNodeView.Visibility = Visibility.Visible;
                            SpecialNodeView.Visibility = Visibility.Hidden;
                        }
                        else
                        {
                            DefaultNodeView.Visibility = Visibility.Hidden;
                            SpecialNodeView.Visibility = Visibility.Visible;
                            SpecialElementType.SelectedItem = SpecialElementType.Items[(int)MainViewModel.SelectedElementNode.ElementContent.Type - 200];
                        }
                    }
                    else
                    {
                        NodeContentDisplay.Visibility = Visibility.Hidden;
                    }
                    NodeContentDisplay.Visibility = MainViewModel.SelectedElementNode.Type == NodeViewModel.NodeType.Element
                        ? Visibility.Visible
                        : Visibility.Hidden;
                }
            }
        }

        private async void OnNewSessionClicked(object sender, RoutedEventArgs e)
        {
            var settings = new MetroDialogSettings { DefaultText = "myApp.exe" };
            var result = await this.ShowInputAsync("Create new Project", "Type in the applications executable name\ne.g. for Paint = 'paint.exe'", settings);
            if(string.IsNullOrWhiteSpace(result))
                return;

            TreeView.MouseDoubleClick -= OnTreeViewDoubleClick;
            TreeView.MouseDown -= OnTreeViewOnMouseDown;
            MainViewModel.PropertyChanged -= MainViewModelOnPropertyChanged;
            CreateNewSession(result);
        }

        private void OnLoadClicked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
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
            TreeView.MouseDown += OnTreeViewOnMouseDown;

            MainViewModel.SelectedElementNode = MainViewModel.TreeElements.FirstOrDefault();
            NodeContentDisplay.Visibility = Visibility.Hidden;
        }

        private void OnTreeViewOnMouseDown(object o, MouseButtonEventArgs args)
        {
            if(TreeView.SelectedItem == null)
                return;

            var hit = VisualTreeHelper.HitTest(TreeView, args.GetPosition(TreeView));
            if (!(hit.VisualHit is TextBlock))
            {
                TreeView.ClearSelection();
                MainViewModel.SelectedElementNode = null;
            }
        }

        private async void OnTreeViewDoubleClick(object sender, MouseButtonEventArgs args)
        {
            if(MainViewModel.SelectedElementNode == null)
                return;

            var settings = new MetroDialogSettings();
            switch (MainViewModel.SelectedElementNode.Type)
            {
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

        private void OnSpecialElementSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainViewModel?.SelectedElementNode == null)
                return;

            var index = SpecialElementType.SelectedIndex;
            switch (index)
            {
                case 0:
                    MainViewModel.SelectedElementNode.ElementContent.Type = ElementViewModel.ElementType.Emoji;
                    break;
                case 1:
                    MainViewModel.SelectedElementNode.ElementContent.Type = ElementViewModel.ElementType.FlexibleSpace;
                    break;
                case 2:
                    MainViewModel.SelectedElementNode.ElementContent.Type = ElementViewModel.ElementType.SmallSpace;
                    break;
                case 3:
                    MainViewModel.SelectedElementNode.ElementContent.Type = ElementViewModel.ElementType.LargeSpace;
                    break;
                default:
                    break;
            }
        }
    }
}
