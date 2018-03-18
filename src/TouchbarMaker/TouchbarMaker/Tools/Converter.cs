using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using TouchbarMaker.Core.Container;
using TouchbarMaker.Core.Elements;
using TouchbarMaker.ViewModels;

namespace TouchbarMaker.Tools
{
    public static class Converter
    {
        public static RootTouchbar ConvertFromNodes(this List<NodeViewModel> graph, string appName)
        {
            var root = new RootTouchbar(appName + "_" + Guid.NewGuid().ToString().Substring(0, 8));

            foreach (var element in graph[0].Elements)
            {
                switch (element.Type)
                {
                    case NodeViewModel.NodeType.Container:
                        root.Elements.Add(element.ContainerContent.ConvertToTouchbarElement());
                        break;
                    case NodeViewModel.NodeType.Element:
                        root.Elements.Add(element.ElementContent.ConvertToTouchbarElement());
                        break;
                }
            }

            return root;
        }

        public static ITouchbarElement ConvertToTouchbarElement(this ElementViewModel node)
        {
            //TODO Here should be a switch to act on the element type
            var button = new ButtonElement(node.Id, node.KeyCode, node.Title)
            {
                ScaleImage2X = node.ScaleImage2X,
                Width = node.Width
            };

            if (node.Bitmap != null)
                button.Image = node.EncodedIcon;
            if (node.TextColor.HasValue)
                button.TextColor = node.TextColor.Value.ConvertColorToHex();
            if (node.BackgroundColor.HasValue)
                button.BackgroundColor = node.BackgroundColor.Value.ConvertColorToHex();

            return button;
        }

        public static ITouchbarElement ConvertToTouchbarElement(this ContainerViewModel container)
        {
            ITouchbarElement element = null;
            var children = container.Elements.Select(node => node.ElementContent.ConvertToTouchbarElement()).ToList();

            switch (container.Type)
            {
                case ContainerViewModel.ContainerType.ScrollView:
                    element = new ScrollViewControl(container.Id)
                    {
                        ChildElements = children
                    };
                    break;
                case ContainerViewModel.ContainerType.Popover:
                    throw new NotImplementedException();
                    break;
                case ContainerViewModel.ContainerType.Segmented:
                    throw new NotImplementedException();
                    break;
            }

            return element;
        }

        public static Bitmap ConvertBitmapImage(BitmapImage image)
        {
            using (var stream = new MemoryStream())
            {
                var encoder = GetEncoderFromFile(Path.GetExtension(image.UriSource.AbsolutePath));
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(stream);
                var bitmap = new Bitmap(stream);

                return new Bitmap(bitmap);
            }
        }

        private static BitmapEncoder GetEncoderFromFile(string extension)
        {
            switch (extension)
            {
                case ".png":
                    return new PngBitmapEncoder();
                case ".jpg":
                    return new JpegBitmapEncoder();
                case ".jpeg":
                    return new JpegBitmapEncoder();
                default:
                    return new BmpBitmapEncoder();
            }
        }

        private static string ConvertColorToHex(this Color color)
        {
            return color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

    }
}
