using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;

namespace Simple.Testing.Resharper
{
    [SolutionComponent]
    public class UnitTestElementManager
    {
        private readonly IUnitTestElementManager _unitTestElementManager;

        public UnitTestElementManager(IUnitTestElementManager unitTestElementManager)
        {
            _unitTestElementManager = unitTestElementManager;
        }

        public IUnitTestElement GetElementById(IProject project, string id)
        {
            return _unitTestElementManager.GetElementById(project, id);
        }
    }
}