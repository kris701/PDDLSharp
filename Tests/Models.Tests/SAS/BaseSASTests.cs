using PDDLSharp.Models.SAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.Tests.SAS
{
    public abstract class BaseSASTests
    {
        internal List<Fact> GenerateRandomFacts(int fromID, int amount)
        {
            var facts = new List<Fact>();
            var rnd = new Random();
            for (int i = 0; i < amount; i++)
            {
                var newFact = new Fact($"fact-{fromID + i}");
                newFact.ID = fromID + i;
                facts.Add(newFact);
            }

            return facts;
        }

        internal List<Operator> GenerateRandomOperator(int fromID, int amount)
        {
            var ops = new List<Operator>();
            var rnd = new Random();
            for (int i = 0; i < amount; i++)
            {
                var newOp = new Operator($"fact-{fromID + i}", new string[0], new Fact[0], new Fact[0], new Fact[0]);
                newOp.ID = fromID + i;
                ops.Add(newOp);
            }

            return ops;
        }
    }
}
