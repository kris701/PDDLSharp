using PDDLSharp.Models.AST;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.FastDownward.SAS.Sections
{
    public class AxiomDecl : BaseSASNode
    {
        public List<ValuePair> Conditions { get; set; }
        public int EffectedVariable { get; set; }
        public int VariablePrecondition { get; set; }
        public int NewVariableValue { get; set; }

        public AxiomDecl(ASTNode node, List<ValuePair> conditions, int effectedVariable, int variablePrecondition, int newVariableValue) : base(node)
        {
            Conditions = conditions;
            EffectedVariable = effectedVariable;
            VariablePrecondition = variablePrecondition;
            NewVariableValue = newVariableValue;
        }

        public AxiomDecl(List<ValuePair> conditions, int effectedVariable, int variablePrecondition, int newVariableValue)
        {
            Conditions = conditions;
            EffectedVariable = effectedVariable;
            VariablePrecondition = variablePrecondition;
            NewVariableValue = newVariableValue;
        }

        public override bool Equals(object? obj)
        {
            if (obj is AxiomDecl other)
            {
                if (EffectedVariable != other.EffectedVariable) return false;
                if (VariablePrecondition != other.VariablePrecondition) return false;
                if (NewVariableValue != other.NewVariableValue) return false;
                if (!EqualityHelper.AreListsEqual(Conditions, other.Conditions)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = EffectedVariable ^ VariablePrecondition ^ NewVariableValue;
            foreach (var child in Conditions)
                hash ^= child.GetHashCode();
            return hash;
        }

        public AxiomDecl Copy()
        {
            var conditions = new List<ValuePair>();
            foreach (var con in Conditions)
                conditions.Add(con.Copy());
            return new AxiomDecl(conditions, EffectedVariable, VariablePrecondition, NewVariableValue);
        }
    }
}
