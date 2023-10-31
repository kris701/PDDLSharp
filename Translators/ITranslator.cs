using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Translators
{
    public interface ITranslator<From, To>
    {
        public To Translate(From from);
    }
}
