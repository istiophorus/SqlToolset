using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Meisui.Random;
using Microsoft.SqlServer.Server;

namespace SqlToolset
{
	public class Series
	{
		#region Create Series

		[SqlFunction(DataAccess = DataAccessKind.None, FillRowMethodName = "FillRowSeries", IsDeterministic = true, IsPrecise = true, TableDefinition = @"
			n int,
			value bigint
		")]
		public static IEnumerable CreateSeries(SqlInt32 _a, SqlInt32 _b, SqlInt32 _n1, SqlInt32 _n2)
		{
			Int64 a = _a.Value;

			Int64 b = _b.Value;

			Int32 n1 = _n1.Value;

			Int32 n2 = _n2.Value;

			if (n2 < n1)
			{
				throw new ArgumentException();
			}

			List<Pair<Int32, Int64>> values = new List<Pair<Int32, Int64>>();

			for (Int32 q = 0, x = n1; q < n2 - n1 + 1; q++, x++)
			{
				Pair<Int32, Int64> pair = new Pair<Int32, Int64>(x, a * (Int64)x + b);

				values.Add(pair);
			}

			return values.ToArray();
		}

		[SqlFunction(DataAccess = DataAccessKind.None, FillRowMethodName = "FillRowSeries", IsDeterministic = false, IsPrecise = true, TableDefinition = @"
			n int,
			value bigint")]
		public static IEnumerable CreateRandomIntSeries(SqlInt32 range, SqlInt32 count)
		{
			if (count.IsNull)
			{
				yield break;
			}

			Int32 counter = 0;

			MersenneTwister mt = new MersenneTwister((UInt32)Stopwatch.GetTimestamp());

			while (counter < count.Value)
			{
				Int64 result = mt.genrand_Int32();

				if (!range.IsNull)
				{
					result = result % range.Value;
				}

				Pair<Int32, Int64> pair = new Pair<Int32, Int64>(counter, result);

				yield return pair;

				counter++;
			}
		}

		private static void FillRowSeries(Object obj, out SqlInt32 n, out SqlInt64 value)
		{
			Pair<Int32, Int64> pair = (Pair<Int32, Int64>)obj;

			n = pair.A;
			value = pair.B;
		}

		#endregion Create Series
	}
}
