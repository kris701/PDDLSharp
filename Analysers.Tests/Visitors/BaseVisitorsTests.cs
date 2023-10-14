using PDDLSharp.Contextualisers;
using PDDLSharp.Contextualisers.PDDL;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Analysers.Tests.Visitors
{
    public class BaseVisitorsTests
    {
        internal PDDLDecl GetDeclaration(string domain, string problem, IErrorListener listener)
        {
            PDDLParser parser = new PDDLParser(listener);
            var decl = parser.ParseDecl(domain, problem);
            IContextualiser contextualiser = new PDDLContextualiser(listener);
            contextualiser.Contexturalise(decl);
            return decl;
        }

        internal INode GetNode(PDDLDecl decl, int[] target, IErrorListener listener)
        {
            if (target[0] == 0)
                return GetNode(decl.Domain, 1, target, listener);
            if (target[0] == 1)
                return GetNode(decl.Problem, 1, target, listener);
            return null;
        }

        internal INode GetNode(IWalkable source, int index, int[] target, IErrorListener listener)
        {
            if (index == target.Length)
                return source;
            int counter = 0;
            foreach (var item in source)
            {
                if (index == target.Length && target[index] == counter)
                    return item;
                else if (target[index] == counter)
                    return item;
                counter++;
            }
            return null;
        }
    }
}
