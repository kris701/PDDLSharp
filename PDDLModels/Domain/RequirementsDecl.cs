using PDDLModels.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace PDDLModels.Domain
{
    public class RequirementsDecl : BaseWalkableNode, IDecl
    {
        public List<NameExp> Requirements {  get; set; }

        public RequirementsDecl(ASTNode node, INode parent, List<NameExp> requirements) : base(node, parent)
        {
            Requirements = requirements;
        }

        public override string ToString()
        {
            var reqStr = "";
            foreach (var requirement in Requirements)
                reqStr += $" {requirement}";
            return $"(:requirements{reqStr})";
        }

        public override HashSet<INamedNode> FindNames(string name)
        {
            HashSet<INamedNode> res = new HashSet<INamedNode>();
            foreach (var requirement in Requirements)
                res.AddRange(requirement.FindNames(name));
            return res;
        }
        public override HashSet<T> FindTypes<T>()
        {
            HashSet<T> res = new HashSet<T>();
            if (this is T v)
                res.Add(v);
            foreach (var req in Requirements)
                res.AddRange(req.FindTypes<T>());
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var req in Requirements)
                hash *= req.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is RequirementsDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Requirements.GetEnumerator();
        }
    }
}
