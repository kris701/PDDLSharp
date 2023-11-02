namespace PDDLSharp.CodeGenerators.Visitors
{
    public partial class GeneratorVisitors
    {
        private bool _printType = false;
        private bool _readable = false;
        public GeneratorVisitors(bool readable)
        {
            _readable = readable;
        }

        internal string IndentStr(int by)
        {
            if (!_readable)
                return "";
            return new string('\t', by);
        }

        private bool _printTypeOverride = false;
        internal void PrintTypes(bool state)
        {
            if (_printTypeOverride)
                return;
            _printType = state;
        }
    }
}
