using ErrorListeners;
using PDDLModels;
using PDDLModels.Domain;
using PDDLModels.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contextualisers
{
    public class PDDLDeclContextualiser : BaseContextualiser<PDDLDecl>
    {
        public override void Contexturalise(PDDLDecl decl, IErrorListener listener)
        {
            IContextualiser<DomainDecl> domainContextualiser = new PDDLDomainDeclContextualiser();
            domainContextualiser.Contexturalise(decl.Domain, listener);
            IContextualiser<ProblemDecl> problemContextualiser = new PDDLProblemDeclContextualiser();
            problemContextualiser.Contexturalise(decl.Problem, listener);

            DecorateAllTypesWithInheritence(decl.Domain, decl.Problem, listener);
        }

        private void DecorateAllTypesWithInheritence(DomainDecl domain, ProblemDecl problem, IErrorListener listener)
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
