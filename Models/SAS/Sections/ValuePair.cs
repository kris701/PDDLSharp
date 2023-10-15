using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.SAS.Sections
{
    public class ValuePair
    {
        public int Left { get; set; }
        public int Right { get; set; }

        public ValuePair(int left, int right)
        {
            Left = left;
            Right = right;
        }
    }
}
