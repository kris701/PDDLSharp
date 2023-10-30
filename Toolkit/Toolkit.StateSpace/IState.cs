﻿using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Toolkit.Grounders;

namespace PDDLSharp.Toolkit.StateSpace
{
    public interface IState<F, O>
    {
        public HashSet<F> State { get; set; }
        public PDDLDecl Declaration { get; }

        public IState<F,O> Copy();
        public int Count { get; }

        public bool Add(F pred);
        public bool Add(string pred, params string[] arguments);
        public bool Del(F pred);
        public bool Del(string pred, params string[] arguments);
        public bool Contains(F pred);
        public bool Contains(string pred, params string[] arguments);

        public int ExecuteNode(O node);
        public bool IsNodeTrue(O node);

        public bool IsInGoal();
    }
}
