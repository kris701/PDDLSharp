using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.SAS.Sections
{
    public class VersionDecl : BaseSASNode
    {
        public int Version { get; set; }

        public VersionDecl(ASTNode node, int version) : base(node)
        {
            Version = version;
        }

        public VersionDecl(int version)
        {
            Version = version;
        }

        public override string? ToString()
        {
            return $"{Version}";
        }
    }
}
