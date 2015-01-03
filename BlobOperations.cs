using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using Microsoft.SqlServer.Server;

namespace Skra.Sql.SqlToolset
{
	public sealed class BlobOperations
	{
		private const Int32 BufferSize = 8192;

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = true)]
		public static SqlBytes DataCompression(SqlBytes blob, SqlBoolean decompress)
		{
			if (blob.IsNull)
			{
				return SqlBytes.Null;
			}

			CompressionMode mode = CompressionMode.Compress;

			if (!decompress.IsNull)
			{
				if (decompress.Value == SqlBoolean.False)
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
		public static SqlBytes CalculateDataMD5(SqlBytes data)
		{
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
			if (data.IsNull)
			{
				return SqlBytes.Null;
			}

			SHA512Managed hash = new SHA512Managed();

			return new SqlBytes(hash.ComputeHash(data.Value));
		}
	}
}
