using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Server;

namespace SqlToolset.Aggregations
{
	[Serializable]
	[SqlUserDefinedAggregate(Format.UserDefined,
		MaxByteSize = HarmonicMean.MaxByteSize,
		IsInvariantToNulls = true,
		IsInvariantToOrder = true,
		IsInvariantToDuplicates = false,
		IsNullIfEmpty = true)]
	public sealed class HarmonicMean : IBinarySerialize
	{
		private const Int32 MaxByteSize = sizeof(Double) + sizeof(Int64);

		private Double _currentSum;

		private UInt64 _items;

		public void Init()
		{
		}

		public void Accumulate(SqlDouble value)
		{
			if (value.IsNull)
			{
				return;
			}

			Double currentValue = value.Value;

			if (currentValue == 0.0)
			{
				throw new DivideByZeroException();
			}

			checked
			{
				_currentSum += 1.0 / currentValue;

				_items++;
			}
		}

		public void Merge(HarmonicMean other)
		{
			if (null == other)
			{
				throw new ArgumentNullException("other");
			}

			checked
			{
				_items += other._items;

				_currentSum += other._currentSum;
			}
		}

		public SqlDouble Terminate()
		{
			if (_items == 0)
			{
				return SqlDouble.Null;
			}

			if (Double.IsNaN(_currentSum))
			{
				return SqlDouble.Null;
			}

			if (Double.IsInfinity(_currentSum))
			{
				return SqlDouble.Zero;
			}

			checked
			{
				Double result = _items / _currentSum;

				if (Double.IsNaN(result))
				{
					return SqlDouble.Null;
				}

				if (Double.IsInfinity(result))
				{
					return SqlDouble.Null;
				}

				return result;
			}
		}

		#region IBinarySerialize Members

		public void Read(System.IO.BinaryReader reader)
		{
			if (null == reader)
			{
				throw new ArgumentNullException("reader");
			}

			_items = reader.ReadUInt64();

			if (_items > 0)
			{
				_currentSum = reader.ReadDouble();
			}
		}

		public void Write(System.IO.BinaryWriter writer)
		{
			if (null == writer)
			{
				throw new ArgumentNullException("writer");
			}

			writer.Write(_items);

			if (_items > 0)
			{
				writer.Write(_currentSum);
			}
		}

		#endregion
	}
}
