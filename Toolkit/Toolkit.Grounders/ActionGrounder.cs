using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.Plans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Grounders
{
    public class ActionGrounder : IGrounder<ActionDecl, GroundedAction>
    {
        public PDDLDecl Declaration { get; }
        private Dictionary<string, List<string>> _objCache = new Dictionary<string, List<string>>();

        public ActionGrounder(PDDLDecl declaration)
        {
            Declaration = declaration;
        }

        public List<GroundedAction> Ground(ActionDecl item)
        {
            List<GroundedAction> groundedActions = new List<GroundedAction>();

            var allPermuations = GenerateParameterPermutations(item.Parameters.Values, new List<string>(item.Parameters.Values.Count));
            foreach (var premutation in allPermuations)
                groundedActions.Add(new GroundedAction(item.Name, premutation.ToArray()));

            return groundedActions;
        }

        private List<List<string>> GenerateParameterPermutations(List<NameExp> parameters, List<string> carried, int index = 0)
        {
            List<List<string>> returnList = new List<List<string>>();

            List<string> allOfType = GetObjsForType(parameters[index].Type.Name);
            foreach (var ofType in allOfType)
            {
                var newParam = new List<string>(carried);
                newParam.Add(ofType);
                if (index >= parameters.Count - 1)
                    returnList.Add(newParam);
                else
                    returnList.AddRange(GenerateParameterPermutations(parameters, newParam, index + 1));
            }

            return returnList;
        }

        private List<string> GetObjsForType(string typeName)
        {
            if (_objCache.ContainsKey(typeName))
                return _objCache[typeName];

            var addItems = new List<NameExp>();
            if (Declaration.Problem.Objects != null)
                addItems.AddRange(Declaration.Problem.Objects.Objs.Where(x => x.Type.IsTypeOf(typeName)));
            if (Declaration.Domain.Constants != null)
                addItems.AddRange(Declaration.Domain.Constants.Constants.Where(x => x.Type.IsTypeOf(typeName)));

            _objCache.Add(typeName, new List<string>());
            foreach (var item in addItems)
                _objCache[typeName].Add(item.Name);
            return _objCache[typeName];
        }
    }
}
