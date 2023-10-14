using PDDLSharp;
using PDDLSharp.Contextualisers;
using PDDLSharp.Contextualisers.Tests;
using PDDLSharp.Contextualisers.Tests.PDDL;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Contextualisers.Tests.PDDL
{
    internal static class ContextualiserTestsHelpers
    {
        internal static bool AreAllNameExpOfTypeOrSubType(IExp exp, string name, string type)
        {
            if (exp is AndExp and)
            {
                foreach (var child in and.Children)
                    if (!AreAllNameExpOfTypeOrSubType(child, name, type))
                        return false;
            }
            else if (exp is OrExp or)
            {
                foreach (var child in or.Options)
                    if (!AreAllNameExpOfTypeOrSubType(child, name, type))
                        return false;
            }
            else if (exp is NotExp not)
            {
                if (!AreAllNameExpOfTypeOrSubType(not.Child, name, type))
                    return false;
            }
            else if (exp is PredicateExp pred)
            {
                foreach (var arg in pred.Arguments)
                    if (!AreAllNameExpOfTypeOrSubType(arg, name, type))
                        return false;
            }
            else if (exp is NameExp nameExp)
            {
                if (nameExp.Name == name)
                {
                    if (!nameExp.Type.IsTypeOf(type))
                        return false;
                }
            }
            return true;
        }
    }
}
