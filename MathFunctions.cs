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
using System.Text;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;
using System.Collections;
using System.Data.SqlClient;

namespace SqlToolset
{
    public class MathFunctions
	{
		private const Byte Int64SizeBits = sizeof(Int64) * 8;

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlBinary RotateLeftBinary(SqlBinary input, SqlInt32 bits)
		{
			if (input.IsNull)
			{
				return SqlBinary.Null;
			}

			if (input.Value.Length > BitsShifter.MaxAllowedBinarySize)
			{
				throw new ArgumentOutOfRangeException("Big varbinary objects are not supported");
			}

			if (bits.IsNull)
			{
				return SqlBinary.Null;
			}

			return BitsShifter.RotateBitsLeft(input.Value, bits.Value);
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlBinary RotateRightBinary(SqlBinary input, SqlInt32 bits)
		{
			if (input.IsNull)
			{
				return SqlBinary.Null;
			}

			if (input.Value.Length > BitsShifter.MaxAllowedBinarySize)
			{
				throw new ArgumentOutOfRangeException("Big varbinary objects are not supported");
			}

			if (bits.IsNull)
			{
				return SqlBinary.Null;
			}

			return BitsShifter.RotateBitsRight(input.Value, bits.Value);
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlBinary ShiftLeftBinary(SqlBinary input, SqlInt32 bits)
		{
			if (input.IsNull)
			{
				return SqlBinary.Null;
			}

			if (input.Value.Length > BitsShifter.MaxAllowedBinarySize)
			{
				throw new ArgumentOutOfRangeException("Big varbinary objects are not supported");
			}

			if (bits.IsNull)
			{
				return SqlBinary.Null;
			}

			return BitsShifter.ShiftBitsLeft(input.Value, bits.Value);
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlBinary ShiftRightBinary(SqlBinary input, SqlInt32 bits)
		{
			if (input.IsNull)
			{
				return SqlBinary.Null;
			}

			if (input.Value.Length > BitsShifter.MaxAllowedBinarySize)
			{
				throw new ArgumentOutOfRangeException("Big varbinary objects are not supported");
			}

			if (bits.IsNull)
			{
				return SqlBinary.Null;
			}

			return BitsShifter.ShiftBitsRight(input.Value, bits.Value);
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlInt64 ShiftLeftBigint(SqlInt64 input, SqlByte bits)
		{
			if (input.IsNull)
			{
				return SqlInt64.Null;
			}

			if (bits.IsNull)
			{
				return SqlInt64.Null;
			}

			Byte bitsValue = bits.Value;

			if (bitsValue >= Int64SizeBits)
			{
				return 0;
			}

			return input.Value << bitsValue;
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlInt64 ShiftRightBigint(SqlInt64 input, SqlByte bits)
		{
			if (input.IsNull)
			{
				return SqlInt64.Null;
			}

			if (bits.IsNull)
			{
				return SqlInt64.Null;
			}

			Byte bitsValue = bits.Value;

			if (bitsValue >= Int64SizeBits)
			{
				return 0;
			}

			return input.Value >> bitsValue;
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlInt64 MaxCommonDividor(SqlInt64 _a, SqlInt64 _b)
        {
			if (_a.IsNull || _b.IsNull)
			{
				return SqlInt64.Null;
			}

			Int64 a = _a.Value;

			Int64 b = _b.Value;

            if (a == 0 || b == 0)
            {
                throw new DivideByZeroException();
            }

            if (a < 0 || b < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (a == b)
            {
                return a;
            }

            if (a < b)
            {
                Int64 c = a;

                a = b;

                b = c;
            }

            Int64 temp = a % b;

            if (0 == temp)
            {
				return new SqlInt64(b);
            }

            return MaxCommonDividor(b, temp);
		}

		private const Int32 MinBaseValue = 2;

		[SqlFunction(DataAccess = DataAccessKind.None, FillRowMethodName = "FillRowDigits", IsDeterministic = true, IsPrecise = true, TableDefinition = @"digit bigint, multiplier bigint")]
		public static IEnumerable Digits(SqlInt64 value, SqlInt16 baseValue)
		{
			if (value.IsNull || baseValue.IsNull)
			{
				yield break;
			}

			Int64 numValue = value.Value;

			Int16 numBaseValue = baseValue.Value;

			if (numBaseValue < MinBaseValue)
			{
				throw new ArgumentOutOfRangeException("Base value cannot be lower than " + MinBaseValue);
			}

			Int64 currentMultiplier = 1;

			if (numValue != 0)
			{
				while (numValue != 0)
				{
					yield return new Pair<Int64, Int64>(numValue % numBaseValue, currentMultiplier);

					currentMultiplier *= numBaseValue;

					numValue /= numBaseValue;
				}
			}
			else
			{
				yield return new Pair<Int64, Int64>(0, 1);
			}
		}

		private static void FillRowDigits(Object obj, out SqlInt64 value, out SqlInt64 multiplier)
		{
			Pair<Int64, Int64> tempValue = (Pair<Int64, Int64>)obj;

			value = new SqlInt64(tempValue.A);

			multiplier = new SqlInt64(tempValue.B);
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlInt64 ApplyLimitsInt(SqlInt64 input, SqlInt64 lowerLimit, SqlInt64 upperLimit)
		{
			if (input.IsNull)
			{
				return SqlInt64.Null;
			}

			if (lowerLimit.IsNull)
			{
				lowerLimit = SqlInt64.MinValue;
			}

			if (upperLimit.IsNull)
			{
				upperLimit = SqlInt64.MaxValue;
			}

			return InternalTools.ApplyLimits(input, lowerLimit, upperLimit);
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlByte ApplyLimitsByte(SqlByte input, SqlByte lowerLimit, SqlByte upperLimit)
		{
			if (input.IsNull)
			{
				return SqlByte.Null;
			}

			if (lowerLimit.IsNull)
			{
				lowerLimit = SqlByte.MinValue;
			}

			if (upperLimit.IsNull)
			{
				upperLimit = SqlByte.MaxValue;
			}

			return InternalTools.ApplyLimits(input, lowerLimit, upperLimit);
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = false, IsDeterministic = true)]
		public static SqlDouble ApplyLimitsDouble(SqlDouble input, SqlDouble lowerLimit, SqlDouble upperLimit)
		{
			if (input.IsNull)
			{
				return SqlDouble.Null;
			}

			if (lowerLimit.IsNull)
			{
				lowerLimit = SqlDouble.MinValue;
			}

			if (upperLimit.IsNull)
			{
				upperLimit = SqlDouble.MaxValue;
			}

			return InternalTools.ApplyLimits(input, lowerLimit, upperLimit);
		}
	}
}
