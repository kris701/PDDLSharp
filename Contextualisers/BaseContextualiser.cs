using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Contextualisers
{
    public abstract class BaseContextualiser<T> : IContextualiser<T>
    {
        public IErrorListener Listener { get; }

        protected BaseContextualiser(IErrorListener listener)
        {
            Listener = listener;
        }

        public abstract void Contexturalise(T decl);

        internal void ReplaceNameExpTypeWith(IExp node, NameExp with)
        {
            if (node is AndExp and)
            {
                foreach (var child in and.Children)
                    ReplaceNameExpTypeWith(child, with);
            }
            else if (node is OrExp or)
            {
                foreach (var child in or.Options)
                    ReplaceNameExpTypeWith(child, with);
            }
            else if (node is NotExp not)
            {
                ReplaceNameExpTypeWith(not.Child, with);
            }
            else if (node is PredicateExp pred)
            {
                for (int i = 0; i < pred.Arguments.Count; i++)
                {
                    if (pred.Arguments[i].Name == with.Name)
                    {
                        pred.Arguments[i].Type = with.Type;
                    }
                }
            }
        }
    }
}
