using PDDLSharp.Models;
using PDDLSharp.Models.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Contextualisers.Tests
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
                if (!AreAllNameExpOfTypeOrSubType(or.Option1, name, type))
                    return false;
                if (!AreAllNameExpOfTypeOrSubType(or.Option2, name, type))
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
