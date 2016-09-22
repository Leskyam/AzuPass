using System.ServiceModel;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace TEICOCF.WebServices
{

  [System.Web.Script.Services.ScriptService()]
	/// <summary>
	/// AzuPass es un servicio Web XML que utiliza registros de usuarios en una base de datos personalizada 
	/// con el fin de proporcionar a sus "aplicaciones clientes" un mecanismo centralizado para la identificaci�n 
	/// de los mismos, para ello expondr� los m�todos necesarios a trav�s de los cuales estos clientes podr�n 
	/// implementar funciones b�sicas como: identificar usuarios, obtener lista de sitios o aplicaciones con sus 
	/// correspondientes Nombre y URL que utilicen este servicio, obtener la lista de los x sitios y/o aplicaciones 
	/// m�s utilizadas por el usuario que se identifica, etc. Los clientes especializados de este servicio, como 
	/// la aplicaci�n Administrador de perfiles AzuPass (AzuPassProfileMgr) tendr�n acceso a otros miembros que 
	/// les permitir�n realizar entre otras las siguientes operaciones: agregar nuevo perfil de usuario, modificar 
	/// datos del perfil del usuario, etc. Gracias al desarrollo de la aplicaci�n especializada (AzuPassProfileMgr)
	/// los desarrolladores de aplicaciones que empleen este servicio para identificaci�n de usuarios no tendr�n 
	/// que preocuparse por las implementaciones, en la aplicaci�n que desarrollen, de todos los mecanismos propios 
	/// para la manipulaci�n de los datos de los perfiles de los usuario, sino que podr�n remitir a los usuarios a 
	/// la URL de la aplicaci�n que lo hace (AzuPassProfileMgr), implementando solamente el m�todo para la validaci�n 
	/// de las credenciales que proporciona el usuario.
	/// </summary>
	[WebService(Name = "AzuPass", Namespace = "http://webservices.cf.minaz.cu/", Description = "Este es un servicio Web XML que utiliza " + 
	"registros de usuarios en una base de datos personalizada con el fin de proporcionar a sus \"aplicaciones clientes\" " +
	"un mecanismo centralizado para la identificaci�n de los mismos, para ello expondr� los m�todos necesarios a trav�s " + 
	"de los cuales estos clientes podr�n implementar funciones b�sicas como: identificar usuarios, obtener una lista de " + 
	"sitios o aplicaciones con sus correspondientes Nombre y URL que utilicen este servicio, obtener una lista de los " + 
	"sitios y/o aplicaciones m�s utilizadas por el usuario que se identifica, etc. Los clientes especializados de este " + 
	"servicio, como la aplicaci�n \"Administrador de perfiles AzuPass\" (AzuPassProfileMgr) tendr�n acceso a todos los " + 
	"miembros que las aplicaciones clientes, pero adem�s podr�n utilizar otros miembros tambi�n p�blicos, pero protegidos, " + 
	"que permiten realizar entre otras las siguientes operaciones: agregar nuevo perfil de usuario � modificar datos de "+
	"�ste, cambiar la contrase�a � la direcci�n electr�nica del perfil, etc. Gracias al desarrollo de la aplicaci�n " +
	"especializada (AzuPassProfileMgr) los desarrolladores de aplicaciones que empleen este servicio para identificaci�n " + 
	"de usuarios no tendr�n que preocuparse por las implementaciones, en la aplicaci�n que desarrollen, de todos los " + 
	"mecanismos propios para la manipulaci�n de los datos de los perfiles de los usuario, sino que podr�n remitir a los " + 
	"usuarios a la URL de la aplicaci�n que lo hace (AzuPassProfileMgr), implementando solamente alguno de los m�todos " +
	"existentes (\"_LogOn\" y \"_LogOnPlus\") para la validaci�n de las credenciales que proporciona el usuario.")]
	public class AzuPass : System.Web.Services.WebService
	{
		public AuthHeader wsAuthentication;

        #region "PROCEDIMIENTOS PRIVADOS"

		/// <summary>
		/// Chequear los datos que se pasan en el encabezado SOAP de algunos miembros para validaci�n de 
		/// credenciales de aplicaci�n cliente.
		/// </summary>
		private void checkSoapHeader()
		{
			string soapApp = "AzuPassProfileMgr";
			// Valor que debe pasar la aplicaci�n pero encriptado.
			string soapSecret = "ww0KaUl/mb";

			/*
			for(int i=0; i<=(HttpContext.Current.Application.Contents.AllKeys.Length-1); i++)
			{
				System.Diagnostics.Debug.WriteLine(HttpContext.Current.Application.Contents.AllKeys[i]);
			}
			*/

			if(wsAuthentication==null)
			{
        string errorMessage = "Este miembro es de tipo \"protegido\" y aunque sea " +
          "p�blico no puede ser ejecutado sin previa coordinaci�n con los desarrolladores " +
          "del servicio \"" + wsSettings.ServiceShortName + "\".";
        throw new SoapException(errorMessage, SoapException.ClientFaultCode);
			}

      if (wsAuthentication.AppCliente != soapApp || Crypto.Decrypt(wsAuthentication.Password) != soapSecret)
      {
        string errorMessage = "Las credenciales proporcionadas por la aplicaci�n cliente para la ejecuci�n " + 
          "de miembros privados del servicio " + wsSettings.ServiceShortName + " no son las esperadas.";
        throw new SoapException(errorMessage, SoapException.ClientFaultCode);
      }

		}


		/// <summary>
		/// Identifica el perfil, chequea si se requiere verificaci�n de la URL de la aplicaci�n cliente y ordena hacerlo 
		/// si es preciso, actualiza el registro de uso a nombre del usuario identificado.
		/// </summary>
		/// <param name="e_mail">email del perfil.</param>
		/// <param name="Passwd">contrase�a del perfil.</param>
		/// <param name="clientAppName">Nombre de la aplicaci�n cliente.</param>
		/// <param name="clientAppURL">URL que representa el punto de entrada de la aplicaci�n cliente.</param>
		/// <param name="appState">Estado de la aplicaci�n, "Debug" � "Release", cuando es debug no se 
		/// realizan algunas funciones y esto aligera la carga, permitiendo que el procedimientos se 
		/// ejecute con m�s rapidez.</param>
		/// <returns>Devolver� el ID del usuario identificado en caso de ser correctos los elementos 
		/// e_mail y Passwd, en caso contrario devolver� cero (0).</returns>
		private int LogOn(string e_mail, string Passwd, string clientAppName, string clientAppURL, ServicedApplication.enuApplicationState appState)
		{
			int IdPerfil = 0;
			try
			{
				Perfil perfil = new Perfil();
				IdPerfil = perfil.Identificar(e_mail, Passwd, true);
				if(IdPerfil==0)
				{
					perfil = null;
					return IdPerfil;
				}
				/* 
					Cuando se indica que la aplicaci�n est� a�n en modo de depuraci�n, 
					evitar entonces el registro de la misma ya que muchos elementos, 
					entre ellos la URL donde se hospeda, pueden variar.
				*/
				if(appState == ServicedApplication.enuApplicationState.Debug)
				{
					perfil = null;
					return IdPerfil;
				}

				// Verificar el formato y corregir la URL si es necesario y posible.
				clientAppURL = wsSettings.parseURL(clientAppURL, "minaz.cu");
				System.Uri appUri = new System.Uri(clientAppURL);

				ServicedApplication servicedApplication = new ServicedApplication();
				servicedApplication.ID = servicedApplication.Identificar(clientAppName, appUri);
				if(servicedApplication.ID==0)
				{
					System.Object[] ArgsServicedApp = {null, clientAppName, appUri.AbsoluteUri, false, System.DateTime.Now};
					appUri = null;
					servicedApplication.dsEntidad.Tables[0].Rows[0].ItemArray = ArgsServicedApp;
					servicedApplication.Agregar();
				}
				RegistroUso regUso = new RegistroUso(0);
				System.Object[] ArgsRegUso = {null, IdPerfil, servicedApplication.ID, System.DateTime.Now};
				regUso.dsEntidad.Tables[0].Rows[0].ItemArray = ArgsRegUso;
				regUso.Agregar();
				regUso = null;

				// Si se exige el chequeo de las URLs y la URL de la aplicaci�n en cuesti�n no est� chequeada �
				// si ya venci� el tiempo y debe volver a ser verificada, entonces verificarla.
				if((wsSettings.InternetAccess.CheckClientAppsURL & !servicedApplication.urlChecked) ||
					(wsSettings.InternetAccess.CheckClientAppsURL & (servicedApplication.lastURLCheck.AddMonths(wsSettings.InternetAccess.MonthBeforeReCheck) < System.DateTime.Now)))
				{
					servicedApplication.urlChecked = servicedApplication.isURLReachable();                        
					servicedApplication.lastURLCheck = System.DateTime.Now;
					// Revisar esto porque recuerda que hicistes un cambio en el procedimiento pa_tbl_ServicedApplication_U
					System.Diagnostics.Debug.WriteLine(servicedApplication.Actualizar());
				}
				
				servicedApplication = null;
				return IdPerfil;
			}
			catch(System.Exception Ex)
			{
				string errorMessage = ProcessError.getMessageToUser(Ex) + " " + System.Environment.NewLine;
				try
				{
					ProcessError processError = new ProcessError();
					if(processError.GuardarError(Ex, "AzuPass", "_LogOn", clientAppName, clientAppURL, IdPerfil))
					{
						errorMessage = errorMessage + processError.ErrorRecordedNotification;
					}
					processError = null;
				}
				catch(System.Exception)
				{
					; // No hacer nada si falla el mecanismo de guardar los errores. 
				}
        throw new SoapException(errorMessage, SoapException.ClientFaultCode);
      }
		
		} // Fin de LogOn

		
		/// <summary>
		/// Validar algunos de los datos que se pasan como valores para los campos de los perfiles de usuarios.
		/// </summary>
		/// <param name="Nombre"></param>
		/// <param name="Apellidos"></param>
		/// <param name="e_mail"></param>
		/// <param name="FechaNac"></param>
		/// <param name="IdCatOcupacional"></param>
		private void valUserData(string Nombre, string Apellidos, string e_mail, System.DateTime FechaNac, int IdCatOcupacional)
		{
			System.Text.StringBuilder errorMessage = new System.Text.StringBuilder("");
			// Nombre
			if (Nombre.Length == 0) { errorMessage.Append("'Nombre' es un elemento requerido." + System.Environment.NewLine); }
			// Apellidos
			if (Apellidos.Length == 0) { errorMessage.Append("'Apellidos' es un elemento requerido." + System.Environment.NewLine); }
			// Email
			if (!Perfil.isValidEmail(e_mail))
			{
				errorMessage.Append("'Direcci�n de correo' es un elemento requerido, debe tener un formato v�lido y pertenecer a un dominio permitido." + System.Environment.NewLine);
			}
			else
			{
				if (Perfil.existEmail(e_mail))
				{
					errorMessage.Append("La 'Direcci�n de correo' especificada ya se encuentra en uso." + System.Environment.NewLine);
				}
			}
			// Fecha de nacimiento
			if (!Perfil.valFechaNac(FechaNac))
			{
				errorMessage.Append("'Fecha de nacimiento' indica que usted no cumple con los requerimientos de edad para hacer uso del servicio 'AzuPass'." + System.Environment.NewLine);
			}
			CatOcupacional catOcupacional = new CatOcupacional();
			if (!catOcupacional.existByID(IdCatOcupacional))
			{
				errorMessage.Append("'Categor�a ocupacional' no es v�lida, contacte al responsable de la aplicaci�n." + System.Environment.NewLine);
			}
			catOcupacional = null;
			// Fin de las validaciones

			if (errorMessage.ToString() != string.Empty)
			{
        throw new SoapException(errorMessage.ToString(), SoapException.ClientFaultCode);
        //throw new System.Exception(errorMessage.ToString());
			}

		}


		#endregion "FIN DE PROCEDIMIENTOS PRIVADOS"

		#region "PROCEDIMIENTOS P�BLICOS"

		#region "MIEMBROS NO PROTEGIDOS POR ENCABEZADOS SOAP"

		/// <summary>
		/// Identifica el perfil, chequea si se requiere verificaci�n de la URL de la aplicaci�n cliente y ordena hacerlo 
		/// si es preciso, actualiza el registro de uso a nombre del usuario identificado.
		/// </summary>
		/// <param name="e_mail">email del perfil.</param>
		/// <param name="Passwd">contrase�a del perfil.</param>
		/// <param name="clientAppName">Nombre de la aplicaci�n cliente.</param>
		/// <param name="clientAppURL">URL que representa el punto de entrada de la aplicaci�n cliente.</param>
		/// <param name="appState">Estado de la aplicaci�n, "Debug" � "Release", cuando es debug no se 
		/// realizan algunas funciones y esto aligera la carga, permitiendo que el procedimientos se 
		/// ejecute con m�s rapidez.</param>
		/// <returns>Devolver� "true" en caso de ser identificado un perfil que corresponda con los par�metros 
		/// e_mail y Passwd, en caso contrario devolver� "false".</returns>
		/// <example>
		/// <code>
		/// TEICOCF.WebServices.AzuPass AzuPass = new TEICOCF.WebServices.AzuPass();
		/// if(AzuPass._LogOn(this.txtEmail.Text, this.txtPasswd.Text, 
		///		System.Configuration.ConfigurationSettings.AppSettings["AppName"], 
		///		HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.LocalPath, HttpContext.Current.Request.ApplicationPath))
		///	{
		///		// Crear y formar el Ticket
		///		FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
		///		1, // Version: Ticket version
		///		this.txtEmail.Text, // Name: Direcci�n de correo del usuario asociado con el ticket.
		///		DateTime.Now, // issueDate: Fecha/Hora del ticket.
		///		DateTime.Now.AddMinutes(HttpContext.Current.Session.Timeout), // expiration: Fecha/Hora a la que expira.
		///		false, // isPersistent: "true" para una cookie persistente de usuario.
		///		this.txtPasswd.Text, // userData: Password del usuario para ser guardado en esta cookie (recuerda encriptarlo).
		///		FormsAuthentication.FormsCookiePath );
		///		
		///		// Cifrar la cookie para un transporte seguro.
		///		string encTicket = FormsAuthentication.Encrypt(ticket);
		///		// Crear la cookie
		///		System.Web.HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket); // ticket con el Hash aplicado.
		///		// Establecer el tiempo de expiraci�n de la cookie al tiempo de expiraci�n del ticket.
		///		if (ticket.IsPersistent) cookie.Expires = ticket.Expiration;
		///		FormsAuthentication.SetAuthCookie(this.txtEmail.Text, false, FormsAuthentication.FormsCookiePath);
		///		// Agregar la cookie a la respuesta.
		///		Response.Cookies.Add(cookie);
		///		Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
		///		
		///		// Redireccionar a la URL solicitada, o la URL donde se encontraba 
		///		// cuando solicit� identificaci�n
		///		string returnUrl = Request.QueryString["ReturnUrl"];
		///		// Suponiendo que su p�gina de entrada se llame "default.aspx"
		///		if ((returnUrl == null) || (returnUrl.ToLower().IndexOf("default.aspx") > -1))
		///		{
		///			// Url de retorno a la direcci�n donde se encuentra el usuario actualmente.
		///			returnUrl = Request.Url.ToString();
		///		}
		///		// No utilizar FormsAuthentication.RedirectFromLoginPage ya que reemplazar�a el ticket de authentication 
		///		(la cookie) que acabamos de agregar.
		///		Response.Redirect(returnUrl, false);
		///	}
		/// </code>
		/// </example>
		[WebMethod(MessageName = "_LogOn", Description = "_LogOn: comprueba las credenciales del usuario que se pasan como valor de los par�metros " +
			 "\"e_mail\" y \"passwd\" para devolver un valor \"true\" si existe alguna coincidencia y \"false\" para cuando " + 
			 "no la hay. Tambi�n registra el nombre de la aplicaci�n y la URL que se pasan como valor de los par�metros " + 
			 "\"clientAppName\" y \"clientAppURL\" a nombre del usuario identificado para formarle una lista de sitios favoritos.")]
        [SoapDocumentMethod(Action = "http://webservices.cf.minaz.cu/_LogOn")]
        public bool _LogOn(string e_mail, string Passwd, string clientAppName, string clientAppURL, ServicedApplication.enuApplicationState appState)
		{
			return this.LogOn(e_mail, Passwd, clientAppName, clientAppURL, appState)>0?true:false;

		} // Fin de _LogOn

	
		/// <summary>
		/// Identifica el perfil, chequea si se requiere verificaci�n de la URL de la aplicaci�n cliente y ordena hacerlo 
		/// si es preciso, actualiza el registro de uso a nombre del usuario identificado.
		/// </summary>
		/// <param name="e_mail">email del perfil.</param>
		/// <param name="Passwd">contrase�a del perfil.</param>
		/// <param name="clientAppName">Nombre de la aplicaci�n cliente.</param>
		/// <param name="clientAppURL">URL que representa el punto de entrada de la aplicaci�n cliente.</param>
		/// <param name="appState">Estado de la aplicaci�n, "Debug" � "Release", cuando es debug no se 
		/// realizan algunas funciones y esto aligera la carga, permitiendo que el procedimientos se 
		/// ejecute con m�s rapidez.</param>
		/// <returns>Devolver� un DataSet "true" en caso de ser identificado un perfil que corresponda con los par�metros 
		/// e_mail y Passwd, en caso contrario devolver� "false".</returns>
		[WebMethod(MessageName = "_LogOnPlus", Description = "_LogOnPlus: comprueba las credenciales del usuario que se pasan como valor de los par�metros " +
			 "\"e_mail\" y \"passwd\". Si el usuario es identificado de manera correcta devuelve un DataSet con dos tablas la " + 
			 "primera, llamada \"Perfil\" contiene los siguientes campos: \"ID\", \"Nombre\", \"Apellidos\", \"e_mail\", \"FechaNac\", " + 
			 "\"Sexo\" y \"FechaRegistro\". La segunda tabla llamada \"Favoritos\" contiene los siguientes campos: \"Visitas\", \"Desde\", " + 
			 "\"Hasta\", \"appName\" y \"appURL\" y representan las cinco (5) aplicaciones m�s utilizadas por el usuario. Si las " + 
			 "credenciales del usuario no son v�lidas � si sucediera alg�n error en el proceso de identificaci�n que impidiera el " +
             "buen funcionamiento del sistema, entonces se devolver� un error que podr� ser mostrado al usuario para informaci�n.")]
        [SoapDocumentMethod(Action = "http://webservices.cf.minaz.cu/_LogOnPlus")]
        public System.Data.DataSet _LogOnPlus(string e_mail, string Passwd, string clientAppName, string clientAppURL, ServicedApplication.enuApplicationState appState)
		{
			int IdPerfil = 0;
			IdPerfil = this.LogOn(e_mail, Passwd, clientAppName, clientAppURL, appState);  
			if(IdPerfil==0)
			{
				throw new SoapException("Acceso denegado. Verifique que sus credenciales sean las correctas, " + 
                    "si est� seguro de ello, verifique que su cuenta no est� deshabilitada.",SoapException.ClientFaultCode);
			}

			try
			{
				Perfil perfil = new Perfil();
				perfil.ID = IdPerfil;
				// Registrar el usuario y la aplicaci�n en tabla tbl_RegistroUso no es necesario 
				// porque el propio procedimiento almacenado "pa_tbl_Perfil_GvIDxEmailPasswd_Plus"
				// valida las credenciales y agrega un nuevo registro a la tabla "tbl_RegistroUso".
				System.Data.DataSet ds = new System.Data.DataSet("dsPerfil");
				ds.Tables.Add(perfil.dsEntidad.Tables[0].Copy());
				ds.Tables[0].TableName = "Perfil";
				/* Eliminar el c�digo para agregar el Role como campo, porque no se considera necesario.
				System.Data.DataColumn dtRole = new DataColumn("Role", System.Type.GetType("System.String"));
				ds.Tables[0].Columns.Add(dtRole);
				ds.Tables[0].Rows[0]["Role"] = "Usuario";
				for(int i=0; i<=(wsSettings.wsAdmins.Length-1); i++)
				{
					if(e_mail == wsSettings.wsAdmins[i])
					{
						ds.Tables[0].Rows[0]["Role"] = "Administrador";
					}
				}
				*/
				//ds.Tables[0].Columns.Remove("ID");
				ds.Tables[0].Columns.Remove("Passwd");
				ds.Tables[0].Columns.Remove("IdCatOcupacional");
				ds.Tables[0].Columns.Remove("Habilitado");
				ds.Tables[0].Columns.Remove("DescripIntereses");
				ds.Tables.Add(perfil.getFavoritos(perfil.ID, 5).Tables[0].Copy());
				ds.Tables[1].TableName = "Favoritos";
				perfil = null;
				return ds;
			}
			catch(System.Exception Ex)
			{
				string errorMessage = ProcessError.getMessageToUser(Ex) + " " + System.Environment.NewLine;
				try
				{
					ProcessError processError = new ProcessError();
					if(processError.GuardarError(Ex, "AzuPass", "_LogOnPlus", clientAppName, clientAppURL, IdPerfil))
					{
						errorMessage = errorMessage + processError.ErrorRecordedNotification;
					}
					processError = null;
				}
				catch(System.Exception)
				{
					; // No hacer nada si falla el mecanismo de guardar los errores. 
				}
        throw new SoapException(errorMessage, SoapException.ClientFaultCode);
      }

		} // Fin de _LogOnPlus


		[WebMethod(MessageName = "_getProfileFavorites", Description = "_getProfileFavorites: valida las credenciales que se pasan como valor " + 
			 "de los par�metros \"e_mail\" y \"Passwd\", de ser correctas devuelve un DataSet con una tabla llamada " + 
			 "\"Favoritos\" que contiene los siguientes campos: \"Visitas\", \"Desde\", \"Hasta\", \"appName\" y " + 
			 "\"appURL\" y representan las aplicaciones m�s utilizadas por el usuario identificado. La lista de las " + 
			 "aplicaciones estar� limitada en cantidad por el valor del par�metro \"cantEnLista\".")]
        [SoapDocumentMethod(Action = "http://webservices.cf.minaz.cu/_getProfileFavorites")]
        public System.Data.DataSet _getProfileFavorites(string e_mail, string Passwd, int cantEnLista)
		{
			try
			{
				Perfil perfil = new Perfil();
				int IdPerfil =  perfil.Identificar(e_mail, Passwd, true);
				if(IdPerfil>0)
				{
					return perfil.getFavoritos(IdPerfil, cantEnLista);
				}
				else
				{
          throw new SoapException("Error al validar las credenciales del usuario.", SoapException.ClientFaultCode);
					//throw new System.Exception("Error al validar las credenciales del usuario.");
				}
			}
			catch(System.Exception Ex)
			{
				string errorMessage = ProcessError.getMessageToUser(Ex) + " " + System.Environment.NewLine;
				try
				{
					ProcessError processError = new ProcessError();
					if(processError.GuardarError(Ex, "AzuPass", "_getProfileFavorites"))
					{
						errorMessage = errorMessage + processError.ErrorRecordedNotification;
					}
					processError = null;
				}
				catch(System.Exception)
				{
					; // No hacer nada si falla el mecanismo de guardar los errores. 
				}
        throw new SoapException(errorMessage, SoapException.ClientFaultCode);
        //throw new System.Exception(errorMessage);
			}
		}


		[WebMethod(MessageName = "_getServicedApplications", Description = "_getServicedApplications: devuelve un DataSet con una tabla llamada " + 
			 "\"ServicedApplications\" que contiene los siguientes campos: \"Visitas\", \"Desde\", \"Hasta\", \"appName\" y " + 
			 "\"appURL\" y representan todas las aplicaciones que hacen uso de este servicio para identificaci�n de usuario " + 
			 "cuya URL ha sido verificada como existente. La lista de las aplicaciones estar� limitada en cantidad por el " + 
			 "valor del par�metro \"cantEnLista\".")]
        [SoapDocumentMethod(Action = "http://webservices.cf.minaz.cu/_getServicedApplications")]
        public System.Data.DataSet _getServicedApplications(int listTop)
		{
			try
			{
				RegistroUso regUso = new RegistroUso();
				return regUso.getFavoritos(listTop);
			}
			catch(System.Exception Ex)
			{
				string errorMessage = ProcessError.getMessageToUser(Ex) + " " + System.Environment.NewLine;
				try
				{
					ProcessError processError = new ProcessError();
					if(processError.GuardarError(Ex, "AzuPass", "_getServicedApplications"))
					{
						errorMessage = errorMessage + processError.ErrorRecordedNotification;
					}
					processError = null;
				}
				catch(System.Exception)
				{
					; // No hacer nada si falla el mecanismo de guardar los errores. 
				}
        throw new SoapException(errorMessage, SoapException.ClientFaultCode);
				//throw new System.Exception(errorMessage);
			}

		} // Fin de _getServicedApplications

        [WebMethod(MessageName = "_getServiceAdmins", Description = "_getServiceAdmins: devuelve un DataSet con una tabla llamada \"WebServiceAdmins\" " + 
			 "con el �nico campo \"e_mail\" donde aparecer� la direcci�n electr�nica de cada uno de las personas que aparezca en la " + 
			 "configuraci�n de este servicio como administrador del mismo.")]
        [SoapDocumentMethod(Action = "http://webservices.cf.minaz.cu/_getServiceAdmins")]
        public System.Data.DataSet _getServiceAdmins()
		{
			try
			{
				string[] aryAdmins = wsSettings.wsAdmins;
				System.Data.DataSet dsAdmins = new System.Data.DataSet();
				dsAdmins.Tables.Add("WebServiceAdmins");
				dsAdmins.Tables[0].Columns.Add("e_mail", System.Type.GetType("System.String"));
				System.Data.DataRow dr;
				for(int i=0; i<=(aryAdmins.Length-1); i++)
				{
					dr = dsAdmins.Tables[0].NewRow();
					dr["e_mail"] = aryAdmins[i];
					dsAdmins.Tables[0].Rows.Add(dr);
				}

				return dsAdmins;
			}
			catch(System.Exception Ex)
			{
				string errorMessage = ProcessError.getMessageToUser(Ex) + " " + System.Environment.NewLine;
				try
				{
					ProcessError processError = new ProcessError();
					if(processError.GuardarError(Ex, "AzuPass", "_getServiceAdmins"))
					{
						errorMessage = errorMessage + processError.ErrorRecordedNotification;
					}
					processError = null;
				}
				catch(System.Exception)
				{
					; // No hacer nada si falla el mecanismo de guardar los errores. 
				}
        throw new SoapException(errorMessage, SoapException.ClientFaultCode);
				//throw new System.Exception(errorMessage);
			}
		
		} // Fin de _getServiceAdmins


		[WebMethod(MessageName = "_getProfileManagerUrl", Description = "_getProfileManagerUrl: devuelve una cadena con la URL de donde se hospeda la aplicaci�n " + 
			 "\"Administrador de perfiles AzuPass\", tambi�n llamada: AzuPassProfileMgr.")]
        [SoapDocumentMethod(Action = "http://webservices.cf.minaz.cu/_getProfileManagerUrl")]
        public string _getProfileManagerUrl()
		{
			try
			{
				return wsSettings.AzuPassProfileMgrURL;
			}
			catch(System.Exception Ex)
			{
				string errorMessage = ProcessError.getMessageToUser(Ex) + " " + System.Environment.NewLine;
				try
				{
					ProcessError processError = new ProcessError();
					if(processError.GuardarError(Ex, "AzuPass", "_getProfileManagerURL"))
					{
						errorMessage = errorMessage + processError.ErrorRecordedNotification;
					}
					processError = null;
				}
				catch(System.Exception)
				{
					; // No hacer nada si falla el mecanismo de guardar los errores. 
				}
        throw new SoapException(errorMessage, SoapException.ClientFaultCode);
				//throw new System.Exception(errorMessage);
			}
		
		} // Fin de _getProfileManagerUrl 


		[WebMethod(MessageName = "_getAbout", Description = "_getAbout: devuelve una cadena en formato HTML codificado con el contenido del \"Acerca de\" de este servicio.")]
        [SoapDocumentMethod(Action = "http://webservices.cf.minaz.cu/_getAbout")]
        public string _getAbout()
		{
			try
			{
				string wsVersion = wsSettings.ServiceVersion;
				string wsAssemblyName = wsSettings.ServiceAssemblyName;

				string strTab = System.Web.UI.HtmlTextWriter.DefaultTabString;
				// Las comillas dobles.
				//char strQuote = System.Web.UI.HtmlTextWriter.DoubleQuoteChar;
				// Nueva l�nea
				string strNewLine = System.Environment.NewLine;

				/*
				System.Diagnostics.Debug.WriteLine("Request.ApplicationPath: " + HttpContext.Current.Request.ApplicationPath);
				System.Diagnostics.Debug.WriteLine("Url.AbsolutePath: " + HttpContext.Current.Request.Url.AbsolutePath);
				System.Diagnostics.Debug.WriteLine("Url.AbsoluteUri: " + HttpContext.Current.Request.Url.AbsoluteUri);
				System.Diagnostics.Debug.WriteLine("Url.Authority: " + HttpContext.Current.Request.Url.Authority);
				System.Diagnostics.Debug.WriteLine("Url.Host: " + HttpContext.Current.Request.Url.Host);
				System.Diagnostics.Debug.WriteLine("Url.LocalPath: " + HttpContext.Current.Request.Url.LocalPath);
				System.Diagnostics.Debug.WriteLine("Url.PathAndQuery: " + HttpContext.Current.Request.Url.PathAndQuery);
				System.Diagnostics.Debug.WriteLine("Url.Query: " + HttpContext.Current.Request.Url.Query);
				System.Diagnostics.Debug.WriteLine("Url.Scheme: " + HttpContext.Current.Request.Url.Scheme);
				System.Diagnostics.Debug.WriteLine("Url.UserInfo: " + HttpContext.Current.Request.Url.UserInfo);
				*/
						
				string logotipoPath = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.LocalPath, HttpContext.Current.Request.ApplicationPath + "/imagenes/logo_small.gif");

				System.Text.StringBuilder sbHTML = new System.Text.StringBuilder("");
			
				sbHTML.Append("<table border=\"0\" cellpadding=\"5\" cellspacing=\"0\" style=\"width: 100%\">" + strNewLine);
				sbHTML.Append(strTab + "<tr>" + strNewLine);
				sbHTML.Append(strTab + strTab + "<td nowrap style=\"font-weight: bold; font-size: 9pt; color: black; font-family: Verdana; background-color: #99ccff; text-decoration: none\">" + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "Servicio Web XML: " + wsAssemblyName + strNewLine);
				sbHTML.Append(strTab + strTab + "</td>" + strNewLine);
				sbHTML.Append(strTab + strTab + "<td nowrap align=\"right\" style=\"font-weight: bold; font-size: 9pt; color: black; font-family: Verdana; background-color: #99ccff; text-decoration: none\">" + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "Versi�n: " + wsVersion + strNewLine);
				sbHTML.Append(strTab + strTab + "</td>" + strNewLine);
				sbHTML.Append(strTab + "</tr>" + strNewLine);
				sbHTML.Append(strTab + "<tr>" + strNewLine);
				sbHTML.Append(strTab + strTab + "<td colspan=\"2\" style=\"font-weight: normal; font-size: 8pt; color: black; font-family: Verdana; text-decoration: none\">" + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "<b>Nombre:</b><br>" + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "AzuPass es un nombre compuesto formado de la uni�n del vocablo en lengua inglesa ");
				sbHTML.Append(strTab + strTab + strTab + "\"Pass\" que en su forma verbal significa, entre otros, \"pasar\", \"aprobar\", \"cruzar\" ");
				sbHTML.Append(strTab + strTab + strTab + "y del diminutivo \"Azu\" que se refiere a \"Az�car\" porque en su conjunto este servicio ");
				sbHTML.Append(strTab + strTab + strTab + "ha sido desarrollado para el Ministerio del Az�car.<br>" + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "<b>Logotipo:</b> <img align=\"absmiddle\" height=\"20\" width=\"40\" border=\"0\"	src=\"" + logotipoPath + "\"><br>" + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "<b>Descripci�n:</b><br>" + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "AzuPass es un servicio Web XML que utiliza registros de usuarios en una base de " + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "datos personalizada con el fin de proporcionar a sus \"aplicaciones clientes\" un " + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "mecanismo centralizado para la identificaci�n de los mismos, para ello expondr� " + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "los m�todos necesarios a trav�s de los cuales estos clientes podr�n implementar " + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "funciones b�sicas como: identificar usuarios, obtener lista de sitios o aplicaciones " + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "con sus correspondientes Nombre y URL que utilicen este servicio, obtener la lista " + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "de los x sitios y/o aplicaciones m�s utilizadas por el usuario que se identifica, " + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "etc. Los clientes especializados de este servicio, como la aplicaci�n Administrador " + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "de perfiles AzuPass (AzuPassProfileMgr) tendr�n acceso a otros miembros que les " + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "permitir�n realizar entre otras las siguientes operaciones: agregar nuevo perfil " + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "de usuario, modificar datos del perfil del usuario, etc. Gracias al desarrollo de " + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "la aplicaci�n especializada (AzuPassProfileMgr) los desarrolladores de aplicaciones " + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "que empleen este servicio para identificaci�n de usuarios no tendr�n que preocuparse " + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "por las implementaciones, en la aplicaci�n que desarrollen, de todos los mecanismos " + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "propios para la manipulaci�n de los datos de los perfiles de los usuario, sino que " + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "podr�n remitir a los usuarios a la URL de la aplicaci�n que lo hace (AzuPassProfileMgr), " + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "implementando solamente el m�todo para la validaci�n de las credenciales que proporciona " + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "el usuario.<br>" + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "<b>Desarrolladores:</b><br>" + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "<a href=\"mailto:leskyam@cf.minaz.cu?subject=Acerca de AzuPass&body=Mensaje generado desde el cuadro 'Acerca de' del servicio Web XMl AzuPass\">Lesky Alfonso M.</a><br>" + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + "<b>Administradores:</b><br>" + strNewLine);
				string[] aryServiceAdmins = wsSettings.wsAdmins;
				for(int i =0; i<=(aryServiceAdmins.Length-1); i++)
				{
					// <a href="mailto:user@dominio.cu?subject=Asunto&body=Cuerpo del mensaje">mailto sintaxis</a>
					sbHTML.Append(strTab + strTab + strTab + "<a href=\"mailto:" + aryServiceAdmins[i] + "?subject=Acerca de AzuPass&body=Mensaje generado desde el cuadro 'Acerca de' del servicio Web XMl AzuPass\">" + aryServiceAdmins[i] + "</a><br>" + strNewLine);
				}
				sbHTML.Append(strTab + strTab + "</td>" + strNewLine);
				sbHTML.Append(strTab + "</tr>" + strNewLine);
				sbHTML.Append(strTab + "<tr>" + strNewLine);
				sbHTML.Append(strTab + strTab + "<td colspan=\"2\" style=\"text-align: center; font-weight: bold; font-size: 6pt; color: black; font-family: Verdana; background-color: #99ccff; text-decoration: none\">" + strNewLine);
				sbHTML.Append(strTab + strTab + strTab + wsSettings.getCopyright() + strNewLine);
				sbHTML.Append(strTab + strTab + "</td>" + strNewLine);
				sbHTML.Append(strTab + "</tr>" + strNewLine);
				sbHTML.Append("</table>");

				/*
				System.Diagnostics.Debug.WriteLine("DECODED");
				System.Diagnostics.Debug.WriteLine(HttpContext.Current.Server.HtmlDecode(sbHTML.ToString()));
				System.Diagnostics.Debug.WriteLine("ENCODED");
				System.Diagnostics.Debug.WriteLine(HttpContext.Current.Server.HtmlEncode(sbHTML.ToString()));
				*/
				//return sbHTML.ToString();
				return HttpContext.Current.Server.HtmlEncode(sbHTML.ToString());

			}
			catch (System.Exception Ex)
			{
				string errorMessage = ProcessError.getMessageToUser(Ex) + " " + System.Environment.NewLine;
				try
				{
					ProcessError processError = new ProcessError();
					if(processError.GuardarError(Ex, "AzuPass", "_getAbout"))
					{
						errorMessage = errorMessage + processError.ErrorRecordedNotification;
					}
					processError = null;
				}
				catch(System.Exception)
				{
					; // No hacer nada si falla el mecanismo de guardar los errores. 
				}
        throw new SoapException(errorMessage, SoapException.ClientFaultCode);
				//throw new System.Exception(errorMessage);
			}

		} // Fin de _getAbout


		[WebMethod(MessageName = "_getLogoImageSrc", Description = "_getLogoImageSrc: devuelve una cadena con la direcci�n http donde se encuentra la imagen que representa el logo de este servicio.")]
        [SoapDocumentMethod(Action = "http://webservices.cf.minaz.cu/_getLogoImageSrc")]
        public string _getLogoImageSrc()
		{
			try
			{
        return TEICOCF.WebServices.wsSettings.ApplicationBaseURL + "imagenes/logo_small.gif";
			}
			catch (System.Exception Ex)
			{
				string errorMessage = ProcessError.getMessageToUser(Ex) + " " + System.Environment.NewLine;
				try
				{
					ProcessError processError = new ProcessError();
					if(processError.GuardarError(Ex, "AzuPass", "_getLogoImageSrc"))
					{
						errorMessage = errorMessage + processError.ErrorRecordedNotification;
					}
					processError = null;
				}
				catch(System.Exception)
				{
					; // No hacer nada si falla el mecanismo de guardar los errores. 
				}
        throw new SoapException(errorMessage, SoapException.ClientFaultCode);
				//throw new System.Exception(errorMessage);
			}

		} // Fin de _getLogoImageSrc


		[WebMethod(MessageName = "_getServiceShortName", Description = "")]
        [SoapDocumentMethod(Action = "http://webservices.cf.minaz.cu/_getServiceShortName")]
        public string _getServiceShortName()
		{
			try
			{
				return wsSettings.ServiceShortName;
			}
			catch (System.Exception Ex)
			{
				string errorMessage = ProcessError.getMessageToUser(Ex) + " " + System.Environment.NewLine;
				try
				{
					ProcessError processError = new ProcessError();
					if(processError.GuardarError(Ex, "AzuPass", "_getServiceShortName"))
					{
						errorMessage = errorMessage + processError.ErrorRecordedNotification;
					}
					processError = null;
				}
				catch(System.Exception)
				{
					; // No hacer nada si falla el mecanismo de guardar los errores. 
				}
        throw new SoapException(errorMessage, SoapException.ClientFaultCode);
				//throw new System.Exception(errorMessage);
			}

		} // Fin de _getServiceShortName


		[WebMethod(MessageName = "_getServiceFullName", Description = "")]
        [SoapDocumentMethod(Action = "http://webservices.cf.minaz.cu/_getServiceFullName")]
        public string _getServiceFullName()
		{
			try
			{
				return wsSettings.ServiceFullName;
			}
			catch (System.Exception Ex)
			{
				string errorMessage = ProcessError.getMessageToUser(Ex) + " " + System.Environment.NewLine;
				try
				{
					ProcessError processError = new ProcessError();
					if(processError.GuardarError(Ex, "AzuPass", "_getServiceFullName"))
					{
						errorMessage = errorMessage + processError.ErrorRecordedNotification;
					}
					processError = null;
				}
				catch(System.Exception)
				{
					; // No hacer nada si falla el mecanismo de guardar los errores. 
				}
        throw new SoapException(errorMessage, SoapException.ClientFaultCode);
				//throw new System.Exception(errorMessage);
			}

		} // Fin de _getServiceFullName

		[WebMethod(MessageName = "_getListCatOcupacionales", Description = "_getListCatOcupacionales: devuelve un DataSet con " + 
			 "una �nica tabla con la lista de las categor�as ocupacionales registradas para los perfiles de este servicio. La " + 
			 "tabla devuelta tiene los siguienjtes campos \"Id\" y \"Descripcion\".")]
        [SoapDocumentMethod(Action = "http://webservices.cf.minaz.cu/_getListCatOcupacionales")]
        public System.Data.DataSet _getListCatOcupacionales()
		{
			try
			{
				CatOcupacional catOcupacional = new CatOcupacional();
                return catOcupacional.getList();
			}
			catch (System.Exception Ex)
			{
				string errorMessage = ProcessError.getMessageToUser(Ex) + " " + System.Environment.NewLine;
				try
				{
					ProcessError processError = new ProcessError();
					if(processError.GuardarError(Ex, "AzuPass", "_getListCatOcupacionales"))
					{
						errorMessage = errorMessage + processError.ErrorRecordedNotification;
					}
					processError = null;
				}
				catch(System.Exception)
				{
					; // No hacer nada si falla el mecanismo de guardar los errores. 
				}
        throw new SoapException(errorMessage, SoapException.ClientFaultCode);
				//throw new System.Exception(errorMessage);
			}

		} // Fin de _getServiceFullName

		
		#endregion "FIN DE MIEMBROS NO PROTEGIDOS POR ENCABEZADOS SOAP"

		#region "MIEMBROS PROTEGIDOS POR ENCABEZADOS SOAP"

        [SoapHeader("wsAuthentication", Direction = SoapHeaderDirection.In)]
		[WebMethod(MessageName = "Register", Description = "Agrega un nuevo perfil con los valores de los par�metros que se pasan.")]
        [SoapDocumentMethod(Action = "http://webservices.cf.minaz.cu/Register")]
        public bool Register(string Nombre, string Apellidos, string e_mail, System.DateTime FechaNac, Perfil.enuSexo Sexo, int IdCatOcupacional, string DescripcionIntereses)
		{
			// Chequear el encabezado SOAP.
			this.checkSoapHeader();

			try
			{
				// Arreglar errores menores en las siguientes variables.
				Nombre = Nombre==null?string.Empty:Nombre.Trim();
				Apellidos = Apellidos==null?string.Empty:Apellidos.Trim();
				e_mail = e_mail==null?string.Empty:e_mail.ToLower().Trim();
				// Validar los datos proporcionados por la aplicaci�n cliente.
				valUserData(Nombre, Apellidos, e_mail, FechaNac, IdCatOcupacional);
				if(DescripcionIntereses!=null)
				{
					if (DescripcionIntereses.Trim() == string.Empty){DescripcionIntereses=null;}
				}

				// Obtener un valor encriptado para enviar por correo como Passwd inicial para el usuario.
				string encrypted = Crypto.Encrypt(e_mail + System.DateTime.Now.Ticks.ToString());
				int largoMinPasswd = wsSettings.PasswdRequeriments.pwdMinChars;
				// Obtener la cadena encriptada que se le enviar� al usuario.
        string Passwd = encrypted.Substring((encrypted.Length - largoMinPasswd) - 3, largoMinPasswd).ToLower();
				// El valor Passwd que se pasa para almacenarse es otra vez encriptado.
				System.Object[] Args = { null, Nombre, Apellidos, e_mail, Crypto.Encrypt(Passwd), FechaNac, Sexo, IdCatOcupacional, 
										   System.DateTime.Now, true, DescripcionIntereses };
				Perfil perfil = new Perfil();
				perfil.dsEntidad.Tables[0].Rows[0].ItemArray = Args;
				int IdPerfil = perfil.Agregar();
			
				// Enviar el correo de notificaci�n. Si no se puede eliminar� los registros recien agregados.
				if(!perfil.sendWelcomeEmail())
				{
					perfil.Eliminar();
					perfil = null;

					string[] aryServiceAdmins = wsSettings.wsAdmins;
					string ServiceAdmins = string.Empty;
				
					for(int i =0; i<=(aryServiceAdmins.Length-1); i++)
					{
						ServiceAdmins = i!=0?ServiceAdmins + ", ":"";
						ServiceAdmins = ServiceAdmins + aryServiceAdmins[i];
					}
          string errorMessage = "El sistema para env�o de notificaciones no ha funcionado como se esperaba, no se ha registrado el Perfil. " +
                        "Por favor, int�ntelo m�s tarde, si el problema persiste no dude en consultar a alguno de los administradores (" + ServiceAdmins + ") de este servcio.";
          throw new SoapException(errorMessage, SoapException.ClientFaultCode);
				}

				perfil = null;
				return true;
			}
			catch(System.Exception Ex)
			{
				string errorMessage = ProcessError.getMessageToUser(Ex) + " " + System.Environment.NewLine;
				try
				{
					ProcessError processError = new ProcessError();
					if(processError.GuardarError(Ex, "AzuPass", "Register"))
					{
						errorMessage = errorMessage + processError.ErrorRecordedNotification;
					}
					processError = null;
				}
				catch(System.Exception)
				{
					; // No hacer nada si falla el mecanismo de guardar los errores. 
				}
        throw new SoapException(errorMessage, SoapException.ClientFaultCode);
			}


		} // Fin de Register

		
		[SoapHeader("wsAuthentication")]
		[WebMethod(MessageName = "EditProfile", Description = "Devuelve un DataSet con los datos que corresponden " + 
			 "al usuario cuyos email y passwd se pasan como valor de los respectivos par�metros, el DataSet contiene " + 
			 "dos tablas con los siguientes nombres \"tbl_Perfil\" y \"lst_CatOcupacional\" con los datos del perfil " + 
			 "que se solicita editar y con la lista de categor�as ocupacionales disponibles, respectivamente.")]
        [SoapDocumentMethod(Action = "http://webservices.cf.minaz.cu/EditProfile")]
        public System.Data.DataSet EditProfile(string Email, string Passwd)
		{
			// Comprobar encabezados SOAP
			this.checkSoapHeader();

			try
			{
				Perfil perfil = new Perfil();
				perfil.ID = perfil.Identificar(Email, Passwd, false);
				perfil.dsEntidad.DataSetName = "dsPerfil";
				perfil.dsEntidad.Tables[0].TableName = "tbl_Perfil";
				perfil.dsEntidad.Tables[1].TableName = "lst_CatOcupacional";
				// Preparar para remover las columnas que no se editan a trav�s de este miembro.
				//System.Data.DataSet ds = new System.Data.DataSet();
				//ds = perfil.dsEntidad.Copy();
				perfil.dsEntidad.Tables[0].Columns.Remove("ID");
				perfil.dsEntidad.Tables[0].Columns.Remove("e_mail");
				perfil.dsEntidad.Tables[0].Columns.Remove("Passwd");
				perfil.dsEntidad.Tables[0].Columns.Remove("FechaRegistro");
				perfil.dsEntidad.Tables[0].Columns.Remove("Habilitado");
				//perfil = null;
				return perfil.dsEntidad;
			}
			catch(System.Exception Ex)
			{
				string errorMessage = ProcessError.getMessageToUser(Ex) + " " + System.Environment.NewLine;
				try
				{
					ProcessError processError = new ProcessError();
					if(processError.GuardarError(Ex, "AzuPass", "EditProfile"))
					{
						errorMessage = errorMessage + processError.ErrorRecordedNotification;
					}
					processError = null;
				}
				catch(System.Exception)
				{
					; // No hacer nada si falla el mecanismo de guardar los errores. 
				}
        throw new SoapException(errorMessage, SoapException.ClientFaultCode);
        //throw new System.Exception(errorMessage);
			}

		} // Fin de EditProfile

		
		[SoapHeader("wsAuthentication")]
		[WebMethod(MessageName = "PrepareNewProfile", Description = "Obtener un DataSet con los espacios que corresponden " + 
			 "a un nuevo Perfil, el DataSet contiene dos tablas con los siguientes nombres \"Perfil\" y \"lstCatOcupacionales\". " + 
			 "Las tablas contienen los siguientes campos: \"Nombre\", \"Apellidos\", \"e_mail\", \"FechaNac\", \"Sexo\", " + 
			 "\"IdCatOcupacional\" para la tabla \"Perfil\" y los campos: \"Id\" y \"Descripcion\" para la tabla \"lstCatOcupacionales\".")]
        [SoapDocumentMethod(Action = "http://webservices.cf.minaz.cu/PrepareNewProfile")]
        public System.Data.DataSet PrepareNewProfile()
		{
			// Comprobar encabezados SOAP
			this.checkSoapHeader();

			try
			{
				Perfil perfil = new Perfil(0);
				perfil.dsEntidad.DataSetName = "dsPerfil";
				perfil.dsEntidad.Tables[0].TableName = "Perfil";
				perfil.dsEntidad.Tables[1].TableName = "lstCatOcupacionales";
				System.Data.DataSet ds = new System.Data.DataSet();
				ds = perfil.dsEntidad.Copy();
				ds.Tables[0].Columns.Remove("ID");
				ds.Tables[0].Columns.Remove("Passwd");
				ds.Tables[0].Columns.Remove("FechaRegistro");
				ds.Tables[0].Columns.Remove("Habilitado");
				perfil = null;
				return ds;
			}
			catch(System.Exception Ex)
			{
				string errorMessage = ProcessError.getMessageToUser(Ex) + " " + System.Environment.NewLine;
				try
				{
					ProcessError processError = new ProcessError();
					if(processError.GuardarError(Ex, "AzuPass", "_LogOnPlus"))
					{
						errorMessage = errorMessage + processError.ErrorRecordedNotification;
					}
					processError = null;
				}
				catch(System.Exception)
				{
					; // No hacer nada si falla el mecanismo de guardar los errores. 
				}
        throw new SoapException(errorMessage, SoapException.ClientFaultCode);
        //throw new System.Exception(errorMessage);
			}

		} // Fin de PrepareNewProfile

		
		[SoapHeader("wsAuthentication")]
		[WebMethod(MessageName = "UpdateProfile", Description = "Actualizar los datos del perfil que concuerde " + 
			 "con las credenciales que se pasan (e_mail y Passwd) con los datos que contienen los dem�s par�metros.")]
        [SoapDocumentMethod(Action = "http://webservices.cf.minaz.cu/UpdateProfile")]
        public bool UpdateProfile(string e_mail, string Passwd, string Nombre, string Apellidos, System.DateTime FechaNac, Perfil.enuSexo Sexo, int IdCatOcupacional, string DescripcionIntereses)
		{
			// Chequear el encabezado SOAP.
			this.checkSoapHeader();

			try
			{
				Perfil perfil = new Perfil(0);
				perfil.ID = perfil.Identificar(e_mail, Passwd, false);
				if(perfil.ID==0)
				{
          throw new SoapException("Las credenciales proporcionadas no son v�lidas. Acceso denegado.", SoapException.ClientFaultCode);
          //throw new System.Exception("Las credenciales proporcionadas no son v�lidas. Acceso denegado.");
				}
                
				System.Text.StringBuilder errorMessage = new System.Text.StringBuilder("");

				// Nombre
				try
				{
					perfil.Nombre = Nombre;
				}
				catch(System.Exception Ex)
				{
					errorMessage.Append(Ex.Message + System.Environment.NewLine);
				}
				// Apellidos
				try
				{
					perfil.Apellidos = Apellidos;
				}
				catch(System.Exception Ex)
				{
					errorMessage.Append(Ex.Message + System.Environment.NewLine);
				}
				// FechaNac
				try
				{
					perfil.FechaNac = FechaNac;
				}
				catch(System.Exception Ex)
				{
					errorMessage.Append(Ex.Message + System.Environment.NewLine);
				}
				// Sexo
				try
				{
					perfil.Sexo = Sexo;
				}
				catch(System.Exception Ex)
				{
					errorMessage.Append(Ex.Message + System.Environment.NewLine);
				}
				// IdCatOcupacional
				try
				{
					perfil.IdCatOcupacional = IdCatOcupacional;
				}
				catch(System.Exception Ex)
				{
					errorMessage.Append(Ex.Message + System.Environment.NewLine);
				}
				// DescripIntereses
				try
				{
					perfil.DescripIntereses = DescripcionIntereses;
				}
				catch(System.Exception Ex)
				{
					errorMessage.Append(Ex.Message + System.Environment.NewLine);
				}
				// Si se registr� alg�n error emitir una Exception cion las descripciones correspondientes.
				if(errorMessage.ToString()!=string.Empty)
				{
					perfil = null;
          throw new SoapException(errorMessage.ToString(), SoapException.ClientFaultCode); 
          //throw new System.Exception(errorMessage.ToString());
				}
				// El valor Passwd que se pasa para almacenarse es encriptado.
				//System.Object[] Args = { perfil.ID, perfil.Nombre, perfil.Apellidos, perfil.Email, perfil.Passwd, perfil.FechaNac, perfil.Sexo, perfil.IdCatOcupacional, perfil.FechaRegistro, perfil.Habilitado};
				//newPerfil.dsEntidad.Tables[0].Rows[0].ItemArray = Args;
				perfil.Actualizar();
				perfil = null;
				return true;
			}
			catch(System.Exception Ex)
			{
				string errorMessage = ProcessError.getMessageToUser(Ex) + " " + System.Environment.NewLine;
				try
				{
					ProcessError processError = new ProcessError();
					if(processError.GuardarError(Ex, "AzuPass", "UpdateProfile"))
					{
						errorMessage = errorMessage + processError.ErrorRecordedNotification;
					}
					processError = null;
				}
				catch(System.Exception)
				{
					; // No hacer nada si falla el mecanismo de guardar los errores. 
				}
        throw new SoapException(errorMessage, SoapException.ClientFaultCode);
				//throw new System.Exception(errorMessage);
			}

		} // Fin de UpdateProfile


		[SoapHeader("wsAuthentication")]
		[WebMethod(MessageName = "getPasswdRequeriments", Description = "Obtener la estructura que almacena los " + 
			 "valores que representan los requerimientos de la contrase�a.")]
        [SoapDocumentMethod(Action = "http://webservices.cf.minaz.cu/getPasswdRequeriments")]
        public wsSettings.passwordRequeriments getPasswdRequeriments()
		{
			// Chequear los encabezados SOAP
			//this.checkSoapHeader();

			try
			{
				return wsSettings.PasswdRequeriments;
			}
			catch(System.Exception Ex)
			{
				string errorMessage = ProcessError.getMessageToUser(Ex) + " " + System.Environment.NewLine;
				try
				{
					ProcessError processError = new ProcessError();
					if(processError.GuardarError(Ex, "AzuPass", "getPasswdRequeriments"))
					{
						errorMessage = errorMessage + processError.ErrorRecordedNotification;
					}
					processError = null;
				}
				catch(System.Exception)
				{
					; // No hacer nada si falla el mecanismo de guardar los errores. 
				}
        throw new SoapException(errorMessage,SoapException.ClientFaultCode);
				//throw new System.Exception(errorMessage);
			}


		} // Fin de getPasswdRequeriments


		[SoapHeader("wsAuthentication")]
		[WebMethod(MessageName = "getChangeProfilePasswdWarning", Description = "getChangeProfilePasswdWarning: emite un mensaje " +
			 "para que sirva de advertencia al usuario sobre las caracter�sticas que se espera tenga la nueva contrase�a, sobre " +
			 "cantidad de caracteres m�nimos y m�ximos admitidos, y si se exige que la nueva contrase�a difiera del nombre, los " + 
			 "apellidos, el email o alguna parte de ellos o la uni�n de los mismos.")]
        [SoapDocumentMethod(Action = "http://webservices.cf.minaz.cu/getChangeProfilePasswdWarning")]
        public string getChangeProfilePasswdWarning()
		{

			// Chequear los encabezados SOAP
			this.checkSoapHeader();

			try
			{

                wsSettings.passwordRequeriments passwdReq = wsSettings.PasswdRequeriments;
				string returnMessage = "Los valores de configuraci�n de este servicio indican que su contrase�a debe terner " + 
					"como m�nimo " + passwdReq.pwdMinChars + " caracteres y como m�ximo " + passwdReq.pwdMaxChars + ".";
				if(passwdReq.pwdMustDiferUserName)
				{
					returnMessage += " Adem�s tambi�n se especifica que la cadena seleccionada como contrase�a no podr� aparecer " + 
						"contenida dentro de su nombre, apellidos, direcci�n electr�nica o cualquier uni�n de estos.";
				}

				return returnMessage;
			}
			catch(System.Exception Ex)
			{
				string errorMessage = ProcessError.getMessageToUser(Ex) + " " + System.Environment.NewLine;
				try
				{
					ProcessError processError = new ProcessError();
					if(processError.GuardarError(Ex, "AzuPass", "getChangeProfilePasswdWarning"))
					{
						errorMessage = errorMessage + processError.ErrorRecordedNotification;
					}
					processError = null;
				}
				catch(System.Exception)
				{
					; // No hacer nada si falla el mecanismo de guardar los errores. 
				}
        throw new SoapException(errorMessage,SoapException.ClientFaultCode);
        //throw new System.Exception(errorMessage);
			}

		} // Fin de getChangeProfilePasswdWarning


		[SoapHeader("wsAuthentication")]
		[WebMethod(MessageName = "changeProfilePasswd", Description = "changeProfilePasswd: cambiar la contrase�a del perfil " +
			 "que corresponda con las credenciales que se pasan como valor de los par�metros \"e_mail\" y \"current_Passwd\".")]
        [SoapDocumentMethod(Action = "http://webservices.cf.minaz.cu/changeProfilePasswd")]
        public bool changeProfilePasswd(string e_mail, string current_Passwd, string new_Passwd)
		{

			// Chequear los encabezados SOAP
			this.checkSoapHeader();

			try
			{
				// Validar la existencia dle perfil seg�n las credenciales.
				Perfil perfil = new Perfil();
				perfil.ID = perfil.Identificar(e_mail, current_Passwd, true);
				if(perfil.ID==0)
				{
          string errorMessage = "Acceso denegado. Las credenciales proporcionadas no corresponden con ning�n perfil. " +
            "Si est� seguro que son estas correctas, entonces verifique con los administradores de este servicio si su " +
            "perfil se encuentra deshabilitado.";
          throw new SoapException(errorMessage, SoapException.ClientFaultCode);
				}

				// Pasar el valor natural, para que lo valide seg�n lo configurado porque esta propiedad hace la validaci�n.
				perfil.Passwd = new_Passwd;
				// Ahora asignar el valor encriptado directamente al campo del DataSet del objeto perfil porque si lo hacemos 
				// a trav�s de la propiedad Passwd y es mayor en largo de los caracteres especificados en la cionfiguraci�n 
				// nos devolver� un error.
				perfil.dsEntidad.Tables[0].Rows[0]["Passwd"] = Crypto.Encrypt(new_Passwd);
				return perfil.Actualizar()>0?true:false;
			}
			catch(System.Exception Ex)
			{
				string errorMessage = ProcessError.getMessageToUser(Ex) + " " + System.Environment.NewLine;
				try
				{
					ProcessError processError = new ProcessError();
					if(processError.GuardarError(Ex, "AzuPass", "changeProfilePasswd"))
					{
						errorMessage = errorMessage + processError.ErrorRecordedNotification;
					}
					processError = null;
				}
				catch(System.Exception)
				{
					; // No hacer nada si falla el mecanismo de guardar los errores. 
				}
        throw new SoapException(errorMessage, SoapException.ClientFaultCode);
				//throw new System.Exception(errorMessage);
			}

		} // Fin de changePasswd


		[SoapHeader("wsAuthentication")]
		[WebMethod(MessageName = "getChangeProfileEmailWarning", Description = "getChangeProfileEmailWarning: emite un mensaje " +
			 "para que sirva de advertencia al usuario sobre el proceso de cambio de su direcci�n de correo, anunci�ndole que " + 
			 "si el cambio es efectivo tambi�n se cambiar� la contrase�a a un valor autogenerado por cuestiones de seguridad y " +
			 "entonces recibir� un correo con la nueva contrase�a como la primera vez que se registr�.")]
        [SoapDocumentMethod(Action = "http://webservices.cf.minaz.cu/getChangeProfileEmailWarning")]
        public string getChangeProfileEmailWarning()
		{

			// Chequear los encabezados SOAP
			this.checkSoapHeader();

			try
			{

				string[] aryEmailTopDomains = wsSettings.EmailTopDomains;
				string EmailTopDomains = string.Empty;
				
				for(int i =0; i<=(aryEmailTopDomains.Length-1); i++)
				{
					EmailTopDomains = i!=0?EmailTopDomains + ", ":"";
					EmailTopDomains = EmailTopDomains + aryEmailTopDomains[i];
				}
				if(EmailTopDomains == "*"){EmailTopDomains = string.Concat(EmailTopDomains, " (TODOS)");}

				string returnMessage = "Si el cambio de su direcci�n electr�nica es efectivo, se cambiar� tambi�n su actual contrase�a " + 
					"por un valor autogenerado por este servicio y �sta le llegar� por correo a la nueva direcci�n especificada, como sucedi� la " + 
					"primera vez que se registro como usuario del servicio \"" + wsSettings.ServiceShortName + "\". Tenga en cuenta que, seg�n la " + 
					"configuraci�n actual de este servicio, solo se admiten direcciones de correo cuyo formato indique que pertenezcen a los siguiente(s) " + 
					"dominio(s): " + EmailTopDomains;

				return returnMessage;
			}
			catch(System.Exception Ex)
			{
				string errorMessage = ProcessError.getMessageToUser(Ex) + " " + System.Environment.NewLine;
				try
				{
					ProcessError processError = new ProcessError();
					if(processError.GuardarError(Ex, "AzuPass", "getChangeProfileEmailWarning"))
					{
						errorMessage = errorMessage + processError.ErrorRecordedNotification;
					}
					processError = null;
				}
				catch(System.Exception)
				{
					; // No hacer nada si falla el mecanismo de guardar los errores. 
				}
        throw new SoapException(errorMessage, SoapException.ClientFaultCode);
				//throw new System.Exception(errorMessage);
			}

		} // Fin de getChangeProfileEmailWarning


		[SoapHeader("wsAuthentication")]
		[WebMethod(MessageName = "changeProfileEmail", Description = "changeProfileEmail: cambia la direcci�n electr�nica " +
			 "del perfil que se identifique seg�n los valores de las credenciales que se pasan en los par�metros \"currente_e_mail\" " + 
			 "y \"Passwd\" por la direcci�n que se pasa como valor de \"new_e_mail\". Los valores devueltos en forma de cadena " + 
			 "son los mensajes que indican lo que ha sucedido, si fue efectivo el cambio, y si no pudo ser, entonces la causa.")]
        [SoapDocumentMethod(Action = "http://webservices.cf.minaz.cu/changeProfileEmail")]
        public string changeProfileEmail(string current_e_mail, string Passwd, string new_e_mail)
		{

			// Chequear los encabezados SOAP
			this.checkSoapHeader();

			try
			{
				string NoChangeMessage = "No se realiz� el cambio solicitado. ";

				// Breve correcci�n del nuevo e_mail si es necesario.
				new_e_mail = new_e_mail==null?string.Empty:new_e_mail.ToLower().Trim();
			
				if(current_e_mail.ToLower() == new_e_mail)
				{
					return NoChangeMessage + "La direcci�n electr�nica actual y la nueva no difieren.";
				}
				// Validar la existencia dle perfil seg�n las credenciales.
				Perfil perfil = new Perfil();
				perfil.ID = perfil.Identificar(current_e_mail, Passwd, true);
				if(perfil.ID==0)
				{
          string errorMessage = "Acceso denegado. Las credenciales proporcionadas no corresponden con ning�n perfil. " +
            "Si est� seguro que son estas correctas, entonces verifique con los administradores de este servicio si su " +
            "perfil se encuentra deshabilitado.";
          throw new SoapException(errorMessage, SoapException.ClientFaultCode);
				}

				// Comprobar que o est�n en uso el nuevo e_mail.
				if(Perfil.existEmail(new_e_mail))
				{
					return NoChangeMessage + "La direcci�n electr�nica " + new_e_mail + " ya se encuentra en uso.";
				}
				// Pasar el valor a la propiedad para que lo valide seg�n lo configurado porque esta propiedad hace la validaci�n.
				perfil.Email = new_e_mail;
				perfil.dsEntidad.Tables[0].Rows[0]["e_mail"] = new_e_mail;
				// Crear una nueva contrase�a para el usuario porque por razones de seguridad le repetimos 
				// el proceso de env�o de la contrase�a por correo como cuando se registr� por primera vez.
				string encrypted = Crypto.Encrypt(new_e_mail + System.DateTime.Now.Ticks.ToString());
				int lagroMinPasswd = wsSettings.PasswdRequeriments.pwdMinChars;
				// Obtener la cadena encriptada que se le enviar� al usuario.
				string new_Passwd = encrypted.Substring((encrypted.Length-lagroMinPasswd)-3, lagroMinPasswd).ToLower();
				perfil.dsEntidad.Tables[0].Rows[0]["Passwd"] = Crypto.Encrypt(new_Passwd);
				if(perfil.Actualizar()>0)
				{
					if(!perfil.sendWelcomeEmail())
					{
						// Si no se env�a el correo revertir el cambio.
						perfil.dsEntidad.Tables[0].Rows[0]["e_mail"] = current_e_mail;
						perfil.dsEntidad.Tables[0].Rows[0]["Passwd"] = Passwd;
						perfil.Actualizar();
						return NoChangeMessage + "El sistema de env�o de notificaciones por correo no ha " + 
							"funcionado como se esperaba. Por favor, int�ntelo m�s tarde. Se ha guardado " + 
							"un registro del suceso para que el personal de soporte de este servicio lo " + 
							"revise si es necesario.";
					}
					return "Su direcci�n electr�nica ha cambiado, espere recibir a la nueva direcci�n un " + 
						"correo con una nueva contrase�a, la cual ha sido cambiada por razones de seguridad.";
				}
				else
				{
					string[] aryServiceAdmins = wsSettings.wsAdmins;
					string ServiceAdmins = string.Empty;
				
					for(int i =0; i<=(aryServiceAdmins.Length-1); i++)
					{
						ServiceAdmins = i!=0?ServiceAdmins + ", ":"";
						ServiceAdmins = ServiceAdmins + aryServiceAdmins[i];
					}

					return NoChangeMessage + "El servicio ha experimentado un comportamiento fuera de lo normal. " + 
						"Por favor, int�ntelo m�s tarde, si el problema persiste no dude en consultar a algunos " + 
						"de los administradores (" + ServiceAdmins + ") de este servicio.";
				}
			}
			catch(System.Exception Ex)
			{
				string errorMessage = ProcessError.getMessageToUser(Ex) + " " + System.Environment.NewLine;
				try
				{
					ProcessError processError = new ProcessError();
					if(processError.GuardarError(Ex, "AzuPass", "changeProfileEmail"))
					{
						errorMessage = errorMessage + processError.ErrorRecordedNotification;
					}
					processError = null;
				}
				catch(System.Exception)
				{
					; // No hacer nada si falla el mecanismo de guardar los errores. 
				}
        throw new SoapException(errorMessage, SoapException.ClientFaultCode);
        //throw new System.Exception(errorMessage);
			}

		} // Fin de changeProfileEmail


		[SoapHeader("wsAuthentication")]
		[WebMethod(MessageName = "resendWelcomeEmail", Description = "resendWelcomeEmail: Reenv�a, a la direcci�n de correo " + 
			 "especificada a trav�s del par�metro \"e_mail\", el correo electr�nico de bienvenida que genera este servicio " + 
			 "para los nuevos usuarios al registrarse la primera vez, este correo incluye entre otros datos la contrase�a " + 
			 "del usuario. De no existir el perfil que coincida con la direcci�n de correo especificada o si sucede alg�n " + 
			 "contratiempo que impida el env�o, se devolver� un error.")]
        [SoapDocumentMethod(Action = "http://webservices.cf.minaz.cu/resendWelcomeEmail")]
        public bool resendWelcomeEmail(string e_mail)
		{

			// Chequear los encabezados SOAP
			this.checkSoapHeader();

			try
			{
				Perfil perfil = new Perfil();
				return perfil.ResendPasswd(e_mail);
			}
			catch(System.Exception Ex)
			{
				string errorMessage = ProcessError.getMessageToUser(Ex) + " " + System.Environment.NewLine;
				try
				{
					ProcessError processError = new ProcessError();
					if(processError.GuardarError(Ex, "AzuPass", "resendWelcomeEmail"))
					{
						errorMessage = errorMessage + processError.ErrorRecordedNotification;
					}
					processError = null;
				}
				catch(System.Exception)
				{
					; // No hacer nada si falla el mecanismo de guardar los errores. 
				}
        throw new SoapException(errorMessage, SoapException.ClientFaultCode);
				//throw new System.Exception(errorMessage);
			}
		
		} // Fin de resendWelcomeEmail


		#endregion "FIN DE MIEMBROS PROTEGIDOS POR ENCABEZADOS SOAP"

		#region "MIEMBROS EN DEPURACI�N"
		#endregion "FIN DE MIEMBROS EN DEPURACI�N"

		#region "MIEMBROS NO DEPURADOS"
		#endregion "FIN DE MIEMBROS NO DEPURADOS"

		#region "MIEMBROS DE PRUEBA"


		//[SoapHeader("wsAuthentication")]
		[WebMethod(MessageName = "Test", Description = "M�todo Web de prueba.")]
        [SoapDocumentMethod(Action = "http://webservices.cf.minaz.cu/Test")]
		public string Test()
		{
			// Chequear los encabezados SOAP
			//checkSoapHeader();

			// Chequear si es alcanzable la URL.
			//			System.Uri myUri = new Uri(URL);
			//			try
			//			{
			//				;//return ServicedApplication.isURLReachable(myUri);
			//			}
			//			catch(System.Exception Ex)
			//			{
			//				System.Diagnostics.Debug.WriteLine(Ex.ToString());
			//				return false;
			//			}
			//
			//			//wsSettings.parseURL(URL, "minaz.cu");
			//			return true;

			// Recuperando la IP del cliente de este servicio Web.
			string IPAddr = this.Context.Request.UserHostAddress;
			return IPAddr;

		}


		#endregion "FIN DE MIEMBROS DE PRUEBA"

		
		#endregion "FIN DE PROCEDIMIENTOS P�BLICOS"
		
		#region "PROCEDIMIENTOS OBSOLETOS"


		#endregion "FIN DE PROCEDIMIENTOS OBSOLETOS"


	} // Fin de la clase "AzuPass"


	/// <summary>
	/// Clase para definir los par�metros para los encabezados SOAP personalizados.
	/// </summary>
	public class AuthHeader : System.Web.Services.Protocols.SoapHeader 
	{
		public string AppCliente = string.Empty;
		public string Password = string.Empty;
	}


} // Fin del namespace TEICOCF.WebServices
