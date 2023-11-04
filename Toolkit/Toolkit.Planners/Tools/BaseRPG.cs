using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.StateSpace.SAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Tools
{
    public abstract class BaseRPG
    {
        internal List<Operator> GetNewApplicableOperators(ISASState state, List<Operator> operators, bool[] covered)
        {
            var result = new List<Operator>();
            for (int i = 0; i < covered.Length; i++)
            {
                if (!covered[i] && state.IsNodeTrue(operators[i]))
                {
                    result.Add(operators[i]);
                    covered[i] = true;
                }
            }
            return result;
        }
    }
}
