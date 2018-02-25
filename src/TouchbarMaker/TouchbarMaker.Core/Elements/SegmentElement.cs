namespace TouchbarMaker.Core.Elements
{
    /// <summary>
    /// A type of element that needs to be used in tandem with a SegmentedControl
    /// </summary>
    public class SegmentElement : ButtonElement
    {
        protected override string ElementName => "Segment";

        public SegmentElement(string id, string keyCode, string title = null, string imageData = null)
            : base(id, keyCode, title, imageData)
        {
        }
    }
}