using System;

namespace Simple.Testing.Framework
{
    static class DelegateExtensions
    {
        public static object InvokeIfNotNull(this Delegate d)
        {
            return d != null ? d.DynamicInvoke() : null;
        }
    }
}