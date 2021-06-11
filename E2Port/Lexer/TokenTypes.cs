using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E2Port.Lexer
{

	public enum TokenType
	{
		LParen,		// (
		RParen,		// )
		LBrace,				// {
		RBrace,				// }
		LBracket,			// [
		RBracket,			// ]

		Comma,				// ,
		Colon,				// :

		Bang,				// !

		Equal,				// ==
		NotEqual,			// !=

		Assignment,			// =

		Add,				// +
		AddAssignment,		// +=
		Increment,			// ++

		Sub,				// -
		SubAssignment,		// -=
		Decrement,			// --

		Mul,				// *
		MulAssignment,		// *=
		Pow,				// **

		Div,				// /
		DivAssignment,		// /=

		Mod,				// %
		ModAssignment,      // %=

		// [E2 Stupidities Pt1]
		And,				// &
		BitAnd,				// &&
		Or,					// |
		BitOr,				// ||

		GreaterThan,		// >
		GreaterThanOrEqual, // >=
		LessThan,			// <
		LessThanOrEqual,	// <=

		// Literals
		NumberLiteral,		// 13.4 or 5 or .99
		StringLiteral,		// "bruh" or "" (or r"\//\/\")

		Identifier,         // Something or something

		// E2 Specific
		// TODO: ~, $, ->
		Directive,          // @persist ...

		// Keywords
		IfKeyword,
		ElseKeyword,
		ElseIfKeyword,
		SwitchKeyword,

		ForKeyword,
		ForEachKeyword,
		WhileKeyword,
		ContinueKeyword,
		BreakKeyword,

		FunctionKeyword,
		ReturnKeyword,

		// Other
		EndOfFile,

		Invalid,
	}
}
