using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Grounders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.StateSpace.SAS
{
    public class SASStateSpace : IState<Fact, Operator>
    {
        public HashSet<Fact> State { get; set; }
        public HashSet<Fact> Goals { get; }
        public PDDLDecl Declaration { get; }
        public int Count => State.Count;

        public SASStateSpace(PDDLDecl declaration)
        {
            Declaration = declaration;
            State = new HashSet<Fact>();
            if (declaration.Problem.Init != null)
                foreach (var child in declaration.Problem.Init.Predicates)
                    if (child is PredicateExp pred)
                        State.Add(new Fact(pred));
            Goals = new HashSet<Fact>();
            if (declaration.Problem.Goal != null && declaration.Problem.Goal.GoalExp is AndExp and)
            {
                foreach (var child in and.Children)
                    if (child is PredicateExp pred)
                        Goals.Add(new Fact(pred));
            }
            else
                throw new Exception("Error! Goal expression was not and and expression!");
        }

        public SASStateSpace(PDDLDecl declaration, HashSet<Fact> state, HashSet<Fact> goal)
        {
            Declaration = declaration;
            State = state;
            Goals = goal;
        }

        public virtual IState<Fact, Operator> Copy()
        {
            var newState = new Fact[State.Count];
            State.CopyTo(newState);
            return new SASStateSpace(Declaration, newState.ToHashSet(), Goals);
        }

        public bool Add(Fact pred) => State.Add(pred);
        public bool Add(string pred, params string[] arguments) => Add(new Fact(pred, arguments));
        public bool Del(Fact pred) => State.Remove(pred);
        public bool Del(string pred, params string[] arguments) => Del(new Fact(pred, arguments));
        public bool Contains(Fact pred) => State.Contains(pred);
        public bool Contains(string pred, params string[] arguments) => Contains(new Fact(pred, arguments));

        public override bool Equals(object? obj)
        {
            if (obj is IState<Fact, Operator> other)
                foreach (var item in State)
                    if (!other.State.Contains(item))
                        return false;
            return true;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (var item in State)
                hash ^= item.GetHashCode();
            return hash;
        }

        public virtual int ExecuteNode(Operator node)
        {
            int changes = 0;
            foreach (var fact in node.Del)
                if (State.Remove(fact))
                    changes--;
            foreach (var fact in node.Add)
                if (State.Add(fact))
                    changes++;
            return changes;
        }

        public bool IsNodeTrue(Operator node)
        {
            foreach (var fact in node.Pre)
                if (!State.Contains(fact))
                    return false;
            return true;
        }

        public bool IsInGoal()
        {
            foreach (var fact in Goals)
                if (!State.Contains(fact))
                    return false;
            return true;
        }
    }
}
