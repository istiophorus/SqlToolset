using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlTypes;

namespace Skra.Sql.SqlToolset.Aggregations
{
	internal sealed class EMAData : IAggregation<SqlDecimal>
	{
		public EMAData(Int32 length)
		{
			if (length <= 0)
			{
				throw new ArgumentOutOfRangeException("length " + length.ToString());
			}

			_length = length;

			_currentValue = 0.0M;

			_kValue = 2.0M / (1.0M + length);

			this.oneMinusK = 1.0M - _kValue;
		}

		private SqlDecimal _currentValue = 0.0M;

		private readonly Int32 _length = 1;

		public Int32 Length
		{
			get
			{
				return _length;
			}
		}

		private static readonly SqlDecimal Zero = new SqlDecimal(0);

		private readonly Decimal _kValue;

		private Decimal oneMinusK;

		private Int64 currentLength = 0;

		public void AddNextValue(SqlDecimal outValue)
		{
			currentLength++;

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

			_currentValue = (value * _kValue) + _currentValue * oneMinusK;

			if (currentLength >= _length)
			{
				isReady = true;
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
					return _currentValue;
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

		object IAggregation.CurrentValue
		{
			get
			{
				return (SqlDecimal)CurrentValue;
			}
		}

		#endregion
	}
}
