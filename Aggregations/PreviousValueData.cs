using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlTypes;

namespace Skra.Sql.SqlToolset.Aggregations
{
	internal sealed class PreviousValueData : IAggregation
	{
		public PreviousValueData()
		{
			isReady = false;
		}

		private Object previousValue = null;

		private Object currentValue = null;

		private Int32 counter = 0;

		#region IAggregation Members

		public void AddNextValue(Object value)
		{
			previousValue = currentValue;

			currentValue = value;

			if (counter > 0)
			{
				isReady = true;
			}
			else
			{
				counter++;
			}
		}

		public Object CurrentValue
		{
			get
			{
				if (isReady)
				{
					return previousValue;
				}
				else
				{
					return null;
				}
			}
		}

		#endregion

		private Boolean isReady;

		public Boolean IsReady
		{
			get
			{
				return isReady;
			}
		}
	}
}
