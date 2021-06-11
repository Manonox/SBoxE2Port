using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E2Port
{
	enum ErrorType
	{
		Unknown,
		BadDirective,
		IllegalChar,
		InvalidSyntax,
		MalformedNumber,
		UnexpectedSymbol,
		EndOfFile
	}

	class Error
	{
		public ErrorType Type { get; set; } = ErrorType.Unknown;
		public Position Start { get; set; }
		public Position End { get; set; }
		public string Details { get; set; }

		public static Error Unknown { get; } = new Error
		{
			Type = ErrorType.Unknown
		};

		public Error()
		{

		}

		public Error(ErrorType type, Position start, Position end, string details)
		{
			Start = start;
			End = end;
			Type = type;
			Details = details;
		}

		public override string ToString()
		{
			return Type + "(at " + Start + "): " + Details;
		}
	}
}
