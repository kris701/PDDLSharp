using ErrorListeners;
using Models;
using Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contextualisers
{
    public class PDDLProblemDeclContextualiser : BaseContextualiser<ProblemDecl>
    {
        public override void Contexturalise(ProblemDecl decl, IErrorListener listener)
        {
            SetGoalContext(decl.Goal);
            DecorateObjects(decl);
        }

        private void SetGoalContext(GoalDecl goal)
        {
            if (goal.GoalExp != null)
            {
                goal.PredicateCount = GetPredicateCountInExp(goal.GoalExp);

                List<PredicateExp> truePredicates = new List<PredicateExp>();
                List<PredicateExp> falsePredicates = new List<PredicateExp>();
                GetPredicatesInExp(goal.GoalExp, truePredicates, falsePredicates);
                goal.TruePredicates = truePredicates;
                goal.FalsePredicates = falsePredicates;

                goal.DoesContainAnd = DoesExpContainNodeType<AndExp>(goal.GoalExp);
                goal.DoesContainOr = DoesExpContainNodeType<OrExp>(goal.GoalExp);
                goal.DoesContainNot = DoesExpContainNodeType<NotExp>(goal.GoalExp);
                goal.DoesContainPredicates = DoesExpContainNodeType<PredicateExp>(goal.GoalExp);
                goal.DoesContainNames = DoesExpContainNodeType<NameExp>(goal.GoalExp);
            }
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
