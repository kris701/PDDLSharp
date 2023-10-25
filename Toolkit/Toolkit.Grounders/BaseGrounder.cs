using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Tools;

namespace PDDLSharp.Toolkit.Grounders
{
    public abstract class BaseGrounder<T> : IGrounder<T>
    {
        public PDDLDecl Declaration { get; internal set; }

        private Dictionary<int, TypeExp> _typeDict = new Dictionary<int, TypeExp>();
        private Dictionary<TypeExp, int> _typeRef = new Dictionary<TypeExp, int>();
        private Dictionary<int, NameExp> _objDict = new Dictionary<int, NameExp>();
        private Dictionary<NameExp, int> _objRef = new Dictionary<NameExp, int>();
        private Dictionary<int, int[]> _objCache = new Dictionary<int, int[]>();

        protected BaseGrounder(PDDLDecl declaration)
        {
            Declaration = declaration;
            IndexItems();
        }

        public abstract List<T> Ground(T item);

        private void IndexItems()
        {
            var addObjects = new List<NameExp>();
            if (Declaration.Problem.Objects != null)
                addObjects.AddRange(Declaration.Problem.Objects.Objs);
            if (Declaration.Domain.Constants != null)
                addObjects.AddRange(Declaration.Domain.Constants.Constants);

            var tempDict = new Dictionary<int, List<int>>();
            int typeIndex = 0;
            tempDict.Add(typeIndex, new List<int>());
            _typeDict.Add(typeIndex, new TypeExp("object"));
            _typeRef.Add(new TypeExp("object"), typeIndex++);
            if (Declaration.Domain.Types != null)
            {
                foreach (var type in Declaration.Domain.Types.Types)
                {
                    tempDict.Add(typeIndex, new List<int>());
                    _typeDict.Add(typeIndex, type);
                    _typeRef.Add(type, typeIndex++);
                }
            }

            int objectIndex = 0;
            foreach (var obj in addObjects)
            {
                if (tempDict.ContainsKey(_typeRef[obj.Type]))
                    tempDict[_typeRef[obj.Type]].Add(objectIndex);
                _objDict.Add(objectIndex, obj);
                _objRef.Add(obj, objectIndex++);
            }

            foreach (var key in tempDict.Keys)
                _objCache.Add(key, tempDict[key].ToArray());
        }

        public int GetIndexFromObject(NameExp obj) => _objRef[obj];
        public NameExp GetObjectFromIndex(int index) => _objDict[index];
        public int GetIndexFromType(TypeExp type) => _typeRef[type];
        public TypeExp GetTypeFromIndex(int index) => _typeDict[index];

        private Queue<int[]> _tempQueue = new Queue<int[]>();
        public Queue<int[]> GenerateParameterPermutations(List<NameExp> parameters)
        {
            var indexedParams = new int[parameters.Count];
            for (int i = 0; i < indexedParams.Length; i++)
                indexedParams[i] = _typeRef[parameters[i].Type];
            _tempQueue.Clear();
            GenerateParameterPermutations(indexedParams, new int[parameters.Count], 0);
            return _tempQueue;
        }
        private void GenerateParameterPermutations(int[] parameters, int[] carried, int index)
        {
            var allOfType = _objCache[parameters[index]];
            foreach (var ofType in allOfType)
            {
                var newParam = new int[parameters.Length];
                Array.Copy(carried, newParam, parameters.Length);
                newParam[index] = ofType;
                if (index >= parameters.Length - 1)
                    _tempQueue.Enqueue(newParam);
                else
                    GenerateParameterPermutations(parameters, newParam, index + 1);
            }
        }
    }
}
