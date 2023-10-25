using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.StateSpace
{
    public interface IState
    {
        public HashSet<PredicateExp> State { get; }
        public PDDLDecl Declaration { get; }

        public IState Copy();
        public int Count { get; }

        public void Add(PredicateExp pred);
        public void Add(string pred, params string[] arguments);
        public void Del(PredicateExp pred);
        public void Del(string pred, params string[] arguments);
        public bool Contains(PredicateExp pred);
        public bool Contains(string pred, params string[] arguments);

        public void ExecuteNode(INode node);
        public bool IsNodeTrue(INode node);
        public int IsTrueCount(INode node);

        public bool IsInGoal();
    }
}
