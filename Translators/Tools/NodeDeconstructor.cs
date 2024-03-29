﻿using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Translators.Grounders;

namespace PDDLSharp.Translators.Tools
{
    public class NodeDeconstructor
    {
        private readonly OrDeconstructor _orDeconstructor;
        private readonly ForAllDeconstructor _forAllDeconstructor;
        private readonly ExistsDeconstructor _existsDeconstructor;
        private readonly ImplyDeconstructor _implyDeconstructor;
        private readonly ConditionalDeconstructor _conditionalDeconstructor;

        public NodeDeconstructor(IGrounder<IParametized> grounder)
        {
            _orDeconstructor = new OrDeconstructor();
            _forAllDeconstructor = new ForAllDeconstructor(grounder);
            _existsDeconstructor = new ExistsDeconstructor(grounder);
            _implyDeconstructor = new ImplyDeconstructor();
            _conditionalDeconstructor = new ConditionalDeconstructor();
        }

        public T Deconstruct<T>(T item) where T : INode
        {
            if (item.FindTypes<ForAllExp>().Count > 0)
                item = _forAllDeconstructor.DeconstructForAlls(item);
            if (item.FindTypes<ExistsExp>().Count > 0)
                item = _existsDeconstructor.DeconstructExists(item);
            if (item.FindTypes<ImplyExp>().Count > 0)
                item = _implyDeconstructor.DeconstructImplies(item);
            return item;
        }

        public void Abort()
        {
            _orDeconstructor.Aborted = true;
            _forAllDeconstructor.Aborted = true;
            _existsDeconstructor.Aborted = true;
            _implyDeconstructor.Aborted = true;
            _conditionalDeconstructor.Aborted = true;
        }

        public List<ActionDecl> DeconstructAction(ActionDecl act)
        {
            // Initial unary deconstruction
            act = Deconstruct(act);

            // Multiple deconstruction
            var conditionalsDeconstructed = _conditionalDeconstructor.DecontructConditionals(act);

            if (act.FindTypes<OrExp>().Count > 0)
            {
                var orDeconstructed = new List<ActionDecl>();
                foreach (var decon in conditionalsDeconstructed)
                    orDeconstructed.AddRange(_orDeconstructor.DeconstructOrs(decon));
                return orDeconstructed;
            }

            return conditionalsDeconstructed;
        }
    }
}
