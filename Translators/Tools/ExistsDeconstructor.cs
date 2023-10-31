﻿using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Translators.Grounders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Translators.Tools
{
    public class ExistsDeconstructor
    {
        public IGrounder<IParametized> Grounder { get; }

        public ExistsDeconstructor(IGrounder<IParametized> grounder)
        {
            Grounder = grounder;
        }

        public T DeconstructExists<T>(T node) where T : INode
        {
            var copy = node.Copy();
            var exists = copy.FindTypes<ExistsExp>();
            foreach (var exist in exists)
            {
                if (exist.Parent is IWalkable walk)
                {
                    var newNode = new OrExp(exist.Parent);

                    var result = Grounder.Ground(exist).Cast<ExistsExp>();
                    foreach (var item in result)
                        newNode.Add(item.Expression);

                    walk.Replace(exist, newNode);
                }
                else
                    throw new Exception("Parent for exists deconstruction must be a IWalkable!");
            }

            return (T)copy;
        }
    }
}