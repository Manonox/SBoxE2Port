using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E2Port.Parser
{
	class ParseResult
	{
		public AST.Node Node { get; set; } = null;
		public Error Error { get; set; } = null;

		public ParseResult() { }

		public AST.Node Register(ParseResult res)
		{
			if (res.Error != null)
				Error = res.Error;
			return res.Node;
		}
		public dynamic Register(dynamic res) // Pass-through
		{
			return res;
		}

		public ParseResult Success(AST.Node node)
		{
			Node = node;
			return this;
		}

		public ParseResult Failure(Error error)
		{
			Error = error;
			return this;
		}

		public override string ToString()
		{
			string str = Error!=null ? Error.ToString() : Node.ToString();
			return base.ToString() + $"[\n{str}\n]";
		}
	}
}
