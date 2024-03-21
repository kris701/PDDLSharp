using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.FastDownward.SAS;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers.FastDownward.Plans;
using PDDLSharp.Parsers.FastDownward.SAS;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Tools;

namespace TestTools
{
    public abstract class BenchmarkBuilder
    {
        public static string DataPath = "../../../../../Dependencies/";

        private static readonly long MaxPDDLFileSize = 10000;
        private static readonly long MaxProblemsPrDomain = 5;

        private static readonly long MaxPlanFileSize = 10000;
        private static readonly long MaxPlansPrDomain = 5;

        private static readonly long MaxSASFileSize = 10000;
        private static readonly long MaxSASPrDomain = 5;

        public static Dictionary<string, List<string>> _pddlFiles = new Dictionary<string, List<string>>();
        public static bool _isPDDLSetup = false;
        public static void SetupPDDL()
        {
            var targetPath = $"{DataPath}/downward-benchmarks";
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
            var targetPath = $"{DataPath}/PDDLBenchmarkPlans/lama-first";
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
            var targetPath = $"{DataPath}/PDDLBenchmarkPlans/lama-first";
            if (!Directory.Exists(targetPath))
                throw new DirectoryNotFoundException("Benchmarks not found! Please read the readme in the Dependencies folder!");

            foreach (var domainPath in Directory.GetDirectories(targetPath))
            {
                var domainName = new DirectoryInfo(domainPath).Name;
                if (!_sasFiles.ContainsKey(domainName))
                {
                    _sasFiles.Add(domainName, new List<string>());
                    foreach (var sasFile in Directory.GetFiles(domainPath))
                    {
                        if (sasFile.EndsWith(".sas") && new FileInfo(sasFile).Length < MaxSASFileSize)
                            _sasFiles[domainName].Add(sasFile);
                        if (_sasFiles[domainName].Count >= MaxSASPrDomain)
                            break;
                    }
                }
            }
            _isSASSetup = true;
        }

        public static IEnumerable<object[]> GetDomains()
        {
            if (!_isPDDLSetup)
                SetupPDDL();
            foreach (var domainFile in _pddlFiles.Keys)
                yield return new object[] { domainFile };
        }

        public static IEnumerable<object[]> GetProblems()
        {
            if (!_isPDDLSetup)
                SetupPDDL();
            foreach (var domainFile in _pddlFiles.Keys)
                foreach (var problemFile in _pddlFiles[domainFile])
                    yield return new object[] { problemFile };
        }

        public static IEnumerable<object[]> GetDomainsAndProblems()
        {
            if (!_isPDDLSetup)
                SetupPDDL();
            foreach (var doaminFile in _pddlFiles.Keys)
                foreach (var problemFile in _pddlFiles[doaminFile])
                    yield return new object[] { doaminFile, problemFile };
        }

        public static IEnumerable<object[]> GetPlans()
        {
            if (!_isPlansSetup)
                SetupPlans();
            foreach (var domainName in _planFiles.Keys)
                foreach (var planFile in _planFiles[domainName])
                    yield return new object[] { planFile };
        }

        public static IEnumerable<object[]> GetDomainsAndPlans()
        {
            if (!_isPlansSetup)
                SetupPlans();
            foreach (var domainName in _planFiles.Keys)
                foreach (var planFile in _planFiles[domainName])
                    yield return new object[] { domainName, planFile };
        }

        public static IEnumerable<object[]> GetSASs()
        {
            if (!_isSASSetup)
                SetupSAS();
            foreach (var domainName in _sasFiles.Keys)
                foreach (var sasFile in _sasFiles[domainName])
                    yield return new object[] { sasFile };
        }

        public static IEnumerable<object[]> GetDomainsAndSASs()
        {
            if (!_isSASSetup)
                SetupSAS();
            foreach (var domainName in _sasFiles.Keys)
                foreach (var sasFile in _sasFiles[domainName])
                    yield return new object[] { domainName, sasFile };
        }

        private static readonly Dictionary<string, PDDLDecl> _declCache = new Dictionary<string, PDDLDecl>();
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

        private static readonly Dictionary<string, ActionPlan> _planCache = new Dictionary<string, ActionPlan>();
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

        private static readonly Dictionary<string, SASDecl> _sasCache = new Dictionary<string, SASDecl>();
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
