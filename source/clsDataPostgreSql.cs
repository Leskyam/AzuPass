using System;

namespace TEICOCF.WebServices
{
	/// <summary>
	/// Esta es la clase para acceso a datos específicos de PostgreSql Server.
	/// </summary>
	public class DataPostgreSQL : TEICOCF.WebServices.DataGen
	{
		#region "CONSTRUCTORES"

		public DataPostgreSQL()
		{
			//
			// TODO: agregar aquí la lógica del constructor
			//
		}

		public DataPostgreSQL(string ConnectionString)
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
					// PostgreSQL Server como servidor de Base de datos.
					if(wsSettings.dbServerType.ToUpper()!=wsSettings.DataServerTypes.PostgreSQL.ToString().ToUpper())
					{
                        throw new System.ArgumentException("Para utilizar esta clase es preciso que el valor de " + 
							"la llave 'dbServerType' en el fichero 'Web.config' se establezca a:'PostgreSql'");
					}
					System.Text.StringBuilder mCadena = new System.Text.StringBuilder("");
					mCadena.Append("Server=" + wsSettings.dbServerName + ";");
					mCadena.Append("Port=" + wsSettings.dbServerTCPPort + ";");
					mCadena.Append("User id=" + wsSettings.dbUserName + ";");
					mCadena.Append("Password=" + wsSettings.dbUserPass + ";");
					mCadena.Append("Database=" + wsSettings.dbName + ";");
				}
				return m_ConnectionString.ToString();
			}
			set {this.m_ConnectionString = value;}
		}

		#endregion "FIN DE PROCEDIMIENTOS PROPERTY"

		#region "PROCEDIMIENTOS PROTEGIDOS"

		protected override void loadParameters(System.Data.IDbCommand mCommand, params Object[] Args)
		{
			int limit = mCommand.Parameters.Count;
			for(int i=1; i<mCommand.Parameters.Count; i++)
			{
				//Npgsql.NpgsqlParameter
				Npgsql.NpgsqlParameter param = (Npgsql.NpgsqlParameter) mCommand.Parameters[i];
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
			Npgsql.NpgsqlCommand mCommand;
			if(CommandsCollection.Contains(storedProcedure))
			{
				mCommand = (Npgsql.NpgsqlCommand) CommandsCollection[storedProcedure];
			}
			else
			{
				Npgsql.NpgsqlConnection Conn = new Npgsql.NpgsqlConnection(this.ConnectionString);
				Conn.Open();
				mCommand = new Npgsql.NpgsqlCommand(storedProcedure,Conn);
				mCommand.CommandType = System.Data.CommandType.StoredProcedure;
				Npgsql.NpgsqlCommandBuilder.DeriveParameters(mCommand);
				Conn.Close();
				Conn.Dispose();
				CommandsCollection.Add(storedProcedure, mCommand);
			}
			mCommand.Connection = (Npgsql.NpgsqlConnection) this.Connection;
			return (System.Data.IDbCommand) mCommand;
		}

		protected override System.Data.IDbConnection getConnection(string strConnect)
		{
			return (System.Data.IDbConnection) new Npgsql.NpgsqlConnection(this.ConnectionString);
		}

		protected override System.Data.IDataAdapter getDataAdapter(string storedProcedure)
		{
			Npgsql.NpgsqlDataAdapter da = new Npgsql.NpgsqlDataAdapter((Npgsql.NpgsqlCommand) getCommand(storedProcedure));
			return (System.Data.IDbDataAdapter) da;
		}

		protected override System.Data.IDataAdapter getDataAdapter(string storedProcedure, params Object[] Args)
		{
			Npgsql.NpgsqlDataAdapter da = new Npgsql.NpgsqlDataAdapter((Npgsql.NpgsqlCommand) getCommand(storedProcedure));
			if(Args.Length!=0)
			{
				loadParameters(da.SelectCommand, Args);
			}
			return (System.Data.IDbDataAdapter) da;
		}
 

		#endregion "FIN DE PROCEDIMIENTOS PROTEGIDOS"

	}
}
