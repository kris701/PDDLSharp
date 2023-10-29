using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Toolkit.Grounders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.StateSpace
{
    public interface IState
    {
        public HashSet<PredicateExp> State { get; set; }
        public PDDLDecl Declaration { get; }
        public IGrounder<IParametized> Grounder { get; }

        public IState Copy();
        public int Count { get; }

        public bool Add(PredicateExp pred);
        public bool Add(string pred, params string[] arguments);
        public bool Del(PredicateExp pred);
        public bool Del(string pred, params string[] arguments);
        public bool Contains(PredicateExp pred);
        public bool Contains(string pred, params string[] arguments);

        public int ExecuteNode(INode node);
        public bool IsNodeTrue(INode node);
        public int IsTrueCount(INode node);

        public bool IsInGoal();
    }
}
