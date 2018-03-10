using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using TouchbarMaker.Core.Container;
using TouchbarMaker.ViewModels;

namespace TouchbarMaker.Tools
{
    public static class Converter
    {
        public static RootTouchbar ConvertFromNodes(this List<NodeViewModel> graph)
        {
            var root = new RootTouchbar(graph[0].Content.Id);

            foreach (var element in graph[0].Elements)
            {
                switch (element.Type)
                {
                    case NodeViewModel.ElementType.Root:
                        break;
                    case NodeViewModel.ElementType.Container:
                        
                        break;
                    case NodeViewModel.ElementType.Element:

                        break;
                }
            }

            return root;
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
    }
}
