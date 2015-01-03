using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlTypes;

namespace Skra.Sql.SqlToolset.Aggregations
{
	internal sealed class ValueBackData : IAggregation
	{
		public ValueBackData(Int32 length)
		{
			if (length < 1)
			{
				throw new ArgumentOutOfRangeException("length");
			}

			_isReady = false;

			_length = length;
		}

		private readonly Queue<Object> _values = new Queue<Object>();

		private Int32 _length;

		#region IAggregation Members

		public void AddNextValue(Object value)
		{
			_values.Enqueue(value);

			if (_values.Count > _length)
			{
				_isReady = true;

				while (_values.Count > _length + 1)
				{
					_values.Dequeue();
				}
			}
		}

		public Object CurrentValue
		{
			get
			{
				if (_isReady)
				{
					return _values.Peek();
				}
				else
				{
					return null;
				}
			}
		}

		#endregion

		private Boolean _isReady;

		public Boolean IsReady
		{
			get
			{
				return _isReady;
			}
		}
	}
}
