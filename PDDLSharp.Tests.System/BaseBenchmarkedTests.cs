using PDDLSharp.Analysers;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Parsers;
using PDDLSharp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.PDDLSharp.Tests.System
{
    public class BaseBenchmarkedTests
    {
        public static List<string> ExcludedDomains = new List<string>()
        {
            // It has a malformed domain
            "zenotravel",
            // It have some malformed parameters
            "tpp"
        };

        public static long MaxFileSize = 10000;
        public static long MaxProblemsPrDomain = 5;
        public static Dictionary<string, List<string>> _testDict = new Dictionary<string, List<string>>();

        public static async Task Setup()
        {
            await BenchmarkFetcher.CheckAndDownloadBenchmarksAsync();
            Random rnd = new Random();
            foreach (var domainPath in Directory.GetDirectories(BenchmarkFetcher.OutputPath))
            {
                if (!ExcludedDomains.Contains(new DirectoryInfo(domainPath).Name))
                {
                    var domainFile = Path.Combine(domainPath, "domain.pddl");
                    if (File.Exists(domainFile) && IsDomainSupported(domainFile))
                    {
                        if (!_testDict.ContainsKey(domainFile))
                        {
                            _testDict.Add(domainFile, new List<string>());
                            foreach (var problem in Directory.GetFiles(domainPath).OrderBy(x => rnd.Next()))
                            {
                                if (problem != domainFile && problem.EndsWith(".pddl") && new FileInfo(problem).Length < MaxFileSize && PDDLFileHelper.IsFileProblem(problem))
                                    _testDict[domainFile].Add(problem);
                                if (_testDict[domainFile].Count >= MaxProblemsPrDomain)
                                    break;
                            }
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

        public static bool IsDomainSupported(string domainFile)
        {
            IAnalyser<string> preanalyser = new GeneralPreAnalyser(new ErrorListener(ParseErrorType.Error));
            var text = File.ReadAllText(domainFile);
            preanalyser.PreAnalyse(text);
            if (preanalyser.Listener.CountErrorsOfTypeOrAbove(ParseErrorType.Warning) > 0)
                return false;
            return true;
        }
    }
}
