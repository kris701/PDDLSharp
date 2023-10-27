using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Tools
{
    public class Layer
    {
        public HashSet<ActionDecl> Actions { get; set; }
        public HashSet<PredicateExp> Propositions { get; set; }

        public Layer(HashSet<ActionDecl> actions, HashSet<PredicateExp> propositions)
        {
            Actions = actions;
            Propositions = propositions;
        }

        public Layer()
        {
            Actions = new HashSet<ActionDecl>();
            Propositions = new HashSet<PredicateExp>();
        }
    }
}
