using System;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;

namespace Simple.Testing.Resharper.Helpers
{
    public static class PsiExtensions
    {
        public static bool IsAssignableFrom(this Type type, IDeclaredType c)
        {
            if (c == null)
                return false;

            return type.FullName == c.GetClrName().FullName || c.GetAllSuperTypes().Any(superType => type.FullName == superType.GetClrName().FullName);
        }
    }
}