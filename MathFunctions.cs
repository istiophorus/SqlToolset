using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;
using System.Collections;
using System.Data.SqlClient;

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
	}
}
