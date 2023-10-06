﻿using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class AxiomDecl : BaseWalkableNode, IDecl
    {
        public ParameterExp Vars { get; set; }
        public IExp Context { get; set; }
        public IExp Implies { get; set; }

        public AxiomDecl(ASTNode node, INode parent, ParameterExp vars, IExp context, IExp implies) : base(node, parent)
        {
            Vars = vars;
            Context = context;
            Implies = implies;
        }

        public AxiomDecl(INode parent, ParameterExp vars, IExp context, IExp implies) : base(parent)
        {
            Vars = vars;
            Context = context;
            Implies = implies;
        }

        public AxiomDecl(ParameterExp vars, IExp context, IExp implies) : base()
        {
            Vars = vars;
            Context = context;
            Implies = implies;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash *= Vars.GetHashCode();
            hash *= Context.GetHashCode();
            hash *= Implies.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Vars;
            yield return Context;
            yield return Implies;
        }
    }
}
