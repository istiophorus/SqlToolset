using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlTypes;

namespace Skra.Sql.SqlToolset.Aggregations
{
	internal sealed class MovingAverageData : IAggregation<SqlDecimal>
	{
		public MovingAverageData(Int32 length)
		{
			if (length <= 0)
			{
				throw new ArgumentOutOfRangeException("length " + length.ToString());
			}

			this.length = length;

			this.currentSum = 0.0M;
		}

		private Queue<SqlDecimal> values = new Queue<SqlDecimal>();

		private SqlDecimal currentSum = 0.0M;

		private Int32 length = 1;

		public Int32 Length
		{
			get
			{
				return length;
			}
		}

		private SqlDecimal Zero = new SqlDecimal(0);

		public void AddNextValue(SqlDecimal outValue)
		{
			SqlDecimal value;

			if (outValue.IsNull)
			{
				////////////////////////////////////////////////////
				/// treat null as zero
				/// 

				value = Zero;
			}
			else
			{
				value = outValue;
			}

			values.Enqueue(value);

			currentSum += value;

			if (values.Count >= length)
			{
				isReady = true;

				while (values.Count > length)
				{
					currentSum -= values.Dequeue();
				}
			}
		}

		private Boolean isReady = false;

		public Boolean IsReady
		{
			get
			{
				return isReady;
			}
		}

		public SqlDecimal CurrentValue
		{
			get
			{
				if (isReady)
				{
					return currentSum / length;
				}
				else
				{
					return SqlDecimal.Null;
				}
			}
		}

		#region IAggregation Members

		public void AddNextValue(Object value)
		{
			AddNextValue((SqlDecimal)value);
		}

		Object IAggregation.CurrentValue
		{
			get 
			{
				return (SqlDecimal)CurrentValue;
			}
		}

		#endregion
	}
}
