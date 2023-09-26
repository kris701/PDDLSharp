using PDDL.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PDDL.Tools;

namespace PDDL.Models.Problem
{
    public class GoalDecl : BaseWalkableNode, IDecl
    {
        public IExp GoalExp { get; set; }

        // Context
        public int PredicateCount { get; set; }
        public List<PredicateExp> TruePredicates { get; set; }
        public List<PredicateExp> FalsePredicates { get; set; }
        public bool DoesContainOr { get; set; }
        public bool DoesContainAnd { get; set; }
        public bool DoesContainNot { get; set; }
        public bool DoesContainPredicates { get; set; }
        public bool DoesContainNames { get; set; }

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
