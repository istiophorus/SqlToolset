using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;
using System.Collections;
using System.Data.SqlClient;
using Skra.Sql.SqlToolset.Aggregations;

namespace Skra.Sql.SqlToolset
{
    public class MathFunctions
	{
		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlInt64 MaxCommonDividor(SqlInt64 _a, SqlInt64 _b)
        {
			if (_a.IsNull || _b.IsNull)
			{
				return SqlInt64.Null;
			}

			Int64 a = _a.Value;

			Int64 b = _b.Value;

            if (a == 0 || b == 0)
            {
                throw new DivideByZeroException();
            }

            if (a < 0 || b < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (a == b)
            {
                return a;
            }

            if (a < b)
            {
                Int64 c = a;

                a = b;

                b = c;
            }

            Int64 temp = a % b;

            if (0 == temp)
            {
				return new SqlInt64(b);
            }

            return MaxCommonDividor(b, temp);
		}

		#region Moving average

		private static readonly Object _cacheSynchro = new Object();

		private static readonly Dictionary<SqlGuid, IAggregation> _aggregationsCache = new Dictionary<SqlGuid, IAggregation>();

		//private static readonly Object o = new ConcurrentCollection();

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = false)]
		public static SqlDecimal MovingAverage(SqlGuid id, Int32 length, SqlDecimal value)
		{
			lock (_cacheSynchro)
			{
				IAggregation aggregation;

				if (_aggregationsCache.ContainsKey(id))
				{
					aggregation = _aggregationsCache[id];
				}
				else
				{
					aggregation = new MovingAverageData(length);

					_aggregationsCache.Add(id, aggregation);
				}

				aggregation.AddNextValue(value);

				return ((IAggregation<SqlDecimal>)aggregation).CurrentValue;
			}
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = false)]
		public static SqlDecimal EMA(SqlGuid id, Int32 length, SqlDecimal value)
		{
			lock (_cacheSynchro)
			{
				IAggregation aggregation;

				if (_aggregationsCache.ContainsKey(id))
				{
					aggregation = _aggregationsCache[id];
				}
				else
				{
					aggregation = new EMAData(length);

					_aggregationsCache.Add(id, aggregation);
				}

				aggregation.AddNextValue(value);

				return ((IAggregation<SqlDecimal>)aggregation).CurrentValue;
			}
		}

		[SqlProcedure()]
		public static void ClearCache(SqlGuid id)
		{
			lock (_cacheSynchro)
			{
				_aggregationsCache.Remove(id);
			}
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = false)]
		public static SqlDecimal RollingSum(SqlGuid id, SqlDecimal value)
		{
			lock (_cacheSynchro)
			{
				IAggregation aggregation;

				if (_aggregationsCache.ContainsKey(id))
				{
					aggregation = _aggregationsCache[id];
				}
				else
				{
					aggregation = new SumData();

					_aggregationsCache.Add(id, aggregation);
				}

				aggregation.AddNextValue(value);

				return ((IAggregation<SqlDecimal>)aggregation).CurrentValue;
			}
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = false)]
		public static Object PreviousValue(SqlGuid id, Object value)
		{
			lock (_cacheSynchro)
			{
				IAggregation aggregation;

				if (_aggregationsCache.ContainsKey(id))
				{
					aggregation = _aggregationsCache[id];
				}
				else
				{
					aggregation = new PreviousValueData();

					_aggregationsCache.Add(id, aggregation);
				}

				aggregation.AddNextValue(value);

				return aggregation.CurrentValue;
			}
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = false)]
		public static Object ValueBack(SqlGuid id, Int32 length, Object value)
		{
			lock (_cacheSynchro)
			{
				IAggregation aggregation;

				if (_aggregationsCache.ContainsKey(id))
				{
					aggregation = _aggregationsCache[id];
				}
				else
				{
					aggregation = new ValueBackData(length);

					_aggregationsCache.Add(id, aggregation);
				}

				aggregation.AddNextValue(value);

				return aggregation.CurrentValue;
			}
		}

		#endregion Moving average
	}
}
