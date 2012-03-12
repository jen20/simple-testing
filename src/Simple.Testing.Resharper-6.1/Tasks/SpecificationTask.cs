using System;
using System.Globalization;
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Simple.Testing.Resharper.Tasks
{
    [Serializable]
    public class SpecificationTask : RemoteTask, IEquatable<SpecificationTask>
    {
        private readonly string _assemblyLocation;
        private readonly string _specificationContainerName;
        private readonly string _specificationName;
        private readonly bool _runExplicitly;

        public SpecificationTask(XmlElement element) : base(element)
        {
            _assemblyLocation = GetXmlAttribute(element, XmlAttributeNames.AssemblyLocation);
            _specificationContainerName = GetXmlAttribute(element, XmlAttributeNames.SpecificationContainerName);
            _specificationName = GetXmlAttribute(element, XmlAttributeNames.SpecificationName);
            _runExplicitly = bool.Parse(GetXmlAttribute(element, XmlAttributeNames.RunExplicitly));
        }

        public SpecificationTask(string assemblyLocation, string specificationContainerName, string specificationName, bool runExplicitly) : base(SimpleTestingTaskRunner.RunnerId)
        {
            _assemblyLocation = assemblyLocation;
            _specificationContainerName = specificationContainerName;
            _specificationName = specificationName;
            _runExplicitly = runExplicitly;
        }

        public string AssemblyLocation
        {
            get { return _assemblyLocation; }
        }

        public string SpecificationContainerName
        {
            get { return _specificationContainerName; }
        }

        public string SpecificationName
        {
            get { return _specificationName; }
        }

        #region Overrides of RemoteTask

        public override bool IsMeaningfulTask
        {
            get { return true; }
        }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, XmlAttributeNames.AssemblyLocation, _assemblyLocation);
            SetXmlAttribute(element, XmlAttributeNames.SpecificationContainerName, _specificationContainerName);
            SetXmlAttribute(element, XmlAttributeNames.SpecificationName, _specificationName);
            SetXmlAttribute(element, XmlAttributeNames.RunExplicitly, _runExplicitly.ToString(CultureInfo.InvariantCulture));
        }

        #endregion

        #region Implementation of IEquatable<SpecificationTask>

        public bool Equals(SpecificationTask other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(other._assemblyLocation, _assemblyLocation) && Equals(other._specificationContainerName, _specificationContainerName) && Equals(other._specificationName, _specificationName) && other._runExplicitly.Equals(_runExplicitly);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as SpecificationTask);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result*397) ^ (_assemblyLocation != null ? _assemblyLocation.GetHashCode() : 0);
                result = (result*397) ^ (_specificationContainerName != null ? _specificationContainerName.GetHashCode() : 0);
                result = (result*397) ^ (_specificationName != null ? _specificationName.GetHashCode() : 0);
                result = (result*397) ^ _runExplicitly.GetHashCode();
                return result;
            }
        }

        public static bool operator ==(SpecificationTask left, SpecificationTask right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SpecificationTask left, SpecificationTask right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region Formatting Members

        public override string ToString()
        {
            return string.Format("{0} - {1} - {2}", _assemblyLocation, _specificationContainerName, _specificationName);
        }

        #endregion
    }
}
