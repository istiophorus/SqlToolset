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
using System.Data;
using System.Data.SqlTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SqlToolset.Tests
{
	[TestClass]
	public class BlobOperationsTests
	{
		[TestMethod]
		public void DataCompressionWithBothNull()
		{
			SqlBytes result = BlobOperations.DataCompression(SqlBytes.Null, SqlBoolean.Null);

			Assert.IsNotNull(result);

			Assert.IsTrue(result.IsNull, "Result should be Null when arguments are Null");
		}

		[TestMethod]
		public void DataCompressionWithNullReference()
		{
			SqlBytes result = BlobOperations.DataCompression(null, SqlBoolean.Null);

			Assert.IsNotNull(result);

			Assert.IsTrue(result.IsNull, "Result should be Null when arguments are Null");
		}

		[TestMethod]
		public void DataCompressionWithFirstNull()
		{
			SqlBytes result = BlobOperations.DataCompression(SqlBytes.Null, SqlBoolean.True);

			Assert.IsNotNull(result);

			Assert.IsTrue(result.IsNull, "Result should be null");
		}

		private const Int32 TestsCount = 64;

		[TestMethod]
		public void DataCompressionCompressDecompress()
		{
			for (Int32 q = 0; q < TestsCount; q++)
			{
				Byte[] bytes = RandomTools.GetBytes(128);

				SqlBytes compressed = BlobOperations.DataCompression(new SqlBytes(bytes), SqlBoolean.True);

				Assert.IsNotNull(compressed);

				Assert.IsFalse(compressed.IsNull);

				SqlBytes decompressed = BlobOperations.DataCompression(compressed, SqlBoolean.False);

				Assert.IsNotNull(decompressed);

				Assert.IsFalse(decompressed.IsNull);

				CollectionAssert.AreEqual(bytes, decompressed.Value);
			}
		}

		[TestMethod]
		public void DataCompressionCompress()
		{
			Byte[] bytes = new Byte[2048];

			SqlBytes compressed = BlobOperations.DataCompression(new SqlBytes(bytes), SqlBoolean.True);

			Assert.IsNotNull(compressed);

			Assert.IsFalse(compressed.IsNull);

			Assert.IsTrue(bytes.Length >= compressed.Value.Length);
		}

		[TestMethod]
		public void HexBufferToStringNullReference()
		{
			SqlString result = BlobOperations.HexBufferToString(null);

			Assert.IsTrue(result.IsNull);
		}

		[TestMethod]
		public void HexBufferToStringNullValue()
		{
			SqlString result = BlobOperations.HexBufferToString(SqlBytes.Null);

			Assert.IsTrue(result.IsNull);
		}

		[TestMethod]
		public void HexBufferToStringEmpty()
		{
			RawTest(new Byte[0], String.Empty);
		}

		[TestMethod]
		public void HexBufferToString01()
		{
			Byte[] arg = new Byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f };

			RawTest(arg, "000102030405060708090A0B0C0D0E0F");
		}

		[TestMethod]
		public void HexBufferToString02()
		{
			Byte[] arg = new Byte[] { 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF, 0x1A, 0x2B, 0x3B, 0x4B, 0x5B };

			RawTest(arg, "AABBCCDDEEFF1A2B3B4B5B");
		}

		private void RawTest(Byte[] input, String expectedOutput)
		{
			SqlString result = BlobOperations.HexBufferToString(new SqlBytes(input));

			Assert.IsFalse(result.IsNull);

			Assert.AreEqual(expectedOutput, result.Value);
		}

		[TestMethod]
		public void CalculateDataHashNullReference()
		{
			SqlBytes result = BlobOperations.CalculateDataHash(null, SqlString.Null);

			Assert.IsNotNull(result);

			Assert.IsTrue(result.IsNull);
		}

		[TestMethod]
		public void CalculateDataHashNullValue()
		{
			SqlBytes result = BlobOperations.CalculateDataHash(SqlBytes.Null, SqlString.Null);

			Assert.IsNotNull(result);

			Assert.IsTrue(result.IsNull);
		}
	}
}
