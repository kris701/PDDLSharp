using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.SAS.Sections
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
    }
}
