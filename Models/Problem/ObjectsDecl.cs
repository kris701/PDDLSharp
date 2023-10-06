using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Tools;
using PDDLSharp.Models.Expressions;

namespace PDDLSharp.Models.Problem
{
    public class ObjectsDecl : BaseWalkableNode, IDecl
    {
        public List<NameExp> Objs { get; set; }

        public ObjectsDecl(ASTNode node, INode parent, List<NameExp> types) : base(node, parent)
        {
            Objs = types;
        }

        public ObjectsDecl(INode parent, List<NameExp> types) : base(parent)
        {
            Objs = types;
        }

        public ObjectsDecl(List<NameExp> types) : base()
        {
            Objs = types;
        }

        public ObjectsDecl() : base()
        {
            Objs = new List<NameExp>();
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var obj in Objs)
                hash *= obj.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Objs.GetEnumerator();
        }
    }
}
