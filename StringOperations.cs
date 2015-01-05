using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Server;
using System.Linq;

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
