using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E2Port.Lexer
{
	class Lexer
	{
		public E2Instance Instance { get; set; }
		public Position Pos { get; set; }

		public Lexer(E2Instance chip, string ftxt, string fn = "<stdin>")
		{
			Instance = chip;
			Pos = new Position(ftxt, fn);
		}

		public Lexer(string ftxt, string fn = "<stdin>")
		{
			Pos = new Position(ftxt, fn);
		}

		public enum DirectiveType
		{
			String,
			Identifiers,
			Flag
		}
		public static Dictionary<DirectiveType, string> directives = new Dictionary<DirectiveType, string>()
		{
			[DirectiveType.String] = "name model trigger",
			[DirectiveType.Identifiers] = "inputs outputs persist",
			[DirectiveType.Flag] = "autoupdate",
		};

		public Dictionary<string, dynamic> GetDefaultDirectives()
		{
			return new Dictionary<string, dynamic>(){
				["name"] = "generic",
				["model"] = "",
				["inputs"] = "",
				["outputs"] = "",
				["persist"] = "",
				["trigger"] = "all",
				["autoupdate"] = false
			};
		}

		public Error ReadDirectives(Dictionary<string, dynamic> d)
		{
			while ("@ \n\t".Contains(Pos.GetChar())) // Skip everything until @
			{
				if (Pos.GetChar() == '@')
				{
					bool found = false;
					foreach (var entry in directives)
					{
						foreach (var directiveName in entry.Value.Split(' '))
						{
							string fullText = "@" + directiveName;
							if (Pos.GetString(fullText.Length) == fullText)
							{
								Pos.Advance(fullText.Length);
								while (Pos.GetChar() == ' ')
									Pos.Advance();
								string text = Pos.ReadLine();
								switch (entry.Key)
								{
									case DirectiveType.String:
										d[directiveName] = text;
										break;
									case DirectiveType.Identifiers:
										d[directiveName] += text + " ";
										break;
									case DirectiveType.Flag:
										d[directiveName] = true;
										break;
								}
								found = true; // Skip the rest of the directive checks
								break;
							}
						}
						if (found) // Skip
							break;
					}
					if (!found) // If none of the directives fit
					{
						string line = Pos.ReadLine();
						string name = line;
						if (line.Contains(' '))
							name = line.Substring(0, line.IndexOf(' '));
						return new Error(ErrorType.BadDirective, Pos, Pos, $" there's no '{name}' directive");
					}
				}
				else
					Pos.Advance();
			}
			return null;
		}

		public (List<Token>, Dictionary<string, dynamic>, List<Error>) Tokenize()
		{
			var token_list = new List<Token>();
			var directives = GetDefaultDirectives();
			var error_list = new List<Error>();
			if (Pos.Source.Length == 0)
				return (token_list, directives, error_list);

			var error_d = ReadDirectives(directives);
			if (error_d != null)
				error_list.Add(error_d);

			do
			{
				while (" \t\n".Contains(Pos.GetChar()))
					Pos.Advance();
				if (Pos.IsEndOfFile()) break;

				Position start = Pos.Copy();

				(Token t, Error e) = MakeToken();

				if (t != null) token_list.Add(t);
				if (e != null) error_list.Add(e);
			} while (!Pos.IsEndOfFile());

			return (token_list, directives, error_list);
		}

		private Dictionary<string, TokenType> _keywords = new Dictionary<string, TokenType>()
		{
			["if"] = TokenType.IfKeyword,
			["else"] = TokenType.ElseKeyword,
			["elseif"] = TokenType.ElseIfKeyword,
			["switch"] = TokenType.SwitchKeyword,

			["for"] = TokenType.ForKeyword,
			["foreach"] = TokenType.ForEachKeyword,
			["while"] = TokenType.WhileKeyword,
			["continue"] = TokenType.ContinueKeyword,
			["break"] = TokenType.BreakKeyword,

			["function"] = TokenType.FunctionKeyword,
			["return"] = TokenType.ReturnKeyword,
		};
		private (Token, Error) MakeToken()
		{
			Position start = Pos.Copy();
			char c = Pos.GetChar();
			Pos.Advance();
			switch (c)
			{
				//	case '\n':
				//		return (new Token(TokenType.EndOfLine, start, Pos.Copy()), null);
				case '#':
					if (Pos.GetChar() == '[')
					{
						int comment_stack = 1;
						while (comment_stack > 0)
						{
							switch (Pos.GetString(2))
							{
								case "#[":
									comment_stack++;
									break;
								case "]#":
									comment_stack--;
									break;
							}
							Pos.Advance();
						}
						Pos.Advance(2);
					}
					else
					{
						while (Pos.GetChar() != '\n')
							Pos.Advance();
						Pos.Advance();
					}
					return (null, null);

				case '(':
					return (new Token(TokenType.LParenthesis, start, Pos.Copy()), null);
				case ')':
					return (new Token(TokenType.RParenthesis, start, Pos.Copy()), null);
				case '{':
					return (new Token(TokenType.LBrace, start, Pos.Copy()), null);
				case '}':
					return (new Token(TokenType.RBrace, start, Pos.Copy()), null);
				case '[':
					return (new Token(TokenType.LBracket, start, Pos.Copy()), null);
				case ']':
					return (new Token(TokenType.RBracket, start, Pos.Copy()), null);

				case ',':
					return (new Token(TokenType.Comma, start, Pos.Copy()), null);
				case ':':
					return (new Token(TokenType.Colon, start, Pos.Copy()), null);

				case '!':
					if (Pos.GetChar() != '=')
						return (new Token(TokenType.Bang, start, Pos.Copy()), null);
					Pos.Advance();
					return (new Token(TokenType.NotEqual, start, Pos.Copy()), null);

				case '=':
					if (Pos.GetChar() != '=')
						return (new Token(TokenType.Assignment, start, Pos.Copy()), null);
					Pos.Advance();
					return (new Token(TokenType.Equal, start, Pos.Copy()), null);

				case '+':
					if (Pos.GetChar() == '=')
					{
						Pos.Advance();
						return (new Token(TokenType.AddAssignment, start, Pos.Copy()), null);
					}
					if (Pos.GetChar() == '+')
					{
						Pos.Advance();
						return (new Token(TokenType.Increment, start, Pos.Copy()), null);
					}
					return (new Token(TokenType.Add, start, Pos.Copy()), null);
				case '-':
					if (Pos.GetChar() == '=')
					{
						Pos.Advance();
						return (new Token(TokenType.SubAssignment, start, Pos.Copy()), null);
					}
					if (Pos.GetChar() == '-')
					{
						Pos.Advance();
						return (new Token(TokenType.Decrement, start, Pos.Copy()), null);
					}
					return (new Token(TokenType.Sub, start, Pos.Copy()), null);
				case '*':
					if (Pos.GetChar() == '=')
					{
						Pos.Advance();
						return (new Token(TokenType.MulAssignment, start, Pos.Copy()), null);
					}
					if (Pos.GetChar() == '*')
					{
						Pos.Advance();
						return (new Token(TokenType.Pow, start, Pos.Copy()), null);
					}
					return (new Token(TokenType.Mul, start, Pos.Copy()), null);
				case '/':
					if (Pos.GetChar() != '=')
						return (new Token(TokenType.Div, start, Pos.Copy()), null);
					Pos.Advance();
					return (new Token(TokenType.DivAssignment, start, Pos.Copy()), null);
				case '%':
					if (Pos.GetChar() != '=')
						return (new Token(TokenType.Mod, start, Pos.Copy()), null);
					Pos.Advance();
					return (new Token(TokenType.ModAssignment, start, Pos.Copy()), null);

				case '&':
					if (Pos.GetChar() != '&')
						return (new Token(TokenType.And, start, Pos.Copy()), null);
					Pos.Advance();
					return (new Token(TokenType.BitAnd, start, Pos.Copy()), null);
				case '|':
					if (Pos.GetChar() != '|')
						return (new Token(TokenType.Or, start, Pos.Copy()), null);
					Pos.Advance();
					return (new Token(TokenType.BitOr, start, Pos.Copy()), null);

				case '>':
					if (Pos.GetChar() != '=')
						return (new Token(TokenType.GreaterThanOrEqual, start, Pos.Copy()), null);
					Pos.Advance();
					return (new Token(TokenType.GreaterThan, start, Pos.Copy()), null);
				case '<':
					if (Pos.GetChar() != '=')
						return (new Token(TokenType.LessThanOrEqual, start, Pos.Copy()), null);
					Pos.Advance();
					return (new Token(TokenType.LessThan, start, Pos.Copy()), null);

				default:
					// Number Literal
					if ("0123456789.".Contains(c))
					{
						bool dot = c == '.';
						string full = c.ToString();
						while ("0123456789.".Contains(Pos.GetChar()))
						{
							if (Pos.GetChar() == '.')
								if (dot)
								{
									char symb = Pos.GetChar();
									Pos.Advance();
									return (
										new Token(TokenType.Invalid, start, Pos.Copy()),
										new Error(ErrorType.UnexpectedSymbol, start, Pos.Copy(), $"'{symb}'"));
								}
								else
									dot = true;
							full += Pos.GetChar();
							Pos.Advance();
						}
						if (full.Last() == '.')
							return (
								new Token(TokenType.Invalid, start, Pos.Copy()),
								new Error(ErrorType.MalformedNumber, start, Pos.Copy(), "expected decimals"));
						var value = Convert.ToSingle(full, System.Globalization.CultureInfo.InvariantCulture);
						return (new Token(TokenType.NumberLiteral, start, Pos.Copy(), value), null);
					}

					// String Literal
					if (c == '\"')
					{
						string full = "";
						while (Pos.GetChar() != '"')
						{
							if (Pos.IsEndOfFile())
								return (
									new Token(TokenType.Invalid, start, Pos.Copy()),
									new Error(ErrorType.EndOfFile, start, Pos.Copy(), "string is not closed"));

							if (Pos.GetChar() == '\\')
							{
								Pos.Advance();
								char next = Pos.GetChar();
								switch (next)
								{
									case '"':
										full += '"';
										break;

									case 'n':
										full += '\n';
										break;
									case 't':
										full += '\t';
										break;
									case 'r':
										full += '\r';
										break;

									default:
										full += next;
										break;
								}
							}
							else
								full += Pos.GetChar();
							Pos.Advance();
						}

						Pos.Advance();
						return (new Token(TokenType.StringLiteral, start, Pos.Copy(), full), null);
					}

					// Keywords
					foreach (var entry in _keywords)
						if (start.GetString(entry.Key.Length) == entry.Key)
						{
							Pos.Advance(entry.Key.Length - 1);
							return (new Token(entry.Value, start, Pos.Copy()), null);
						}

					// Identifier
					string letters = "qwertyuiopasdfghjklzxcvbnm";
					letters += letters.ToUpper();
					string numbers = "0123456789";
					string valid_symbols = letters + numbers;
					if (letters.Contains(c))
					{
						string full = c.ToString();
						while (valid_symbols.Contains(Pos.GetChar()))
						{
							
							full += Pos.GetChar();
							Pos.Advance();
						}

						return (new Token(TokenType.Identifier, start, Pos.Copy(), full), null);
					}

					return (
						new Token(TokenType.Invalid, start, Pos.Copy()),
						new Error(ErrorType.IllegalChar, start, Pos.Copy(), "'" + Pos.GetChar() + "'")
						);
			}
		}

	}
}
