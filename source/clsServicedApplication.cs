using System;

namespace TEICOCF.WebServices
{
	/// <summary>
	/// Descripción breve de clsServicedApplication.
	/// </summary>
	public class ServicedApplication : Entidad
	{
		
		#region "CONSTRUCTORES"

		/// <summary>
		/// Contructor. Carga el objeto como si se tratase del intento de agregar un nuevo 
		/// registro, con el valor de ID en cero(0). Pasa ala clase base "Entidad" el nombre
		/// de la tabla "tbl_RegistroUso", donde se almacenan los datos que maneja esta clase.
		/// </summary>
		public ServicedApplication() : base(EntityTableName)
	{
		this.ID = 0;
	}

	/// <summary>
	/// Contructor. Carga el objeto con los valores que se corresponden con el ID que se 
	/// pasa como valor del parámetro Id y si el valor del ID no existe en la base de datos
	/// entonces carga el objeto como si se tratase del intento de agregar un nuevo registro.
	/// Pasa ala clase base "Entidad" el nombre de la tabla "tbl_RegistroUso", donde se almacenan 
	/// los datos que maneja esta clase.
	/// </summary>
	/// <param name="Id"></param>
	public ServicedApplication(int Id) : base(EntityTableName)
{
	this.ID = Id;
}

	#endregion "FIN DE CONTRUCTORES"

		#region "ESTRUCTURAS"

		public enum enuApplicationState
		{
			Debug = 0,
            Release = 1
		}



		#endregion "FIN DE ESTRUCTURAS"

		#region "VARIABLES PRIVADAS"

		private static string EntityTableName = "tbl_ServicedApplication";

		private string m_appName;
		private string m_appURL;
		private bool m_urlChecked;
		private System.DateTime m_lastURLCheck;

		#endregion "FIN DE VARIABLES PRIVADAS"

		#region "PROCEDIMIENTOS PROPERTY"
		
		public string appName
		{
			get {return this.m_appName;}
			set 
			{
				this.m_appName = value;
				this.dsEntidad.Tables[0].Rows[0]["appName"] = value;
			}
		}

		public string appURL
		{
			get {return this.m_appURL;}
			set 
			{
				this.m_appURL = value;
				this.dsEntidad.Tables[0].Rows[0]["appURL"] = value;
			}
		}

		public bool urlChecked
		{
			get {return this.m_urlChecked;}
			set 
			{
				this.m_urlChecked = value;
				this.dsEntidad.Tables[0].Rows[0]["urlChecked"] = value;
			}
		}

		public System.DateTime lastURLCheck
		{
			get {return this.m_lastURLCheck;}
			set 
			{
				this.m_lastURLCheck = value;
				this.dsEntidad.Tables[0].Rows[0]["lastURLCheck"] = value;
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
				System.Object[] data = this.dsEntidad.Tables[0].Rows[0].ItemArray;
				this.m_ID = System.Convert.ToInt32(data.GetValue(0));
				this.m_appName = data.GetValue(1).ToString();
				this.m_appURL = data.GetValue(2).ToString(); 
				this.m_urlChecked = System.Convert.ToBoolean(data.GetValue(3));
				this.m_lastURLCheck = System.Convert.ToDateTime(data.GetValue(4));
			}
		}


		#endregion "FIN DE PROCEDIMIENTOS PRIVADOS"

		#region "PROCEDIMIENTOS PÚBLICOS"

		/// <summary>
		/// Chequea si es alcanzable la dirección URL almacenada en la variables provada "m_appURL"  
		/// </summary>
		/// <returns></returns>
		public bool isURLReachable()
		{
			try
			{
				string strResponse = string.Empty;
				string strMethod = "GET";
				System.Net.WebRequest WRequest;
				System.Net.WebResponse WResponse;
					
				System.Uri clientAppUri = new System.Uri(this.m_appURL);

				WRequest = System.Net.WebRequest.Create( clientAppUri );
				WRequest.Method = strMethod;

				// Determinar si se utiliza Proxy específico.
				System.Net.WebProxy myProxy = new System.Net.WebProxy();
				myProxy = (System.Net.WebProxy)WRequest.Proxy;
				if(wsSettings.InternetAccess.proxyAddress!=null)
				{
					myProxy = new System.Net.WebProxy(wsSettings.InternetAccess.proxyAddress);
					myProxy.BypassProxyOnLocal = true;
				}
				// Determinar si se configuraron credenciales para el proxy.
				System.Net.NetworkCredential proxyCredentials = new System.Net.NetworkCredential();
				if(wsSettings.InternetAccess.proxyUser.Length!=0 & wsSettings.InternetAccess.proxyPasswd.Length!=0)
				{
					proxyCredentials.UserName = wsSettings.InternetAccess.proxyUser;
					proxyCredentials.Password = wsSettings.InternetAccess.proxyPasswd;
					if(wsSettings.InternetAccess.NetBIOSDomain.Length!=0)
					{
						proxyCredentials.Domain = wsSettings.InternetAccess.NetBIOSDomain;
					}
				}
				else
				{
					proxyCredentials = (System.Net.NetworkCredential)System.Net.CredentialCache.DefaultCredentials;
				}
			  
				myProxy.Credentials = proxyCredentials;

				// Las credenciales por defecto son usualmente las credenciales de Windows 
				// (user name, contraseña, y dominio) de la cuenta bajo la que corre la aplicación.
				WRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;
				WRequest.Proxy = myProxy;
				WResponse = (System.Net.WebResponse)WRequest.GetResponse();
				return true;
			}
			catch(System.Exception Ex)
			{
				// Registrar Error aquí de manera detallada a través de la clase ProcessError.
				ProcessError processError = new ProcessError();
				processError.GuardarError(Ex, "ServicedApplication", "isURLReachable", this.m_appName, this.m_appURL, 0);
				processError = null;
				return false;
			}

		} // Fin de isURLReachable()

		public int Identificar(string clientAppName, System.Uri clientAppUri)
		{
			System.Object[] Args = {null, clientAppName, clientAppUri.AbsoluteUri};
			System.Object result = this.getValue("IDxNameUri", Args);

			if(result!=System.DBNull.Value)
			{
				return (int)result;
			}
			else
			{
				return 0;
			}
		}

		#endregion "FIN DE PROCEDIMIENTOS PÚBLICOS"

} // Fin de la clase.

} // Fin del namespace.
