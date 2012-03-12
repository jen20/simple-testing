using System;
using System.Text.RegularExpressions;

namespace Simple.Testing.Resharper.Helpers
{
    public static class SpecificationNameConverter
    {
        public static Func<string, string> DisplayNameForSpecificationName = s =>
                                                            {
                                                                var withoutUnderscores = s.Replace("_", " ");
                                                                return new Regex(@"(^[a-z])|\.\s+(.)", RegexOptions.ExplicitCapture).Replace(withoutUnderscores, f => f.Value.ToUpper());
                                                            };

        public static Func<string, string> DisplayNameForSpecificationContainerName = s =>
                                                                         {
                                                                             var separated = Regex.Replace(s, "([A-Z][a-z]+)", " $1", RegexOptions.Compiled).Trim();
                                                                             return separated;
                                                                         };
    }
}