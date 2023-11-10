using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Translators.Tools
{
    public class OrDeconstructor
    {
        public bool Aborted { get; set; } = false;
        public List<ActionDecl> DeconstructOrs(ActionDecl action)
        {
            var deconstructed = new List<ActionDecl>();
            if (action.FindTypes<OrExp>().Count == 0)
                return new List<ActionDecl>() { action };
            deconstructed.AddRange(GeneratePossibleActions(action.Copy()));
            return deconstructed;
        }

        private List<ActionDecl> GeneratePossibleActions(ActionDecl source)
        {
            source.Preconditions = EnsureAnd(source.Preconditions);
            source.Effects = EnsureAnd(source.Effects);
            return DeconstructNodeRec(source);
        }

        private List<ActionDecl> DeconstructNodeRec(ActionDecl act)
        {
            if (Aborted)
                return new List<ActionDecl>();
            var ors = act.Preconditions.FindTypes<OrExp>();
            if (ors.Count <= 0)
                return new List<ActionDecl> { act };

            var retList = new List<ActionDecl>();
            foreach(var opt in ors[0].Options)
            {
                var copy = act.Copy();
                var or = copy.Preconditions.FindTypes<OrExp>();
                if (or[0].Parent is IWalkable walk)
                    walk.Replace(or[0], opt);

                if (or.Count == 1)
                    retList.Add(copy);
                else
                    retList.AddRange(DeconstructNodeRec(copy));
            }
            return retList;
        }

        private IExp EnsureAnd(IExp exp)
        {
            if (exp is AndExp)
                return exp;
            return new AndExp(new List<IExp>() { exp });
        }
    }
}
