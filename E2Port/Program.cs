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

			Lexer.Lexer lexer = new Lexer.Lexer(File.ReadAllText(@"D:\E2Port\E2Port\test.txt"));
			(var tokens, var directives, var errors) = lexer.Tokenize();

			Console.WriteLine("---Tokens---");
			foreach (var t in tokens)
				Console.WriteLine(t);
			Console.WriteLine("---Directives---");
			foreach (var d in directives)
				Console.WriteLine(d);
			Console.WriteLine("---Errors---");
			foreach (var e in errors)
				Console.WriteLine(e);
		}
	}
}
