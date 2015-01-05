using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;
using System.IO;
using System.Security.AccessControl;
using System.Runtime.InteropServices;

namespace Skra.Sql.SqlToolset
{
	internal static class InternalTools
	{
		private const Int32 BytesCount = 256;

		static InternalTools()
		{
			BytesConverter2 = new Char[BytesCount][];

			StringBuilder sb = new StringBuilder();

			for (Int32 q = 0; q < BytesConverter2.Length; q++)
			{
				Byte b = (Byte)q;

				Byte hi = (Byte)((b & 0xF0) >> 4);

				Byte lo = (Byte)(b & 0x0F);

				Char[] chars = new Char[2];

				if (hi < 10)
				{
					chars[0] = (Char)((Byte)'0' + hi);
				}
				else
				{
					chars[0] = (Char)((Byte)'A' + hi - 10);
				}

				if (lo < 10)
				{
					chars[1] = (Char)((Byte)'0' + lo);
				}
				else
				{
					chars[1] = (Char)((Byte)'A' + lo - 10);
				}

				BytesConverter2[q] = chars;

				sb.Length = 0;
			}
		}

		private static readonly Char[][] BytesConverter2;

		internal static String GetEveryoneName()
		{
			SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

			NTAccount ntAccount = (NTAccount)sid.Translate(typeof(NTAccount));

			return ntAccount.ToString();
		}

		internal static T ApplyLimits<T>(T value, T minValue, T maxValue) where T : IComparable
		{
			if (maxValue.CompareTo(minValue) < 0)
			{
				throw new ArgumentOutOfRangeException();
			}

			T result = value;

			if (result.CompareTo(minValue) < 0)
			{
				result = minValue;
			}

			if (result.CompareTo(maxValue) > 0)
			{
				result = maxValue;
			}

			return result;
		}

		/// <summary>
		/// converts bytes array to hexadecimal string
		/// </summary>
		/// <param name="dataBuffer"></param>
		/// <returns></returns>
		internal static String HexBufferToString(Byte[] dataBuffer)
		{
			if (null == dataBuffer)
			{
				return null;
			}

			Char[] chars = new Char[dataBuffer.Length * 2];

			for (Int32 q = 0; q < dataBuffer.Length; q++)
			{
				Byte b = dataBuffer[q];

				Char[] chs = BytesConverter2[b];

				chars[q * 2] = chs[0];

				chars[q * 2 + 1] = chs[1];
			}

			return new String(chars);
		}

		/// <summary>
		/// serializes structure to bytes array
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="input"></param>
		/// <returns></returns>
		internal static Byte[] GetStructureBytes<T>(T input) where T : struct
		{
			Int32 size = Marshal.SizeOf(input);

			Byte[] arr = new Byte[size];

			IntPtr ptr = Marshal.AllocHGlobal(size);

			if (ptr == IntPtr.Zero)
			{
				throw new ApplicationException("Marshalling error - could not convert structure");
			}

			try
			{
				Marshal.StructureToPtr(input, ptr, true);

				Marshal.Copy(ptr, arr, 0, size);
			}
			finally
			{
				Marshal.FreeHGlobal(ptr);
			}

			return arr;
		}
	}
}
