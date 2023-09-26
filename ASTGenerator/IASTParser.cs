using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDL.ASTGenerator
{
    public interface IASTParser<T>
    {
        T Parse(string text);
        string TokenizeSpecials(string text);
    }
}
