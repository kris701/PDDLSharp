using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class WhenExp : BaseWalkableNode, IExp
    {
        public IExp Condition { get; set; }
        public IExp Effect { get; set; }

        public WhenExp(ASTNode node, INode? parent, IExp condition, IExp effect) : base(node, parent)
        {
            Condition = condition;
            Effect = effect;
        }

        public WhenExp(INode? parent, IExp condition, IExp effect) : base(parent)
        {
            Condition = condition;
            Effect = effect;
        }

        public WhenExp(IExp condition, IExp effect) : base()
        {
            Condition = condition;
            Effect = effect;
        }

        public WhenExp(ASTNode node, INode? parent) : base(node, parent)
        {
            Condition = new AndExp(this, new List<IExp>());
            Effect = new AndExp(this, new List<IExp>());
        }

        public WhenExp(INode? parent) : base(parent)
        {
            Condition = new AndExp(this, new List<IExp>());
            Effect = new AndExp(this, new List<IExp>());
        }

        public WhenExp() : base()
        {
            Condition = new AndExp(this, new List<IExp>());
            Effect = new AndExp(this, new List<IExp>());
        }

        public override bool Equals(object? obj)
        {
            if (obj is WhenExp other)
            {
                if (!base.Equals(other)) return false;
                if (!Condition.Equals(other.Condition)) return false;
                if (!Effect.Equals(other.Effect)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash ^= Condition.GetHashCode();
            hash ^= Effect.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Condition;
            yield return Effect;
        }

        public override WhenExp Copy(INode? newParent = null)
        {
            var newNode = new WhenExp(new ASTNode(Line, "", ""), newParent);
            var newCondition = ((dynamic)Condition).Copy(newNode);
            var newEffect = ((dynamic)Effect).Copy(newNode);
            newNode.Condition = newCondition;
            newNode.Effect = newEffect;
            newNode.IsHidden = IsHidden;
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            if (Condition == node && with is IExp exp1)
                Condition = exp1;
            if (Effect == node && with is IExp exp2)
                Effect = exp2;
        }
    }
}
