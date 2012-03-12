using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;
using Simple.Testing.Resharper.Helpers;
using Simple.Testing.Resharper.Tasks;

namespace Simple.Testing.Resharper.Elements
{
    public class SpecificationContainerElement : IUnitTestElement, ISerializableUnitTestElement, IEquatable<SpecificationContainerElement>
    {
        private readonly string _assemblyLocation;
        private readonly CacheManager _cacheManager;
        private readonly ProjectModelElementEnvoy _projectModelElementEnvoy;
        private readonly PsiModuleManager _psiModuleManager;
        private readonly IClrTypeName _typeName;

        public Guid TempId { get; set; }

        public SpecificationContainerElement(SimpleTestingTestProvider provider, ProjectModelElementEnvoy projectModelElementEnvoy,
                                              CacheManager cacheManager, PsiModuleManager psiModuleManager, string id, IClrTypeName typeName, string assemblyLocation)
        {
            Provider = provider;
            _projectModelElementEnvoy = projectModelElementEnvoy;
            _cacheManager = cacheManager;
            _psiModuleManager = psiModuleManager;
            Id = id;
            _typeName = typeName;
            _assemblyLocation = assemblyLocation;

            State = UnitTestElementState.Valid;

            TempId = Guid.NewGuid();

            Children = new List<IUnitTestElement>();
        }

        public void AddChild(SpecificationElement element)
        {
            Children.Add(element);
        }

        public void RemoveChild(SpecificationElement element)
        {
            Children.Remove(element);
        }

        public string AssemblyLocation
        {
            get { return _assemblyLocation; }
        }

        public IClrTypeName TypeName
        {
            get { return _typeName; }
        }

        #region Implementation of IEquatable<IUnitTestElement>

        public bool Equals(IUnitTestElement other)
        {
            return Equals(other as SpecificationContainerElement);
        }

        #endregion

        #region Implementation of IUnitTestElement

        public IProject GetProject()
        {
            return _projectModelElementEnvoy.GetValidProjectElement() as IProject;
        }

        public string GetPresentation()
        {
            return SpecificationNameConverter.DisplayNameForSpecificationContainerName(ShortName);
        }

        public UnitTestNamespace GetNamespace()
        {
            return new UnitTestNamespace(_typeName.GetNamespaceName());
        }

        public UnitTestElementDisposition GetDisposition()
        {
            var element = GetDeclaredElement();
            if (element == null || !element.IsValid())
                return UnitTestElementDisposition.InvalidDisposition;

            var locations = from declaration in element.GetDeclarations()
                            let file = declaration.GetContainingFile()
                            where file != null
                            select new UnitTestElementLocation(file.GetSourceFile().ToProjectFile(),
                                                            declaration.GetNameDocumentRange().TextRange,
                                                            declaration.GetDocumentRange().TextRange);

            return new UnitTestElementDisposition(locations, this);
        }

        public IDeclaredElement GetDeclaredElement()
        {
            var project = GetProject();
            if (project == null)
                return null;

            var psiModule = _psiModuleManager.GetPrimaryPsiModule(project);
            if (psiModule == null)
                return null;

            return _cacheManager.GetDeclarationsCache(psiModule, false, true).GetTypeElementByCLRName(_typeName);
        }

        public IEnumerable<IProjectFile> GetProjectFiles()
        {
            var declaredElement = GetDeclaredElement();
            if (declaredElement == null)
                return EmptyArray<IProjectFile>.Instance;

            return from sourceFile in declaredElement.GetSourceFiles()
                   select sourceFile.ToProjectFile();
        }

        public IList<UnitTestTask> GetTaskSequence(IList<IUnitTestElement> explicitElements)
        {
            return new List<UnitTestTask>();
        }

        public string Kind
        {
            get { return "Simple.Testing Specification Collection"; }
        }

        public IEnumerable<UnitTestElementCategory> Categories
        {
            get { return UnitTestElementCategory.Uncategorized; }
        }

        public string ExplicitReason
        {
            get { return string.Empty; }
        }

        public string Id { get; private set; }

        public IUnitTestProvider Provider { get; private set; }

        public IUnitTestElement Parent { get; set; }

        public ICollection<IUnitTestElement> Children { get; private set; }

        public string ShortName
        {
            get { return _typeName.ShortName; }
        }

        public bool Explicit
        {
            get { return false; }
        }

        public UnitTestElementState State { get; set; }

        #endregion

        #region IEquatable<SpecificationContainerElement> Members

        public bool Equals(SpecificationContainerElement other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Id, Id) && Equals(other._typeName, _typeName) && Equals(other._assemblyLocation, _assemblyLocation);
        }

        #endregion

        #region Equality Members

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (SpecificationContainerElement)) return false;
            return Equals((SpecificationContainerElement) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Id != null ? Id.GetHashCode() : 0);
                result = (result*397) ^ (_typeName != null ? _typeName.GetHashCode() : 0);
                result = (result*397) ^ (_assemblyLocation != null ? _assemblyLocation.GetHashCode() : 0);
                return result;
            }
        }

        public static bool operator ==(SpecificationContainerElement left, SpecificationContainerElement right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SpecificationContainerElement left, SpecificationContainerElement right)
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

            element.SetAttribute("typeName", _typeName.FullName);
        }

        internal static IUnitTestElement ReadFromXml(XmlElement parent, IUnitTestElement parentElement, ISolution solution, SimpleTestingElementFactory elementFactory)
        {
            var projectId = parent.GetAttribute("projectId");
            var typeName = parent.GetAttribute("typeName");

            var project = (IProject)ProjectUtil.FindProjectElementByPersistentID(solution, projectId);
            if (project == null)
                return null;

            var assemblyLocation = UnitTestManager.GetOutputAssemblyPath(project).FullPath;

            return elementFactory.GetOrCreateSpecificationContainerElement(project, new ClrTypeName(typeName), assemblyLocation);
        }

        #endregion
    }
}