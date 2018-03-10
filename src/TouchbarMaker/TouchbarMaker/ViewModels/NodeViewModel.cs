using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TouchbarMaker.ViewModels
{
    public class NodeViewModel : INotifyPropertyChanged
    {
        public enum ElementType
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

        public ElementType Type { get; set; }
        public NodeViewModel Parent { get; set; }
        public ObservableCollection<NodeViewModel> Elements { get; set; } = new ObservableCollection<NodeViewModel>();
        public ElementViewModel Content { get; set; }

        public NodeViewModel()
        {
            Content = new ElementViewModel();

            Content.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(ElementViewModel.Title))
                    Name = Content.Title;
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}