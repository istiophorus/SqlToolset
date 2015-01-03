using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using Microsoft.SqlServer.Server;

namespace Skra.Sql.SqlToolset
{
	public sealed class DateTimeOperations
	{
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
