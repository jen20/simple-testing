using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;
using Simple.Testing.Resharper.Helpers;
using Simple.Testing.Resharper.Tasks;

namespace Simple.Testing.Resharper.Elements
{
    public class SpecificationElement : IUnitTestElement, ISerializableUnitTestElement, IEquatable<SpecificationElement>
    {
        private readonly CacheManager _cacheManager;
        private readonly string _specificationName;
        private readonly ProjectModelElementEnvoy _projectModelElementEnvoy;
        private readonly PsiModuleManager _psiModuleManager;
        private readonly IClrTypeName _typeName;
        private SpecificationContainerElement _parent;

        public Guid TempId { get; set; }

        public SpecificationElement(IUnitTestProvider provider, SpecificationContainerElement testClass, ProjectModelElementEnvoy projectModelElementEnvoy, CacheManager cacheManager,
                                    PsiModuleManager psiModuleManager, string id, IClrTypeName typeName, string specificationName, string skipReason)
        {
            Provider = provider;
            Parent = testClass;
            _projectModelElementEnvoy = projectModelElementEnvoy;
            _cacheManager = cacheManager;
            _psiModuleManager = psiModuleManager;

            Id = id;
            _typeName = typeName;
            _specificationName = specificationName;

            ExplicitReason = skipReason;

            TempId = Guid.NewGuid();

            State = UnitTestElementState.Valid;
        }

        #region Implementation of IEquatable<IUnitTestElement>

        public bool Equals(IUnitTestElement other)
        {
            return Equals(other as SpecificationElement);
        }

        #endregion

        #region Implementation of IUnitTestElement

        public IProject GetProject()
        {
            return _projectModelElementEnvoy.GetValidProjectElement() as IProject;
        }

        public string GetPresentation()
        {
            return SpecificationNameConverter.DisplayNameForSpecificationName(_specificationName);
        }

        public UnitTestNamespace GetNamespace()
        {
            return new UnitTestNamespace(_typeName.GetNamespaceName());
        }

        public UnitTestElementDisposition GetDisposition()
        {
            IDeclaredElement element = GetDeclaredElement();
            if (element == null || !element.IsValid())
                return UnitTestElementDisposition.InvalidDisposition;

            IEnumerable<UnitTestElementLocation> locations = from declaration in element.GetDeclarations()
                                                             let file = declaration.GetContainingFile()
                                                             where file != null
                                                             select new UnitTestElementLocation(file.GetSourceFile().ToProjectFile(),
                                                                                                declaration.GetNameDocumentRange().TextRange,
                                                                                                declaration.GetDocumentRange().TextRange);
            return new UnitTestElementDisposition(locations, this);
        }

        public IDeclaredElement GetDeclaredElement()
        {
            var declaredType = GetDeclaredType();
            return declaredType == null ? null : declaredType.EnumerateMembers(_specificationName, false).FirstOrDefault(member => member as IField != null);
        }

        public IEnumerable<IProjectFile> GetProjectFiles()
        {
            ITypeElement declaredType = GetDeclaredType();
            if (declaredType != null)
            {
                List<IProjectFile> result = (from sourceFile in declaredType.GetSourceFiles()
                                             select sourceFile.ToProjectFile()).ToList<IProjectFile>();
                if (result.Count == 1)
                    return result;
            }

            IDeclaredElement declaredElement = GetDeclaredElement();
            if (declaredElement == null)
                return EmptyArray<IProjectFile>.Instance;

            return from sourceFile in declaredElement.GetSourceFiles()
                   select sourceFile.ToProjectFile();
        }

        public IList<UnitTestTask> GetTaskSequence(IList<IUnitTestElement> explicitElements)
        {
            return new List<UnitTestTask> { new UnitTestTask(this, new SpecificationTask(_parent.AssemblyLocation, _typeName.FullName, _specificationName, false)) };
        }

        public string Kind
        {
            get { return "Simple.Testing Specification"; }
        }

        public IEnumerable<UnitTestElementCategory> Categories
        {
            get { return UnitTestElementCategory.Uncategorized; }
        }

        public string ExplicitReason { get; private set; }

        public string Id { get; private set; }

        public IUnitTestProvider Provider { get; private set; }

        public IUnitTestElement Parent
        {
            get { return _parent; }
            set
            {
                if (ReferenceEquals(_parent, value))
                    return;

                if (_parent != null)
                    _parent.RemoveChild(this);
                _parent = (SpecificationContainerElement) value;
                if (_parent != null)
                    _parent.AddChild(this);
            }
        }

        public ICollection<IUnitTestElement> Children
        {
            get { return EmptyArray<IUnitTestElement>.Instance; }
        }

        public string ShortName
        {
            get { return _specificationName; }
        }

        public bool Explicit
        {
            get { return !string.IsNullOrEmpty(ExplicitReason); }
        }

        public UnitTestElementState State { get; set; }

        private ITypeElement GetDeclaredType()
        {
            IProject p = GetProject();
            if (p == null)
                return null;

            IPsiModule psiModule = _psiModuleManager.GetPrimaryPsiModule(p);
            if (psiModule == null)
                return null;

            return _cacheManager.GetDeclarationsCache(psiModule, true, true).GetTypeElementByCLRName(_typeName);
        }

        #endregion

        #region Equality Members

        public bool Equals(SpecificationElement other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Id, Id) && Equals(other._typeName.FullName, _typeName.FullName) && Equals(other._specificationName, _specificationName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (SpecificationElement)) return false;
            return Equals((SpecificationElement) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Id != null ? Id.GetHashCode() : 0);
                result = (result*397) ^ (_typeName != null ? _typeName.GetHashCode() : 0);
                result = (result*397) ^ (_specificationName != null ? _specificationName.GetHashCode() : 0);
                return result;
            }
        }

        public static bool operator ==(SpecificationElement left, SpecificationElement right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SpecificationElement left, SpecificationElement right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region XML Serialization
        
        public void WriteToXml(XmlElement element)
        {
            var project = GetProject();
            if (project != null)
                element.SetAttribute("projectId", project.GetPersistentID());
            
            element.SetAttribute("collectionName", _typeName.FullName);
            element.SetAttribute("specificationName", _specificationName);
        }

        internal static IUnitTestElement ReadFromXml(XmlElement parent, IUnitTestElement parentElement, ISolution solution, SimpleTestingElementFactory unitTestElementFactory)
        {
            var testClass = parentElement as SpecificationContainerElement;
            if (testClass == null)
                throw new InvalidOperationException("parentElement should be Specification Container Element");

            var projectId = parent.GetAttribute("projectId");
            var collectionName = parent.GetAttribute("collectionName");
            var specificationName = parent.GetAttribute("specificationName");
            

            var project = (IProject)ProjectUtil.FindProjectElementByPersistentID(solution, projectId);
            if (project == null)
                return null;

            return unitTestElementFactory.GetOrCreateSpecificationElement(project, testClass, new ClrTypeName(collectionName), specificationName, string.Empty);
        }

        #endregion
    }
}