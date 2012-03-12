using System.Collections.Generic;
using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using Simple.Testing.Resharper.Elements;
using Simple.Testing.Resharper.Helpers;

namespace Simple.Testing.Resharper
{
    [MetadataUnitTestExplorer]
    public class SimpleTestingMetadataExplorer : IUnitTestMetadataExplorer
    {
        readonly SimpleTestingTestProvider _provider;
        readonly SimpleTestingElementFactory _elementFactory;
        
        public SimpleTestingMetadataExplorer(SimpleTestingTestProvider provider, SimpleTestingElementFactory elementFactory)
        {
            _provider = provider;
            _elementFactory = elementFactory;
        }

        #region Implementation of IUnitTestMetadataExplorer

        public void ExploreAssembly(IProject project, IMetadataAssembly assembly, UnitTestElementConsumer consumer)
        {
            var types = GetExportedTypes(assembly.GetTypes()).ToArray();
            
            foreach (var metadataTypeInfo in types)
            {
                ExploreType(project, assembly, consumer, metadataTypeInfo);
            }
        }

        public IUnitTestProvider Provider
        {
            get { return _provider; }
        }

        #endregion

        #region Type Exploration

        private void ExploreType(IProject project, IMetadataAssembly assembly, UnitTestElementConsumer consumer, IMetadataTypeInfo metadataTypeInfo)
        {
            if (!metadataTypeInfo.ContainsSpecifications())
                return;

            var specificationCollectionElement = _elementFactory.GetOrCreateSpecificationContainerElement(project, new ClrTypeName(metadataTypeInfo.FullyQualifiedName), assembly.Location.FullPath);
            consumer(specificationCollectionElement);

            ExploreSpecificationContainer(project, specificationCollectionElement, consumer, metadataTypeInfo);
        }

        private void ExploreSpecificationContainer(IProject project, SpecificationContainerElement container, UnitTestElementConsumer consumer, IMetadataTypeInfo metadataTypeInfo)
        {
            foreach (var field in metadataTypeInfo.GetFields())
            {
                if (!field.IsPublic)
                    continue;

                var x = field.Name;
                var methodElement = _elementFactory.GetOrCreateSpecificationElement(project, container, new ClrTypeName(metadataTypeInfo.FullyQualifiedName), x, string.Empty);
                consumer(methodElement);
            }            
        }

        #endregion

        #region Helpers

        // Thanks for XUnitContrib for this!
        // ReSharper's IMetadataAssembly.GetExportedTypes always seems to return an empty list, so
        // let's roll our own. MSDN says that Assembly.GetExportTypes is looking for "The only types
        // visible outside an assembly are public types and public types nested within other public types."
        // However, this returns items in alphabetical ordering. Assembly.GetExportedTypes returns back in
        // the order in which classes are compiled (so the order in which their files appear in the msbuild file!)
        // with dependencies appearing first. 
        private static IEnumerable<IMetadataTypeInfo> GetExportedTypes(IEnumerable<IMetadataTypeInfo> types)
        {
            foreach (var type in (types ?? Enumerable.Empty<IMetadataTypeInfo>()).Where(x => x.IsPublic || x.IsNestedPublic))
            {
                foreach (var nestedType in GetExportedTypes(type.GetNestedTypes()))
                {
                    yield return nestedType;
                }

                yield return type;
            }
        }

        #endregion
    }
}