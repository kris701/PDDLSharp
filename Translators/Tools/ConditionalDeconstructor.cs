using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.PDDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Translators.Tools
{
    public class ConditionalDeconstructor
    {
        public List<ActionDecl> DecontructConditionals(ActionDecl action)
        {
            List<ActionDecl> newActions = new List<ActionDecl>();
            newActions.AddRange(GeneratePossibleActions(action.Copy()));
            return newActions;
        }

        private List<ActionDecl> GeneratePossibleActions(ActionDecl source)
        {
            List<ActionDecl> newActions = new List<ActionDecl>();
            source.Preconditions = EnsureAnd(source.Preconditions);
            source.Effects = EnsureAnd(source.Effects);
            var allWhens = source.Effects.FindTypes<WhenExp>();
            foreach (var when in allWhens)
            {
                var useful = GetMostUsefullParent(when);
                if (useful.Parent is IListable list)
                    list.Remove(useful);
            }

            var permutations = GeneratePermutations(allWhens.Count);
            foreach(var permutation in permutations)
            {
                var newAct = source.Copy();

                for (int i = 0; i < permutation.Length; i++)
                {
                    if (permutation[i])
                    {
                        if (newAct.Preconditions is AndExp preAnd)
                            preAnd.Add(allWhens[i].Condition.Copy());
                        if (newAct.Effects is AndExp effAnd)
                            effAnd.Add(allWhens[i].Effect.Copy());
                    }
                }

                newActions.Add(newAct);
            }

            return newActions;
        }

        private INode GetMostUsefullParent(INode from)
        {
            if (from.Parent is AndExp)
                return from;
            if (from.Parent == null)
                throw new ArgumentNullException("Expected a parent");
            return GetMostUsefullParent(from.Parent);
        }

        private IExp EnsureAnd(IExp exp)
        {
            if (exp is AndExp)
                return exp;
            return new AndExp(new List<IExp>() { exp });
        }

        private Queue<bool[]> GeneratePermutations(int count)
        {
            var returnQueue = new Queue<bool[]>();
            GeneratePermutations(count, new bool[count], 0, returnQueue);
            return returnQueue;
        }

        private void GeneratePermutations(int count, bool[] source, int index, Queue<bool[]> returnQueue)
        {
            var trueSource = new bool[count];
            trueSource[index] = true;
            returnQueue.Enqueue(trueSource);
            if (index < count)
                GeneratePermutations(count, trueSource, index + 1, returnQueue);

            var falseSource = new bool[count];
            falseSource[index] = true;
            returnQueue.Enqueue(falseSource);
            if (index < count)
                GeneratePermutations(count, falseSource, index + 1, returnQueue);
        }
    }
}
