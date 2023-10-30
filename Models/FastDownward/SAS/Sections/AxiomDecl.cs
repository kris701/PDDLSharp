using PDDLSharp.Models.AST;

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
    }
}
