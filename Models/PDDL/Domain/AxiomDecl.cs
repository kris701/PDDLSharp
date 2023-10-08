﻿using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class AxiomDecl : BaseWalkableNode, IDecl, IParametized
    {
        public ParameterExp Parameters { get; set; }
        public IExp Context { get; set; }
        public IExp Implies { get; set; }

        public AxiomDecl(ASTNode node, INode parent, ParameterExp vars, IExp context, IExp implies) : base(node, parent)
        {
            Parameters = vars;
            Context = context;
            Implies = implies;
        }

        public AxiomDecl(INode parent, ParameterExp vars, IExp context, IExp implies) : base(parent)
        {
            Parameters = vars;
            Context = context;
            Implies = implies;
        }

        public AxiomDecl(ParameterExp vars, IExp context, IExp implies) : base()
        {
            Parameters = vars;
            Context = context;
            Implies = implies;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash *= Parameters.GetHashCode();
            hash *= Context.GetHashCode();
            hash *= Implies.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Parameters;
            yield return Context;
            yield return Implies;
        }
    }
}
