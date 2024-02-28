using System.Collections.Generic;
using BenchmarkDotNet.Running;
using DotTraceExamplesTaskSolve.Programs;

namespace DotTraceExamplesTaskSolve;

internal class Program
{
	public static void Main(string[] args)
	{
		var dict = new Dictionary<string, string>(1000);
		dict.Add("adb", "adb");
		dict["cdb"] = "cdb";
		dict["cdb"] = "sdb";
		BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
		// ProgramRunner.Run(new ComplexOperationTestProgram());
		// ProgramRunner.Run(new EdgePreservingSmoothingProgram());
		// ProgramRunner.Run(new MeanShiftProgram());
	}
}