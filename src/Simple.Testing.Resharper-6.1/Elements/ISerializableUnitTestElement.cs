using System.Xml;

namespace Simple.Testing.Resharper.Elements
{
    internal interface ISerializableUnitTestElement
    {
        void WriteToXml(XmlElement element);
    }
}