using System;
using Simple.Testing.Framework;
using Simple.Testing.Resharper.Helpers;

namespace Simple.Testing.Resharper
{
    public static class RunResultFormatter
    {
        public static string FormatRunResult(RunResult result)
        {
            var formatted = string.Empty;

            formatted += SpecificationNameConverter.DisplayNameForSpecificationName(result.Name) + Environment.NewLine;
            var on = result.GetOnResult();
            if (on != null)
            {
                formatted += Environment.NewLine;
                formatted += "On:" + Environment.NewLine;
                formatted += "\t" + @on + Environment.NewLine;
            }
            if (result.Result != null)
            {
                formatted += Environment.NewLine;
                formatted += "Results with:" + Environment.NewLine;
                if (result.Result is Exception)
                    formatted += "\t" + result.Result.GetType() + "\n\t" + ((Exception)result.Result).Message + Environment.NewLine;
                else
                    formatted += "\t" + result.Result + Environment.NewLine;
                formatted += Environment.NewLine;
            }

            formatted += "Expectations:" + Environment.NewLine;
            foreach (var expectation in result.Expectations)
            {
                formatted += "\t" + (expectation.Passed ? "[PASSED] - " : "[FAILED] - ") + expectation.Text + Environment.NewLine;
                if (!expectation.Passed)
                {
                    formatted += "\t" + expectation.Exception.Message + Environment.NewLine;
                }
  
            }
            if (result.Thrown != null)
            {
                formatted += "Specification failed: " + result.Message + Environment.NewLine + Environment.NewLine;
                formatted += result.Thrown + Environment.NewLine;
            }

            return formatted;
        }
    }
}