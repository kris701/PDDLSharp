using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.PDDLSharp.Tests.System
{
    internal static class BenchmarkFetcher
    {
        public static string OutputPath { get {
                Assembly asm = Assembly.GetExecutingAssembly();
                var asmPath = Path.GetDirectoryName(asm.Location);
                if (asmPath == null)
                    throw new Exception("Could not find the assembly path!");
                string path = Path.Combine(asmPath, "benchmarks");
                return path;
            } 
        }
        public static async Task CheckAndDownloadBenchmarksAsync()
        {
            var path = OutputPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = "git",
                        Arguments = $"clone https://github.com/aibasel/downward-benchmarks \"{path}\"",
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                    }
                };
                process.OutputDataReceived += (sender, e) => {
                    Debug.WriteLine(e.Data);
                };
                process.ErrorDataReceived += (sender, e) => {
                    Debug.WriteLine(e.Data);
                };
                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                await process.WaitForExitAsync();
            }
        }
    }
}
