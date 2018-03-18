using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ColorPickerWPF.Code;
using Microsoft.Win32;
using TouchbarMaker.Core;

namespace TouchbarMaker.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<NodeViewModel> TreeElements { get; set; }

        private string _applicationName;
        public string ApplicationName
        {
            get => _applicationName;
            set
            {
                _applicationName = value;
                OnPropertyChanged();
            }
        }

        private NodeViewModel _selectedElementNode;
        public NodeViewModel SelectedElementNode
        {
            get => _selectedElementNode;
            set
            {
                _selectedElementNode = value;
                OnPropertyChanged();
                AddScrollViewCommand.CanExecute(this);
                AddButtonCommand.CanExecute(this);
                RemoveElementCommand.CanExecute(this);
                AddElementIconCommand.CanExecute(this);
            }
        }

        public ICommand AddScrollViewCommand { get; set; }
        public ICommand AddButtonCommand { get; set; }
        public ICommand RemoveElementCommand { get; set; }
        public ICommand AddElementIconCommand { get; set; }


        public MainViewModel(string appName)
        {
            ApplicationName = appName;

            TreeElements = new ObservableCollection<NodeViewModel>
            {
                new NodeViewModel
                {
                    Name = ApplicationName,
                    Type = NodeViewModel.NodeType.Root
                }
            };

            BuildCommands();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void BuildCommands()
        {
            AddScrollViewCommand = new Commander(o =>
                {
                    var selected = SelectedElementNode;
                    var next = new NodeViewModel
                    {
                        Name = "New Scroll View",
                        Type = NodeViewModel.NodeType.Container
                    };
                    next.ContainerContent =
                        new ContainerViewModel(ContainerViewModel.ContainerType.ScrollView, next.Elements);

                    switch (selected?.Type)
                    {
                        case NodeViewModel.NodeType.Root:
                            next.Parent = selected;
                            next.Parent.Elements.Add(next);
                            break;
                        case NodeViewModel.NodeType.Container:
                            next.Parent = selected;
                            next.Parent.Elements.Add(next);
                            break;
                        case NodeViewModel.NodeType.Element:
                            next.Parent = selected.Parent;
                            next.Parent.Elements.Add(next);
                            break;
                        case null:
                            next.Parent = TreeElements.FirstOrDefault();
                            next.Parent?.Elements.Add(next);
                            break;
                    }
                },
                o =>
                {
                    if (!(SelectedElementNode is NodeViewModel selected))
                        return false;

                    return selected.Type == NodeViewModel.NodeType.Root;
                });

            AddButtonCommand = new Commander(o =>
            {
                var selected = SelectedElementNode;

                var next = new NodeViewModel
                {
                    Name = "New Button",
                    Type = NodeViewModel.NodeType.Element,
                    ElementContent = new ElementViewModel(ElementViewModel.ElementType.Button)
                    {
                        Title = "New Action"
                    }
                };

                switch (selected.Type)
                {
                    case NodeViewModel.NodeType.Root:
                        next.Parent = selected;
                        next.Parent.Elements.Add(next);
                        break;
                    case NodeViewModel.NodeType.Container:
                        next.Parent = selected;
                        next.Parent.Elements.Add(next);
                        break;
                    case NodeViewModel.NodeType.Element:
                        next.Parent = selected.Parent;
                        next.Parent.Elements.Add(next);
                        break;
                }
            }, o =>
            {
                return SelectedElementNode is NodeViewModel selected;
            });

            RemoveElementCommand = new Commander(o =>
            {
                var selected = SelectedElementNode;
                selected.Parent.Elements.Remove(selected);
            }, o =>
            {
                var selected = SelectedElementNode;
                return selected != null && selected.Type != NodeViewModel.NodeType.Root;
            });

            AddElementIconCommand = new Commander(o =>
            {
                var fileDialog = new OpenFileDialog
                {
                    Title = "Select a picture",
                    Filter = "All supported graphics|*.jpg;*.jpeg;*.png;*.bmp|" +
                             "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                             "Portable Network Graphic (*.png)|*.png"
                };

                if (fileDialog.ShowDialog() == true)
                {
                    var image = new BitmapImage(new Uri(fileDialog.FileName));

                    if (!image.IsAcceptedIconSize())
                    {
                        MessageBox.Show("The image has a bad format.");
                    }
                    else
                    {
                        var selected = SelectedElementNode;
                        selected.ElementContent.Icon = image;
                    }
                }
            }, o =>
            {
                var selected = SelectedElementNode;
                return selected != null && selected.Type == NodeViewModel.NodeType.Element;
            });
        }
    }
}