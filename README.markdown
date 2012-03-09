#Simple.Testing with Resharper Support

This is an initial version of Resharper support for Greg Young's Simple.Testing framework.

The Resharper runner currently supports specifications stored as fields. Support will arrive shortly for data-driven specifications and specifications stored as methods.

To use the Resharper plugin, build the solution in Visual Studio in Release configuration, and copy the output of the Simple.Testing.Resharper-6.1/bin/Release directory into your Resharper plugin directory.

When running the project in debug mode, a messagebox appears before the Resharper task runner runs any tests in order that you can attach a debugger to the process.
