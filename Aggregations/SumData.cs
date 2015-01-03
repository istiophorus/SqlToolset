using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlTypes;

namespace Skra.Sql.SqlToolset
{
	sealed class SumData : IAggregation<SqlDecimal>
	{
		public SumData()
		{
			this.currentSum = 0.0M;

			isReady = true;
		}

		private SqlDecimal currentSum = 0.0M;

		public void AddNextValue(SqlDecimal value)
		{
			if (!value.IsNull)
			{
				currentSum += value;
			}
		}

		private Boolean isReady;

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
				return currentSum;
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
