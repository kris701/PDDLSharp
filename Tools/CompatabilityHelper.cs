using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Tools
{
    public static class CompatabilityHelper
    {
        public static List<string> UnsupportedPackages = new List<string>()
        {
            ":action-expansions",
            ":foreach-expansions",
            ":dag-expansions",
            ":subgoals-through-axioms",
            ":safety-constraints",
            ":expression-evaluation",
            ":open-world",
            ":true-negation",
            ":ucpop",
            // The 'either' expression is not supported
            "(either "
        };
        public static bool IsPDDLDomainSpported(FileInfo file) => IsPDDLDomainSpported(File.ReadAllText(file.FullName));
        public static bool IsPDDLDomainSpported(string text)
        {
            foreach (var unsuportedPackage in UnsupportedPackages)
                if (text.Contains(unsuportedPackage))
                    return false;
            return true;
        }
    }
}
