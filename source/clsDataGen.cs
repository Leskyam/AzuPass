using System;

namespace TEICOCF.WebServices
{
	/// <summary>
	/// Esta es una clase abstracta para definiciones e implementaciones para acceso a datos, de esta heredanrán 
	/// las clases específicas de los motores de datos para los cuales se implemente cada una.
	/// </summary>
	public abstract class DataGen
	{
		#region "CONSTRUCTORES"

    /// <summary>
    /// Clase general para acceso a datos.
    /// </summary>
		public DataGen()
		{
			/*
			this.m_dbServerName = wsSettings.dbServerName;
			this.m_dbServerType = wsSettings.dbServerType;
			this.m_dbName = wsSettings.dbName;
			this.m_dbUserName = wsSettings.dbUserName;
			this.m_dbUserPass = wsSettings.dbUserPass;
			*/
		}

		#endregion "FIN DE CONSTRUCTORES"

		#region "VARIABLES PRIVADAS"

		protected string m_dbServerName = string.Empty;
		protected string m_dbServerType = string.Empty;
		protected string m_dbName = string.Empty;
		protected string m_dbUserName = string.Empty;
		protected string m_dbUserPass = string.Empty;
		
		protected string m_ConnectionString = string.Empty;
		protected System.Data.IDbConnection m_Connection = null;

		#endregion "FIN DE VARIABLES PRIVADAS"

		#region "PROCEDIMIENTOS PROPERTY"

		public abstract string ConnectionString
		{
			get;
			set;
		}

		protected System.Data.IDbConnection Connection
		{
			get
			{
				if(m_Connection==null)
				{
					m_Connection = getConnection(this.m_ConnectionString);
				}
				if(m_Connection.State!=System.Data.ConnectionState.Open)
				{
					m_Connection.Open();
				}
				return m_Connection;
			}
		}

		#endregion "FIN DE PROCEDIMIENTOS PROPERTY"

		#region "PROCEDIMIENTOS PROTEGIDOS"

		protected abstract System.Data.IDbConnection getConnection(string strConnect);
		protected abstract System.Data.IDbCommand getCommand(string storedProcedure);
		protected abstract System.Data.IDataAdapter getDataAdapter(string storedProcedure);
		protected abstract System.Data.IDataAdapter getDataAdapter(string storedProcedure, params System.Object[] Args);
		protected abstract void loadParameters(System.Data.IDbCommand mCommand, params System.Object[] Args);

		#endregion "FIN DE PROCEDIMIENTOS PROTEGIDOS"

		#region "PROCEDIMIENTOS PÚBLICOS"

		public System.Data.DataSet getDataSet(string storedProcedure)
		{
			System.Data.DataSet mDataSet = new System.Data.DataSet();
			this.getDataAdapter(storedProcedure).Fill(mDataSet);
			return mDataSet;
		}
		public System.Data.DataSet getDataSet(string storedProcedure, params System.Object[] Args)
		{
			System.Data.DataSet mDataSet = new System.Data.DataSet();
			this.getDataAdapter(storedProcedure, Args).Fill(mDataSet);
			return mDataSet;
		}
		public System.Data.DataTable getDataTable(string storedProcedure)
		{
			return this.getDataSet(storedProcedure).Tables[0].Copy();
		}
		public System.Data.DataTable getDataTable(string storedProcedure, params System.Object[] Args)
		{
			return this.getDataSet(storedProcedure, Args).Tables[0].Copy();
		}
		public System.Object getValue(string storedProcedure)
		{
			System.Data.IDbCommand mCommand = getCommand(storedProcedure);
			mCommand.ExecuteNonQuery();
			System.Object valor = null;
			foreach(System.Data.IDbDataParameter Param in mCommand.Parameters)
			{
				if(Param.Direction == System.Data.ParameterDirection.Output || 
					Param.Direction == System.Data.ParameterDirection.InputOutput)
				{
					valor = Param.Value;
				}
			}
			return valor;
		}

		public System.Object getValue(string storedProcedure, params System.Object[] Args)
		{
			System.Data.IDbCommand mCommand = getCommand(storedProcedure);
			loadParameters(mCommand, Args);
			mCommand.ExecuteNonQuery();
			System.Object valor = null;
			foreach(System.Data.IDbDataParameter Param in mCommand.Parameters)
			{
				if(Param.Direction==System.Data.ParameterDirection.Output ||
					Param.Direction==System.Data.ParameterDirection.InputOutput)
				{
					valor = Param.Value;
				}
			}
			return valor;
		}

		/// <summary>
		/// Devuelve el primer parámetro de tipo Output ó InputOutput que pertenece al comando ejecutado,
		/// en caso práctico debe ser el valor que toma Id del registro agregado.
		/// </summary>
		/// <param name="storedProcedure"></param>
		/// <returns></returns>
		public int Ejecutar(string storedProcedure)
		{
			System.Data.IDbCommand mCommand = getCommand(storedProcedure);
			int result = 0;
			mCommand.ExecuteNonQuery();
			foreach(System.Data.IDbDataParameter Param in mCommand.Parameters)
			{
				if(Param.Direction == System.Data.ParameterDirection.Output || 
					Param.Direction== System.Data.ParameterDirection.InputOutput)
						result = (int)Param.Value;
			}
			return result;
		}
		
		/// <summary>
		/// Devuelve el primer parámetro de tipo Output ó InputOutput que pertenece al comando ejecutado,
		/// en caso práctico debe ser el valor que toma Id del registro agregado.
		/// </summary>
		/// <param name="storedProcedure"></param>
		/// <param name="Args"></param>
		/// <returns></returns>
		public int Ejecutar(string storedProcedure, params System.Object[] Args)
		{
			System.Data.IDbCommand mCommand = getCommand(storedProcedure);
			loadParameters(mCommand, Args);

			int result = mCommand.ExecuteNonQuery();
			foreach(System.Data.IDbDataParameter Param in mCommand.Parameters)
			{
				if(Param.Direction == System.Data.ParameterDirection.Output || 
					Param.Direction== System.Data.ParameterDirection.InputOutput)
					try
					{
						result = (int)Param.Value;
					}
					catch(System.Exception)
					{
						return result;
					}
			}

			return result;

		}


		#endregion "FIN DE PROCEDIMIENTOS PÚBLICOS"

	}

}
