using PDDLSharp.Models.AST;
using PDDLSharp.Models.FastDownward.SAS.Sections;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.FastDownward.SAS
{
    public class SASDecl : BaseSASNode
    {
        public VersionDecl? Version { get; set; }
        public MetricDecl? Metric { get; set; }
        public List<VariableDecl> Variables { get; set; }
        public List<MutexDecl> Mutexes { get; set; }
        public InitStateDecl? InitState { get; set; }
        public GoalStateDecl? GoalState { get; set; }
        public List<OperatorDecl> Operators { get; set; }
        public List<AxiomDecl> Axioms { get; set; }

        public SASDecl(ASTNode ast) : base(ast)
        {
            Variables = new List<VariableDecl>();
            Mutexes = new List<MutexDecl>();
            Operators = new List<OperatorDecl>();
            Axioms = new List<AxiomDecl>();
        }

        public SASDecl()
        {
            Variables = new List<VariableDecl>();
            Mutexes = new List<MutexDecl>();
            Operators = new List<OperatorDecl>();
            Axioms = new List<AxiomDecl>();
        }

        public override bool Equals(object? obj)
        {
            if (obj is SASDecl other)
            {
                if (Version == null && other.Version != null) return false;
                if (Version != null && other.Version == null) return false;
                if (Metric == null && other.Metric != null) return false;
                if (Metric != null && other.Metric == null) return false;
                if (InitState == null && other.InitState != null) return false;
                if (InitState != null && other.InitState == null) return false;
                if (GoalState == null && other.GoalState != null) return false;
                if (GoalState != null && other.GoalState == null) return false;

                if (Version != null && !Version.Equals(other.Version)) return false;
                if (Metric != null && !Metric.Equals(other.Metric)) return false;
                if (!EqualityHelper.AreListsEqual(Variables, other.Variables)) return false;
                if (!EqualityHelper.AreListsEqual(Mutexes, other.Mutexes)) return false;
                if (Version != null && !Version.Equals(other.Version)) return false;
                if (Metric != null && !Metric.Equals(other.Metric)) return false;
                if (!EqualityHelper.AreListsEqual(Operators, other.Operators)) return false;
                if (!EqualityHelper.AreListsEqual(Axioms, other.Axioms)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = 1;
            if (Version != null) hash ^= Version.GetHashCode();
            if (Metric != null) hash ^= Metric.GetHashCode();
            foreach(var variable in Variables)
                hash ^= variable.GetHashCode();
            foreach (var mutex in Mutexes)
                hash ^= mutex.GetHashCode();
            if (InitState != null) hash ^= InitState.GetHashCode();
            if (GoalState != null) hash ^= GoalState.GetHashCode();
            foreach (var op in Operators)
                hash ^= op.GetHashCode();
            foreach (var axi in Axioms)
                hash ^= axi.GetHashCode();

            return hash;
        }
    }
}
