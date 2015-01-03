using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Server;

namespace Skra.Sql.SqlToolset
{
	public sealed class StringOperations
	{
		[SqlFunction(DataAccess = DataAccessKind.None)]
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
	}
}
