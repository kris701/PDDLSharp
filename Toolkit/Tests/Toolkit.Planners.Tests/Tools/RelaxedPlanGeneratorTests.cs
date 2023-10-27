using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Parsers;
using PDDLSharp.Toolkit.Grounders;
using PDDLSharp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Toolkit.StateSpace;
using PDDLSharp.Toolkit.Planners.Tools;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Toolkit.Planners.Tests.Tools
{
    [TestClass]
    public class RelaxedPlanGeneratorTests
    {
        private static Dictionary<string, HashSet<ActionDecl>> _groundedCache = new Dictionary<string, HashSet<ActionDecl>>();
        private static HashSet<ActionDecl> GetGroundedActions(PDDLDecl decl)
        {
            if (_groundedCache.ContainsKey(decl.Domain.Name.Name + decl.Problem.Name.Name))
                return _groundedCache[decl.Domain.Name.Name + decl.Problem.Name.Name];

            IGrounder<ActionDecl> grounder = new ActionGrounder(decl);
            var actions = new HashSet<ActionDecl>();
            foreach (var act in decl.Domain.Actions)
                actions.AddRange(grounder.Ground(act).ToHashSet());
            _groundedCache.Add(decl.Domain.Name.Name + decl.Problem.Name.Name, actions);
            return actions;
        }

        private static Dictionary<string, PDDLDecl> _declCache = new Dictionary<string, PDDLDecl>();
        private static PDDLDecl GetPDDLDecl(string domain, string problem)
        {
            if (_declCache.ContainsKey(domain + problem))
                return _declCache[domain + problem];

            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = new PDDLParser(listener);
            var decl = new PDDLDecl(
                parser.ParseAs<DomainDecl>(new FileInfo(domain)),
                parser.ParseAs<ProblemDecl>(new FileInfo(problem))
                );
            _declCache.Add(domain + problem, decl);
            return decl;
        }

        [TestMethod]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01.pddl")]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob06.pddl")]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p01.pddl")]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p05.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s1-0.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s2-4.pddl")]
        public void Can_GenerateRelaxedPlan_ResultsInGoal(string domain, string problem)
        {
            // ARRANGE
            var decl = GetPDDLDecl(domain, problem);
            IState state = new RelaxedPDDLStateSpace(decl);
            var actions = GetGroundedActions(decl);
            var generator = new RelaxedPlanGenerator(decl);

            // ACT
            var result = generator.GenerateReplaxedPlan(state, actions);

            // ASSERT
            Assert.IsFalse(state.IsInGoal());
            foreach (var item in result)
                state.ExecuteNode(item.Effects);
            Assert.IsTrue(state.IsInGoal());
        }

        [TestMethod]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01.pddl", 9)]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob06.pddl", 57)]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p01.pddl", 16)]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p05.pddl", 70)]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s1-0.pddl", 3)]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s2-4.pddl", 6)]
        public void Can_GenerateRelaxedPlan_Length(string domain, string problem, int expected)
        {
            // ARRANGE
            var decl = GetPDDLDecl(domain, problem);
            IState state = new RelaxedPDDLStateSpace(decl);
            var actions = GetGroundedActions(decl);
            var generator = new RelaxedPlanGenerator(decl);

            // ACT
            var result = generator.GenerateReplaxedPlan(state, actions);

            // ASSERT
            Assert.AreEqual(expected, result.Count);
        }
    }
}
