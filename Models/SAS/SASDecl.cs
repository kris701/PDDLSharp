using PDDLSharp.Models.AST;
using PDDLSharp.Models.SAS.Sections;

namespace PDDLSharp.Models.SAS
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
    }
}
