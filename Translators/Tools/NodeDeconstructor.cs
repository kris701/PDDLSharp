using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Translators.Grounders;

namespace PDDLSharp.Translators.Tools
{
    public class NodeDeconstructor
    {
        private ForAllDeconstructor _forAllDeconstructor;
        private ExistsDeconstructor _existsDeconstructor;
        private ConditionalDeconstructor _conditionalDeconstructor;

        public NodeDeconstructor(IGrounder<IParametized> grounder)
        {
            _forAllDeconstructor = new ForAllDeconstructor(grounder);
            _existsDeconstructor = new ExistsDeconstructor(grounder);
            _conditionalDeconstructor = new ConditionalDeconstructor();
        }

        public T Deconstruct<T>(T item) where T : INode
        {
            if (item.FindTypes<ForAllExp>().Count > 0)
                item = _forAllDeconstructor.DeconstructForAlls(item);
            if (item.FindTypes<ExistsExp>().Count > 0)
                item = _existsDeconstructor.DeconstructExists(item);
            return item;
        }

        public List<ActionDecl> DeconstructAction(ActionDecl act)
        {
            act = Deconstruct(act);
            if (act.Effects.FindTypes<WhenExp>().Count > 0)
                return _conditionalDeconstructor.DecontructConditionals(act);
            return new List<ActionDecl>() { act };
        }
    }
}
