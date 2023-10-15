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

namespace PDDLSharp.PDDLSharp.Tests.System
{
    public class BaseSASBenchmarkedTests : BaseBenchmarkedTests
    {
        public static long MaxSASFileSize = 100000;
        public static long MaxSASsPrDomain = 5;
        public static Dictionary<string, List<string>> _testSASDict = new Dictionary<string, List<string>>();

        public static async Task Setup()
        {
            await BaseBenchmarkedTests.Setup();
            var targetPath = await GitFetcher.CheckAndDownloadBenchmarksAsync("https://github.com/kris701/PDDLBenchmarkPlans", "benchmarks-plans");
            foreach (var domainPath in Directory.GetDirectories(Path.Join(targetPath, "lama-first")))
            {
                if (!ExcludedDomains.Contains(new DirectoryInfo(domainPath).Name))
                {
                    var domainName = new DirectoryInfo(domainPath).Name;
                    if (!_testSASDict.ContainsKey(domainName))
                    {
                        _testSASDict.Add(domainName, new List<string>());
                        foreach (var sas in Directory.GetFiles(domainPath))
                        {
                            if (sas.EndsWith(".sas") && new FileInfo(sas).Length < MaxSASFileSize)
                                _testSASDict[domainName].Add(sas);
                            if (_testSASDict[domainName].Count >= MaxSASsPrDomain)
                                break;
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
            parser.Listener.ThrowIfTypeAbove = ErrorListeners.ParseErrorType.Warning;
            return parser;
        }
    }
}
