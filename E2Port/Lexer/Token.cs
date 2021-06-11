using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E2Port.Lexer
{
	class Token
	{
		public TokenType Type { get; set; } = TokenType.Invalid;

		private Position start;
		public Position Start { get { return start; } set { start = value.Copy(); } }

		private Position end;
		public Position End { get { return end; } set { end = value.Copy(); } }

		public dynamic Value { get; set; }

		public static Token Invalid { get; } = new Token
		{
			Type = TokenType.Invalid
		};

		public Token() { }

		public Token(TokenType type, Position start)
		{
			Type = type;
			Start = start;
			End = start;
		}

		public Token(TokenType type, Position start, Position end)
		{
			Type = type;
			Start = start;
			End = end;
		}

		public Token(TokenType type, Position start, Position end, dynamic value)
		{
			Type = type;
			Start = start;
			End = end;
			Value = value;
		}

		public override string ToString()
		{
			return Type + (Value!=null ? $"[{Value}]" : "");
		}
	}
}
