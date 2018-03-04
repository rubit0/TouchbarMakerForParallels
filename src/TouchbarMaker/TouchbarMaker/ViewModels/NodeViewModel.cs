using System.Collections.ObjectModel;

namespace TouchbarMaker.ViewModels
{
    public class NodeViewModel
    {
        public enum ElementType
        {
            Root,
            Container,
            Element
        }

        public string Name { get; set; }
        public ElementType Type { get; set; }
        public NodeViewModel Parent { get; set; }
        public ObservableCollection<NodeViewModel> Elements { get; set; } = new ObservableCollection<NodeViewModel>();
        public ElementViewModel Content { get; set; }

        public NodeViewModel()
        {
            Content = new ElementViewModel();
            Content.Name = "ABC";
        }
    }
}