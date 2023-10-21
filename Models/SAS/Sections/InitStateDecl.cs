﻿using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.SAS.Sections
{
    public class InitStateDecl : BaseSASNode
    {
        public List<int> Inits { get; set; }

        public InitStateDecl(ASTNode node, List<int> inits) : base(node)
        {
            Inits = inits;
        }

        public InitStateDecl(List<int> inits)
        {
            Inits = inits;
        }

        public override string? ToString()
        {
            var retStr = "";
            foreach (var value in Inits)
                retStr += $"{value} ";
            return retStr.Trim();
        }
    }
}
