using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Sql;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace SqlToolset
{
	[Serializable]
	[SqlUserDefinedAggregate(Format.UserDefined,
	   MaxByteSize = Concatenate.MaxByteSize, 
		IsInvariantToNulls=true,
		IsInvariantToOrder=false,
		IsInvariantToDuplicates=false,
		IsNullIfEmpty=true)]
	public sealed class Concatenate : IBinarySerialize
	{
		public const Int32 MaxByteSize = 8000;

		private StringBuilder sb;

		public void Init()
		{
			sb = new StringBuilder();
		}

		private const Char Separator = ';';

		public void Accumulate(SqlString value)
		{
			if (value.IsNull)
			{
				return;
			}

			String s = value.ToString();

			sb.Append(s).Append(Separator);
		}

		public void Merge(Concatenate other)
		{
			if (null == other)
			{
				throw new ArgumentNullException("other");
			}

			if (null == other.sb)
			{
				throw new ArgumentNullException("other.sb");
			}

			String s = other.sb.ToString();

			sb.Append(s).Append(Separator);
		}

		public SqlString Terminate()
		{
			String result = sb.ToString();

			if (result.Length <= 0)
			{
				return SqlString.Null;
			}
			else
			{
				result = result.Substring(0, result.Length - 1);

				sb.Length = 0;

				return (SqlString)result;
			}
		}

		#region IBinarySerialize Members

		public void Read(System.IO.BinaryReader r)
		{
			if (null == r)
			{
				throw new ArgumentNullException("reader");
			}

			sb = new StringBuilder();

			sb.Append(r.ReadString());
		}

		public void Write(System.IO.BinaryWriter w)
		{
			if (null == w)
			{
				throw new ArgumentNullException("writer");
			}

			w.Write(sb.ToString());
		}

		#endregion
	}
}
