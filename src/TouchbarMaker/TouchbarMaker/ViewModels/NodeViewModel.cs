using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TouchbarMaker.ViewModels
{
    public class NodeViewModel : INotifyPropertyChanged
    {
        public enum NodeType
        {
            Root,
            Container,
            Element
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public NodeType Type { get; set; }

        public NodeViewModel Parent { get; set; }
        public ObservableCollection<NodeViewModel> Elements { get; set; } = new ObservableCollection<NodeViewModel>();
        public ContainerViewModel ContainerContent { get; set; }
        public ElementViewModel ElementContent { get; set; }

        public NodeViewModel(bool isRoot = true)
        {
            if(isRoot)
                Type = NodeType.Root;
        }

        public NodeViewModel(ContainerViewModel.ContainerType containerType)
        {
            Type = NodeType.Container;
            ContainerContent = new ContainerViewModel(containerType, Elements);
        }

        public NodeViewModel(ElementViewModel.ElementType elementType)
        {
            Type = NodeType.Element;
            ElementContent = new ElementViewModel(elementType);
            ElementContent.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(ElementViewModel.Title))
                    Name = ElementContent.Title;
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}