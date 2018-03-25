using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using TouchbarMaker.Core;

namespace TouchbarMaker.Tools
{
    public static class Converter
    {
        public static Bitmap ConvertBitmapImage(BitmapImage image)
        {
            using (var stream = new MemoryStream())
            {
                var encoder = GetEncoderFromFile(Path.GetExtension(image.UriSource.AbsolutePath));
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(stream);
                var bitmap = new Bitmap(stream);

                if (!bitmap.IsAcceptedIconSize())
                {
                    var size = ResizeFit(new Size(image.PixelWidth, image.PixelHeight), new Size(60, 60));
                    return new Bitmap(bitmap, size);
                }
                else
                {
                    return new Bitmap(bitmap);
                }
            }
        }

        // https://stackoverflow.com/a/17197425
        private static Size ResizeFit(Size originalSize, Size maxSize)
        {
            var widthRatio = (double)maxSize.Width / (double)originalSize.Width;
            var heightRatio = (double)maxSize.Height / (double)originalSize.Height;
            var minAspectRatio = Math.Min(widthRatio, heightRatio);
            if (minAspectRatio > 1)
                return originalSize;

            return new Size((int)(originalSize.Width * minAspectRatio), (int)(originalSize.Height * minAspectRatio));
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

        public static string ConvertColorToHex(this Color color)
        {
            return color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }
    }
}
