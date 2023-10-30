﻿using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Grounders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.StateSpace.SAS
{
    public class RelaxedSASStateSpace : SASStateSpace
    {
        public RelaxedSASStateSpace(PDDLDecl declaration) : base(declaration)
        {
        }

        public RelaxedSASStateSpace(PDDLDecl declaration, HashSet<Fact> state, HashSet<Fact> goal) : base(declaration, state, goal)
        {
        }

        public override int ExecuteNode(Operator node)
        {
            int changes = 0;
            //foreach (var fact in node.Del)
            //    if (State.Remove(fact))
            //        changes--;
            foreach (var fact in node.Add)
                if (State.Add(fact))
                    changes++;
            return changes;
        }

        public override IState<Fact, Operator> Copy()
        {
            var newState = new Fact[State.Count];
            State.CopyTo(newState);
            return new RelaxedSASStateSpace(Declaration, newState.ToHashSet(), Goals);
        }
    }
}