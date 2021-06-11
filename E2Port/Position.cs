using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E2Port
{
	class Position
	{
		public int Index { get; set; } = 0;
		public int Line { get; set; } = 1;
		public int Column { get; set; } = 1;

		public string Source { get; set; }
		public string FileName { get; set; }

		public Position()
		{
		}

		public Position(string source, string fn)
		{
			Source = source.Replace("\r\n", "\n").Replace("\r", "\n");
			FileName = fn;
		}

		public Position(int idx, int col, int line, string source, string fn)
		{
			Index = idx;
			Line = line;
			Column = col;

			Source = source.Replace("\r\n", "\n").Replace("\r", "\n");
			FileName = fn;
		}

		public Position Copy()
		{
			Position pos = new Position();
			pos.Index = Index;
			pos.Line = Line;
			pos.Column = Column;

			pos.Source = Source;
			pos.FileName = FileName;
			return pos;
		}

		public char GetChar(int offset = 0)
		{
			int idx = Index + offset;
			return (idx > -1 && idx < Source.Length) ? Source[idx] : '\0';
		}

		public string GetString(int len)
		{
			if (Source.Length - Index < len) return null;
			return Source.Substring(Index, len);
		}

		public string ReadLine()
		{
			string s = "";
			while (GetChar() != '\n') {
				s += GetChar();
				Advance();
				if (IsEndOfFile())
					return s;
			}
			Advance();
			return s;
		}

		public Position Advance(int steps = 1)
		{
			for (int i = 0; i < steps; i++)
			{
				Index++;
				switch (GetChar())
				{
					case '\t':
						int tabsize = 4;
						Column = 1 + Convert.ToInt32(Math.Floor(Convert.ToSingle(Column + 3) / tabsize));
						break;

					case '\n':
						Line++;
						Column = 1;
						break;

					default:
						Column++;
						break;
				}
			}
			return this;
		}

		public bool IsEndOfFile()
		{
			return GetChar() == '\0';
		}

		public override string ToString()
		{
			return FileName + ":" + Line + ":" + Column;
		}
	}
}
