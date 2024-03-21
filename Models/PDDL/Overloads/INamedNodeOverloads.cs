using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.PDDL.Overloads
{
    public static class INamedNodeOverloads
    {
        public static INamedNode Annonymise(this INamedNode self)
        {
            var copy = self.Copy();
            if (copy is INamedNode node)
            {
                node.Name = "Name";
                return node;
            }
            throw new Exception("Invalid copy method!");
        }

        public static INamedNode Prefix(this INamedNode self, string prefix)
        {
            var copy = self.Copy();
            if (copy is INamedNode node)
            {
                node.Name = $"{prefix}{node.Name}";
                return node;
            }
            throw new Exception("Invalid copy method!");
        }

        public static INamedNode Sufix(this INamedNode self, string sufix)
        {
            var copy = self.Copy();
            if (copy is INamedNode node)
            {
                node.Name = $"{node.Name}{sufix}";
                return node;
            }
            throw new Exception("Invalid copy method!");
        }
    }
}
