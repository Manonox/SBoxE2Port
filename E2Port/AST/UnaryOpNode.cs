using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E2Port.AST
{
	internal class UnaryOpNode : Node
	{
		public Lexer.Token OpToken { get; set; }
		public Node Node { get; set; }
	}

	class UnaryPreOpNode : UnaryOpNode
	{
		public UnaryPreOpNode(Lexer.Token token, Node node)
		{
			OpToken = token;
			Node = node;
		}
		public override string ToString()
		{
			return $"({OpToken}, {Node})";
		}
	}
	class UnaryPostOpNode : UnaryOpNode
	{
		public UnaryPostOpNode(Node node, Lexer.Token token)
		{
			Node = node;
			OpToken = token;
		}
		public override string ToString()
		{
			return $"({Node}, {OpToken})";
		}
	}
}
