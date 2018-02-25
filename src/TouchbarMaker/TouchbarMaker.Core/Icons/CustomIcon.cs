namespace TouchbarMaker.Core.Icons
{
    /// <summary>
    /// Represents a custom icon structure where an image is encoded in Base64
    /// </summary>
    public class CustomIcon
    {
        public string Name { get; }

        /// <summary>
        /// Base64 encoded image
        /// </summary>
        public string EncodedIcon { get; }

        public CustomIcon(string name, string encodedIcon)
        {
            Name = name;
            EncodedIcon = encodedIcon;
        }
    }
}