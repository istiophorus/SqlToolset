using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlTypes;

namespace Skra.Sql.SqlToolset
{
	internal interface IAggregation
	{
		void AddNextValue(Object value);

		Boolean IsReady
		{
			get;
		}

		Object CurrentValue
		{
			get;
		}
	}

	internal interface IAggregation<T> : IAggregation
	{
		void AddNextValue(T value);

		new T CurrentValue
		{
			get;
		}
	}
}
