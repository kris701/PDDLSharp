using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

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
    }
}
