using PDDLSharp.Tools;
using System.Xml.Linq;

namespace PDDLSharp.Models.FastDownward.SAS.Sections
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

        public override bool Equals(object? obj)
        {
            if (obj is OperatorEffect other)
            {
                if (EffectedVariable != other.EffectedVariable) return false;
                if (VariablePrecondition != other.VariablePrecondition) return false;
                if (VariableEffect != other.VariableEffect) return false;
                if (!EqualityHelper.AreListsEqual(EffectConditions, other.EffectConditions)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = EffectedVariable.GetHashCode() ^ VariablePrecondition.GetHashCode() ^ VariableEffect.GetHashCode();
            foreach (var child in EffectConditions)
                hash ^= child.GetHashCode();
            return hash;
        }
    }
}
