using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Domain;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class DerivedPredicateExp : PredicateExp
    {
        public List<DerivedDecl> DerivedDecls { get; set; }

        public DerivedPredicateExp(string name, List<DerivedDecl> derivedDecls) : base(name)
        {
            DerivedDecls = derivedDecls;
        }

        public DerivedPredicateExp(string name, List<NameExp> arguments, List<DerivedDecl> derivedDecls) : base(name, arguments)
        {
            DerivedDecls = derivedDecls;
        }

        public DerivedPredicateExp(INode? parent, string name, List<DerivedDecl> derivedDecls) : base(parent, name)
        {
            DerivedDecls = derivedDecls;
        }

        public DerivedPredicateExp(INode? parent, string name, List<NameExp> arguments, List<DerivedDecl> derivedDecls) : base(parent, name, arguments)
        {
            DerivedDecls = derivedDecls;
        }

        public DerivedPredicateExp(ASTNode node, INode? parent, string name, List<DerivedDecl> derivedDecls) : base(node, parent, name)
        {
            DerivedDecls = derivedDecls;
        }

        public DerivedPredicateExp(ASTNode node, INode? parent, string name, List<NameExp> arguments, List<DerivedDecl> derivedDecls) : base(node, parent, name, arguments)
        {
            DerivedDecls = derivedDecls;
        }

        public override DerivedPredicateExp Copy(INode? newParent = null)
        {
            var newNode = new DerivedPredicateExp(new ASTNode(Start, End, Line, "", ""), newParent, Name, DerivedDecls);
            foreach (var node in Arguments)
                newNode.Arguments.Add(((dynamic)node).Copy(newNode));
            return newNode;
        }
    }
}
