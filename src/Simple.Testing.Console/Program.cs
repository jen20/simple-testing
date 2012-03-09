using System;
using System.Linq;
using Simple.Testing.Framework;

namespace Simple.Testing.Runner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var showHelp = false;
            var assemblies = Enumerable.Empty<string>();

            var optionSet = new Options
                                {
                                    {"h|help", "show this message and exit", x => showHelp = x != null},
                                    {"a=|assemblies=", "comma-seperated list of the names of assemblies to test", x => assemblies = x.Split(',')}
                                };

            try
            {
                optionSet.Parse(args);
                if (showHelp)
                {
                    ShowHelp(optionSet);
                    return;
                }
                if (!assemblies.Any())
                {
                    throw new InvalidOperationException("No assemblies specified.");
                }
            }
            catch (InvalidOperationException exception)
            {
                Console.Write(string.Format("{0}: ", AppDomain.CurrentDomain.FriendlyName));
                Console.WriteLine(exception.Message);
                Console.WriteLine("Try {0} --help for more information", AppDomain.CurrentDomain.FriendlyName);
                return;
            }
            assemblies.ForEach(x => new PrintFailuresOutputter().Output(x, SimpleRunner.RunAllInAssembly(x)));
        }

        private static void ShowHelp(Options optionSet)
        {
            Console.WriteLine("Test specification runner for Simple.Testing");
            Console.WriteLine();
            Console.WriteLine("Options:");
            optionSet.WriteOptionDescriptions(Console.Out);
        }
    }
}