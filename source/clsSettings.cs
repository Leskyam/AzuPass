using System;

namespace TEICOCF.WebServices
{
  /// <summary>
  /// Configuraciones en el fichero "Web.config" de este servicio
  /// </summary>
  public class wsSettings
  {
    #region "VARIABLES PRIVADAS"

    // NOMBRES, VERSION Y UBICACIÓN DE ESTE SERVICIO WEB XML
    private static string m_ServiceShortName = string.Empty;
    private static string m_ServiceFullName = string.Empty;
    private static string m_ServiceVersion = string.Empty;
    private static string m_ServiceAssemblyName = string.Empty;
    private static string m_ApplicationBaseURL = string.Empty;

    // DATOS PARA ACCESO A LA RED FUERA DEL DOMINIIO ACTUAL, A TRAVÉS DE UN PROXY
    private static bool m_CheckClientAppsURL;
    private static int m_MonthBeforeReCheck;
    private static System.Uri m_proxyAddress = null;
    private static string m_NetBIOSDomain = string.Empty;
    private static string m_proxyUser = string.Empty;
    private static string m_proxyPasswd = string.Empty;
    private static internetAccess m_InternetAccess;

    // URL DE LA APLICACIÓN QUE ADMINISTRA LOS PERFILES AzuPass
    private static string m_AzuPassProfileMgrURL = string.Empty;

    // ELEMENTOS PARA ACCESO A LA BASE DE DATOS DEL SERVICIO.
    private static string m_dbServerName = string.Empty;
    private static string m_dbServerType = string.Empty;
    private static int m_dbServerTCPPort = 0;
    private static string m_dbName = string.Empty;
    private static string m_dbUserName = string.Empty;
    private static string m_dbUserPass = string.Empty;
		private static bool m_dbPersistSegInfo = false;
    private static bool m_dbPooling = true;
    private static int m_MinPoolSize = 0;
    private static int m_MaxPoolSize = 100;

    // ADMINISTRADORES DEL SERVICIO
    private static string[] m_wsAdmins = new string[0];

    // CONFIGURACIONES PARA LAS CONTRASEÑAS DE LOS PERFILES.
    private static System.Byte m_pwdMinChars;
    private static System.Byte m_pwdMaxChars;
    private static bool m_pwdMustDiferUserName;
    private static passwordRequeriments m_PasswdRequeriments;

    // SERVIDOR Y OTRAS PARA CORREO
    private static string m_SMTPServer = string.Empty;
    private static int m_SMTPPort = 0;
    private static bool m_SMTPAuth = false;
    private static string m_SMTPUser = string.Empty;
    private static string m_SMTPPasswd = string.Empty;
    private static strucSMTPService m_SMTPService;

    private static string[] m_regExpForEmailDomains = new string[0];

    // OTROS VALORES DE CONFIGURACIÓN
    private static System.Byte m_MinAgeToUseThisService;

    #endregion "FIN DE VARIABLES PRIVADAS"

    #region "PROCEDIMIENTOS PROPERTY"

    public static string ApplicationBaseURL
    {
      get
      {
        if (m_ApplicationBaseURL.Length == 0)
        {
          try
          {
            m_ApplicationBaseURL = getWebConfigSimpleValue("AzuPassBaseURL").Trim();
          }
          catch (System.Exception)
          {
            ; // No se encontró la llave: AzuPassBaseURL en el fichero web.config
          }
          if (m_ApplicationBaseURL.Length == 0)
          {
            m_ApplicationBaseURL = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(System.Web.HttpContext.Current.Request.Url.AbsolutePath + System.Web.HttpContext.Current.Request.Url.Query, System.Web.HttpContext.Current.Request.ApplicationPath) + "/";
          }
        }
        if (!m_ApplicationBaseURL.EndsWith("/"))
        {
          m_ApplicationBaseURL = m_ApplicationBaseURL + "/";
        }
        return m_ApplicationBaseURL;
      }
    }

    /// <summary>
    /// Nombre corto para mostrar de este Servicio Web XML.
    /// </summary>
    public static string ServiceShortName
    {
      get
      {
        if (m_ServiceShortName.Length == 0)
        {
          m_ServiceShortName = wsSettings.getWebConfigSimpleValue("ServiceShortName");
          return m_ServiceShortName;
        }
        else
        {
          return m_ServiceShortName;
        }
      }
    }


    /// <summary>
    /// Nombre largo para mostrar de este Servicio Web XML.
    /// </summary>
    public static string ServiceFullName
    {
      get
      {
        if (m_ServiceFullName.Length == 0)
        {
          m_ServiceFullName = wsSettings.getWebConfigSimpleValue("ServiceFullName");
          return m_ServiceFullName;
        }
        else
        {
          return m_ServiceFullName;
        }
      }
    }


    /// <summary>
    /// Versión del emsamblado de este Servicio Web XML.
    /// </summary>
    public static string ServiceVersion
    {
      get
      {
        if (m_ServiceVersion.Length == 0)
        {
          System.Reflection.Assembly thisAssembly = System.Reflection.Assembly.GetExecutingAssembly();
          System.Reflection.AssemblyName thisAssemblyName = thisAssembly.GetName();
          m_ServiceVersion = thisAssemblyName.Version.ToString();
          return m_ServiceVersion;
        }
        else
        {
          return m_ServiceVersion;
        }
      }
    }


    /// <summary>
    /// Nombre del emsamblado de este Servicio Web XML.
    /// </summary>
    public static string ServiceAssemblyName
    {
      get
      {
        if (m_ServiceAssemblyName.Length == 0)
        {
          System.Reflection.Assembly thisAssembly = System.Reflection.Assembly.GetExecutingAssembly();
          System.Reflection.AssemblyName thisAssemblyName = thisAssembly.GetName();
          m_ServiceVersion = thisAssemblyName.Name;
          return m_ServiceAssemblyName;
        }
        else
        {
          return m_ServiceAssemblyName;
        }
      }
    }


    /// <summary>
    /// Devuelve el valor configurado que indica si se debe chequear la accesibilidad de las URLs 
    /// que pasan las aplicaciones.
    /// </summary>
    public static bool CheckClientAppsURL
    {
      get
      {
        try
        {
          m_CheckClientAppsURL = System.Convert.ToBoolean(wsSettings.getWebConfigSimpleValue("CheckClientAppsURL"));
          return m_CheckClientAppsURL;
        }
        catch (System.Exception Ex)
        {
          // Registrar Error aquí de manera detallada a través de la clase ProcessError.
          ProcessError processError = new ProcessError();
          processError.GuardarError(Ex, "wsSettings", "CheckClientAppsURL");
          processError = null;
          return false;
        }
      }
    }

    /// <summary>
    /// Devuelve el valor configurado que indica si se debe chequear la accesibilidad de las URLs 
    /// que pasan las aplicaciones.
    /// </summary>
    public static int MonthBeforeReCheck
    {
      get
      {
        try
        {
          m_MonthBeforeReCheck = System.Convert.ToInt32(wsSettings.getWebConfigSimpleValue("MonthBeforeReCheck"));
          if (m_MonthBeforeReCheck < 1) { m_MonthBeforeReCheck = 6; }
          return m_MonthBeforeReCheck;
        }
        catch (System.Exception Ex)
        {
          // Registrar Error aquí de manera detallada a través de la clase ProcessError.
          ProcessError processError = new ProcessError();
          processError.GuardarError(Ex, "wsSettings", "MonthBeforeReCheck");
          processError = null;
          return 6;
        }
      }
    }


    /// <summary>
    /// Devuelve el valor configurado que indica cual es el proxy que se utiliza para la red.
    /// </summary>
    private static System.Uri proxyAddress
    {
      get
      {
        if (m_proxyAddress != null) { return m_proxyAddress; }
        try
        {
          m_proxyAddress = new Uri(wsSettings.getWebConfigSimpleValue("proxyAddress"));
          return m_proxyAddress;
        }
        catch (System.Exception Ex)
        {
          // Registrar Error aquí de manera detallada a través de la clase ProcessError.
          ProcessError processError = new ProcessError();
          processError.GuardarError(Ex, "wsSettings", "proxyAddress");
          processError = null;
          return null;
        }
      }
    }


    /// <summary>
    /// Devuelve el valor configurado que indica cual es Dominio NetBIOS para presentar ante el proxy 
    /// que se utiliza para la red.
    /// </summary>
    private static string NetBIOSDomain
    {
      get
      {
        if (m_NetBIOSDomain.Length != 0) { return m_NetBIOSDomain; }
        try
        {
          m_NetBIOSDomain = wsSettings.getWebConfigSimpleValue("NetBIOSDomain");
          return m_NetBIOSDomain;
        }
        catch (System.Exception Ex)
        {
          // Registrar Error aquí de manera detallada a través de la clase ProcessError.
          ProcessError processError = new ProcessError();
          processError.GuardarError(Ex, "wsSettings", "NetBIOSDomain");
          processError = null;
          return string.Empty;
        }
      }
    }


    /// <summary>
    /// Devuelve el valor configurado que indica cual es el usuario para presentar ante el 
    /// proxy que se utiliza para la red.
    /// </summary>
    private static string proxyUser
    {
      get
      {
        if (m_proxyUser.Length != 0) { return m_proxyUser; }
        try
        {
          m_proxyUser = wsSettings.getWebConfigSimpleValue("proxyUser");
          return m_proxyUser;
        }
        catch (System.Exception Ex)
        {
          // Registrar Error aquí de manera detallada a través de la clase ProcessError.
          ProcessError processError = new ProcessError();
          processError.GuardarError(Ex, "wsSettings", "proxyUser");
          processError = null;
          return string.Empty;
        }
      }
    }


    /// <summary>
    /// Devuelve el valor configurado que indica cual es el proxy que se utiliza para la red.
    /// </summary>
    private static string proxyPasswd
    {
      get
      {
        if (m_proxyPasswd.Length != 0) { return m_proxyPasswd; }
        try
        {
          m_proxyPasswd = wsSettings.getWebConfigSimpleValue("proxyPasswd");
          return m_proxyPasswd;
        }
        catch (System.Exception Ex)
        {
          // Registrar Error aquí de manera detallada a través de la clase ProcessError.
          ProcessError processError = new ProcessError();
          processError.GuardarError(Ex, "wsSettings", "proxyPasswd");
          processError = null;
          return string.Empty;
        }
      }
    }


    /// <summary>
    /// URL donde se hospeda la aplicación "Administrador de perfiles AzuPass"
    /// </summary>
    public static string AzuPassProfileMgrURL
    {
      get
      {
        if (m_AzuPassProfileMgrURL.Length == 0)
        {
          m_AzuPassProfileMgrURL = wsSettings.getWebConfigSimpleValue("AzuPassProfileMgrURL").Trim();
          return m_AzuPassProfileMgrURL;
        }
        else
        {
          return m_AzuPassProfileMgrURL;
        }
      }
    }


    // ELEMENTOS PARA ACCESO A LA BASE DE DATOS DEL SERVICIO.

    /// <summary>
    /// Nombre del servidor donde se hospeda la base de datos.
    /// </summary>
    public static string dbServerName
    {
      get
      {
        if (m_dbServerName.Length == 0)
        {
          m_dbServerName = wsSettings.getWebConfigSimpleValue("dbServerName");
          m_dbServerName = m_dbServerName.Replace("(local)", "localhost");
          return m_dbServerName;
        }
        else
        {
          return m_dbServerName;
        }
      }
    }


    /// <summary>
    /// Tipo de servidor que se está utilizando, por ahora se permite solamente 'MSSqlServer'.
    /// </summary>
    public static string dbServerType
    {
      get
      {
        if (m_dbServerType.Length == 0)
        {
          m_dbServerType = getWebConfigSimpleValue("dbServerType");
          return m_dbServerType;
        }
        else
        {
          return m_dbServerType;
        }
      }
    }


    /// <summary>
    /// Tipo de servidor que se está utilizando, por ahora se permite solamente 'MSSqlServer'.
    /// </summary>
    public static int dbServerTCPPort
    {
      get
      {
        if (m_dbServerTCPPort == 0)
        {
          m_dbServerTCPPort = System.Convert.ToInt32(getWebConfigSimpleValue("dbServerTCPPort"));
          return m_dbServerTCPPort;
        }
        else
        {
          return m_dbServerTCPPort;
        }
      }
    }


    /// <summary>
    /// Nombre de la base de datos.
    /// </summary>
    public static string dbName
    {
      get
      {
        if (m_dbName.Length == 0)
        {
          m_dbName = getWebConfigSimpleValue("dbName");
          return m_dbName;
        }
        else
        {
          return m_dbName;
        }
      }
    }


    /// <summary>
    /// Nombre del usuario que se emplea para conectarse a la base de datos.
    /// </summary>
    public static string dbUserName
    {
      get
      {
        if (m_dbUserName.Length == 0)
        {
          m_dbUserName = getWebConfigSimpleValue("dbUserName");
          return m_dbUserName;
        }
        else
        {
          return m_dbUserName;
        }
      }
    }


    /// <summary>
    /// Contraseña de acceso del usuario de la base de datos.
    /// </summary>
    public static string dbUserPass
    {
      get
      {
        if (m_dbUserPass.Length == 0)
        {
          m_dbUserPass = Crypto.Decrypt(getWebConfigSimpleValue("dbUserPass"));
          return m_dbUserPass;
        }
        else
        {
          return m_dbUserPass;
        }
      }
    }

		/// <summary>
		/// Opción "Persist Security Info" de la conexión a la base de datos.
		/// </summary>
		public static bool dbPersistSegInfo
		{
			get
			{
				try
				{
					m_dbPersistSegInfo = Crypto.Decrypt(getWebConfigSimpleValue("dbPersistSegInfo")).ToLower() == "true" ? true : false;
				}
				catch (System.Exception)
				{
					m_dbPersistSegInfo = false;
				}
				return m_dbPersistSegInfo;
			}
		}
		
		/// <summary>
    /// Opción "Pooling" de la conexión a la base de datos.
    /// </summary>
    public static bool dbPooling
    {
        get
        {
            try
            {
                m_dbPooling = Crypto.Decrypt(getWebConfigSimpleValue("dbPooling")).ToLower() == "false" ? false : true;
                return m_dbPooling;
            }
            catch (System.Exception)
            {
                m_dbPooling  = true;
                return m_dbPooling;
            }
        }
    }

    /// <summary>
    /// Opción "Min Pool Size" de la conexión a la base de datos.
    /// </summary>
    public static int dbMinPoolSize
    {
        get
        {
            try
            {
                m_MinPoolSize = Convert.ToInt32(Crypto.Decrypt(getWebConfigSimpleValue("dbMinPoolSize")));
            }
            catch (System.Exception)
            {
                m_MinPoolSize = 0; 
            }
            return m_MinPoolSize;
        }
    }

    /// <summary>
    /// Opción "Max Pool Size" de la conexión a la base de datos.
    /// </summary>
    public static int dbMaxPoolSize
    {
        get
        {
					try
					{
						m_MaxPoolSize = Convert.ToInt32(Crypto.Decrypt(getWebConfigSimpleValue("dbMaxPoolSize")));
					}
					catch (System.Exception)
					{
						m_MaxPoolSize = 100;
					}
					return m_MaxPoolSize;
        }
    }

    // ADMINISTRADORES DEL SERVICIO.
    /// <summary>
    /// Arreglo con los administradores del servicio que se configuraron en el Web.config.
    /// </summary>
    public static string[] wsAdmins
    {
      get
      {
        if (m_wsAdmins.Length == 0)
        {
          m_wsAdmins = getWebConfigSimpleValue("wsAdmins").Split('|');
          return m_wsAdmins;
        }
        else
        {
          return m_wsAdmins;
        }
      }
    }


    // CONFIGURACIONES PARA LAS CONTRASEÑAS.
    /// <summary>
    /// Cantidad mínima de caracteres que se permiten para la contraseña.
    /// </summary>
    private static System.Byte pwdMinChars
    {
      get
      {
        try
        {
          if (m_pwdMinChars == 0)
          {
            m_pwdMinChars = System.Convert.ToByte(getWebConfigSimpleValue("pwdMinChars"));
            if (m_pwdMinChars < 6) { m_pwdMinChars = 6; }
            return m_pwdMinChars;
          }
          else
          {
            return m_pwdMinChars;
          }
        }
        catch (System.Exception)
        {
          m_pwdMinChars = 6;
          return m_pwdMinChars;
        }
      }
    }


    /// <summary>
    /// Cantidad máxima de caracteres que se permiten para la contraseña.
    /// </summary>
    private static System.Byte pwdMaxChars
    {
      get
      {
        try
        {
          if (m_pwdMaxChars == 0)
          {
            m_pwdMaxChars = System.Convert.ToByte(getWebConfigSimpleValue("pwdMaxChars"));
            if (m_pwdMaxChars < 6 || pwdMaxChars > 16) { m_pwdMaxChars = 16; }
            return m_pwdMaxChars;
          }
          else
          {
            return m_pwdMaxChars;
          }
        }
        catch (System.Exception)
        {
          m_pwdMinChars = 16;
          return m_pwdMinChars;
        }
      }
    }


    /// <summary>
    /// Indicador para saber si se exige que la contraseña difiera del nombre del mailbox y del nombre del usuario.
    /// </summary>
    private static bool pwdMustDiferUserName
    {
      get
      {
        try
        {
          m_pwdMustDiferUserName = System.Convert.ToBoolean(getWebConfigSimpleValue("pwdMustDiferUserName"));
          return m_pwdMustDiferUserName;
        }
        catch (System.Exception)
        {
          m_pwdMustDiferUserName = true;
          return m_pwdMustDiferUserName;
        }
      }
    }


    /// <summary>
    /// Estructura que contiene los valores establecidos para las características aceptadas de las contraseñas de los perfiles.
    /// </summary>
    public static passwordRequeriments PasswdRequeriments
    {
      get
      {
        m_PasswdRequeriments.pwdMinChars = pwdMinChars;
        m_PasswdRequeriments.pwdMaxChars = pwdMaxChars;
        m_PasswdRequeriments.pwdMustDiferUserName = pwdMustDiferUserName;
        return m_PasswdRequeriments;
      }
    }


    /// <summary>
    /// Estructura que contiene los valores establecidos para las comprobaciones de las URLs que pasan las aplicaciones.
    /// </summary>
    public static internetAccess InternetAccess
    {
      get
      {
        m_InternetAccess.CheckClientAppsURL = CheckClientAppsURL;
        m_InternetAccess.MonthBeforeReCheck = MonthBeforeReCheck;
        m_InternetAccess.proxyAddress = proxyAddress;
        m_InternetAccess.NetBIOSDomain = NetBIOSDomain;
        m_InternetAccess.proxyUser = proxyUser;
        m_InternetAccess.proxyPasswd = proxyPasswd;
        return m_InternetAccess;
      }
    }

    public static string[] EmailTopDomains
    {
      get
      {
        string str = getWebConfigSimpleValue("EmailTopDomains");
        if (str == "*" || str.IndexOf("|*|") >= 0)
        {
          string[] result = { "*" };
          return result;
        }
        return str.Split('|');
      }
    }



    /// <summary>
    /// Devuelve un array de tipo cadena con las "Regular Expressions" necesarias para validar
    /// las direcciones de correo que se pasan cuando se agregan o modifican los datos del perfil.
    /// </summary>
    public static string[] regExpForEmail
    {
      get
      {
        string str = getWebConfigSimpleValue("EmailTopDomains");
        // Comprobar mala configuraciópn tratando de indicar que se necesita aceptar toda dirección de correo.
        if (str == "*" || str.IndexOf("|*|") >= 0) //  || str.IndexOf("*|")>=0 || str.IndexOf("|*")>=0
        {
          m_regExpForEmailDomains[0] = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
          return m_regExpForEmailDomains;
        }

        m_regExpForEmailDomains = str.Split('|');

        for (int i = 0; i <= (m_regExpForEmailDomains.Length - 1); i++)
        {
          // Comprobar mala configuración tratando de permitir subdominios, pero olvidando el asterisco (*)
          if (m_regExpForEmailDomains[i].IndexOf(".") == 0) { m_regExpForEmailDomains[i] = "*" + m_regExpForEmailDomains[i]; }

          // Tratando aquellos casos que especifican subdominios.
          if (m_regExpForEmailDomains[i].IndexOf("*.") >= 0)
          {
            // Para los casos que tienen un asterisco y punto (*.) al comienzo porque se necesita 
            // tener en cuenta que existen subdominios.
            // Como ejemplo serían: @"^([\w-\.]+)@([\w-\.]+)\.minaz.cu$" para "*.minaz.cu"
            //			y			@"^([\w-\.]+)@([\w-\.]+)\.cf.minaz.cu$" para "*.cf.minaz.cu"
            m_regExpForEmailDomains[i] = m_regExpForEmailDomains[i].Substring(2);
            //System.Diagnostics.Debug.WriteLine(m_regExpForEmailDomains[i]);
            m_regExpForEmailDomains[i] = @"^([\w-\.]+)@([\w-\.]+)\." + m_regExpForEmailDomains[i] + "$";
            //System.Diagnostics.Debug.WriteLine(m_regExpForEmailDomains[i]);
          }
          else
          {
            // ESTE ESTÁ OK
            // Para cuando el valor es lo que viene detrás de la arroba "@".
            // Sería ejemplo: @"^([\w-\.]+)@" + "minaz.cu" + "$"
            m_regExpForEmailDomains[i] = @"^([\w-\.]+)@" + m_regExpForEmailDomains[i] + "$";
            //System.Diagnostics.Debug.WriteLine(m_regExpForEmailDomains[i]);
          }
        }
        return m_regExpForEmailDomains;
      }
    }


    /// <summary>
    /// Indicador para saber si se exige que la contraseña difiera del nombre del mailbox y del nombre del usuario.
    /// </summary>
    public static System.Byte MinAgeToUseThisService
    {
      get
      {
        try
        {
          if (m_MinAgeToUseThisService == 0)
          {
            m_MinAgeToUseThisService = System.Convert.ToByte(getWebConfigSimpleValue("MinAgeToUseThisService"));
            return m_MinAgeToUseThisService;
          }
          else
          {
            return m_MinAgeToUseThisService;
          }
        }
        catch (System.Exception)
        {
          m_MinAgeToUseThisService = 16;
          return m_MinAgeToUseThisService;
        }
      }
    }


    // ELEMENTOS DE LA CONFIGURACIÓN DE CORRREO PARA ENVÍO DE NOTIFICACIONES.
    private static string SMTPServer
    {
      get
      {
        if (m_SMTPServer.Length == 0)
        {
          m_SMTPServer = getWebConfigSimpleValue("SMTPServer");
          return m_SMTPServer;
        }
        else
        {
          return m_SMTPServer;
        }
      }
    }


    private static int SMTPPort
    {
      get
      {
        if (m_SMTPPort == 0)
        {
          m_SMTPPort = System.Convert.ToInt32(getWebConfigSimpleValue("SMTPPort"));
          return m_SMTPPort;
        }
        else
        {
          return m_SMTPPort;
        }
      }
    }


    private static bool SMTPAuth
    {
      get
      {
        m_SMTPAuth = System.Convert.ToBoolean(getWebConfigSimpleValue("SMTPAuth"));
        return m_SMTPAuth;
      }
    }


    private static string SMTPUser
    {
      get
      {
        if (m_SMTPUser.Length == 0)
        {
          m_SMTPUser = getWebConfigSimpleValue("SMTPUser");
          return m_SMTPUser;
        }
        else
        {
          return m_SMTPUser;
        }
      }
    }


    private static string SMTPPasswd
    {
      get
      {
        if (m_SMTPPasswd.Length == 0)
        {
          m_SMTPPasswd = getWebConfigSimpleValue("SMTPPasswd");
          return m_SMTPPasswd;
        }
        else
        {
          return m_SMTPPasswd;
        }
      }
    }


    public static strucSMTPService SMTPService
    {
      get
      {
        m_SMTPService.Server = SMTPServer;
        m_SMTPService.Port = SMTPPort;
        m_SMTPService.Auth = SMTPAuth;
        m_SMTPService.User = SMTPUser;
        m_SMTPService.Passwd = SMTPPasswd;
        return m_SMTPService;
      }
    }


    #endregion "FIN DE PROCEDIMIENTOS PROPERTY"

    #region "ESTRUCTURAS"

    public struct internetAccess
    {
      public bool CheckClientAppsURL;
      public int MonthBeforeReCheck;
      public System.Uri proxyAddress;
      public string NetBIOSDomain;
      public string proxyUser;
      public string proxyPasswd;
    }

    public struct passwordRequeriments
    {
      public System.Byte pwdMinChars;
      public System.Byte pwdMaxChars;
      public bool pwdMustDiferUserName;
    }

    public struct strucSMTPService
    {
      public string Server;
      public int Port;
      public bool Auth;
      public string User;
      public string Passwd;
    }

    #endregion "FIN DE ESTRUCTURAS"

    #region "ENUMERACIONES"

    public enum DataServerTypes
    {
      MSSql = 1,
      MySQL,
      PostgreSQL
    }



    #endregion "FIN DE ENUMERACIONES"

    #region "PROCEDIMIENTOS PRIVADOS"

    private static string getWebConfigSimpleValue(string strKeyName)
    {
      if (System.Configuration.ConfigurationManager.AppSettings[strKeyName] != null)
      {
        return System.Configuration.ConfigurationManager.AppSettings[strKeyName];
      }
      else
      {
        throw new System.Configuration.ConfigurationErrorsException("La llave '" + strKeyName + "' no existe en la sección 'appSettings' del fichero de configuración 'web.config'");
      }
    }

    #endregion "FIN DE PROCEDIMIENTOS PRIVADOS"

    #region "PROCEDIMIENTOS PÚBLICOS"

    /// <summary>
    /// De ser válida la URL que se pasa: convierte el "URI scheme" y el nombre del Host a minúsculas, si el nombre del host 
    /// es una dirección IPv6, la dirección IPv6 canónica es utilizada, el "ScopeId" y otros datos opcionales de datos IPv6 
    /// son eliminados, elimina los números de puerto por defecto, si aparecen, canonaliza el camino para URIs gerárquicas
    /// compactando secuencias tales como /./, /../, //, incluyendo secuencias de corrido, para URIs gerárquicas, si el host
    /// no termina con barra diagonal derecha (/), esta es agregada, cualquier caracter reservado en la URI es tratado de 
    /// acuerdo al RFC 2396.
    /// </summary>
    /// <param name="appURL">La URL completa de la aplicación.</param>
    /// <param name="hostEndWith">Parte del final con la que debe terminar el host dentro de la URL.</param>
    /// <returns></returns>
    public static string parseURL(string appURL, string hostEndWith)
    {
      // Se emitirá una exception de tipo System.UriFormatException si la URL que se pasa no es válida.
      System.Uri myUri = new Uri(appURL);

      if (myUri.Query.Length != 0)
      {
        myUri = new Uri(myUri.AbsoluteUri.Replace(myUri.Query, ""));
      }

      System.Text.StringBuilder sbResult = new System.Text.StringBuilder("");

      if (myUri.Scheme != "http" & myUri.Scheme != "https")
      {
        throw new System.UriFormatException("Solo se admiten URLs para los protocolos \"HTTP\" ó \"HTTPS\"");
      }
      /*
       * Eliminar la restricción de que las URL de las aplicaciones estén solamente dentro del dominio "MINAZ.CU"
       * 
      */
      /*
      if (!myUri.Host.EndsWith(hostEndWith))
      {
        throw new System.UriFormatException("Solo se admiten URLs para los dominios pertenecientes a " +
          "\"" + hostEndWith + "\" y no se admiten direcciones IP de ninguna clase para el nombre del \"Host\".");
      }
      */
 			/*
			 * No chequear que la dirección URL que se pasa como válida para la aplicación termine en slash "/".
			 * 
			 */
			/*
      if (!myUri.AbsoluteUri.EndsWith("/"))
      {
        string fileName = myUri.AbsolutePath.Substring(myUri.AbsolutePath.LastIndexOf("/") + 1);
        throw new System.UriFormatException("Solo se admiten URLs que representen punto de entrada a una aplicación, " +
          "por tanto se espera que terminen con el caracter \"/\" o con la ruta a la aplicación, pero no con el " +
          "nombre de un fichero como es el caso que termina con: \"" + fileName + "\", puede que necesite cambiar " +
          "el nombre del fichero que representa el punto de entrada a la aplicación para que coincida con la " +
          "configuración de su servidor HTTP o cambiar la configuración de su servidor HTTP para que reconozca " +
          "los ficheros llamados \"" + fileName + "\" como puntos de entrada de aplicación.");
      }
			*/

      return myUri.AbsoluteUri;
    }


    public static string getHTML_About()
    {
      int intYearInitial = 2007;
      int intYearActual = System.DateTime.Now.Year;
      string strYears = (intYearInitial < intYearActual) ? (intYearInitial.ToString() + "-" + intYearActual.ToString()) : intYearInitial.ToString();

      string strCopyrightFoot = "<table cellspacing='0' cellpadding='0' width='100%' align='center' border='0'>" +
        "<tr>" +
        "<td align='center' style='padding-right: 1px;padding-left: 1px;font-weight: bold;font-size: 7pt;padding-bottom: 1px;vertical-align: baseline;color: dimgray;padding-top: 1px;font-family: Verdana;'>" +
        "Copyright &#169 " + strYears + " Equipo de Servicios Inform&aacute;ticos  - Cienfuegos." +
        "</td>" +
        "</tr>" +
        "</table>";
      return strCopyrightFoot;
    }

    public static string getCopyright()
    {
      int intYearInitial = 2007;
      int intYearActual = System.DateTime.Now.Year;
      string strYears = (intYearInitial < intYearActual) ? (intYearInitial.ToString() + "-" + intYearActual.ToString()) : intYearInitial.ToString();

      string strCopyrightFoot = "Copyright &#169 " + strYears + " Equipo de Servicios Inform&aacute;ticos - Cienfuegos.";
      return strCopyrightFoot;
    }

    public static string getHTML_FootCopyright()
    {
      int intYearInitial = 2007;
      int intYearActual = System.DateTime.Now.Year;
      string strYears = (intYearInitial < intYearActual) ? (intYearInitial.ToString() + "-" + intYearActual.ToString()) : intYearInitial.ToString();

      string strCopyrightFoot = "<table cellspacing='0' cellpadding='0' width='100%' align='center' border='0'>" +
        "<tr>" +
        "<td align='center' style='padding-right: 1px;padding-left: 1px;font-weight: bold;font-size: 7pt;padding-bottom: 1px;vertical-align: baseline;color: dimgray;padding-top: 1px;font-family: Verdana;'>" +
        "Copyright &#169 " + strYears + " Equipo de Servicios Inform&aacute;ticos - Cienfuegos." +
        "</td>" +
        "</tr>" +
        "</table>";
      return strCopyrightFoot;
    }

    #endregion "FIN DE PROCEDIMIENTOS PÚBLICOS"

  } // Fin de la clase

} // Fin del namespace
