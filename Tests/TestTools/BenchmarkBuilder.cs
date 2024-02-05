using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.FastDownward.SAS;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.FastDownward.Plans;
using PDDLSharp.Parsers.FastDownward.SAS;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Tools;

namespace TestTools
{
    public abstract class BenchmarkBuilder
    {
        private static long MaxPDDLFileSize = 10000;
        private static long MaxProblemsPrDomain = 5;

        private static long MaxPlanFileSize = 10000;
        private static long MaxPlansPrDomain = 5;

        private static long MaxSASFileSize = 10000;
        private static long MaxSASPrDomain = 5;

        public static Dictionary<string, List<string>> _pddlFiles = new Dictionary<string, List<string>>();
        public static bool _isPDDLSetup = false;
        public static void SetupPDDL()
        {
            var targetPath = "../../../../../../Dependencies/downward-benchmarks";
            if (!Directory.Exists(targetPath))
                throw new DirectoryNotFoundException("Benchmarks not found! Please read the readme in the Dependencies folder!");

            foreach (var domainPath in Directory.GetDirectories(targetPath))
            {
                var domainFile = Path.Combine(domainPath, "domain.pddl");
                if (File.Exists(domainFile) && CompatabilityHelper.IsPDDLDomainSpported(new FileInfo(domainFile)))
                {
                    if (!_pddlFiles.ContainsKey(domainFile))
                    {
                        _pddlFiles.Add(domainFile, new List<string>());
                        foreach (var problem in Directory.GetFiles(domainPath))
                        {
                            if (problem != domainFile && problem.EndsWith(".pddl") && new FileInfo(problem).Length < MaxPDDLFileSize && PDDLFileHelper.IsFileProblem(problem))
                                _pddlFiles[domainFile].Add(problem);
                            if (_pddlFiles[domainFile].Count >= MaxProblemsPrDomain)
                                break;
                        }
                    }
                }
            }
            _isPDDLSetup = true;
        }

        public static Dictionary<string, List<string>> _planFiles = new Dictionary<string, List<string>>();
        public static bool _isPlansSetup = false;
        public static void SetupPlans()
        {
            var targetPath = "../../../../../../Dependencies/PDDLBenchmarkPlans/lama-first";
            if (!Directory.Exists(targetPath))
                throw new DirectoryNotFoundException("Benchmarks not found! Please read the readme in the Dependencies folder!");

            foreach (var domainPath in Directory.GetDirectories(targetPath))
            {
                var domainName = new DirectoryInfo(domainPath).Name;
                if (!_planFiles.ContainsKey(domainName))
                {
                    _planFiles.Add(domainName, new List<string>());
                    foreach (var planFile in Directory.GetFiles(domainPath))
                    {
                        if (planFile.EndsWith(".plan") && new FileInfo(planFile).Length < MaxPlanFileSize)
                            _planFiles[domainName].Add(planFile);
                        if (_planFiles[domainName].Count >= MaxPlansPrDomain)
                            break;
                    }
                }
            }
            _isPlansSetup = true;
        }

        public static Dictionary<string, List<string>> _sasFiles = new Dictionary<string, List<string>>();
        public static bool _isSASSetup = false;
        public static void SetupSAS()
        {
            var targetPath = "../../../../../../Dependencies/PDDLBenchmarkPlans/lama-first";
            if (!Directory.Exists(targetPath))
                throw new DirectoryNotFoundException("Benchmarks not found! Please read the readme in the Dependencies folder!");

            foreach (var domainPath in Directory.GetDirectories(targetPath))
            {
                var domainFile = Path.Combine(domainPath, "domain.pddl");
                if (File.Exists(domainFile) && CompatabilityHelper.IsPDDLDomainSpported(new FileInfo(domainFile)))
                {
                    if (!_sasFiles.ContainsKey(domainFile))
                    {
                        _sasFiles.Add(domainFile, new List<string>());
                        foreach (var problem in Directory.GetFiles(domainPath))
                        {
                            if (problem != domainFile && problem.EndsWith(".sas") && new FileInfo(problem).Length < MaxSASFileSize && PDDLFileHelper.IsFileProblem(problem))
                                _sasFiles[domainFile].Add(problem);
                            if (_sasFiles[domainFile].Count >= MaxSASPrDomain)
                                break;
                        }
                    }
                }
            }
            _isSASSetup = true;
        }

        public static IEnumerable<object[]> GetDomains()
        {
            if (!_isPDDLSetup)
                SetupPDDL();
            foreach (var key in _pddlFiles.Keys)
                yield return new object[] { key };
        }

        public static IEnumerable<object[]> GetProblems()
        {
            if (!_isPDDLSetup)
                SetupPDDL();
            foreach (var key in _pddlFiles.Keys)
                yield return new object[] { _pddlFiles[key] };
        }

        public static IEnumerable<object[]> GetDomainsAndProblems()
        {
            if (!_isPDDLSetup)
                SetupPDDL();
            foreach (var key in _pddlFiles.Keys)
                yield return new object[] { key, _pddlFiles[key] };
        }

        public static IEnumerable<object[]> GetPlans()
        {
            if (!_isPlansSetup)
                SetupPlans();
            foreach (var key in _planFiles.Keys)
                yield return new object[] { _planFiles[key] };
        }

        public static IEnumerable<object[]> GetDomainsAndPlans()
        {
            if (!_isPlansSetup)
                SetupPlans();
            foreach (var key in _planFiles.Keys)
                yield return new object[] { key, _planFiles[key] };
        }

        public static IEnumerable<object[]> GetSASs()
        {
            if (!_isSASSetup)
                SetupSAS();
            foreach (var key in _sasFiles.Keys)
                yield return new object[] { _sasFiles[key] };
        }

        public static IEnumerable<object[]> GetDomainsAndSASs()
        {
            if (!_isSASSetup)
                SetupSAS();
            foreach (var key in _sasFiles.Keys)
                yield return new object[] { key, _sasFiles[key] };
        }

        private static Dictionary<string, PDDLDecl> _declCache = new Dictionary<string, PDDLDecl>();
        public static PDDLDecl GetPDDLDecl(string domain, string problem = "")
        {
            if (_declCache.ContainsKey(domain + problem))
                return _declCache[domain + problem].Copy();

            var listener = new ErrorListener();
            var parser = new PDDLParser(listener);
            parser.Listener.ThrowIfTypeAbove = PDDLSharp.ErrorListeners.ParseErrorType.Warning;

            var newDomain = new DomainDecl();
            if (domain != "")
                newDomain = parser.ParseAs<DomainDecl>(new FileInfo(domain));
            var newProblem = new ProblemDecl();
            if (problem != "")
                newProblem = parser.ParseAs<ProblemDecl>(new FileInfo(problem));

            var decl = new PDDLDecl(newDomain, newProblem);
            _declCache.Add(domain + problem, decl);
            return decl;
        }

        private static Dictionary<string, ActionPlan> _planCache = new Dictionary<string, ActionPlan>();
        public static ActionPlan GetActionPlan(string plan)
        {
            if (_planCache.ContainsKey(plan))
                return _planCache[plan].Copy();

            var listener = new ErrorListener();
            var parser = new FDPlanParser(listener);
            parser.Listener.ThrowIfTypeAbove = PDDLSharp.ErrorListeners.ParseErrorType.Warning;

            var planDecl = parser.Parse(new FileInfo(plan));
            _planCache.Add(plan, planDecl);
            return planDecl;
        }

        private static Dictionary<string, SASDecl> _sasCache = new Dictionary<string, SASDecl>();
        public static SASDecl GetSASDecl(string sasFile)
        {
            if (_sasCache.ContainsKey(sasFile))
                return _sasCache[sasFile].Copy();

            var listener = new ErrorListener();
            var parser = new FDSASParser(listener);
            parser.Listener.ThrowIfTypeAbove = PDDLSharp.ErrorListeners.ParseErrorType.Warning;

            var sasDecl = parser.ParseAs<SASDecl>(new FileInfo(sasFile));
            _sasCache.Add(sasFile, sasDecl);
            return sasDecl;
        }
    }
}
