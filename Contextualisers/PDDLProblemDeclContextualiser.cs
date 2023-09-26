using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.Expressions;
using PDDLSharp.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Contextualisers
{
    public class PDDLProblemDeclContextualiser : BaseContextualiser<ProblemDecl>
    {
        public PDDLProblemDeclContextualiser(IErrorListener listener) : base(listener)
        {
        }

        public override void Contexturalise(ProblemDecl decl)
        {
            DecorateObjects(decl);
        }

        private void DecorateObjects(ProblemDecl decl)
        {
            if (decl.Objects != null)
            {
                foreach (var obj in decl.Objects.Objs)
                {
                    if (decl.Init != null)
                    {
                        foreach(var init in decl.Init.Predicates)
                        {
                            if (init is PredicateExp pred)
                            {
                                for (int i = 0; i < pred.Arguments.Count; i++)
                                {
                                    if (pred.Arguments[i].Name == obj.Name)
                                        pred.Arguments[i].Type.Name = obj.Type.Name;
                                }
                            }
                        }
                    }

                    if (decl.Goal != null)
                        ReplaceNameExpTypeWith(decl.Goal.GoalExp, obj);
                }
            }
        }
    }
}
