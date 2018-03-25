using System;
using System.Collections.Generic;
using System.Linq;
using TouchbarMaker.Core.Container;
using TouchbarMaker.Core.Elements;
using TouchbarMaker.ViewModels;

namespace TouchbarMaker.Tools
{
    public static class ViewModelConverter
    {
        public static RootTouchbar ConvertFromNodes(this List<NodeViewModel> graph, string appName)
        {
            var root = new RootTouchbar(appName + "_" + Guid.NewGuid().ToString().Substring(0, 8));

            foreach (var node in graph)
            {
                switch (node.Type)
                {
                    case NodeViewModel.NodeType.Container:
                        root.Elements.Add(node.ConvertToTouchbarElement());
                        break;
                    case NodeViewModel.NodeType.Element:
                        root.Elements.Add(node.ElementContent.ConvertToButtonElement());
                        break;
                }
            }

            return root;
        }

        public static ButtonElement ConvertToButtonElement(this ElementViewModel node)
        {
            var button = new ButtonElement(node.Id, node.KeyCode, node.Title, node.Bitmap != null ? node.EncodedIcon : null)
            {
                ScaleImage2X = node.ScaleImage2X,
                Width = node.Width
            };

            if (node.TextColor.HasValue)
                button.TextColor = node.TextColor.Value.ConvertColorToHex();
            if (node.BackgroundColor.HasValue)
                button.BackgroundColor = node.BackgroundColor.Value.ConvertColorToHex();

            return button;
        }

        public static SegmentElement ConvertToSegmentElement(this ElementViewModel node)
        {
            var button = new SegmentElement(node.Id, node.KeyCode, node.Title, node.Bitmap != null ? node.EncodedIcon : null)
            {
                ScaleImage2X = node.ScaleImage2X,
                Width = node.Width
            };

            if (node.TextColor.HasValue)
                button.TextColor = node.TextColor.Value.ConvertColorToHex();
            if (node.BackgroundColor.HasValue)
                button.BackgroundColor = node.BackgroundColor.Value.ConvertColorToHex();

            return button;
        }

        public static ITouchbarElement ConvertToTouchbarElement(this NodeViewModel container)
        {
            ITouchbarElement element = null;

            switch (container.ContainerContent.Type)
            {
                case ContainerViewModel.ContainerType.ScrollView:
                    element = new ScrollViewControl(container.ContainerContent.Id)
                    {
                        ChildElements = container.Elements.Select(node => ConvertToButtonElement(node.ElementContent)).ToList()
                    };
                    break;
                case ContainerViewModel.ContainerType.Popover:
                    throw new NotImplementedException();
                    break;
                case ContainerViewModel.ContainerType.Segmented:
                    element = new SegmentedControl(container.ContainerContent.Id)
                    {
                        ChildElements = container.Elements.Select(node => node.ElementContent.ConvertToSegmentElement()).ToList()
                    };
                    break;
            }

            return element;
        }
    }
}