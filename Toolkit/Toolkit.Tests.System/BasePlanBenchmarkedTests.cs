using PDDLSharp.Analysers;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Parsers;
using PDDLSharp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolit.Tests.System
{
    public class BasePlanBenchmarkedTests : BaseBenchmarkedTests
    {
        public static Dictionary<string, List<string>> _testPlanDict = new Dictionary<string, List<string>>();

        public static async Task Setup()
        {
            await BaseBenchmarkedTests.Setup();
            var targetPath = await GitFetcher.CheckAndDownloadBenchmarksAsync("https://github.com/kris701/PDDLBenchmarkPlans", "benchmarks-plans");
            foreach (var domainPath in Directory.GetDirectories(Path.Join(targetPath, "lama-first")))
            {
                if (!ExcludedDomains.Contains(new DirectoryInfo(domainPath).Name))
                {
                    var domainName = new DirectoryInfo(domainPath).Name;
                    if (!_testPlanDict.ContainsKey(domainName))
                    {
                        _testPlanDict.Add(domainName, new List<string>());
                        foreach (var plan in Directory.GetFiles(domainPath))
                        {
                            if (plan.EndsWith(".plan"))
                                _testPlanDict[domainName].Add(plan);
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
