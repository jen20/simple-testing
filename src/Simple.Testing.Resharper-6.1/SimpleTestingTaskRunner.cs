using System;
using System.Linq;
using System.Reflection;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.Util;
using Simple.Testing.Framework;
using Simple.Testing.Resharper.Tasks;

namespace Simple.Testing.Resharper
{
    public class SimpleTestingTaskRunner : RecursiveRemoteTaskRunner
    {
        public const string RunnerId = "SimpleTestingRunner";

        public SimpleTestingTaskRunner(IRemoteTaskServer server) : base(server)
        {
            #if DEBUG
            MessageBox.ShowInfo(string.Format("Started Task Runner - Attach Debugger to {0}", System.Diagnostics.Process.GetCurrentProcess().ProcessName));
            #endif
        }

        #region Overrides of RemoteTaskRunner

        public override TaskResult Start(TaskExecutionNode node)
        {
            var specificationTask = node.RemoteTask as SpecificationTask;
            if (specificationTask == null)
                return TaskResult.Error;

            var assembly = Assembly.LoadFrom(specificationTask.AssemblyLocation);
            if (assembly == null)
            {
                Server.TaskError(specificationTask, string.Format("Could not load context assembly: {0}", specificationTask.AssemblyLocation));
                return TaskResult.Error;
            }

            var specificationContainer = assembly.GetTypes().FirstOrDefault(t => t.FullName == specificationTask.SpecificationContainerName);
            if (specificationContainer == null)
            {
                Server.TaskError(specificationTask, string.Format("Could not load type '{0}' from assembly {1}", specificationTask.SpecificationContainerName, specificationTask.AssemblyLocation));
                return TaskResult.Error;
            }

            var fieldInfo = specificationContainer.GetField(specificationTask.SpecificationName);
            if (fieldInfo == null)
            {
                Server.TaskError(specificationTask, string.Format("Could not find field '{0}' on type {1}", specificationTask.SpecificationName, specificationTask.SpecificationContainerName));
                return TaskResult.Error;
            }

            if (!typeof(Specification).IsAssignableFrom(fieldInfo.FieldType))
            {
                Server.TaskError(specificationTask, string.Format("'{0}' is not a specification", specificationTask.SpecificationName));
                return TaskResult.Error;
            }

            var specification = (Specification)fieldInfo.GetValue(Activator.CreateInstance(specificationContainer));
            var specificationToRun = new SpecificationToRun(specification, fieldInfo);

            var runner = new SpecificationRunner();
            var runResult = runner.RunSpecification(specificationToRun);

            Server.TaskOutput(specificationTask, RunResultFormatter.FormatRunResult(runResult), TaskOutputType.STDOUT);

            return runResult.Passed ? TaskResult.Success : TaskResult.Error;
        }

        public override TaskResult Execute(TaskExecutionNode node)
        {
            return TaskResult.Error;
        }

        public override TaskResult Finish(TaskExecutionNode node)
        {
            return TaskResult.Success;
        }

        public override void ExecuteRecursive(TaskExecutionNode node)
        {
        }

        #endregion
    }
}