using System;
using System.Collections.Generic;
using System.Xml;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using Simple.Testing.Resharper.Elements;

namespace Simple.Testing.Resharper
{
    namespace XunitContrib.Runner.ReSharper.UnitTestProvider
    {
        using ReadFromXmlFunc = Func<XmlElement, IUnitTestElement, ISolution, SimpleTestingElementFactory, IUnitTestElement>;

        [SolutionComponent]
        public class SimpleTestingElementSerializer : IUnitTestElementSerializer
        {
            private static readonly IDictionary<string, ReadFromXmlFunc> DeserialiseMap = new Dictionary<string, ReadFromXmlFunc>
                                                                                          {
                                                                                              {typeof (SpecificationContainerElement).Name, SpecificationContainerElement.ReadFromXml},
                                                                                              {typeof (SpecificationElement).Name, SpecificationElement.ReadFromXml}
                                                                                          };

            private readonly SimpleTestingTestProvider _provider;
            private readonly SimpleTestingElementFactory _unitTestElementFactory;
            private readonly ISolution _solution;

            public SimpleTestingElementSerializer(SimpleTestingTestProvider provider, SimpleTestingElementFactory unitTestElementFactory, ISolution solution)
            {
                _provider = provider;
                _unitTestElementFactory = unitTestElementFactory;
                _solution = solution;
            }

            public void SerializeElement(XmlElement parent, IUnitTestElement element)
            {
                parent.SetAttribute("type", element.GetType().Name);

                var writableUnitTestElement = (ISerializableUnitTestElement)element;
                writableUnitTestElement.WriteToXml(parent);
            }

            public IUnitTestElement DeserializeElement(XmlElement parent, IUnitTestElement parentElement)
            {
                if (!parent.HasAttribute("type"))
                    throw new ArgumentException("Element has no Type attribute");

                ReadFromXmlFunc func;
                if (DeserialiseMap.TryGetValue(parent.GetAttribute("type"), out func))
                    return func(parent, parentElement, _solution, _unitTestElementFactory);

                throw new ArgumentException("Element has no Type attribute");
            }

            public IUnitTestProvider Provider
            {
                get { return _provider; }
            }
        }
    }
}