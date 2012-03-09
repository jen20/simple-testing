using System;
using System.Collections.Generic;
using System.Linq;
using Simple.Testing.Framework;

namespace Simple.Testing.Runner
{
    internal class PrintFailuresOutputter
    {
        public void Output(string assembly, IEnumerable<RunResult> results)
        {
            Console.WriteLine("\nRunning all specifications from {0}\n", assembly);
            Console.WriteLine(new string('-', 80));
            var totalCount = 0;
            var totalAsserts = 0;
            var fail = 0;
            var failAsserts = 0;
            foreach (var result in results)
            {
                PrintSpec(result);
                if (!result.Passed)
                {
                    failAsserts += result.Expectations.Count(x => x.Passed == false);
                    fail++;
                }
                totalAsserts += result.Expectations.Count;
                totalCount++;
            }
            Console.WriteLine("\nRan {0} specifications {1} failures. {2} total assertions {3} failures.", totalCount, fail, totalAsserts, failAsserts);
            Console.WriteLine(new string('*', 80));
        }

        private static void PrintSpec(RunResult result)
        {
            var passed = result.Passed ? "PASSED" : "FAILED";
            Console.WriteLine(result.Name + " - " + passed);
            var on = result.GetOnResult();
            if (on != null)
            {
                Console.WriteLine();
                Console.WriteLine("On:");
                Console.WriteLine("\t" + @on);
                Console.WriteLine();
            }
            if (result.Result != null)
            {
                Console.WriteLine();
                Console.WriteLine("Results with:");
                if (result.Result is Exception)
                    Console.WriteLine("\t" + result.Result.GetType() + "\n\t" + ((Exception) result.Result).Message);
                else
                    Console.WriteLine("\t" + result.Result);
                Console.WriteLine();
            }

            Console.WriteLine("Expectations:");
            foreach (var expecation in result.Expectations)
            {
                if (expecation.Passed)
                    Console.WriteLine("\t" + expecation.Text + " - " + (expecation.Passed ? "PASSED" : "FAILED"));
                else
                    Console.WriteLine("\t" + expecation.Exception.Message);
            }
            if (result.Thrown != null)
            {
                Console.WriteLine("Specification failed: " + result.Message);
                Console.WriteLine();
                Console.WriteLine(result.Thrown);
            }
            Console.WriteLine(new string('-', 80));
        }
    }
}