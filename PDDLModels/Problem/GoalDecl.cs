using PDDLModels.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tools;

namespace PDDLModels.Problem
{
    public class GoalDecl : BaseWalkableNode, IDecl
    {
        public IExp GoalExp { get; set; }

        // Context
        public int PredicateCount { get; internal set; }
        public List<PredicateExp> TruePredicates { get; internal set; }
        public List<PredicateExp> FalsePredicates { get; internal set; }
        public bool DoesContainOr { get; internal set; }
        public bool DoesContainAnd { get; internal set; }
        public bool DoesContainNot { get; internal set; }
        public bool DoesContainPredicates { get; internal set; }
        public bool DoesContainNames { get; internal set; }

        public GoalDecl(ASTNode node, INode parent, IExp goalExp) : base(node, parent)
        {
            GoalExp = goalExp;
        }

        public override string ToString()
        {
            return $"(:goal {GoalExp})";
        }

        public override HashSet<INamedNode> FindNames(string name)
        {
            return GoalExp.FindNames(name);
        }

        public override HashSet<T> FindTypes<T>()
        {
            HashSet<T> res = new HashSet<T>();
            if (this is T v)
                res.Add(v);
            res.AddRange(GoalExp.FindTypes<T>());
            return res;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + GoalExp.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is GoalDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return GoalExp;
        }
    }
}
