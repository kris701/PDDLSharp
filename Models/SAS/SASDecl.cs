﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.SAS
{
    public class SASDecl
    {
        public HashSet<string> DomainVariables { get; set; }
        public List<Operator> Operators { get; set; }
        public HashSet<Fact> Goal { get; set; }
        public HashSet<Fact> Init { get; set; }

        public SASDecl(HashSet<string> domainVariables, List<Operator> operators, HashSet<Fact> goal, HashSet<Fact> init)
        {
            DomainVariables = domainVariables;
            Operators = operators;
            Goal = goal;
            Init = init;
        }

        public SASDecl()
        {
            DomainVariables = new HashSet<string>();
            Operators = new List<Operator>();
            Goal = new HashSet<Fact>();
            Init = new HashSet<Fact>();
        }
    }
}
