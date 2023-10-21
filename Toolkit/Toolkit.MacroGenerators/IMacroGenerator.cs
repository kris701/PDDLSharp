using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.Plans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.MacroGenerators
{
    public interface IMacroGenerator<T>
    {
        public List<ActionDecl> FindMacros(T from);
    }
}
