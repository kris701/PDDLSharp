namespace PDDLSharp.Models.SAS.Sections
{
    public class OperatorEffect
    {
        public List<ValuePair> EffectConditions { get; set; }
        public int EffectedVariable { get; set; }
        public int VariablePrecondition { get; set; }
        public int VariableEffect { get; set; }

        public OperatorEffect(List<ValuePair> effectConditions, int effectedVariable, int variablePrecondition, int variableEffect)
        {
            EffectConditions = effectConditions;
            EffectedVariable = effectedVariable;
            VariablePrecondition = variablePrecondition;
            VariableEffect = variableEffect;
        }

        public override string? ToString()
        {
            var retStr = $"{EffectConditions.Count} ";
            foreach (var condition in EffectConditions)
                retStr += $"{condition} ";
            retStr += $"{EffectedVariable} ";
            retStr += $"{VariablePrecondition} ";
            retStr += $"{VariableEffect}";
            return retStr;
        }
    }
}
