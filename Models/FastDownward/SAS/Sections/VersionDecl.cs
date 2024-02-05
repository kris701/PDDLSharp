using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.FastDownward.SAS.Sections
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

        public override bool Equals(object? obj)
        {
            if (obj is VersionDecl other)
            {
                if (Version != other.Version) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Version.GetHashCode();
        }

        public VersionDecl Copy() => new VersionDecl(Version);
    }
}
