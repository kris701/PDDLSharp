using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Models.PDDL.Overloads
{
    public static class ActionDeclOverloads
    {
        public static void EnsureAnd(this ActionDecl self)
        {
            if (self.Preconditions is not AndExp)
                self.Preconditions = new AndExp(self, new List<IExp>() { self.Preconditions });
            if (self.Effects is not AndExp)
                self.Effects = new AndExp(self, new List<IExp>() { self.Effects });
        }

        private static ActionDecl Annonymise(this ActionDecl action)
        {
            int argIndex = 0;
            var copy = action.Copy();
            copy.Name = "Name";
            foreach (var param in copy.Parameters.Values)
            {
                var find = copy.FindNames(param.Name);
                foreach (var found in find)
                    found.Name = $"?{argIndex}";
                argIndex++;
            }
            return copy;
        }

        public static List<ActionDecl> Distinct(this List<ActionDecl> candidates)
        {
            var returnList = new List<ActionDecl>();
            var nonDuplicateIndexes = new List<int>();
            var annonymizedCandidates = Annonymise(candidates);

            for (int i = 0; i < candidates.Count; i++)
                if (!annonymizedCandidates.GetRange(i + 1, candidates.Count - i - 1).Any(x => x.Equals(annonymizedCandidates[i])))
                    nonDuplicateIndexes.Add(i);

            foreach (var index in nonDuplicateIndexes)
                returnList.Add(candidates[index]);
            return returnList;
        }

        public static List<ActionDecl> Distinct(this List<ActionDecl> candidates, List<ActionDecl> others)
        {
            var returnList = new List<ActionDecl>();
            var nonDuplicateIndexes = new List<int>();
            var annonymizedCandidates = Annonymise(candidates);
            var annonymizedActions = Annonymise(others);

            for (int i = 0; i < candidates.Count; i++)
            {
                if (!annonymizedActions.Any(x => x.Equals(annonymizedCandidates[i])) &&
                    !annonymizedCandidates.GetRange(i + 1, candidates.Count - i - 1).Any(x => x.Equals(annonymizedCandidates[i])))
                {
                    nonDuplicateIndexes.Add(i);
                }
            }

            foreach (var index in nonDuplicateIndexes)
                returnList.Add(candidates[index]);
            return returnList;
        }

        private static List<ActionDecl> Annonymise(List<ActionDecl> actions)
        {
            var returnList = new List<ActionDecl>();

            foreach (var action in actions)
                returnList.Add(action.Annonymise());

            return returnList;
        }
    }
}
