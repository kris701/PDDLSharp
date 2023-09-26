using PDDL.ErrorListeners;
using PDDL.Models;
using PDDL.Models.AST;
using PDDL.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PDDL.Contextualisers
{
    public class PDDLDomainDeclContextualiser : BaseContextualiser<DomainDecl>
    {
        public PDDLDomainDeclContextualiser(IErrorListener listener) : base(listener)
        {
        }

        public override void Contexturalise(DomainDecl decl)
        {
            InsertDefaultPredicates(decl);
            DecorateAllTypesWithInheritence(decl);
            DecorateActionParameters(decl);
            DecorateAxiomVars(decl);
        }

        private void InsertDefaultPredicates(DomainDecl decl)
        {
            if (decl.Predicates != null)
            {
                decl.Predicates.Predicates.Add(
                    new PredicateExp(
                        new ASTNode(), 
                        decl.Predicates, 
                        "=", 
                        new List<NameExp>() { 
                            new NameExp(new ASTNode(), decl.Predicates, "l"),
                            new NameExp(new ASTNode(), decl.Predicates, "r")
                        }));
            }
        }

        private void DecorateAllTypesWithInheritence(DomainDecl decl)
        {
            if (decl.Types != null)
            {
                var allTypes = decl.FindTypes<TypeExp>();
                foreach(var typeDecl in decl.Types.Types)
                {
                    foreach (var type in allTypes)
                    {
                        if (type != typeDecl)
                        {
                            if (typeDecl.Name == type.Name)
                            {
                                type.SuperTypes = typeDecl.SuperTypes;
                            }
                        }
                    }
                }
            }
        }

        private void DecorateActionParameters(DomainDecl decl)
        {
            if (decl.Actions != null)
            {
                foreach(var act in decl.Actions)
                {
                    foreach(var param in act.Parameters.Values)
                    {
                        ReplaceNameExpTypeWith(act.Preconditions, param);
                        ReplaceNameExpTypeWith(act.Effects, param);
                    }
                }
            }
        }

        private void DecorateAxiomVars(DomainDecl decl)
        {
            if (decl.Axioms != null)
            {
                foreach (var axi in decl.Axioms)
                {
                    foreach (var param in axi.Vars.Values)
                    {
                        ReplaceNameExpTypeWith(axi.Context, param);
                        ReplaceNameExpTypeWith(axi.Implies, param);
                    }
                }
            }
        }
    }
}
