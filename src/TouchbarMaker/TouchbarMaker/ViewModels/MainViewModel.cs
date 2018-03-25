using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public ICommand AddSegmentedControlCommand { get; set; }
        public ICommand AddPopoverControlCommand { get; set; }
        public ICommand AddButtonCommand { get; set; }
        public ICommand AddSpecialElementCommand { get; set; }
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

        private bool CanAddContainer()
        {
            if (SelectedElementNode == null)
                return true;

            if (SelectedElementNode != null && SelectedElementNode.Type == NodeViewModel.NodeType.Element)
                return true;

            return false;
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
                }, o => CanAddContainer());

            AddSegmentedControlCommand = new Commander(o =>
            {
                var node = new NodeViewModel(ContainerViewModel.ContainerType.Segmented)
                {
                    Name = "New Segmented Control",
                    ContainerContent = new ContainerViewModel(ContainerViewModel.ContainerType.Segmented)
                };
                TreeElements.Add(node);
            }, o => CanAddContainer());

            AddPopoverControlCommand = new Commander(o =>
            {
                var node = new NodeViewModel(ContainerViewModel.ContainerType.Popover)
                {
                    Name = "New Popover Control",
                    ContainerContent = new ContainerViewModel(ContainerViewModel.ContainerType.Popover)
                };

                node.Elements.Add(new NodeViewModel(ElementViewModel.ElementType.PopoverTouchbar)
                {
                    Name = "Sub Touchbar",
                    Parent = node
                });

                node.Elements.Add(new NodeViewModel(ElementViewModel.ElementType.PopverPressAndHold)
                {
                    Name = "Press And Hold",
                    Parent = node
                });

                TreeElements.Add(node);

            }, o => CanAddContainer());

            AddButtonCommand = new Commander(o =>
            {
                var selected = SelectedElementNode;

                var node = new NodeViewModel(ElementViewModel.ElementType.Button)
                {
                    Name = "New Button",
                    ElementContent = new ElementViewModel(ElementViewModel.ElementType.Button)
                    {
                        Title = "New Action",
                        KeyCode = "escape"
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
            });

            AddSpecialElementCommand = new Commander(o =>
            {
                var selected = SelectedElementNode;

                var node = new NodeViewModel(ElementViewModel.ElementType.Button)
                {
                    Name = "Special Element",
                    ElementContent = new ElementViewModel(ElementViewModel.ElementType.Emoji)
                    {
                        Title = "Special"
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
            });

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
            }, o => SelectedElementNode != null && (SelectedElementNode.Type == NodeViewModel.NodeType.Element && (int)SelectedElementNode.ElementContent.Type < 100));
        }
    }
}