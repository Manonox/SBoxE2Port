using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E2Port.AST
{
	class BinOpNode : Node
	{
		public Node LNode { get; set; }
		public Lexer.Token OpToken { get; set; }
		public Node RNode { get; set; }
		public BinOpNode(Node lnode, Lexer.Token token, Node rnode)
		{
			LNode = lnode;
			OpToken = token;
			RNode = rnode;
		}
		public override string ToString()
		{
			return $"({LNode}, {OpToken}, {RNode})";
		}
	}
}
