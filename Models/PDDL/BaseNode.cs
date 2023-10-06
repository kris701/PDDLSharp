using PDDLSharp.Models.AST;
using PDDLSharp.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.PDDL
{
    public abstract class BaseNode : INode
    {
        public INode? Parent { get; }
        public int Start { get; set; }
        public int End { get; set; }
        public int Line { get; set; }

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

        public List<INamedNode> FindNames(string name)
        {
            List<INamedNode> returnSet = new List<INamedNode>();
            List<PropertyInfo> myPropertyInfo = GetType().GetProperties().ToList();
            if (this is INamedNode node)
                if (node.Name == name)
                    returnSet.Add(node);
            foreach (var prop in myPropertyInfo)
            {
                if (prop.Name.ToUpper() != "PARENT")
                {
                    var value = prop.GetValue(this, null);
                    if (value is INode valueNode)
                        returnSet.AddRange(valueNode.FindNames(name));
                    else if (value != null && IsList(value))
                        if (value is IEnumerable enu)
                            foreach (var innerValueNode in enu)
                                if (innerValueNode is INode actualInnerValueNode)
                                    returnSet.AddRange(actualInnerValueNode.FindNames(name));
                }
            }
            return returnSet;
        }

        public List<T> FindTypes<T>(List<Type>? stopIf = null)
        {
            List<T> returnSet = new List<T>();
            if (stopIf != null && stopIf.Contains(GetType()))
                return returnSet;

            List<PropertyInfo> myPropertyInfo = GetType().GetProperties().ToList();
            if (this is T self)
                returnSet.Add(self);
            foreach (var prop in myPropertyInfo)
            {
                if (prop.Name.ToUpper() != "PARENT")
                {
                    var value = prop.GetValue(this, null);
                    if (value is INode valueNode)
                        returnSet.AddRange(valueNode.FindTypes<T>(stopIf));
                    else if (value != null && IsList(value))
                        if (value is IEnumerable enu)
                            foreach (var innerValueNode in enu)
                                if (innerValueNode is INode actualInnerValueNode)
                                    returnSet.AddRange(actualInnerValueNode.FindTypes<T>(stopIf));
                }
            }
            return returnSet;
        }

        private bool IsList(object o)
        {
            if (o == null) return false;
            return o is IList &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        public override int GetHashCode()
        {
            return 50;
            //return Start.GetHashCode() + End.GetHashCode() + Line.GetHashCode();
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
