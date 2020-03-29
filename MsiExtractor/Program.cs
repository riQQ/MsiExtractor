using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Package;
using System;

namespace MsiExtractor
{
	internal class Program
	{
		private static int Main(string[] args)
		{
			Console.WriteLine($"Args: {string.Join(" ", args)}");

			try
			{
				return ExtractMsi(args);
			}
			catch (Exception exception)
			{
				Console.WriteLine($"Exception extracting msi{Environment.NewLine}{exception}");
				return 1;
			}
		}

		private static int ExtractMsi(string[] args)
		{
			const int expectedNumberOfArgs = 2;
			if (args.Length != expectedNumberOfArgs)
			{
				Console.WriteLine($"Expected exactly {expectedNumberOfArgs} argument");
				return 2;
			}

			string packagePath = args[0];
			string fileToExtract = args[1];

			Console.WriteLine($"Starting to extract {fileToExtract} from {packagePath}");

			using (InstallPackage pkg = new InstallPackage(packagePath, DatabaseOpenMode.ReadOnly))
			{
				string workingDirectory = pkg.WorkingDirectory;
				InstallPathMap files = pkg.Files;
				if (!files.TryGetValue(fileToExtract, out InstallPath installPath))
				{
					Console.WriteLine($"Couldn't find {fileToExtract} in msi file");
					return 3;
				}
				// SourcePath is appended to the InstallPackage.WorkingDirectory and this is used as target path for the extraction.
				// Use just the file name to extract directly to working directory.
				installPath.SourcePath = fileToExtract;
				pkg.ExtractFiles(new string[] { fileToExtract });
				Console.WriteLine($"Successfully extracted {fileToExtract} to {workingDirectory}");
			}

			return 0;
		}
	}
}
