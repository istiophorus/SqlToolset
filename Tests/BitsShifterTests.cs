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

		private static void SingleShiftBitsLeftTest(Byte[] input, Byte[] expected, Byte bits)
		{
			Byte[] res = BitsShifter.ShiftBitsLeft(input, bits);

			CollectionAssert.AreEqual(expected, res);
		}

		private static void SingleShiftBitsRightTest(Byte[] input, Byte[] expected, Byte bits)
		{
			Byte[] res = BitsShifter.ShiftBitsRight(input, bits);

			CollectionAssert.AreEqual(expected, res);
		}

		[TestMethod]
		public void TestShiftBitsLeftSampleBytes()
		{
			SingleShiftBitsLeftTest(new Byte[] { 0x00, 0x01 }, new Byte[] { 0x00, 0x02 }, 1);

			SingleShiftBitsLeftTest(new Byte[] { 0x00, 0x01 }, new Byte[] { 0x00, 0x04 }, 2);

			SingleShiftBitsLeftTest(new Byte[] { 0x00, 0x01 }, new Byte[] { 0x00, 0x08 }, 3);

			SingleShiftBitsLeftTest(new Byte[] { 0x00, 0x01 }, new Byte[] { 0x02, 0x00 }, 9);

			SingleShiftBitsLeftTest(new Byte[] { 0x00, 0x01 }, new Byte[] { 0x08, 0x00 }, 11);
		}

		[TestMethod]
		public void TestShiftBitsRightSampleBytes()
		{
			SingleShiftBitsRightTest(new Byte[] { 0x00, 0x02 }, new Byte[] { 0x00, 0x01 }, 1);

			SingleShiftBitsRightTest(new Byte[] { 0x00, 0x04 }, new Byte[] { 0x00, 0x01 }, 2);

			SingleShiftBitsRightTest(new Byte[] { 0x00, 0x08 }, new Byte[] { 0x00, 0x01 }, 3);

			SingleShiftBitsRightTest(new Byte[] { 0x02, 0x00 }, new Byte[] { 0x00, 0x01 }, 9);

			SingleShiftBitsRightTest(new Byte[] { 0x08, 0x00 }, new Byte[] { 0x00, 0x01 }, 11);
		}
	}
}
