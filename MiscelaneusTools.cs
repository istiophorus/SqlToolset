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
using System.Collections.Generic;
using System.Text;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data.SqlClient;
using System.Transactions;
using System.Data;
using System.Diagnostics;
using System.Security.Permissions;
using System.Security;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.IO.Compression;
using System.Security.Principal;

namespace SqlToolset
{
	public class MiscelaneusTools
	{
		public const String ContextConnectionString = "context connection=true";

		[SqlProcedure()]
		public static void BasicPivot(SqlString commandText)
		{
			if (commandText.IsNull)
			{
				throw new ArgumentException("Command text is NULL");
			}

			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection connection = new SqlConnection(ContextConnectionString))
				{
					connection.Open();

					using (SqlCommand command = new SqlCommand(commandText.Value, connection))
					{
						command.CommandType = System.Data.CommandType.Text;

						Type baseColumnType = null;

						using (DataSet dataSet = new DataSet())
						{
							using (SqlDataAdapter adapter = new SqlDataAdapter(command))
							{
								adapter.Fill(dataSet);
							}

							DataTable dataTable = dataSet.Tables[0];

							Int32 rowsCount = dataTable.Rows.Count;

							if (rowsCount <= 0)
							{
								return;
							}

							Int32 columnsCount = dataTable.Columns.Count;

							if (columnsCount <= 0)
							{
								return;
							}

							SqlMetaData[] metaData = new SqlMetaData[rowsCount];

							Object baseValue = dataTable.Rows[0][0];

							for (Int32 q = 0; q < rowsCount; q++)
							{
								metaData[q] = SqlMetaData.InferFromValue(baseValue, "c" + q.ToString());
							}

							baseColumnType = baseValue.GetType();

							SqlDataRecord record = new SqlDataRecord(metaData);

							SqlContext.Pipe.SendResultsStart(record);

							try
							{

								for (Int32 column = 0; column < columnsCount; column++)
								{
									for (Int32 row = 0; row < rowsCount; row++)
									{
										Object currentValue = dataTable.Rows[row][column];

										if (!currentValue.GetType().Equals(baseColumnType))
										{
											throw new ArgumentException("Invalid cell type " + row.ToString() + " " + column.ToString());
										}

										record.SetValue(row, currentValue);
									}

									SqlContext.Pipe.SendResultsRow(record);
								}
							}
							finally
							{
								SqlContext.Pipe.SendResultsEnd();
							}
						}
					}
				}

				scope.Complete();
			}
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = false)]
		public static SqlString GetSystemUserNameFromBinary(SqlBinary sidBin, SqlBoolean nullOnError)
		{
			if (sidBin.IsNull)
			{
				return SqlString.Null;
			}

			try
			{
				SecurityIdentifier sid = new SecurityIdentifier(sidBin.Value, 0);

				NTAccount ntAccount = (NTAccount)sid.Translate(typeof(NTAccount));

				return new SqlString(ntAccount.Value);
			}
			catch
			{
				if (!nullOnError.IsNull && nullOnError.Value)
				{
					return SqlString.Null;
				}

				throw;
			}
		}

		[SqlFunction(DataAccess = DataAccessKind.None, IsPrecise = true, IsDeterministic = false)]
		public static SqlString GetSystemUserNameFromString(SqlString sidText)
		{
			if (sidText.IsNull)
			{
				return SqlString.Null;
			}

			SecurityIdentifier sid = new SecurityIdentifier(sidText.Value);

			NTAccount ntAccount = (NTAccount)sid.Translate(typeof(NTAccount));

			return new SqlString(ntAccount.Value);
		}
	}
}
