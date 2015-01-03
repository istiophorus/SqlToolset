using System;
using System.Collections.Generic;
using System.Text;

namespace Skra.Sql.SqlToolset
{
	internal sealed class Pair<T, V>
	{
		public Pair(T t, V v)
		{
			_a = t;
			_b = v;
		}

		private readonly T _a;

		public T A
		{
			get { return _a; }
		}

		private readonly V _b;

		public V B
		{
			get { return _b; }
		}
	}
}
