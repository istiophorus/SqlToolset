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
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Server;

namespace SqlToolset.Aggregations
{
	[Serializable]
	[SqlUserDefinedAggregate(Format.UserDefined,
		MaxByteSize = GeometricMean.MaxByteSize,
		IsInvariantToNulls = true,
		IsInvariantToOrder = true,
		IsInvariantToDuplicates = false,
		IsNullIfEmpty = true)]
	public sealed class GeometricMean : IBinarySerialize
	{
		private const Int32 MaxByteSize = sizeof(Double) + sizeof(UInt64) + sizeof(Byte);

		private Double _currentSum;

		private Boolean _resultIsZero;

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

			if (_resultIsZero)
			{
				return;
			}

			Double currentValue = value.Value;

			if (currentValue < 0.0)
			{
				throw new ArithmeticException("Unexpected negative value " + currentValue);
			}

			if (currentValue == 0.0)
			{
				_resultIsZero = true;

				return;
			}

			checked
			{
				_currentSum += Math.Log(currentValue);

				_items++;
			}
		}

		public void Merge(GeometricMean other)
		{
			if (null == other)
			{
				throw new ArgumentNullException("other");
			}

			if (other._resultIsZero)
			{
				_resultIsZero = true;
			}

			if (_resultIsZero)
			{
				return;
			}

			checked
			{
				_items += other._items;

				_currentSum += other._currentSum;
			}
		}

		public SqlDouble Terminate()
		{
			if (_resultIsZero)
			{
				return SqlDouble.Zero;
			}

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
				return SqlDouble.Null;
			}

			checked
			{
				Double result = Math.Exp(_currentSum / _items);

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

			_resultIsZero = reader.ReadBoolean();

			if (!_resultIsZero)
			{
				_items = reader.ReadUInt64();

				if (_items > 0)
				{
					_currentSum = reader.ReadDouble();
				}
			}
		}

		public void Write(System.IO.BinaryWriter writer)
		{
			if (null == writer)
			{
				throw new ArgumentNullException("writer");
			}

			writer.Write(_resultIsZero);

			if (!_resultIsZero)
			{
				writer.Write(_items);

				if (_items > 0)
				{
					writer.Write(_currentSum);
				}
			}
		}

		#endregion
	}
}
