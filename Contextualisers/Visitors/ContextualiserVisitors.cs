using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.Domain;
using PDDLSharp.Models.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Contextualisers.Visitors
{
    public partial class ContextualiserVisitors
    {
        public IErrorListener Listener { get; internal set; }
        public PDDLDecl Declaration { get; internal set; }

        public ContextualiserVisitors(IErrorListener listener, PDDLDecl declaration)
        {
            Listener = listener;
            Declaration = declaration;
        }

        private void DecorateTypesNamesWithParameterType(ParameterExp parameters, List<INode> targets)
        {
            var allParameters = new List<NameExp>();
            allParameters.AddRange(parameters.Values);
            if (Declaration.Domain.Constants != null)
                allParameters.AddRange(Declaration.Domain.Constants.Constants);

            var allTypes = new List<NameExp>();
            foreach (var target in targets)
                allTypes.AddRange(target.FindTypes<NameExp>());

            foreach(var name in allTypes)
            {
                var target = allParameters.FirstOrDefault(x => x.Name == name.Name);
                if (target != null)
                {
                    name.Type.Name = target.Type.Name;
                    name.Type.SuperType = target.Type.SuperType;
                }
            }
        }
    }
}
