using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TouchbarMaker.ViewModels
{
    public class ContainerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public enum ContainerType
        {
            ScrollView,
            Popover,
            Segmented
        }

        public string Id { get; }
        public ContainerType Type { get; set; }
        public ObservableCollection<NodeViewModel> Elements { get; set; }

        public ContainerViewModel(ContainerType type, ObservableCollection<NodeViewModel> elements = null, string id = null)
        {
            Type = type;
            Elements = elements ?? new ObservableCollection<NodeViewModel>();
            Id = id ?? Guid.NewGuid().ToString().Substring(0, 8);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}