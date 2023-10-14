using PDDLSharp.Analysers;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolit.Tests.System
{
    public class BaseBenchmarkedTests
    {
        public static List<string> ExcludedDomains = new List<string>()
        {
            // It has a malformed domain
            "zenotravel",
            // It have some malformed parameters
            "tpp",
            // Both have a missmatch of ( and )
            "airport-adl",
            "freecell"
        };

        public static long MaxFileSize = 10000;
        public static long MaxProblemsPrDomain = 5;
        public static Dictionary<string, List<string>> _testDict = new Dictionary<string, List<string>>();

        public static async Task Setup()
        {
            var targetPath = await GitFetcher.CheckAndDownloadBenchmarksAsync("https://github.com/aibasel/downward-benchmarks", "benchmarks");
            Random rnd = new Random();
            foreach (var domainPath in Directory.GetDirectories(targetPath))
            {
                if (!ExcludedDomains.Contains(new DirectoryInfo(domainPath).Name))
                {
                    var domainFile = Path.Combine(domainPath, "domain.pddl");
                    if (File.Exists(domainFile) && CompatabilityHelper.IsPDDLDomainSpported(new FileInfo(domainFile)))
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

        public IParser<INode> GetParser(string domain, IErrorListener listener)
        {
            if (!CompatabilityHelper.IsPDDLDomainSpported(domain))
                Assert.Inconclusive("Domain is unsupported");
            IParser<INode> parser = new PDDLParser(listener);
            parser.Listener.ThrowIfTypeAbove = PDDLSharp.ErrorListeners.ParseErrorType.Warning;
            return parser;
        }
    }
}
