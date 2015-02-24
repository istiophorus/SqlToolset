using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlToolset
{
	public static class EmptyArrayTemplate<T>
	{
		private static readonly T[] _instance = new T[0];

		public static T[] Instance
		{
			get
			{
				return _instance;
			}
		}
	}
}
