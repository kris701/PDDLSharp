using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Toolkit.Grounders
{
    public abstract class BaseGrounder<T> : IGrounder<T>
    {
        public PDDLDecl Declaration { get; }
        private Dictionary<string, List<string>> _objCache = new Dictionary<string, List<string>>();

        protected BaseGrounder(PDDLDecl declaration)
        {
            Declaration = declaration;
        }

        public abstract List<T> Ground(T item);

        public List<List<string>> GenerateParameterPermutations(List<NameExp> parameters)
        {
            return GenerateParameterPermutations(parameters, new List<string>(parameters.Count), 0);
        }
        private List<List<string>> GenerateParameterPermutations(List<NameExp> parameters, List<string> carried, int index)
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
