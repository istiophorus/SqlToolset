﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skra.Sql.SqlToolset
{
	/// <summary>
	/// Levenshtein algorithm implementation taken from 
	/// http://www.dotnetperls.com/levenshtein
	/// </summary>
	internal static class LevenshteinDistance
	{
		public static Int32 Compute(String s, String t)
		{
			if (null == s)
			{
				throw new ArgumentNullException("s");
			}

			if (null == t)
			{
				throw new ArgumentNullException("t");
			}

			Int32 n = s.Length;
			Int32 m = t.Length;
			Int32[,] d = new Int32[n + 1, m + 1];

			// Step 1
			if (n == 0)
			{
				return m;
			}

			if (m == 0)
			{
				return n;
			}

			// Step 2
			for (Int32 i = 0; i <= n; d[i, 0] = i++)
			{
			}

			for (Int32 j = 0; j <= m; d[0, j] = j++)
			{
			}

			// Step 3
			for (Int32 i = 1; i <= n; i++)
			{
				//Step 4
				for (Int32 j = 1; j <= m; j++)
				{
					// Step 5
					Int32 cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

					// Step 6
					d[i, j] = Math.Min(
						Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
						d[i - 1, j - 1] + cost);
				}
			}
		
			// Step 7
			return d[n, m];
		}
	}
}
