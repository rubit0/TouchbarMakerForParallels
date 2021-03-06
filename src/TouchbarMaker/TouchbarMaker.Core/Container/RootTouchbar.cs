﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using TouchbarMaker.Core.Elements;

namespace TouchbarMaker.Core.Container
{
    public class RootTouchbar
    {
        public List<ITouchbarElement> Elements { get; private set; } = new List<ITouchbarElement>();

        private readonly string _id;

        public RootTouchbar(string id = null)
        {
            if (id == null)
                _id = "touchbar" + new Random().Next(1000, 9999);
            else
                _id = id;
        }

        /// <summary>
        /// Converts this Touchbar definition with all it's children to XML
        /// </summary>
        /// <returns>XML serialization of this container</returns>
        public XmlDocument ToXml()
        {
            if (!Elements.Any<ITouchbarElement>())
                throw new InvalidOperationException("There are no elements!");

            //Create doc
            var doc = new XmlDocument();
            var root = doc.CreateElement("Touchbar");
            doc.AppendChild(root);

            //Add root attributes
            var idAtr = doc.CreateAttribute("id");
            idAtr.Value = _id;
            root.Attributes.Append(idAtr);
            var elementsAtr = doc.CreateAttribute("defaultItemIdentifiers");
            elementsAtr.Value = GetElemenIds();
            root.Attributes.Append(elementsAtr);

            //Add child nodes
            foreach (var element in Elements)
            {
                var node = element.ToNode(doc);
                if (node != null)
                    root.AppendChild(node);
            }

            return doc;
        }

        /// <summary>
        /// Converts this Touchbar definition with all it's children to XML as string
        /// </summary>
        /// <returns>XML serialization of this container as string</returns>
        public string ToXmlAsString()
        {
            var doc = ToXml();
            string text;

            var writerSettings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true
            };

            using (var writer = new StringWriter())
            using (var xmlWriter = XmlWriter.Create(writer, writerSettings))
            {
                doc.WriteTo(xmlWriter);
                xmlWriter.Flush();

                text = writer.GetStringBuilder().ToString();
            }

            return text;
        }

        private string GetElemenIds()
        {
            var builder = new StringBuilder();
            foreach (var element in Elements)
                builder.Append(element.Id + ",");

            //Remove ',' from tail
            builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }
    }
}
