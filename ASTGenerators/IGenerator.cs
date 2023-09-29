using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.ASTGenerators
{
    public interface IGenerator<T>
    {
        public T Generate(FileInfo file);
        public T Generate(string text);
    }
}
