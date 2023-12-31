﻿using PDDLSharp.Contextualisers.Visitors;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Contextualisers.PDDL
{
    public class PDDLContextualiser : IContextualiser
    {
        public IErrorListener Listener { get; internal set; }

        public PDDLContextualiser(IErrorListener listener)
        {
            Listener = listener;
        }

        public void Contexturalise(PDDLDecl decl)
        {
            var visitors = new ContextualiserVisitors(Listener, decl);
            visitors.Visit(decl.Domain);
            visitors.Visit(decl.Problem);

            if (decl.Domain.Types != null)
            {
                DecorateNodeWithTypeInheritence(decl.Domain.Types, decl.Domain);
                DecorateNodeWithTypeInheritence(decl.Domain.Types, decl.Problem);
            }

            decl.IsContextualised = true;
        }

        private void DecorateNodeWithTypeInheritence(TypesDecl decl, INode node)
        {
            var allTypes = node.FindTypes<TypeExp>();
            foreach (var typeDecl in decl.Types)
            {
                if (typeDecl.SuperTypes.Contains(""))
                    typeDecl.SuperTypes.Remove("");
                foreach (var type in allTypes)
                {
                    if (type != typeDecl)
                    {
                        if (typeDecl.Name == type.Name)
                        {
                            type.SuperTypes = typeDecl.SuperTypes;
                            type.SuperType = typeDecl.SuperType;
                        }
                    }
                }
            }
        }
    }
}
