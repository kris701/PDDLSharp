namespace PDDLSharp.ASTGenerators.FastDownward.SAS
{
    public static class SASTextPreprocessing
    {
        public static string ReplaceSpecialCharacters(string text)
        {
            text = text.Replace('\r', ' ');
            text = text.Replace('\t', ' ');
            return text;
        }

        public static string TokenizeSpecials(string text)
        {
            text = text.Replace("begin_", SASASTTokens.BeginToken);
            text = text.Replace("end_", SASASTTokens.EndToken);
            return text;
        }
    }
}
