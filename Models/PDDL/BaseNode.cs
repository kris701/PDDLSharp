using PDDLSharp.Models.AST;
using System.Collections;
using System.Reflection;

namespace PDDLSharp.Models.PDDL
{
    public abstract class BaseNode : INode
    {
        public INode? Parent { get; }
        public int Start { get; set; }
        public int End { get; set; }
        public int Line { get; set; }
        public bool IsHidden { get; set; } = false;

        private List<PropertyInfo> _metaInfo = new List<PropertyInfo>();

        public BaseNode(ASTNode node, INode parent) : this(parent)
        {
            Line = node.Line;
            Start = node.Start;
            End = node.End;
        }

        public BaseNode(INode parent)
        {
            Line = -1;
            Start = -1;
            End = -1;
            Parent = parent;
        }

        public BaseNode()
        {
            Line = -1;
            Start = -1;
            End = -1;
            Parent = null;
        }

        private void CacheMetaInfo()
        {
            if (_metaInfo.Count > 0)
                return;
            _metaInfo = GetType().GetProperties().ToList();
            _metaInfo.RemoveAll(x => x.PropertyType.IsPrimitive || x.Name == "Parent");
        }

        public List<INamedNode> FindNames(string name)
        {
            List<INamedNode> returnSet = new List<INamedNode>();
            FindNames(returnSet, name);
            return returnSet;
        }

        public void FindNames(List<INamedNode> returnSet, string name)
        {
            if (IsHidden)
                return;

            if (this is INamedNode node)
                if (node.Name == name)
                    returnSet.Add(node);

            CacheMetaInfo();

            foreach (var prop in _metaInfo)
            {
                if (prop.PropertyType.IsAssignableTo(typeof(INode)))
                {
                    var testNode = prop.GetValue(this);
                    if (testNode is INode valueNode)
                        valueNode.FindNames(returnSet, name);
                }
                else if (IsList(prop.PropertyType))
                {
                    var value = prop.GetValue(this);
                    if (value is IEnumerable enu)
                        foreach (var innerValueNode in enu)
                            if (innerValueNode is INode actualInnerValueNode)
                                actualInnerValueNode.FindNames(returnSet, name);
                }
            }
        }

        public List<T> FindTypes<T>(List<Type>? stopIf = null, bool ignoreFirst = false)
        {
            List<T> returnSet = new List<T>();
            FindTypes(returnSet, stopIf, ignoreFirst);
            return returnSet;
        }
        public void FindTypes<T>(List<T> returnSet, List<Type>? stopIf = null, bool ignoreFirst = false)
        {
            if (IsHidden || (stopIf != null && !ignoreFirst && stopIf.Contains(GetType())))
                return;

            if (this is T self)
                returnSet.Add(self);

            CacheMetaInfo();

            foreach (var prop in _metaInfo)
            {
                if (prop.PropertyType.IsAssignableTo(typeof(INode)))
                {
                    var testNode = prop.GetValue(this);
                    if (testNode is INode valueNode)
                        valueNode.FindTypes(returnSet, stopIf, false);
                }
                else if (IsList(prop.PropertyType))
                {
                    var value = prop.GetValue(this);
                    if (value is IEnumerable enu)
                        foreach (var innerValueNode in enu)
                            if (innerValueNode is INode actualInnerValueNode)
                                actualInnerValueNode.FindTypes(returnSet, stopIf, false);
                }
            }
        }

        private bool IsList(Type o)
        {
            return o.IsGenericType &&
                   o.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        public override int GetHashCode()
        {
            return 50;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
            if (obj is not INode)
                return false;
            var hash1 = obj.GetHashCode();
            var hash2 = GetHashCode();
            return hash1 == hash2;
        }
    }
}
