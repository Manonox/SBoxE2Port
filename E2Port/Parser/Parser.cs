using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using E2Port.Lexer;
using E2Port.AST;

namespace E2Port.Parser
{
	class Parser
	{
		public List<Token> Tokens { get; set; }
		public int Index { get; set; } = -1;
		public Token Current { get; set; }

		public Parser(List<Token> tokens)
		{
			Tokens = tokens;
			Advance();
		}

		public Token Advance()
		{
			Index++;
			if (Index < Tokens.Count())
				Current = Tokens[Index];
			return Current;
		}

		// Grammar
		public ParseResult Parse()
		{
			var res = Expr();
			if (res.Error == null && Current.Type != TokenType.EndOfFile)
				return res.Failure(new Error(
					ErrorType.InvalidSyntax,
					Current.Start, Current.End,
					"expected something?.."
				));
			return res;
		}

		private ParseResult Factor()
		{
			var res = new ParseResult();
			var tok = Current;
			switch (Current.Type)
			{
				case TokenType.Add:
				case TokenType.Sub:
					res.Register(Advance());
					var factor = res.Register(Factor());
					if (res.Error != null)
						return res;
					return res.Success(new UnaryPreOpNode(tok, factor));

				case TokenType.NumberLiteral:
					res.Register(Advance());
					return res.Success(new NumberNode(tok));

				case TokenType.LParen:
					res.Register(Advance());
					var expr = res.Register(Expr());
					if (res.Error != null)
						return res;
					if (Current.Type == TokenType.RParen)
					{
						res.Register(Advance());
						return res.Success(expr);
					}

					return res.Failure(new Error(
						ErrorType.InvalidSyntax,
						tok.Start, tok.End,
						"expected ')'"
					));
			}

			return res.Failure(new Error(
				ErrorType.InvalidSyntax,
				tok.Start, tok.End,
				"expected '+', '-', '*' or '/'"
			));
		}

		private ParseResult Term()
		{
			return BinaryOperation(
				Factor, 
				new List<TokenType>() { TokenType.Mul, TokenType.Div }
			);
		}

		private ParseResult Expr()
		{
			return BinaryOperation(
				Term,
				new List<TokenType>() { TokenType.Add, TokenType.Sub }
			);
		}

		// Grammar Helpers

		private ParseResult BinaryOperation(Func<ParseResult> Func, List<TokenType> ops)
		{
			var res = new ParseResult();
			var left = res.Register(Func());
			if (res.Error != null) return res;

			while (ops.Contains(Current.Type))
			{
				Token op_token = Current;
				res.Register(Advance());
				var right = res.Register(Func());
				if (res.Error != null) return res;
				left = new BinOpNode(left, op_token, right);
			}
			return res.Success(left);
		}
	}
}
