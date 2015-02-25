using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SqlToolset.Tests
{
	[TestClass]
	public class BitsShifterTests
	{
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestNull()
		{
			BitsShifter.ShiftBitsLeft(null, 1);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void TestBigger()
		{
			BitsShifter.ShiftBitsLeft(new Byte[BitsShifter.MaxAllowedBinarySize + 1], 1);
		}

		[TestMethod]
		public void TestBig()
		{
			BitsShifter.ShiftBitsLeft(new Byte[BitsShifter.MaxAllowedBinarySize],01);
		}

		private static void SingleSampleDataTest(Byte[] input, Byte[] expected, Byte bits)
		{
			Byte[] res = BitsShifter.ShiftBitsLeft(input, bits);

			CollectionAssert.AreEqual(expected, res);
		}

		[TestMethod]
		public void TestSampleBytes()
		{
			SingleSampleDataTest(new Byte[] { 0x00, 0x01 }, new Byte[] { 0x00, 0x02 }, 1);

			SingleSampleDataTest(new Byte[] { 0x00, 0x01 }, new Byte[] { 0x00, 0x04 }, 2);

			SingleSampleDataTest(new Byte[] { 0x00, 0x01 }, new Byte[] { 0x00, 0x08 }, 3);

			SingleSampleDataTest(new Byte[] { 0x00, 0x01 }, new Byte[] { 0x02, 0x00 }, 9);

			SingleSampleDataTest(new Byte[] { 0x00, 0x01 }, new Byte[] { 0x08, 0x00 }, 11);
		}
	}
}
