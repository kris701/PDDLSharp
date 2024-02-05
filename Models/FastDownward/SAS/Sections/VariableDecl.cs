using PDDLSharp.Models.AST;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.FastDownward.SAS.Sections
{
    public class VariableDecl : BaseSASNode
    {
        public string VariableName { get; set; }
        public int AxiomLayer { get; set; }
        public List<string> SymbolicNames { get; set; }

        public VariableDecl(ASTNode node, string variableName, int axiomLayer, List<string> symbolicNames) : base(node)
        {
            VariableName = variableName;
            AxiomLayer = axiomLayer;
            SymbolicNames = symbolicNames;
        }

        public VariableDecl(string variableName, int axiomLayer, List<string> symbolicNames)
        {
            VariableName = variableName;
            AxiomLayer = axiomLayer;
            SymbolicNames = symbolicNames;
        }

        public override string? ToString()
        {
            return $"{VariableName}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is VariableDecl other)
            {
                if (VariableName != other.VariableName) return false;
                if (AxiomLayer != other.AxiomLayer) return false;
                if (!EqualityHelper.AreListsEqual(SymbolicNames, other.SymbolicNames)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = VariableName.GetHashCode() ^ AxiomLayer.GetHashCode();
            foreach (var child in SymbolicNames)
                hash ^= child.GetHashCode();
            return hash;
        }

        public VariableDecl Copy()
        {
            var symNames = new List<string>();
            foreach (var sym in SymbolicNames)
                symNames.Add($"{sym}");
            return new VariableDecl(VariableName, AxiomLayer, symNames);
        }
    }
}
