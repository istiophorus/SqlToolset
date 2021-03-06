﻿#region License

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
		MaxByteSize = Xor.MaxByteSize,
		IsInvariantToNulls = true,
		IsInvariantToOrder = true,
		IsInvariantToDuplicates = false,
		IsNullIfEmpty = true)]
	public sealed class Xor : IBinarySerialize
	{
		private const Int32 MaxByteSize = sizeof(Int64) + sizeof(UInt64) + sizeof(Byte);

		private Int64 _currentSum;

		private UInt64 _items;

		public void Init()
		{
		}

		public void Accumulate(SqlInt64 value)
		{
			if (value.IsNull)
			{
				return;
			}

			Int64 currentValue = value.Value;

			checked
			{
				_items++;

				_currentSum ^= currentValue;
			}
		}

		public void Merge(Xor other)
		{
			if (null == other)
			{
				throw new ArgumentNullException("other");
			}

			checked
			{
				_currentSum ^= other._currentSum;

				_items += other._items;
			}
		}

		public SqlInt64 Terminate()
		{
			if (_items == 0)
			{
				return SqlInt64.Null;
			}

			return _currentSum;
		}

		#region IBinarySerialize Members

		public void Read(System.IO.BinaryReader reader)
		{
			if (null == reader)
			{
				throw new ArgumentNullException("reader");
			}

			Byte any = reader.ReadByte();

			if (any > 0)
			{
				_items = reader.ReadUInt64();

				_currentSum = reader.ReadInt64();
			}
		}

		public void Write(System.IO.BinaryWriter writer)
		{
			if (null == writer)
			{
				throw new ArgumentNullException("writer");
			}

			if (_items > 0)
			{
				writer.Write((Byte)1);

				writer.Write(_items);

				writer.Write(_currentSum);
			}
			else
			{
				writer.Write((Byte)0);
			}
		}

		#endregion
	}
}