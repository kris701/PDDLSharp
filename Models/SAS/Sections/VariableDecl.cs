using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.SAS.Sections
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
    }
}
