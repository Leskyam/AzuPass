using System;

namespace TEICOCF.WebServices
{
	/// <summary>
	/// Esta es la clase para acceso a datos específicos de Microsoft SQL Server.
	/// </summary>
	public class DataMSSql : TEICOCF.WebServices.DataGen
	{
		#region "CONSTRUCTORES"

		public DataMSSql()
		{
			//
			// TODO: agregar aquí la lógica del constructor
			//
		}

		public DataMSSql(string ConnectionString)
		{
			this.m_ConnectionString = ConnectionString;
		}

		#endregion "FIN DE CONSTRUCTORES"

		#region "DECLARACIÓN DE VARIABLES"

		static System.Collections.Hashtable CommandsCollection = new System.Collections.Hashtable();

		#endregion "FIN DE DECLARACIÓN DE VARIABLES"

		#region "PROCEDIMIENTOS PROPERTY"

		public override string ConnectionString
		{
			get
			{
				if(this.m_ConnectionString.Length==0)
				{
					// Antes de formar la cadena de conexión revisar que esté configurado 
					// MS SQL Server como servidor de Base de datos.
					if(wsSettings.dbServerType.ToUpper()!=wsSettings.DataServerTypes.MSSql.ToString().ToUpper())
					{
						throw new System.ArgumentException("Para utilizar esta clase es preciso que el valor de " + 
							"la llave 'dbServerType' en el fichero 'Web.config' se establezca a:'MSSql'");
					}
					System.Text.StringBuilder mCadena = new System.Text.StringBuilder("");
					mCadena.Append("Server=" + wsSettings.dbServerName + "," + wsSettings.dbServerTCPPort + ";");
					mCadena.Append("Database=" + wsSettings.dbName + ";");
					mCadena.Append("User id=" + wsSettings.dbUserName + ";");
					mCadena.Append("Password=" + wsSettings.dbUserPass + ";");
					mCadena.Append("persist security info=" + wsSettings.dbPersistSegInfo + ";");
					mCadena.Append("Pooling=" + wsSettings.dbPooling + ";");
					mCadena.Append("Min Pool Size=" + wsSettings.dbMinPoolSize + ";");
					mCadena.Append("Max Pool Size=" + wsSettings.dbMaxPoolSize + ";");
					mCadena.Append("Application Name=" + wsSettings.ServiceShortName + ";");
					this.m_ConnectionString = mCadena.ToString();
				}
				return this.m_ConnectionString;
			}
			
			set 
			{
				this.m_ConnectionString = value;
			}
		}

		#endregion "FIN DE PROCEDIMIENTOS PROPERTY"

		#region "PROCEDIMIENTOS PROTEGIDOS"

		protected override void loadParameters(System.Data.IDbCommand mCommand, params Object[] Args)
		{
			int limit = mCommand.Parameters.Count;
			for(int i=1; i<mCommand.Parameters.Count; i++)
			{
				System.Data.SqlClient.SqlParameter param = (System.Data.SqlClient.SqlParameter) mCommand.Parameters[i];
				if(i<=Args.Length)
				{
					param.Value=Args[i-1];
				}
				else
				{
					param=null;
				}
			}
		}

		protected override System.Data.IDbCommand getCommand(string storedProcedure)
		{
			System.Data.SqlClient.SqlCommand mCommand;
			if(CommandsCollection.Contains(storedProcedure))
			{
				mCommand = (System.Data.SqlClient.SqlCommand) CommandsCollection[storedProcedure];
			}
			else
			{
				System.Data.SqlClient.SqlConnection Conn = new System.Data.SqlClient.SqlConnection(this.ConnectionString);
				Conn.Open();
				mCommand = new System.Data.SqlClient.SqlCommand(storedProcedure,Conn);
				mCommand.CommandType = System.Data.CommandType.StoredProcedure;
				System.Data.SqlClient.SqlCommandBuilder.DeriveParameters(mCommand);
				Conn.Close();
				Conn.Dispose();
                CommandsCollection.Add(storedProcedure, mCommand);
            }
			mCommand.Connection = (System.Data.SqlClient.SqlConnection) this.Connection;
			return (System.Data.IDbCommand) mCommand;
		}

		protected override System.Data.IDbConnection getConnection(string strConnect)
		{
			return (System.Data.IDbConnection) new System.Data.SqlClient.SqlConnection(this.ConnectionString);
		}

		protected override System.Data.IDataAdapter getDataAdapter(string storedProcedure)
		{
			System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter((System.Data.SqlClient.SqlCommand) getCommand(storedProcedure));
			return (System.Data.IDbDataAdapter) da;
		}

		protected override System.Data.IDataAdapter getDataAdapter(string storedProcedure, params Object[] Args)
		{
			System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter((System.Data.SqlClient.SqlCommand) getCommand(storedProcedure));
			if(Args.Length!=0)
			{
				loadParameters(da.SelectCommand, Args);
			}
			return (System.Data.IDbDataAdapter) da;
		}
 

		#endregion "FIN DE PROCEDIMIENTOS PROTEGIDOS"

	}
}
