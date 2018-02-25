using System.Xml;

namespace TouchbarMaker.Core.Elements
{
    /// <summary>
    /// A special element that can act as spacing and emoji picker.
    /// </summary>
    public class SpecialElement : ITouchbarElement
    {
        private const string SmallSpace = "NSTouchBarItemIdentifierFixedSpaceSmall";
        private const string LargeSpace = "NSTouchBarItemIdentifierFixedSpaceLarge";
        private const string FlexibleSpace = "NSTouchBarItemIdentifierFlexibleSpace";
        private const string EmojiPicker = "NSTouchBarItemIdentifierCharacterPicker";

        public enum SpecialType
        {
            Flexible,
            Small,
            Large,
            EmojiPicker
        }

        public string Id { get; set; }

        public SpecialElement(SpecialType specialType)
        {
            switch (specialType)
            {
                case SpecialType.Flexible:
                    Id = FlexibleSpace;
                    break;
                case SpecialType.Small:
                    Id = SmallSpace;
                    break;
                case SpecialType.Large:
                    Id = LargeSpace;
                    break;
                case SpecialType.EmojiPicker:
                    Id = EmojiPicker;
                    break;
            }
        }

        public XmlNode ToNode(XmlDocument doc)
        {
            return null;
        }
    }
}