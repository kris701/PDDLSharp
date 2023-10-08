using Microsoft.VisualBasic;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;

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

        public void DecorateTypesNamesWithParameterType(IParametized node)
        {
            var sourceParams = new List<NameExp>();
            if (Declaration.Domain.Constants != null)
                sourceParams.AddRange(Declaration.Domain.Constants.Constants);
            DecorateTypesNamesWithParameterType(node, sourceParams);
        }
        private void DecorateTypesNamesWithParameterType(IParametized node, List<NameExp> parentParams)
        {
            // Normal decorate
            parentParams.AddRange(node.Parameters.Values);
            var allNames = node.FindTypes<NameExp>(new List<Type>() { typeof(ForAllExp), typeof(ExistsExp) }, true);
            foreach (var name in allNames)
            {
                var target = parentParams.FirstOrDefault(x => x.Name == name.Name);
                if (target != null)
                {
                    name.Type.Name = target.Type.Name;
                    name.Type.SuperType = target.Type.SuperType;
                }
            }

            // Decorate for other parametized items
            var allParametized = node.FindTypes<IParametized>();
            foreach (var param in allParametized)
                if (param != node)
                    DecorateTypesNamesWithParameterType(param, parentParams);
        }
    }
}
