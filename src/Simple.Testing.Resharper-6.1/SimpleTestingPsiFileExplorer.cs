using System.Collections.Generic;
using System.Linq;
using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using Simple.Testing.Framework;
using Simple.Testing.Resharper.Elements;
using Simple.Testing.Resharper.Helpers;

namespace Simple.Testing.Resharper
{
    public class SimpleTestingPsiFileExplorer : IRecursiveElementProcessor
    {
        private readonly UnitTestElementLocationConsumer _consumer;
        private readonly IFile _psiFile;
        private readonly CheckForInterrupt _interrupted;
        private readonly SimpleTestingElementFactory _elementFactory;

        private readonly Dictionary<ITypeElement, SpecificationContainerElement> _specificationCollections;

        public SimpleTestingPsiFileExplorer(UnitTestElementLocationConsumer consumer, IFile psiFile, CheckForInterrupt interrupted, SimpleTestingElementFactory elementFactory)
        {
            _consumer = consumer;
            _psiFile = psiFile;
            _interrupted = interrupted;
            _elementFactory = elementFactory;

             _specificationCollections = new Dictionary<ITypeElement, SpecificationContainerElement>();
        }

        #region Implementation of IRecursiveElementProcessor

        public bool InteriorShouldBeProcessed(ITreeNode element)
        {
            if (element is ITypeMemberDeclaration)
                return (element is ITypeDeclaration);

            return true;
        }

        public void ProcessBeforeInterior(ITreeNode element)
        {
            var declaration = element as IDeclaration;
            if (declaration == null)
                return;

            var declaredElement = declaration.DeclaredElement;

            IUnitTestElement unitTestElement = null;

            var specificationContainer = declaredElement as IClass;
            if (specificationContainer != null)
                unitTestElement = ProcessSpecificationContainer(specificationContainer);

            var specification = declaredElement as IField;
            if (specification != null)
                unitTestElement = ProcessSpecification(specification) ?? unitTestElement;

            if (unitTestElement == null)
                return;
            
            var nameRange = declaration.GetNameDocumentRange().TextRange;
            var documentRange = declaration.GetDocumentRange();

            if (!nameRange.IsValid || !documentRange.IsValid()) 
                return;
            
            var disposition = new UnitTestElementDisposition(unitTestElement, _psiFile.GetSourceFile().ToProjectFile(),
                                                             nameRange, documentRange.TextRange, unitTestElement.Children.ToList());
            _consumer(disposition);
        }

        public void ProcessAfterInterior(ITreeNode element)
        {
        }

        public bool ProcessingIsFinished
        {
            get
            {
                if (_interrupted())
                    throw new ProcessCancelledException();

                return false;
            }
        }

        #endregion

        private IUnitTestElement ProcessSpecificationContainer(IClass specificationContainer)
        {
            if (!IsSpecificationContainer(specificationContainer))
                return null;

            var project = _psiFile.GetProject();
            var containerElement = _elementFactory.GetOrCreateSpecificationContainerElement(project, specificationContainer.GetClrName(), UnitTestManager.GetOutputAssemblyPath(project).FullPath);

            _specificationCollections.Add(specificationContainer, containerElement);

            return containerElement;
        }

        private IUnitTestElement ProcessSpecification(IField specification)
        {
            var containingClass = specification.GetContainingType() as IClass;
            if (containingClass == null || !IsSpecificationContainer(containingClass))
                return null;

            if (!typeof(Specification).IsAssignableFrom(specification.Type as IDeclaredType))
                return null;

            var project = _psiFile.GetProject();
            var containerElement = _specificationCollections[containingClass];

            return _elementFactory.GetOrCreateSpecificationElement(project, containerElement, containingClass.GetClrName(), specification.ShortName, string.Empty);
        }

        private static bool IsSpecificationContainer(IClass potentialSpecificationContainer)
        {
            if (potentialSpecificationContainer == null)
                return false;

            if (potentialSpecificationContainer.GetAccessRights() != AccessRights.PUBLIC)
                return false;

            var fields = potentialSpecificationContainer.GetMembers().OfType<IField>();

            return fields.Any(f => typeof(Specification).IsAssignableFrom(f.Type as IDeclaredType));
        }
    }
}