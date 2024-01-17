using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;
using PDDLSharp.StateSpaces.SAS;
using PDDLSharp.Toolkit.Planners.Search.Classical;
using PDDLSharp.Toolkit.Planners.Tools;
using System.Diagnostics;
using System.Timers;

namespace PDDLSharp.Toolkit.Planners.Search.BlackBox
{
    public abstract class BaseBlackBoxSearch : BaseClassicalSearch
    {
        public BaseBlackBoxSearch(SASDecl decl, IHeuristic heuristic) : base(decl, heuristic)
        {
        }

        public List<Operator> GetApplicables(ISASState state)
        {
            var returnList = new List<Operator>();
            foreach (var op in Declaration.Operators)
            {
                if (Aborted) break;
                if (state.IsNodeTrue(op))
                    returnList.Add(op);
            }
            return returnList;
        }
    }
}
