using System.Xml;

namespace TouchbarMaker.Core.Elements
{
    public interface ITouchbarElement
    {
        string Id { get; }
        XmlNode ToNode(XmlDocument doc);
    }
}
