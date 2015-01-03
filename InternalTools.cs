using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;
using System.IO;
using System.Security.AccessControl;

namespace Skra.Sql.SqlToolset
{
	internal static class InternalTools
	{
		internal static String GetEveryoneName()
		{
			SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

			NTAccount ntAccount = (NTAccount)sid.Translate(typeof(NTAccount));

			return ntAccount.ToString();
		}
	}
}
