using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace TouchbarMaker.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<NodeViewModel> TreeElements { get; set; } = new ObservableCollection<NodeViewModel>();

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
            }
        }

        public ICommand AddScrollViewCommand { get; set; }
        public ICommand AddButtonCommand { get; set; }
        public ICommand RemoveElementCommand { get; set; }

        public MainViewModel(string appName)
        {
            ApplicationName = appName;
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
                    var node = new NodeViewModel(ContainerViewModel.ContainerType.ScrollView)
                    {
                        Name = "New Scroll View",
                        ContainerContent = new ContainerViewModel(ContainerViewModel.ContainerType.ScrollView)
                    };
                    TreeElements.Add(node);
                },
                o =>
                {
                    if (SelectedElementNode == null)
                        return true;

                    if (SelectedElementNode != null && SelectedElementNode.Type == NodeViewModel.NodeType.Element)
                        return true;

                    return false;
                });

            AddButtonCommand = new Commander(o =>
            {
                var selected = SelectedElementNode;

                var node = new NodeViewModel(ElementViewModel.ElementType.Button)
                {
                    Name = "New Button",
                    ElementContent = new ElementViewModel(ElementViewModel.ElementType.Button)
                    {
                        Title = "New Action"
                    }
                };

                if (selected == null)
                {
                    TreeElements.Add(node);
                }
                else
                {
                    switch (selected.Type)
                    {
                        case NodeViewModel.NodeType.Container:
                            node.Parent = selected;
                            node.Parent.Elements.Add(node);
                            break;
                        case NodeViewModel.NodeType.Element:
                            if (selected.Parent == null)
                            {
                                TreeElements.Add(node);
                            }
                            else if (selected.Parent.Type == NodeViewModel.NodeType.Container)
                            {
                                node.Parent = selected.Parent;
                                node.Parent.Elements.Add(node);
                            }
                            else
                            {
                                throw new InvalidOperationException("The selected item seems to be neested by-one too deep, this is invalid!");
                            }
                            break;
                    }
                }
            }, o => true);

            RemoveElementCommand = new Commander(o =>
            {
                if (SelectedElementNode.Parent == null)
                {
                    TreeElements.Remove(SelectedElementNode);
                }
                else
                {
                    SelectedElementNode.Parent.Elements.Remove(SelectedElementNode);
                }
            }, o => SelectedElementNode != null);
        }
    }
}