using System;
using System.Drawing;

namespace TouchbarMaker.Core
{
    public static class Tools
    {
        private static readonly ImageConverter Converter = new ImageConverter();
        private const int ValidMinImageSize = 36;
        private const int ValidMaxImageSize = 60;

        /// <summary>
        /// Convert an Image to an base64 encoded string for touchbar use
        /// </summary>
        /// <param name="image">Source image</param>
        /// <returns>Base64 encoded icon</returns>
        public static string ToEncodedIconFromBitmap(this Bitmap image)
        {
            var bytes = Converter.ConvertTo(image, typeof(byte[])) as byte[];
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Check if the image has the accepted touchbar icon size
        /// </summary>
        /// <param name="image">Target image</param>
        /// <returns>Is accapted size?</returns>
        public static bool IsAcceptedIconSize(this Bitmap image)
        {
            return IsAcceptedIconSize(image.Height, image.Width);
        }

        /// <summary>
        /// Check if the wight and width is in accepted touchbar icon size range
        /// </summary>
        /// <returns>Is accapted size?</returns>
        public static bool IsAcceptedIconSize(int height, int width)
        {
            return height >= ValidMinImageSize && height <= ValidMaxImageSize &&
                   width >= ValidMinImageSize && width <= ValidMaxImageSize;
        }

        /// <summary>
        /// Remove the '#' pound symbol from a hex string as is not accepted by Parallels touchbar system
        /// </summary>
        /// <param name="color"></param>
        /// <returns>No more pund symbol</returns>
        public static string SanitizeColorHexString(string color)
        {
            if (color.Contains("#") && color.Length == 9)
            {
                var sanitized = color.Remove(0, 1);
                return sanitized;
            }

            throw new InvalidOperationException($"The provided color string [{color}] is not in a correct hex-color format.");
        }
    }
}
