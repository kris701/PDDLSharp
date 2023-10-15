using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.ASTGenerators.Tests.PositionTestsData
{
    internal class PositionNode
    {
        public int Start { get; set; } = 0;
        public int End { get; set; } = 0;
        public string NodeType { get; set; } = "";
        public int Layer { get; set; } = 0;
        public List<PositionNode> Children { get; set; } = new List<PositionNode>();
        public PositionNode() { }
        public PositionNode(string line)
        {
            var split = line.Split(':');
            Start = Int32.Parse(split[0]);
            End = Int32.Parse(split[1]);
            NodeType = split[2];
            Layer = line.Count(x => x == '\t');
        }

        public static PositionNode ParseExpectedFile(string expectedFile)
        {
            string[] lines = File.ReadAllLines(expectedFile);
            PositionNode root = new PositionNode(lines[0]);
            List<PositionNode> layerParents = new List<PositionNode>();
            layerParents.Insert(root.Layer, root);
            foreach (var line in lines.Skip(1))
            {
                var node = new PositionNode(line);
                if (layerParents.Count <= node.Layer)
                    layerParents.Insert(node.Layer, node);
                else
                    layerParents[node.Layer] = node;
                layerParents[node.Layer - 1].Children.Add(node);
            }
            return root;
        }
    }
}
