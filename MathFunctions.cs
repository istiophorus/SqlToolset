using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;
using System.Collections;
using System.Data.SqlClient;

namespace SqlToolset
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

		private const Int32 MinBaseValue = 2;

		[SqlFunction(DataAccess = DataAccessKind.None, FillRowMethodName = "FillRowDigits", IsDeterministic = true, IsPrecise = true, TableDefinition = @"digit bigint, multiplier bigint")]
		public static IEnumerable Digits(SqlInt64 value, SqlInt16 baseValue)
		{
			if (value.IsNull || baseValue.IsNull)
			{
				yield break;
			}

			Int64 numValue = value.Value;

			Int16 numBaseValue = baseValue.Value;

			if (numBaseValue < MinBaseValue)
			{
				throw new ArgumentOutOfRangeException("Base value cannot be lower than " + MinBaseValue);
			}

			Int64 currentMultiplier = 1;

			if (numValue != 0)
			{
				while (numValue != 0)
				{
					yield return new Pair<Int64, Int64>(numValue % numBaseValue, currentMultiplier);

					currentMultiplier *= numBaseValue;

					numValue /= numBaseValue;
				}
			}
			else
			{
				yield return new Pair<Int64, Int64>(0, 1);
			}
		}

		private static void FillRowDigits(Object obj, out SqlInt64 value, out SqlInt64 multiplier)
		{
			Pair<Int64, Int64> tempValue = (Pair<Int64, Int64>)obj;

			value = new SqlInt64(tempValue.A);

			multiplier = new SqlInt64(tempValue.B);
		}
	}
}
