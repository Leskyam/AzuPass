using System;

namespace LAMSoft.WebServices
{
	/// <summary>
	/// Descripción breve de clsProcessError.
	/// </summary>
	public class ProcessError : Entidad
	{
		
		#region "CONSTRUCTORES"

		/// <summary>
		/// Contructor. Carga el objeto como si se tratase del intento de agregar un nuevo 
		/// registro, con el valor de ID en cero(0). Pasa ala clase base "Entidad" el nombre
		/// de la tabla "tbl_Perfil", donde se almacenan los datos que maneja esta clase.
		/// </summary>
		public ProcessError() : base(EntityTableName)
		{
			this.ID = 0;
		}
	
		/// <summary>
		/// Contructor. Carga el objeto con los valores que se corresponden con el ID que se 
		/// pasa como valor del parámetro Id y si el valor del ID no existe en la base de datos
		/// entonces carga el objeto como si se tratase del intento de agregar un nuevo registro.
		/// Pasa ala clase base "Entidad" el nombre de la tabla "tbl_Perfil", donde se almacenan 
		/// los datos que maneja esta clase.
		/// </summary>
		/// <param name="Id"></param>
		public ProcessError(int Id) : base(EntityTableName)
		{
			this.ID = Id;
		}
		
		#endregion "FIN DE CONSTRUCTORES"

		#region "VARIABLES PRIVADAS"

		private static string EntityTableName = "tbl_error_log";
		private System.DateTime m_FechaHora;
		private string m_className;
		private string m_memberName;
		private string m_type;
		private string m_source;
		private string m_message;
		// Para la tabla "tbl_error_log_extra"
		private string m_clientAppName;
		private string m_clientAppURL;
		private int m_IdPerfil;

		#endregion "FIN DE VARIABLES PRIVADAS"

		#region "PROCEDIMIENTOS PROPERTY"

		public System.DateTime FechaHora
		{
			get {return this.m_FechaHora;}
			set {this.m_FechaHora = value;}
		}

		public string className
		{
			get {return this.m_className;}
			set {this.m_className = value;}
		}

		public string memberName
		{
			get {return this.m_memberName;}
			set {this.m_memberName = value;}
		}

		public string type
		{
			get {return this.m_type;}
			set {this.m_type = value;}
		}

		public string source
		{
			get {return this.m_source;}
			set {this.m_source = value;}
		}

		public string message
		{
			get {return this.m_message;}
			set {this.m_message = value;}
		}

		public string ErrorRecordedNotification
		{
			get 
			{
				return	"El mensaje anterior junto a otros datos de interés técnico han sido registrados " + 
					"para ser analizados por parte del personal de soporte para este servicio en caso " + 
					"que sea necesario. Rogamos disculpas por las molestias ocasionadas.";
			}
		}


		#endregion "FIN DE PROCEDIMIENTOS PROPERTY"

		#region "PROCEDIMIENTOS PRIVADOS"

		protected sealed override void ActualizarCampos()
		{
			/*
				Solo para cuando el dsEntidad (heredado de la clase abstracta Entidad) 
				contenga el registro del perfil actual.
			*/
			if(this.dsEntidad.Tables[0].Rows.Count==1)
			{
				//ID, FechaHora, className, memberName, type, source, message, clientAppName, clientAppURL, IdPerfil
				System.Object[] data = this.dsEntidad.Tables[0].Rows[0].ItemArray;
				this.m_ID = System.Convert.ToInt32(data.GetValue(0));
				this.m_FechaHora = System.Convert.ToDateTime(data.GetValue(1));
				this.m_className = data.GetValue(2).ToString(); 
				this.m_memberName = data.GetValue(3).ToString();
				this.m_type = data.GetValue(4).ToString();
				this.m_source = data.GetValue(5).ToString();
				this.m_message = data.GetValue(6).ToString();
			}
		}

		#endregion "FIN DE PROCEDIMIENTOS PRIVADOS"

		#region "PROCEDIMIENTOS PÚBLICOS"

		public static string getMessageToUser(System.Exception Ex)
		{
			string resultMessage = Ex.Message;

			switch(Ex.GetType().FullName)
			{
				case "System.Data.SqlClient.SqlException":
				case "MySql.Data.MySqlClient.MySqlException":
				case "Npgsql.NpgsqlException":
                    resultMessage = "Servicio de datos no disponible.";
					break;
			}

			return resultMessage;
		}

		// Revisar esto.
		public override int Agregar()
		{
			/*
			int newID = base.Agregar ();
			// Agregar datos a la tabla "tbl_error_log_extra" si es pertinente.

			return newID;
			*/
			return 0;

		}

		public bool GuardarError(System.Exception Ex, string className, string Member)
		{
			try
			{
				System.Object[] data = {null, System.DateTime.Now, className, Member, Ex.GetType().FullName, Ex.Source, Ex.Message};
				this.dsEntidad.Tables[0].Rows[0].ItemArray = data;
				this.m_ID = base.Agregar();
				return true;
			}
			catch(System.Exception)
			{
				return false;
			}
		}

		public bool GuardarError(System.Exception Ex, string className, string Member, string appName, string appURL, int IdPerfil)
		{
			if(this.GuardarError(Ex, className, Member))
			{
				try
				{
					System.Object[] data = {this.m_ID, appName, appURL, IdPerfil};
					this.daCom().Ejecutar("pa_tbl_error_log_extra_I", data);
					this.m_clientAppName = appName;
					this.m_clientAppURL = appURL;
					this.m_IdPerfil = IdPerfil;
					return true;
				}
				catch(System.Exception)
				{
					return false;
				}
			}
			return false;
		}

		#endregion "FIN DE PROCEDIMIENTOS PÚBLICOS"


	} // Fin de la clase.

} // Fin del namespace.
