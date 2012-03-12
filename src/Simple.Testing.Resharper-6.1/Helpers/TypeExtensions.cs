using System;
using System.Linq;
using JetBrains.Metadata.Reader.API;

namespace Simple.Testing.Resharper.Helpers
{
    public static class TypeExtensions
    {
        // typeof(Base).IsAssignableFrom(typeof(Derived)) == true
        // typeof(Interface).IsAssignableFrom(typeof(Implementor)) == true
        public static bool IsAssignableFrom(this Type type, IMetadataTypeInfo c)
        {
            if (type.FullName == c.FullyQualifiedName)
                return true;

            if (type.IsInterface)
                return c.Interfaces.Any(i => type.FullName == i.Type.FullyQualifiedName);

            return c.Base != null && type.IsAssignableFrom((Type) c.Base.Type);
        }
    }
}