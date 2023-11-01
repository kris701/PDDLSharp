using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Translators.Tools
{
    public class ConditionalDeconstructor
    {
        public bool Aborted { get; set; } = false;
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

            if (allWhens.Count == 0)
                return new List<ActionDecl>() { source };

            foreach (var when in allWhens)
            {
                var useful = GetMostUsefullParent(when);
                if (useful.Parent is IWalkable walk)
                    walk.Replace(useful, new EmptyExp());
            }

            var permutations = GeneratePermutations(allWhens.Count);
            foreach (var permutation in permutations)
            {
                if (Aborted) return new List<ActionDecl>();
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
            if (from.Parent is IWalkable)
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
            if (Aborted) return;

            var trueSource = new bool[count];
            Array.Copy(source, trueSource, count);
            trueSource[index] = true;
            if (index < count - 1)
                GeneratePermutations(count, trueSource, index + 1, returnQueue);
            else
                returnQueue.Enqueue(trueSource);

            var falseSource = new bool[count];
            Array.Copy(source, falseSource, count);
            falseSource[index] = false;
            if (index < count - 1)
                GeneratePermutations(count, falseSource, index + 1, returnQueue);
            else
                returnQueue.Enqueue(falseSource);
        }
    }
}
