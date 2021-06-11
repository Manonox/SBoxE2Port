using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E2Port.AST
{
	class NumberNode : Node
	{
		public Lexer.Token Token { get; set; }
		public NumberNode(Lexer.Token token)
		{
			Token = token;
		}
		public override string ToString()
		{
			return Token.ToString();
		}
	}
}
