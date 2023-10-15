﻿using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.SAS.Sections
{
    public class MetricDecl : BaseSASNode
    {
        public bool IsUsingMetrics { get; set; }

        public MetricDecl(ASTNode node, bool isUsingMetrics) : base(node)
        {
            IsUsingMetrics = isUsingMetrics;
        }

        public override string? ToString()
        {
            if (IsUsingMetrics)
                return "true";
            return "false";
        }
    }
}