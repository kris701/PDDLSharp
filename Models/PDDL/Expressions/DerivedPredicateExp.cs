using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Domain;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class DerivedPredicateExp : PredicateExp
    {
        private List<DerivedDecl> _derivedDecls;
        public List<DerivedDecl> GetDecls() => _derivedDecls;
        public void AddDecl(DerivedDecl decl) => _derivedDecls.Add(decl);

        public DerivedPredicateExp(string name, List<DerivedDecl> derivedDecls) : base(name)
        {
            _derivedDecls = derivedDecls;
        }

        public DerivedPredicateExp(string name, List<NameExp> arguments, List<DerivedDecl> derivedDecls) : base(name, arguments)
        {
            _derivedDecls = derivedDecls;
        }

        public DerivedPredicateExp(INode? parent, string name, List<DerivedDecl> derivedDecls) : base(parent, name)
        {
            _derivedDecls = derivedDecls;
        }

        public DerivedPredicateExp(INode? parent, string name, List<NameExp> arguments, List<DerivedDecl> derivedDecls) : base(parent, name, arguments)
        {
            _derivedDecls = derivedDecls;
        }

        public DerivedPredicateExp(ASTNode node, INode? parent, string name, List<DerivedDecl> derivedDecls) : base(node, parent, name)
        {
            _derivedDecls = derivedDecls;
        }

        public DerivedPredicateExp(ASTNode node, INode? parent, string name, List<NameExp> arguments, List<DerivedDecl> derivedDecls) : base(node, parent, name, arguments)
        {
            _derivedDecls = derivedDecls;
        }

        public override DerivedPredicateExp Copy(INode? newParent = null)
        {
            var newNode = new DerivedPredicateExp(new ASTNode(Start, End, Line, "", ""), newParent, Name, _derivedDecls);
            foreach (var node in Arguments)
                newNode.Arguments.Add(((dynamic)node).Copy(newNode));
            return newNode;
        }
    }
}
