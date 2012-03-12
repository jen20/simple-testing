using System.Diagnostics;
using System.Drawing;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework;
using Simple.Testing.Resharper.Elements;
using Simple.Testing.Resharper.Properties;

namespace Simple.Testing.Resharper
{
    [UnitTestProvider]
    public class SimpleTestingTestProvider : IUnitTestProvider
    {
        readonly UnitTestElementComparer _unitTestElementComparer = new UnitTestElementComparer(new[] { typeof(SpecificationContainerElement), typeof(SpecificationElement) });

        public SimpleTestingTestProvider()
        {
            Debug.Listeners.Add(new DefaultTraceListener());
        }

        public string ID
        {
            get { return SimpleTestingTaskRunner.RunnerId; }
        }

        public string Name
        {
            get { return "Simple.Testing"; }
        }

        public Image Icon
        {
            get { return Resources.Logo16; }
        }

        public void ExploreSolution(ISolution solution, UnitTestElementConsumer consumer)
        {
        }

        public void ExploreExternal(UnitTestElementConsumer consumer)
        {
        }
        
        public RemoteTaskRunnerInfo GetTaskRunnerInfo()
        {
            return new RemoteTaskRunnerInfo(typeof(SimpleTestingTaskRunner));
        }

        public int CompareUnitTestElements(IUnitTestElement x, IUnitTestElement y)
        {
            return _unitTestElementComparer.Compare(x, y);
        }

        public bool IsElementOfKind(IUnitTestElement element, UnitTestElementKind elementKind)
        {
            switch (elementKind)
            {
                case UnitTestElementKind.Test:
                    return element is SpecificationElement;
                case UnitTestElementKind.TestContainer:
                    return element is SpecificationContainerElement;
                default:
                    return false;
            }
        }

        public bool IsElementOfKind(IDeclaredElement declaredElement, UnitTestElementKind elementKind)
        {
            //This never seems to get called, and by experiment I see no adverse effect of not implementing it.
            return false;
        }

        public bool IsSupported(IHostProvider hostProvider)
        {
            return true;
        }
    }
}