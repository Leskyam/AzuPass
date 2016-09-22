using System;

namespace TEICOCF.WebServices
{
	/// <summary>
	/// Esta es la clase para acceso a datos específicos de MySQL Server.
	/// </summary>
	public class DataMySQL : TEICOCF.WebServices.DataGen
	{
		#region "CONSTRUCTORES"

		public DataMySQL()
		{
			//
			// TODO: agregar aquí la lógica del constructor
			//
		}

		public DataMySQL(string ConnectionString)
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
					// My SQL Server como servidor de Base de datos.
					if(wsSettings.dbServerType.ToUpper()!=wsSettings.DataServerTypes.MySQL.ToString().ToUpper())
					{
                        throw new System.ArgumentException("Para utilizar esta clase es preciso que el valor de " + 
							"la llave 'dbServerType' en el fichero 'Web.config' se establezca a:'MySql'");
					}
					System.Text.StringBuilder mCadena = new System.Text.StringBuilder("");
					mCadena.Append("Server=" + wsSettings.dbServerName + ";");
					mCadena.Append("Port=" + wsSettings.dbServerTCPPort + ";");
					mCadena.Append("User id=" + wsSettings.dbUserName + ";");
					mCadena.Append("Password=" + wsSettings.dbUserPass + ";");
					mCadena.Append("Database=" + wsSettings.dbName + ";");
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

		/// <summary>
		/// Es necesario revissar este procedimiento para este tipo de servidor de base de datos.
		/// </summary>
		/// <param name="mCommand"></param>
		/// <param name="Args"></param>
		protected override void loadParameters(System.Data.IDbCommand mCommand, params Object[] Args)
		{
			int limit = mCommand.Parameters.Count;
			for(int i=0; i<(mCommand.Parameters.Count-1); i++)
			{
				MySql.Data.MySqlClient.MySqlParameter param = (MySql.Data.MySqlClient.MySqlParameter) mCommand.Parameters[i];
				if(i<=Args.Length)
				{
					param.Value=Args[i];
				}
				else
				{
					param=null;
				}
			}
		}

		protected override System.Data.IDbCommand getCommand(string storedProcedure)
		{
			MySql.Data.MySqlClient.MySqlCommand mCommand;
			if(CommandsCollection.Contains(storedProcedure))
			{
				mCommand = (MySql.Data.MySqlClient.MySqlCommand) CommandsCollection[storedProcedure];
			}
			else
			{
				MySql.Data.MySqlClient.MySqlConnection Conn = new MySql.Data.MySqlClient.MySqlConnection(this.ConnectionString);
				Conn.Open();
				mCommand = new MySql.Data.MySqlClient.MySqlCommand(storedProcedure,Conn);
				mCommand.CommandType = System.Data.CommandType.StoredProcedure;
				MySql.Data.MySqlClient.MySqlCommandBuilder.DeriveParameters(mCommand);
				Conn.Close();
				Conn.Dispose();
				CommandsCollection.Add(storedProcedure, mCommand);
			}
			mCommand.Connection = (MySql.Data.MySqlClient.MySqlConnection) this.Connection;
			return (System.Data.IDbCommand) mCommand;
		}

		protected override System.Data.IDbConnection getConnection(string strConnect)
		{
			return (System.Data.IDbConnection) new MySql.Data.MySqlClient.MySqlConnection(this.ConnectionString);
		}

		protected override System.Data.IDataAdapter getDataAdapter(string storedProcedure)
		{
			MySql.Data.MySqlClient.MySqlDataAdapter da = new MySql.Data.MySqlClient.MySqlDataAdapter((MySql.Data.MySqlClient.MySqlCommand) getCommand(storedProcedure));
			return (System.Data.IDbDataAdapter) da;
		}

		protected override System.Data.IDataAdapter getDataAdapter(string storedProcedure, params Object[] Args)
		{
			MySql.Data.MySqlClient.MySqlDataAdapter da = new MySql.Data.MySqlClient.MySqlDataAdapter((MySql.Data.MySqlClient.MySqlCommand) getCommand(storedProcedure));
			if(Args.Length!=0)
			{
				loadParameters(da.SelectCommand, Args);
			}
			return (System.Data.IDbDataAdapter) da;
		}
 

		#endregion "FIN DE PROCEDIMIENTOS PROTEGIDOS"

	}
}
