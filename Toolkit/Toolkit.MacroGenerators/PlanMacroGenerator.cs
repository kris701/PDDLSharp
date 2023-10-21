using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.Plans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.MacroGenerators
{
    public class PlanMacroGenerator : IMacroGenerator<List<ActionPlan>>
    {
        public List<ActionDecl> FindMacros(List<ActionPlan> from)
        {
            List<ActionDecl> macros = new List<ActionDecl>();

            return macros;
        }
    }
}
