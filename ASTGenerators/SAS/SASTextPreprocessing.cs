using PDDLSharp.Tools;

namespace PDDLSharp.ASTGenerators.SAS
{
    public static class SASTextPreprocessing
    {
        public static string ReplaceSpecialCharacters(string text)
        {
            text = text.Replace('\r', ' ');
            text = text.Replace('\t', ' ');
            return text;
        }
    }
}
