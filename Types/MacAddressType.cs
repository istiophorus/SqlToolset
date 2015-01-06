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
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Text.RegularExpressions;
using System.Globalization;

namespace SqlToolset.Types
{
	[SqlUserDefinedType(Format.UserDefined, MaxByteSize = MacAddressType.MacAddressSize)]
	public struct MacAddressType : INullable, IBinarySerialize
	{
		private const Int32 MacAddressSize = 6;

		private readonly Boolean _isNull;

		private Byte[] _macAddress;

		private String _toString;

		private const String MacAddressPatternA = "^(?<gxa>[0-9,A-F,a-f]{2})-(?<gxb>[0-9,A-F,a-f]{2})-(?<gxc>[0-9,A-F,a-f]{2})-(?<gxd>[0-9,A-F,a-f]{2})-(?<gxe>[0-9,A-F,a-f]{2})-(?<gxf>[0-9,A-F,a-f]{2})$";

		private const String MacAddressPatternB = "^(?<gxa>[0-9,A-F,a-f]{2})(?<gxb>[0-9,A-F,a-f]{2})(?<gxc>[0-9,A-F,a-f]{2})(?<gxd>[0-9,A-F,a-f]{2})(?<gxe>[0-9,A-F,a-f]{2})(?<gxf>[0-9,A-F,a-f]{2})$";

		public MacAddressType(Byte[] macAddress)
		{
			_isNull = (macAddress == null);

			_macAddress = macAddress;

			if (null != macAddress)
			{
				if (macAddress.Length != MacAddressSize)
				{
					throw new ArgumentException("Invalid MAC address size");
				}

				_toString = ConvertToString(_macAddress);
			}
			else
			{
				_toString = null;
			}
		}

		private static String ConvertToString(Byte[] bytes)
		{
			if (null == bytes)
			{
				return null;
			}

			StringBuilder sb = new StringBuilder();

			for (Int32 q = 0; q < bytes.Length; q++)
			{
				if (q > 0)
				{
					sb.Append("-");
				}

				sb.Append(bytes[q].ToString("x2"));
			}

			return sb.ToString().ToUpper();
		}

		public static MacAddressType Parse(SqlString s)
		{
			if (s.IsNull)
			{
				return MacAddressType.Null;
			}

			Match m = Regex.Match(s.Value, MacAddressPatternA);

			if (!m.Success)
			{
				m = Regex.Match(s.Value, MacAddressPatternB);

				if (!m.Success)
				{
					throw new ArgumentException("Invalid mac address format");
				}
			}

			Byte[] bytes = new Byte[MacAddressSize];

			bytes[0] = Byte.Parse(m.Groups["gxa"].Value, NumberStyles.HexNumber);
			bytes[1] = Byte.Parse(m.Groups["gxb"].Value, NumberStyles.HexNumber);
			bytes[2] = Byte.Parse(m.Groups["gxc"].Value, NumberStyles.HexNumber);
			bytes[3] = Byte.Parse(m.Groups["gxd"].Value, NumberStyles.HexNumber);
			bytes[4] = Byte.Parse(m.Groups["gxe"].Value, NumberStyles.HexNumber);
			bytes[5] = Byte.Parse(m.Groups["gxf"].Value, NumberStyles.HexNumber);

			return new MacAddressType(bytes);
		}

		public override String ToString()
		{
			return _toString;
		}

		private static readonly MacAddressType _null = new MacAddressType(null);

		public static MacAddressType Null
		{
			get
			{
				return _null;
			}
		}

		#region INullable Members

		public Boolean IsNull
		{
			get 
			{
				return _isNull;
			}
		}

		#endregion

		#region IBinarySerialize Members

		public void Read(System.IO.BinaryReader reader)
		{
			if (null == reader)
			{
				throw new ArgumentNullException("reader");
			}

			_macAddress = reader.ReadBytes(MacAddressSize);

			_toString = ConvertToString(_macAddress);
		}

		public void Write(System.IO.BinaryWriter writer)
		{
			if (null == writer)
			{
				throw new ArgumentNullException("writer");
			}

			writer.Write(_macAddress);
		}

		#endregion
	}
}
