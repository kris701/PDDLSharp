using PDDL.ErrorListeners;
using PDDL.Models;
using PDDL.Models.Domain;
using PDDL.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDL.CodeGenerators
{
    public interface IPDDLCodeGenerator
    {
        public IErrorListener Listener { get; }

        public string Generate(INode node);
        public void Generate(INode node, string toFile);
    }
}
