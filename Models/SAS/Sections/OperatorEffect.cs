using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.SAS.Sections
{
    public class OperatorEffect
    {
        public List<ValuePair> EffectConditions { get; set; }
        public int EffectedVariable { get; set; }
        public int VariablePrecondition { get; set; }
        public int VariableEffect { get; set;  }

        public OperatorEffect(List<ValuePair> effectConditions, int effectedVariable, int variablePrecondition, int variableEffect)
        {
            EffectConditions = effectConditions;
            EffectedVariable = effectedVariable;
            VariablePrecondition = variablePrecondition;
            VariableEffect = variableEffect;
        }
    }
}
