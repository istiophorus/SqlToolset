#region License

/* 
   Copyright (C) 2014, Rafal Skotak
   All rights reserved.                          

   Redistribution and use in source and binary forms, with or without
   modification, are permitted provided that the following conditions
   are met:

     1. Redistributions of source code must retain the above copyright
        notice, this list of conditions and the following disclaimer.

     2. Redistributions in binary form must reproduce the above copyright
        notice, this list of conditions and the following disclaimer in the
        documentation and/or other materials provided with the distribution.

     3. The names of its contributors may not be used to endorse or promote 
        products derived from this software without specific prior written 
        permission.
*/

/*
   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
   A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE COPYRIGHT OWNER OR
   CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
   LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
   NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

#endregion License

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Server;
using System.Linq;
using System.Globalization;

namespace SqlToolset
{
	public sealed class StringOperations
	{
		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlString CharAt(SqlString input, SqlInt32 index)
		{
			if (input.IsNull)
			{
				return SqlString.Null;
			}

			if (index.IsNull)
			{
				return SqlString.Null;
			}

			Int32 indexValue = index.Value;

			String text = input.Value;

			if (indexValue < 0 || indexValue >= text.Length)
			{
				return SqlString.Null;
			}

			return new SqlString(new String(text[indexValue], 1));
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlInt32 MatchesPattern(SqlString pattern, SqlString value)
		{
			if (pattern.IsNull)
			{
				return CommonDefinitions.ResultFalse;
			}

			if (value.IsNull)
			{
				return CommonDefinitions.ResultFalse;
			}

			Match m = Regex.Match(value.ToString(), pattern.ToString(), RegexOptions.Singleline);

			return m.Success ? CommonDefinitions.ResultTrue : CommonDefinitions.ResultFalse;
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlInt32 LevenshteinDistance(SqlString textA, SqlString textB)
		{
			if (textA.IsNull)
			{
				return SqlInt32.Null;
			}

			if (textB.IsNull)
			{
				return SqlInt32.Null;
			}

			return SqlToolset.LevenshteinDistance.Compute(textA.Value, textB.Value);
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlString SwitchCase(SqlString input)
		{
			if (input.IsNull)
			{
				return SqlString.Null;
			}

			Char[] chars = input.Value.ToCharArray();

			List<Char> result = new List<Char>();

			for (Int32 q = 0; q < chars.Length; q++)
			{
				Char current = chars[q];

				if (Char.IsLetter(current))
				{
					if (Char.IsUpper(current))
					{
						current = Char.ToLower(current);
					}
					else if (Char.IsLower(current))
					{
						current = Char.ToUpper(current);
					}
				}

				result.Add(current);
			}

			return new SqlString(new String(result.ToArray()));
		}

		private const Char DefaultSeparator = ',';

		[SqlFunction(DataAccess = DataAccessKind.None, FillRowMethodName = "FillRowSplitToInts", IsDeterministic = true, IsPrecise = true, TableDefinition = @"value bigint")]
		public static IEnumerable SplitToInts(SqlString value, SqlString separators)
		{
			if (value.IsNull)
			{
				return null;
			}

			Char[] separatorsArray = new Char[] { DefaultSeparator };

			if (!separators.IsNull)
			{
				String tempSep = separators.Value;

				if (tempSep.Length > 0)
				{
					separatorsArray = tempSep.ToCharArray();
				}
			}

			return value.Value.Split(separatorsArray);
		}

		[SqlFunction(DataAccess = DataAccessKind.None, FillRowMethodName = "FillRowSplit", IsDeterministic = true, IsPrecise = true, TableDefinition = @"value nvarchar(max)")]
		public static IEnumerable Split(SqlString value, SqlString separators)
		{
			if (value.IsNull)
			{
				return null;
			}

			Char[] separatorsArray = new Char[] { DefaultSeparator };

			if (!separators.IsNull)
			{
				String tempSep = separators.Value;

				if (tempSep.Length > 0)
				{
					separatorsArray = tempSep.ToCharArray();
				}
			}

			return value.Value.Split(separatorsArray);
		}

		private static void FillRowSplit(Object obj, out SqlString singleValue)
		{
			String tempValue = (String)obj;

			singleValue = tempValue;
		}

		private static void FillRowSplitToInts(Object obj, out SqlInt64 singleValue)
		{
			String tempValue = (String)obj;

			Int64 tempNumber = 0;

			if (Int64.TryParse(tempValue, out tempNumber))
			{
				singleValue = tempNumber;
			}
			else
			{
				singleValue = SqlInt64.Null;
			}
		}

		[SqlFunction(DataAccess = DataAccessKind.None, FillRowMethodName = "FillRowCalculateCharacters", IsDeterministic = true, IsPrecise = true, TableDefinition = @"value nchar(1), count int")]
		public static IEnumerable CalculateCharacters(SqlString value)
		{
			if (value.IsNull)
			{
				return null;
			}

			Dictionary<Char, Int32> counters = new Dictionary<Char, Int32>();

			Char[] chars = value.Value.ToCharArray();

			return
				from ch in chars
				group ch by ch into grp
				select new Pair<Char, Int32>
					(
						grp.Key,
						grp.Count()
					);
		}

		private static void FillRowCalculateCharacters(Object obj, out SqlString character, out SqlInt32 count)
		{
			Pair<Char, Int32> tempValue = (Pair<Char, Int32>)obj;

			character = new SqlString(new String(tempValue.A, 1));

			count = tempValue.B;
		}
	}
}
