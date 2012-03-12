using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;

namespace Simple.Testing.Resharper.Elements
{
    [SolutionComponent]
    public class SimpleTestingElementFactory
    {
        private readonly SimpleTestingTestProvider _provider;
        private readonly IUnitTestElementManager _unitTestManager;
        private readonly CacheManager _cacheManager;
        private readonly PsiModuleManager _psiModuleManager;

        public SimpleTestingElementFactory(SimpleTestingTestProvider provider, IUnitTestElementManager unitTestManager, CacheManager cacheManager, PsiModuleManager psiModuleManager)
        {
            _provider = provider;
            _unitTestManager = unitTestManager;
            _cacheManager = cacheManager;
            _psiModuleManager = psiModuleManager;
        }

        public SpecificationContainerElement GetOrCreateSpecificationContainerElement(IProject project, IClrTypeName typeName, string assemblyLocation)
        {
            var id = "simple:" + typeName.FullName;
            var element = _unitTestManager.GetElementById(project, id);
            if (element != null)
            {
                element.State = UnitTestElementState.Valid;
                return element as SpecificationContainerElement;
            }

            return new SpecificationContainerElement(_provider, new ProjectModelElementEnvoy(project), _cacheManager, _psiModuleManager, id, typeName.GetPersistent(), assemblyLocation);
        }

        public SpecificationElement GetOrCreateSpecificationElement(IProject project, SpecificationContainerElement parent, IClrTypeName typeName, string methodName, string skipReason)
        {
            var id = string.Format("simple:{0}.{1}", parent.TypeName.FullName, methodName);
            var element = _unitTestManager.GetElementById(project, id);
            if (element != null)
            {
                element.State = UnitTestElementState.Valid;
                return element as SpecificationElement;
            }

            return new SpecificationElement(_provider, parent, new ProjectModelElementEnvoy(project), _cacheManager, _psiModuleManager, id, typeName.GetPersistent(), methodName, skipReason);
        }
    }
}