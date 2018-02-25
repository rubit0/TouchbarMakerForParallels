using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

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
