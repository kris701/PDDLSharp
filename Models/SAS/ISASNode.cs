using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.SAS
{
    public interface ISASNode
    {
        public int Start { get; set; }
        public int End { get; set; }
        public int Line { get; set; }
    }
}
