using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;

namespace TouchbarMaker.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string ApplicationName { get; set; }
        public ObservableCollection<NodeViewModel> TreeElements { get; set; }
        public NodeViewModel SelectedElementNode { get; set; }
        public ICommand AddContainerCommand { get; set; }
        public ICommand AddElementCommand { get; set; }
        public ICommand RemoveElementCommand { get; set; }

        private TreeView _treeView;

        public MainViewModel(TreeView treeView)
        {
            _treeView = treeView;
            _treeView.SelectedItemChanged += (sender, args) =>
            {
                SelectedElementNode = _treeView.SelectedItem as NodeViewModel;
                OnPropertyChanged(nameof(SelectedElementNode));

                AddContainerCommand.CanExecute(sender);
                AddElementCommand.CanExecute(sender);
                RemoveElementCommand.CanExecute(sender); 
            };

            TreeElements = new ObservableCollection<NodeViewModel>
            {
                new NodeViewModel
                {
                    Name = "Root",
                    Type = NodeViewModel.ElementType.Root
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
            AddContainerCommand = new Commander(o =>
                {
                    var selected = _treeView.SelectedItem as NodeViewModel;
                    var next = new NodeViewModel
                    {
                        Name = "New Container",
                        Type = NodeViewModel.ElementType.Container
                    };

                    switch (selected?.Type)
                    {
                        case NodeViewModel.ElementType.Root:
                            next.Parent = selected;
                            next.Parent.Elements.Add(next);
                            break;
                        case NodeViewModel.ElementType.Container:
                            next.Parent = selected;
                            next.Parent.Elements.Add(next);
                            break;
                        case NodeViewModel.ElementType.Element:
                            next.Parent = selected.Parent;
                            next.Parent.Elements.Add(next);
                            break;
                        case null:
                            next.Parent = TreeElements.FirstOrDefault();
                            next.Parent.Elements.Add(next);
                            break;
                    }
                },
                o =>
                {
                    if (!(_treeView.SelectedItem is NodeViewModel selected))
                        return false;

                    return (selected.Type == NodeViewModel.ElementType.Root ||
                            selected.Type == NodeViewModel.ElementType.Container);
                });

            AddElementCommand = new Commander(o =>
            {
                var selected = _treeView.SelectedItem as NodeViewModel;

                var next = new NodeViewModel
                {
                    Name = "New Element",
                    Type = NodeViewModel.ElementType.Element
                };

                switch (selected.Type)
                {
                    case NodeViewModel.ElementType.Root:
                        next.Parent = selected;
                        next.Parent.Elements.Add(next);
                        break;
                    case NodeViewModel.ElementType.Container:
                        next.Parent = selected;
                        next.Parent.Elements.Add(next);
                        break;
                    case NodeViewModel.ElementType.Element:
                        next.Parent = selected.Parent;
                        next.Parent.Elements.Add(next);
                        break;
                }
            }, o =>
            {
                return _treeView.SelectedItem is NodeViewModel selected;
            });

            RemoveElementCommand = new Commander(o =>
            {
                var selected = _treeView.SelectedItem as NodeViewModel;
                selected.Parent.Elements.Remove(selected);
            }, o =>
            {
                var selected = _treeView.SelectedItem as NodeViewModel;
                return !(selected == null || selected.Type == NodeViewModel.ElementType.Root);
            });
        }
    }

    public class Commander : ICommand
    {
        public bool CanExecute(object parameter)
        {
            var result = _canExecuteAction.Invoke(parameter);
            if (_canExecute != result)
            {
                _canExecute = result;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }

            return _canExecute;
        }

        public void Execute(object parameter)
        {
            _execute.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged;

        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecuteAction;
        private bool _canExecute;

        public Commander(Action<object> execute, Func<object, bool> canExecuteAction)
        {
            _execute = execute;
            _canExecuteAction = canExecuteAction;
        }
    }
}