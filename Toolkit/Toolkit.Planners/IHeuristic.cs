﻿using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.Plans;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners
{
    public interface IHeuristic
    {
        public PDDLDecl Declaration { get; }
        public int GetValue(int currentValue, IState state, HashSet<ActionDecl> groundedActions);
    }
}