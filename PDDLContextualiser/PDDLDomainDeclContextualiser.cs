using ErrorListeners;
using PDDLModels;
using PDDLModels.AST;
using PDDLModels.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Contextualisers
{
    public class PDDLDomainDeclContextualiser : BaseContextualiser<DomainDecl>
    {
        public override void Contexturalise(DomainDecl decl, IErrorListener listener)
        {
            InsertDefaultPredicates(decl, listener);
            DecorateAllTypesWithInheritence(decl, listener);
            DecorateActionParameters(decl, listener);
            DecorateAxiomVars(decl, listener);
        }

        private void InsertDefaultPredicates(DomainDecl decl, IErrorListener listener)
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

        private void DecorateAllTypesWithInheritence(DomainDecl decl, IErrorListener listener)
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

        private void DecorateActionParameters(DomainDecl decl, IErrorListener listener)
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

        private void DecorateAxiomVars(DomainDecl decl, IErrorListener listener)
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
