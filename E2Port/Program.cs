using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace E2Port
{
	class Program
	{
		static void Main(string[] args)
		{
			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

			//File.ReadAllText(@"D:\E2Port\E2Port\test.txt")
			var lexer = new Lexer.Lexer("-1.5 * (+.3 - 12)");
			(var tokens, var directives, var errors) = lexer.Tokenize();

			//Console.WriteLine("---Tokens---");
			//foreach (var t in tokens)
			//	Console.WriteLine(t);
			//Console.WriteLine("---Directives---");
			//foreach (var d in directives)
			//	Console.WriteLine(d);
			//Console.WriteLine("---Errors---");
			//foreach (var e in errors)
			//	Console.WriteLine(e);

			if (errors.Count > 0)
			{
				Console.WriteLine(errors[1]);
				return;
			}

			var parser = new Parser.Parser(tokens);
			Console.WriteLine(parser.Parse());
		}
	}
}
