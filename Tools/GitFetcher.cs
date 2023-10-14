using System.Diagnostics;
using System.Reflection;

namespace PDDLSharp.Tools
{
    public static class GitFetcher
    {
        private static string GetFullPath(string target)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            var asmPath = Path.GetDirectoryName(asm.Location);
            if (asmPath == null)
                throw new Exception("Could not find the assembly path!");
            string path = Path.Combine(asmPath, target);
            return path;
        }

        public static async Task<string> CheckAndDownloadBenchmarksAsync(string git, string outPath)
        {
            var path = GetFullPath(outPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = "git",
                        Arguments = $"clone {git} \"{path}\"",
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                    }
                };
                process.OutputDataReceived += (sender, e) =>
                {
                    Debug.WriteLine(e.Data);
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    Debug.WriteLine(e.Data);
                };
                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                await process.WaitForExitAsync();
            }
            return path;
        }
    }
}
