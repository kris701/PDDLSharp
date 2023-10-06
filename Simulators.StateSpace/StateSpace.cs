﻿using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.Plans;

namespace PDDLSharp.Simulators.StateSpace
{
    public class StateSpace
    {
        public PDDLDecl Declaration { get; internal set; }
        private HashSet<GroundedPredicate> _state;

        public StateSpace(PDDLDecl declaration)
        {
            Declaration = declaration;
            _state = new HashSet<GroundedPredicate>();
        }

        public StateSpace(PDDLDecl declaration, InitDecl inits)
        {
            Declaration = declaration;
            _state = new HashSet<GroundedPredicate>();
            foreach (var item in inits.Predicates)
                if (item is PredicateExp predicate)
                    _state.Add(new GroundedPredicate(predicate));
        }

        public int Count => _state.Count;

        public void Add(GroundedPredicate pred) => _state.Add(pred);
        public void Del(GroundedPredicate pred) => _state.Remove(pred);

        public bool Contains(GroundedPredicate op) => _state.Contains(op);
        public bool Contains(string op, params string[] arguments) => Contains(new GroundedPredicate(op, NameExpBuilder.GetNameExpFromString(arguments, Declaration)));
    }
}
