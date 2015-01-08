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
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using Microsoft.SqlServer.Server;

namespace SqlToolset
{
	public sealed class BlobOperations
	{
		private const Int32 BufferSize = 8192;

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlBytes DataCompression(SqlBytes blob, SqlBoolean compress)
		{
			if (null == blob)
			{
				return SqlBytes.Null;
			}

			if (blob.IsNull)
			{
				return SqlBytes.Null;
			}

			CompressionMode mode = CompressionMode.Compress;

			if (!compress.IsNull)
			{
				if (compress.Value == SqlBoolean.False)
				{
					mode = CompressionMode.Decompress;
				}
			}

			Byte[] bytes = blob.Value;

			if (mode == CompressionMode.Compress)
			{
				using (MemoryStream output = new MemoryStream())
				{
					using (GZipStream zipStream = new GZipStream(output, mode))
					{
						zipStream.Write(bytes, 0, bytes.Length);
					}

					return new SqlBytes(output.ToArray());
				}
			}
			else
			{
				using (MemoryStream input = new MemoryStream(bytes))
				{
					using (MemoryStream output = new MemoryStream())
					{
						using (GZipStream zipStream = new GZipStream(input, mode))
						{
							Byte[] buffer = new Byte[BufferSize];

							Int32 bytesRead = 0;

							while ((bytesRead = zipStream.Read(buffer, 0, buffer.Length)) > 0)
							{
								output.Write(buffer, 0, bytesRead);
							}
						}

						return new SqlBytes(output.ToArray());
					}
				}
			}
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlString HexBufferToString(SqlBytes blob)
		{
			if (null == blob)
			{
				return SqlString.Null;
			}

			if (blob.IsNull)
			{
				return SqlString.Null;
			}

			return InternalTools.HexBufferToString(blob.Value);
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlBytes CalculateDataHash(SqlBytes data, SqlString hashName)
		{
			if (null == data)
			{
				return SqlBytes.Null;
			}

			if (data.IsNull)
			{
				return SqlBytes.Null;
			}

			if (hashName.IsNull)
			{
				return SqlBytes.Null;
			}

			HashAlgorithm hash = HashAlgorithm.Create(hashName.Value);

			return new SqlBytes(hash.ComputeHash(data.Value));
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlBytes CalculateDataMD5(SqlBytes data)
		{
			if (null == data)
			{
				return SqlBytes.Null;
			}

			if (data.IsNull)
			{
				return SqlBytes.Null;
			}

			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

			return new SqlBytes(md5.ComputeHash(data.Value));
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlBytes CalculateDataSHA(SqlBytes data)
		{
			if (null == data)
			{
				return SqlBytes.Null;
			}

			if (data.IsNull)
			{
				return SqlBytes.Null;
			}

			SHA512Managed hash = new SHA512Managed();

			return new SqlBytes(hash.ComputeHash(data.Value));
		}
	}
}
