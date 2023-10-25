using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Tools;

namespace PDDLSharp.Toolkit.Grounders
{
    public abstract class BaseGrounder<T> : IGrounder<T>
    {
        public PDDLDecl Declaration { get; internal set; }

        private Dictionary<int, string> _typeDict = new Dictionary<int, string>();
        private Dictionary<string, int> _typeRef = new Dictionary<string, int>();
        private Dictionary<int, string> _objDict = new Dictionary<int, string>();
        private Dictionary<string, int> _objRef = new Dictionary<string, int>();
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
            _typeDict.Add(typeIndex, "object");
            _typeRef.Add("object", typeIndex++);
            if (Declaration.Domain.Types != null)
            {
                foreach (var type in Declaration.Domain.Types.Types)
                {
                    tempDict.Add(typeIndex, new List<int>());
                    _typeDict.Add(typeIndex, type.Name);
                    _typeRef.Add(type.Name, typeIndex++);
                }
            }

            int objectIndex = 0;
            foreach (var obj in addObjects)
            {
                if (tempDict.ContainsKey(_typeRef[obj.Type.Name]))
                    if (!tempDict[_typeRef[obj.Type.Name]].Contains(objectIndex))
                        tempDict[_typeRef[obj.Type.Name]].Add(objectIndex);
                foreach (var superType in obj.Type.SuperTypes)
                    if (tempDict.ContainsKey(_typeRef[superType]))
                        if (!tempDict[_typeRef[superType]].Contains(objectIndex))
                            tempDict[_typeRef[superType]].Add(objectIndex);
                _objDict.Add(objectIndex, obj.Name);
                _objRef.Add(obj.Name, objectIndex++);
            }

            foreach (var key in tempDict.Keys)
                _objCache.Add(key, tempDict[key].ToArray());
        }

        public int GetIndexFromObject(string obj) => _objRef[obj];
        public string GetObjectFromIndex(int index) => _objDict[index];
        public int GetIndexFromType(string type) => _typeRef[type];
        public string GetTypeFromIndex(int index) => _typeDict[index];

        private Queue<int[]> _tempQueue = new Queue<int[]>();
        public Queue<int[]> GenerateParameterPermutations(List<NameExp> parameters)
        {
            var indexedParams = new int[parameters.Count];
            for (int i = 0; i < indexedParams.Length; i++)
                indexedParams[i] = _typeRef[parameters[i].Type.Name];
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
