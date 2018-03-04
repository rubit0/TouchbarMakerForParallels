﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using TouchbarMaker.Core;

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
        public ICommand AddElementIconCommand { get; set; }

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
                AddElementIconCommand.CanExecute(sender);
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
                        var selected = _treeView.SelectedItem as NodeViewModel;
                        selected.Content.Icon = image;
                    }
                }
            }, o =>
            {
                var selected = _treeView.SelectedItem as NodeViewModel;
                return selected != null && selected.Type != NodeViewModel.ElementType.Root;
            });
        }
    }
}