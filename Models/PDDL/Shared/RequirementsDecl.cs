using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Tools;
using System;

namespace PDDLSharp.Models.PDDL.Shared
{
    public class RequirementsDecl : BaseListableNode, IDecl
    {
        public List<NameExp> Requirements { get; set; }

        public RequirementsDecl(ASTNode node, INode? parent, List<NameExp> requirements) : base(node, parent)
        {
            Requirements = requirements;
        }

        public RequirementsDecl(INode? parent, List<NameExp> requirements) : base(parent)
        {
            Requirements = requirements;
        }

        public RequirementsDecl(List<NameExp> requirements) : base()
        {
            Requirements = requirements;
        }

        public RequirementsDecl(ASTNode node, INode? parent) : base(node, parent)
        {
            Requirements = new List<NameExp>();
        }

        public RequirementsDecl(INode? parent) : base(parent)
        {
            Requirements = new List<NameExp>();
        }

        public RequirementsDecl() : base()
        {
            Requirements = new List<NameExp>();
        }

        public override bool Equals(object? obj)
        {
            if (obj is RequirementsDecl other)
            {
                if (!base.Equals(other)) return false;
                if (!EqualityHelper.AreListsEqualUnordered(Requirements, other.Requirements)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var req in Requirements)
                hash *= req.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Requirements.GetEnumerator();
        }

        public override RequirementsDecl Copy(INode? newParent = null)
        {
            var newNode = new RequirementsDecl(new ASTNode(Line, "", ""), newParent);
            foreach (var node in Requirements)
                newNode.Requirements.Add(node.Copy(newNode));
            newNode.IsHidden = IsHidden;
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            for (int i = 0; i < Requirements.Count; i++)
            {
                if (Requirements[i] == node && with is NameExp name)
                    Requirements[i] = name;
            }
        }

        public override void Add(INode node)
        {
            if (node is NameExp exp)
                Requirements.Add(exp);
        }

        public override void Remove(INode node)
        {
            if (node is NameExp exp)
                Requirements.Remove(exp);
        }
    }
}
