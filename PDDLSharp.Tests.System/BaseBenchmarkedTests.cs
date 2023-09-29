using PDDLSharp.Analysers;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.PDDLSharp.Tests.System
{
    public class BaseBenchmarkedTests
    {
        public static long MaxFileSize = 10000;
        public static long MaxProblemsPrDomain = 5;
        public static Dictionary<string, List<string>> _testDict = new Dictionary<string, List<string>>();

        public static async Task Setup()
        {
            await BenchmarkFetcher.CheckAndDownloadBenchmarksAsync();
            Random rnd = new Random();
            List<string> validDomains = new List<string>();
            foreach (var path in Directory.GetDirectories(BenchmarkFetcher.OutputPath))
            {
                if (path.EndsWith("-strips"))
                    validDomains.Add(path);
            }
            foreach (var domainPath in validDomains)
            {
                var domainFile = Path.Combine(domainPath, "domain.pddl");
                if (File.Exists(domainFile))
                {
                    if (!_testDict.ContainsKey(domainFile))
                    {
                        _testDict.Add(domainFile, new List<string>());
                        foreach (var problem in Directory.GetFiles(domainPath).OrderBy(x => rnd.Next()))
                        {
                            if (problem != domainFile && problem.EndsWith(".pddl") && new FileInfo(problem).Length < MaxFileSize)
                                _testDict[domainFile].Add(problem);
                            if (_testDict[domainFile].Count >= MaxProblemsPrDomain)
                                break;
                        }
                    }
                }
            }
        }

        public IParser GetParser(string domain, IErrorListener listener)
        {
            if (!IsDomainSupported(domain))
                Assert.Inconclusive("Domain is unsupported");
            IParser parser = new PDDLParser(listener);
            parser.Listener.ThrowIfTypeAbove = ErrorListeners.ParseErrorType.Warning;
            return parser;
        }

        public bool IsDomainSupported(string domainFile)
        {
            IAnalyser<string> preanalyser = new GeneralPreAnalyser(new ErrorListener());
            var text = File.ReadAllText(domainFile);
            preanalyser.PreAnalyse(text);
            if (preanalyser.Listener.CountErrorsOfTypeOrAbove(ParseErrorType.Warning) > 0)
                return false;
            return true;
        }
    }
}
