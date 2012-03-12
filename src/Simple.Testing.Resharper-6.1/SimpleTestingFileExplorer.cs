using System;
using JetBrains.Application;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using Simple.Testing.Resharper.Elements;

namespace Simple.Testing.Resharper
{
    [FileUnitTestExplorer]
    public class SimpleTestingFileExplorer : IUnitTestFileExplorer
    {
        private readonly SimpleTestingTestProvider _provider;
        private readonly SimpleTestingElementFactory _elementFactory;

        public SimpleTestingFileExplorer(SimpleTestingTestProvider provider, SimpleTestingElementFactory elementFactory)
        {
            _provider = provider;
            _elementFactory = elementFactory;
        }

        #region Implementation of IUnitTestFileExplorer

        public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
        {
            if (psiFile == null)
                throw new ArgumentNullException("psiFile");

            var project = psiFile.GetProject();
            if (project == null)
                return;

            psiFile.ProcessDescendants(new SimpleTestingPsiFileExplorer(consumer, psiFile, interrupted, _elementFactory));
        }

        public IUnitTestProvider Provider
        {
            get { return _provider; }
        }

        #endregion
    }
}