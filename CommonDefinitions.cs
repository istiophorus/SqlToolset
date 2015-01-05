using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlTypes;

namespace SqlToolset
{
	internal static class CommonDefinitions
	{
		private static readonly SqlInt32 False = new SqlInt32(0);

		internal static SqlInt32 ResultFalse
		{
			get
			{
				return False;
			}
		}

		private static readonly SqlInt32 True = new SqlInt32(1);

		internal static SqlInt32 ResultTrue
		{
			get
			{
				return True;
			}
		}
	}
}
