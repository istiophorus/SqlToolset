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
using Microsoft.SqlServer.Server;

namespace SqlToolset
{
	public sealed class DateTimeOperations
	{
		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlDateTime ApplyLimitsDateTime(SqlDateTime input, SqlDateTime lowerLimit, SqlDateTime upperLimit)
		{
			if (input.IsNull)
			{
				return SqlDateTime.Null;
			}

			if (lowerLimit.IsNull)
			{
				lowerLimit = SqlDateTime.MinValue;
			}

			if (upperLimit.IsNull)
			{
				upperLimit = SqlDateTime.MaxValue;
			}

			return InternalTools.ApplyLimits(input, lowerLimit, upperLimit);
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlDateTime MinDT3(SqlDateTime inputA, SqlDateTime inputB, SqlDateTime inputC)
		{
			return InternalTools.GetMinValue3(inputA, inputB, inputC, SqlDateTime.Null);
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlDateTime MaxDT3(SqlDateTime inputA, SqlDateTime inputB, SqlDateTime inputC)
		{
			return InternalTools.GetMaxValue3(inputA, inputB, inputC, SqlDateTime.Null);
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlDateTime MinDT2(SqlDateTime inputA, SqlDateTime inputB)
		{
			return InternalTools.GetMinValue(inputA, inputB, SqlDateTime.Null);
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlDateTime CreateDateExt(SqlInt16 year, SqlByte month, SqlByte day)
		{
			if (year.IsNull || month.IsNull || day.IsNull)
			{
				return SqlDateTime.Null;
			}

			try
			{
				DateTime dt = new DateTime(year.Value, 1, 1);

				dt = dt.AddMonths(month.Value - 1);

				dt = dt.AddDays(day.Value - 1);

				SqlDateTime result = new SqlDateTime(dt);

				return result;
			}
			catch (Exception)
			{
				return SqlDateTime.Null;
			}
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlDateTime CreateDate(SqlInt16 year, SqlByte month, SqlByte day)
		{
			if (year.IsNull || month.IsNull || day.IsNull)
			{
				return SqlDateTime.Null;
			}

			try
			{
				DateTime dt = new DateTime(year.Value, month.Value, day.Value);

				SqlDateTime result = new SqlDateTime(dt);

				return result;
			}
			catch (Exception )
			{
				return SqlDateTime.Null;
			}
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlDateTime MaxDT2(SqlDateTime inputA, SqlDateTime inputB)
		{
			return InternalTools.GetMaxValue(inputA, inputB, SqlDateTime.Null);
		}

		#region Days in month table valued function

		private struct MonthInfo
		{
			internal MonthInfo(Int32 month, Int32 days)
			{
				if (month < 1 || month > 12)
				{
					throw new ArgumentOutOfRangeException("month " + month.ToString());
				}

				if (days < 1 || days > 31)
				{
					throw new ArgumentOutOfRangeException("days " + days.ToString());
				}

				_month = month;
				_days = days;
			}

			private readonly Int32 _month;

			public Int32 Month
			{
				get { return _month; }
			}

			private readonly Int32 _days;

			public Int32 Days
			{
				get { return _days; }
			}
		}

		[SqlFunction(DataAccess = DataAccessKind.None, FillRowMethodName = "FillRowDaysInMonths", IsDeterministic = true, IsPrecise = true, TableDefinition = "Month int, Days int")]
		public static IEnumerable DaysInMonths(SqlInt32 year)
		{
			if (year.IsNull)
			{
				return null;
			}

			if (year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year - 1)
			{
				throw new ArgumentOutOfRangeException("year");
			}

			MonthInfo[] mi = new MonthInfo[12];

			for (Int32 q = 0; q < 12; q++)
			{
				Int32 month = q + 1;

				mi[q] = new MonthInfo(month, DateTime.DaysInMonth(year.Value, month));
			}

			return mi;
		}

		private static void FillRowDaysInMonths(Object obj, out SqlInt32 month, out SqlInt32 days)
		{
			MonthInfo mi = (MonthInfo)obj;

			month = mi.Month;
			days = mi.Days;
		}

		#endregion Days in month table valued function
	}
}
