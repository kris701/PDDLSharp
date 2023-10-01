using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.Domain;
using PDDLSharp.Models.Expressions;
using PDDLSharp.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Contextualisers
{
    public class PDDLDeclContextualiser : BaseContextualiser<PDDLDecl>
    {
        public PDDLDeclContextualiser(IErrorListener listener) : base(listener)
        {
        }

        public override void Contexturalise(PDDLDecl decl)
        {
            IContextualiser<DomainDecl> domainContextualiser = new PDDLDomainDeclContextualiser(Listener);
            domainContextualiser.Contexturalise(decl.Domain);
            IContextualiser<ProblemDecl> problemContextualiser = new PDDLProblemDeclContextualiser(Listener);
            problemContextualiser.Contexturalise(decl.Problem);

            DecorateConstants(decl.Domain, decl.Problem);
            DecorateAllTypesWithInheritence(decl.Domain, decl.Problem);
        }

        private void DecorateConstants(DomainDecl domain, ProblemDecl problem)
        {
            if (domain.Constants != null)
            {
                foreach(var constant in domain.Constants.Constants)
                {
                    var allOfConstant = problem.FindNames(constant.Name);
                    foreach(var instance in allOfConstant)
                    {
                        if (instance is NameExp named)
                        {
                            named.Type = constant.Type;
                        }
                    }
                }
            }
        }

        private void DecorateAllTypesWithInheritence(DomainDecl domain, ProblemDecl problem)
        {
            if (domain.Types != null)
            {
                var allTypes = problem.FindTypes<TypeExp>();
                foreach (var typeDecl in domain.Types.Types)
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
    }
}
