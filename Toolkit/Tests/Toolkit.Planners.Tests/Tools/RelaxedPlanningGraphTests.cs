using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Toolkit.Grounders;
using PDDLSharp.Toolkit.Planners.Tools;
using PDDLSharp.Toolkit.StateSpace;
using PDDLSharp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Tests.Tools
{
    [TestClass]
    public class RelaxedPlanningGraphTests
    {
        [TestMethod]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01.pddl", 3)]
        public void Can_GenerateGraph_Layer_Size(string domain, string problem, int expected)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = new PDDLParser(listener);
            var decl = new PDDLDecl(
                parser.ParseAs<DomainDecl>(new FileInfo(domain)),
                parser.ParseAs<ProblemDecl>(new FileInfo(problem))
                );
            IState state = new RelaxedPDDLStateSpace(decl);
            IGrounder<ActionDecl> grounder = new ActionGrounder(decl);
            var actions = new HashSet<ActionDecl>();
            foreach (var act in decl.Domain.Actions)
                actions.AddRange(grounder.Ground(act).ToHashSet());

            // ACT
            var graph = RelaxedPlanningGraph.GenerateRelaxedPlanningGraph(state, actions);

            // ASSERT
            Assert.AreEqual(expected, graph.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "No applicable actions found!")]
        public void Cant_GenerateGraph_IfNoApplicableActions_1()
        {
            // ARRANGE
            var decl = new PDDLDecl(new DomainDecl(), new ProblemDecl());
            decl.Problem.Goal = new GoalDecl();
            decl.Problem.Goal.GoalExp = new PredicateExp("abc");
            IState state = new RelaxedPDDLStateSpace(decl);

            // ACT
            RelaxedPlanningGraph.GenerateRelaxedPlanningGraph(state, new HashSet<ActionDecl>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "No applicable actions found!")]
        public void Cant_GenerateGraph_IfNoApplicableActions_2()
        {
            // ARRANGE
            var decl = new PDDLDecl(new DomainDecl(), new ProblemDecl());
            decl.Problem.Goal = new GoalDecl();
            decl.Problem.Goal.GoalExp = new PredicateExp("abc");
            IState state = new RelaxedPDDLStateSpace(decl);

            var actions = new HashSet<ActionDecl>();
            var action = new ActionDecl("non-applicable");
            action.Parameters = new ParameterExp(new List<NameExp>() { new NameExp("?a") });
            action.Preconditions = new PredicateExp("wew", new List<NameExp>() { new NameExp("?a") });
            actions.Add(action);

            // ACT
            RelaxedPlanningGraph.GenerateRelaxedPlanningGraph(state, actions);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Relaxed state did not change!")]
        public void Cant_GenerateGraph_IfActionDoesNothing()
        {
            // ARRANGE
            var decl = new PDDLDecl(new DomainDecl(), new ProblemDecl());
            decl.Problem.Goal = new GoalDecl();
            decl.Problem.Goal.GoalExp = new PredicateExp("abc");
            IState state = new RelaxedPDDLStateSpace(decl);

            var actions = new HashSet<ActionDecl>();
            var action = new ActionDecl("non-applicable");
            action.Parameters = new ParameterExp(new List<NameExp>() { new NameExp("?a") });
            actions.Add(action);

            // ACT
            RelaxedPlanningGraph.GenerateRelaxedPlanningGraph(state, actions);
        }
    }
}
