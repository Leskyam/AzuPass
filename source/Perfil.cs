using System;

namespace LAMSoft.WebServices
{
  /// <summary>
  /// Implementa las opciones específicas para el manejo de los registro de la tabla "tbl_Perfil". 
  /// Hereda de la clase "Entidad" algunos métodos como Agregar, Actualizar, Eliminar y el campo ID.
  /// </summary>
  public class Perfil : Entidad
  {

    #region "CONSTRUCTORES"

    /// <summary>
    /// Contructor. Carga el objeto como si se tratase del intento de agregar un nuevo 
    /// registro, con el valor de ID en cero(0). Pasa ala clase base "Entidad" el nombre
    /// de la tabla "tbl_Perfil", donde se almacenan los datos que maneja esta clase.
    /// </summary>
    public Perfil()
      : base(EntityTableName)
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
    public Perfil(int Id)
      : base(EntityTableName)
    {
      this.ID = Id;
    }

    #endregion "FIN DE CONSTRUCTORES"

    #region "VARIABLES PRIVADAS"

    private static string EntityTableName = "tbl_Perfil";

    private string m_Nombre = string.Empty;
    private string m_Apellidos = string.Empty;
    private string m_Email = string.Empty;
    private string m_Passwd = string.Empty;
    private System.DateTime m_FechaNac;
    private enuSexo m_Sexo = enuSexo.M;
    private int m_IdCatOcupacional = 0;
    private System.DateTime m_FechaRegistro;
    private bool m_Habilitado = true;
    private string m_DescripIntereses = string.Empty;

    #endregion "FIN DE VARIABLES PRIVADAS"

    #region "PROCEDIMIENTOS PROPERTY"

    /// <summary>
    /// Nombre. Devolverá Exception si es una cadena vacía o si la cadena excede los 25 caracteres.
    /// </summary>
    public string Nombre
    {
      get { return this.m_Nombre; }
      set
      {
        value = value.Trim();
        if (value == string.Empty || value.Length > 25)
        {
          throw new System.Exception("El elementos \"Nombre\" no puede ser una cadena vacía y no debe exceder de 25 caracteres.");
        }
        this.m_Nombre = value;
        this.dsEntidad.Tables[0].Rows[0]["Nombre"] = value;
      }
    }


    /// <summary>
    /// Apellidos. Devolverá Exception si es una cadena vacía o si la cadena excede los 75 caracteres.
    /// </summary>
    public string Apellidos
    {
      get { return this.m_Apellidos; }
      set
      {
        value = value.Trim();
        if (value == string.Empty || value.Length > 75)
        {
          throw new System.Exception("El elementos \"Apellidos\" no puede ser una cadena vacía y no debe exceder de 75 caracteres.");
        }
        this.m_Apellidos = value;
        this.dsEntidad.Tables[0].Rows[0]["Apellidos"] = value;
      }
    }


    /// <summary>
    /// Dirección de correo. Devolverá Exception si es una cadena vacía o si la cadena excede los 75 caracteres y
    /// también si no corresponde con alguno de los dominios de correo permitidos en el fiichero "Web.config".
    /// </summary>
    public string Email
    {

      get { return this.m_Email; }
      set
      {
        value = value.Trim();
        if (value == string.Empty || value.Length > 75)
        {
          throw new System.Exception("El elementos \"Dirección de correo\" no puede ser una cadena vacía y no debe exceder de 75 caracteres.");
        }
        if (!isValidEmail(value))
        {
          throw new System.Exception("El elemento \"Dirección de correo\" debe tener un formato válido y pertenecer a un dominio permitido.");
        }
        this.m_Email = value;
        this.dsEntidad.Tables[0].Rows[0]["e_mail"] = value;
      }
    }

    /// <summary>
    /// Contraseña. Devolverá Exception si no cumple con los requerimientos mínimos y/o máximos de cantidad de 
    /// caracteres y también, si se exige en el Web.config, impedirá que esta sea igual o esté contenida dentro 
    /// del Nombre, los Apellidos ó la propia dirección electrónica.
    /// </summary>
    public string Passwd
    {
      get { return this.m_Passwd; }
      set
      {
        value = value.Trim();
        if (value.Length < Settings.PasswdRequeriments.pwdMinChars || value.Length > Settings.PasswdRequeriments.pwdMaxChars)
        {
          throw new System.Exception("El elemento \"Contraseña\" no comple con la cantidad de caracteres requeridos. " +
            "Debe ser mayor o igual a " + Settings.PasswdRequeriments.pwdMinChars + " y menor o igual a " +
            Settings.PasswdRequeriments.pwdMaxChars);
        }
        if (Settings.PasswdRequeriments.pwdMustDiferUserName)
        {
          if (this.m_Nombre.ToLower().IndexOf(value.ToLower(), 0) != -1 ||
            this.m_Apellidos.ToLower().IndexOf(value.ToLower(), 0) != -1 ||
            this.m_Email.ToLower().IndexOf(value.ToLower(), 0) != -1 ||
            string.Concat(this.m_Nombre, this.m_Apellidos, this.m_Email).ToLower().IndexOf(value.ToLower(), 0) != -1)
          {
            //string str = string.Concat(this.m_Nombre,this.m_Apellidos,this.m_Email);
            //System.Diagnostics.Debug.WriteLine();

            throw new System.Exception("El elemento \"Contraseña\" no puede ser igual o estar contenida dentro de los valores " +
              "de \"Nombre\", \"Apellidos\" ó \"e_mail\" porque así lo indica la configuración de este servicio.");
          }
        }
        this.m_Passwd = value;
        this.dsEntidad.Tables[0].Rows[0]["Passwd"] = value;
      }
    }


    /// <summary>
    /// Fecha de nacimiento. Devolverá Exception si la fecha indica que la persona no cumple con los años de vida
    /// necesarios para hacer uso del servicio según lo establecido en el fichero Web.config.
    /// </summary>
    public System.DateTime FechaNac
    {
      get { return this.m_FechaNac; }
      set
      {
        if (value.AddYears(Settings.MinAgeToUseThisService) > System.DateTime.Now)
        {
          throw new System.Exception("El elemento \"Fecha de nacimiento\" indica que no cumple con la edad requerida (" +
            Settings.MinAgeToUseThisService + " años) para hacer uso de este servicio.");
        }
        this.m_FechaNac = value;
        this.dsEntidad.Tables[0].Rows[0]["FechaNac"] = value;
      }
    }


    /// <summary>
    /// Sexo. No tiene validación porque se deriva de uno de los dos valores posibles en la estructura "enuSexo".
    /// </summary>
    public enuSexo Sexo
    {
      get { return this.m_Sexo; }
      set
      {
        this.m_Sexo = value;
        this.dsEntidad.Tables[0].Rows[0]["Sexo"] = value;
      }
    }


    /// <summary>
    /// Id de categoría ocupacional. Devolverá exception si el valor no existe en la tabla "tbl_CatOcupacional".
    /// </summary>
    public int IdCatOcupacional
    {
      get { return this.m_IdCatOcupacional; }
      // Revisar luego si es necesario comprobar aquí que este ID exista en la base de datos.
      set
      {
        // Chequear si existe el Id en la tabla tbl_CatOcupacional.
        CatOcupacional catOcup = new CatOcupacional();
        if (!catOcup.existByID(value)) { throw new System.Exception("El ID de \"Categoría Ocupacional\" " + value + " no está registrado."); }
        this.m_IdCatOcupacional = value;
        this.dsEntidad.Tables[0].Rows[0]["IdCatOcupacional"] = value;
      }
    }


    /// <summary>
    /// Fecha de registro. Devolverá Exception si la fecha es mayor que la actual.
    /// </summary>
    public System.DateTime FechaRegistro
    {
      get { return this.m_FechaRegistro; }
      set
      {
        if (value > System.DateTime.Now)
        {
          throw new System.Exception("La \"Fecha de registro\" no puede ser mayor que la actual.");
        }
        this.m_FechaRegistro = value;
        this.dsEntidad.Tables[0].Rows[0]["FechaRegistro"] = value;
      }
    }


    /// <summary>
    /// Habilitado. No tiene validación porque puede ser uno de los dos valores del tipo de datos boolean.
    /// </summary>
    public bool Habilitado
    {
      get { return this.m_Habilitado; }
      set
      {
        this.m_Habilitado = value;
        this.dsEntidad.Tables[0].Rows[0]["Habilitado"] = value;
      }
    }


    /// <summary>
    /// Intereses. Devolverá Exception si es una cadena vacía.
    /// </summary>
    public string DescripIntereses
    {
      get { return this.m_DescripIntereses; }
      set
      {
        this.m_DescripIntereses = value;
        this.dsEntidad.Tables[0].Rows[0]["DescripIntereses"] = value;
      }
    }

    #endregion "FIN DE PROCEDIMIENTOS PROPERTY"

    #region "ENUMERACIONES"

    public enum enuSexo { F, M };

    #endregion "FIN DE ENUMERACIONES"

    #region "PROCEDIMIENTOS PRIVADOS"

    protected sealed override void ActualizarCampos()
    {
      /*
        Solo para cuando el dsEntidad (heredado de la clase abstracta Entidad) 
        contenga el registro del perfil actual.
      */
      if (this.dsEntidad.Tables[0].Rows.Count == 1)
      {
        System.Object[] data = this.dsEntidad.Tables[0].Rows[0].ItemArray;
        this.m_ID = System.Convert.ToInt32(data.GetValue(0));
        this.m_Nombre = data.GetValue(1).ToString();
        this.m_Apellidos = data.GetValue(2).ToString();
        this.m_Email = data.GetValue(3).ToString();
        this.m_Passwd = data.GetValue(4).ToString();
        this.m_FechaNac = System.Convert.ToDateTime(data.GetValue(5));
        this.m_Sexo = data.GetValue(6).ToString() == "M" ? Perfil.enuSexo.M : Perfil.enuSexo.F;
        this.IdCatOcupacional = System.Convert.ToInt32(data.GetValue(7));
        this.m_FechaRegistro = System.Convert.ToDateTime(data.GetValue(8));
        this.m_Habilitado = System.Convert.ToBoolean(data.GetValue(9));
      }
    }


    /*
    /// <summary>
    /// Comprueba si existe la dirección de correo que se pasa como valor del parámetro "e_mail"
    /// </summary>
    /// <param name="e_mail">Dirección de correo que se necesita comprobar.</param>
    /// <returns>TRUE si existe la dirección de correo, en otro caso FALSE.</returns>
    private int getIdByEmail(string e_mail)
    {
      System.Object[] Args = {e_mail};
      Perfil newPerfil = new Perfil();
      // Devuelve el ID del perfil al que pertenece el Email.
      System.Object result = newPerfil.getValue("IDxEmail", Args);
      newPerfil = null;
      return result!=System.DBNull.Value?(int)result:0;
    }
    */


    #endregion "FIN DE PROCEDIMIENTOS PRIVADOS"

    #region "PROCEDIMIENTOS PÚBLICOS"

    /// <summary>
    /// Enviar correo de notificación para nuevo registro de usuario o para cuando ha olvidado su contraseña.
    /// </summary>
    /// <returns>TRUE si es enviada la notificación, de otra manera FALSE.</returns>
    public bool sendWelcomeEmail()
    {
      try
      {
        string ServiceShortName = Settings.ServiceShortName;
        string ServiceFullName = Settings.ServiceFullName;
        string AzuPassProfileMgrURL = Settings.AzuPassProfileMgrURL;
        string[] aryServiceAdmins = Settings.wsAdmins;
        string ServiceAdmins = string.Empty;

        for (int i = 0; i <= (aryServiceAdmins.Length - 1); i++)
        {
          ServiceAdmins = i != 0 ? ServiceAdmins + ", " : "";
          // <a href="mailto:user@dominio.cu?subject=Asunto&body=Cuerpo del mensaje">mailto sintaxis</a>
          ServiceAdmins = ServiceAdmins + "<a href=\"mailto:" + aryServiceAdmins[i] + "?subject=Sobre servicio " + ServiceShortName + "&body=Hola, soy " + Nombre + " " + Apellidos + " y estoy registrado a través del servicio " + ServiceShortName + ", le escribo porque...\">" + aryServiceAdmins[i] + "</a>";
          //System.Diagnostics.Debug.WriteLine(ServiceAdmins);
        }


        // Cadena que representa el TAB.
        string strTab = System.Web.UI.HtmlTextWriter.DefaultTabString;
        // Las comillas dobles.
        //char strQuote = System.Web.UI.HtmlTextWriter.DoubleQuoteChar;
        // Nueva línea
        string strNewLine = System.Environment.NewLine;
        System.Text.StringBuilder sbHTML = new System.Text.StringBuilder();

        // La base de los links que contiene el correo.
        string strContentBase = AzuPassProfileMgrURL; //HttpContext.Current.Request.Url.ToString().Substring(0, HttpContext.Current.Request.Url.ToString().IndexOf(HttpContext.Current.Request.ApplicationPath)+HttpContext.Current.Request.ApplicationPath.Length+1);

        // Estilos para poner en los elementos style porque algunos clientes web de mensajería no entienden de estilos empotrados, tiene que ser en-línea.
        string style_body = "font-size: 8pt; color: #000000; font-family: Verdana; text-decoration: none";
        string style_td_title = "font-size: 10pt; color: #47639a; font-weight: bold; font-family: Verdana; background-color: #f0f8ff; border-right: #47639a 1px solid; border-top: #47639a 1px solid; border-left: #47639a 1px solid; border-bottom: #47639a 1px solid; padding: 2px 2px 2px 2px";
        string style_td = "font-size: 8pt; color: #000000; font-family: Verdana; text-decoration: none; padding-right: 2px; padding-left: 2px; padding-bottom: 2px; margin: 2px; padding-top: 2px";
        string style_td_content = "font-size: 8pt; color: #000000; font-family: Verdana; text-decoration: none; padding: 2px 2px 2px 2px; margin-top: 2px background-color: #ffffff; border-right: #47639a 1px solid; border-left: #47639a 1px solid; border-bottom: #47639a 1px solid";

        sbHTML.Append("<html>" + strNewLine);
        sbHTML.Append(strTab + "<head>" + strNewLine);
        sbHTML.Append(strTab + strTab + "<title>Bienvenido a: " + ServiceFullName + "</title>" + strNewLine);
        sbHTML.Append(strTab + strTab + "<meta http-equiv=\"Content-Language\" content=\"es\">" + strNewLine);
        sbHTML.Append(strTab + strTab + "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=iso-8859-1\">" + strNewLine);
        sbHTML.Append(strTab + "</head>" + strNewLine);
        sbHTML.Append(strTab + "<body style=\"" + style_body + "\" topmargin=\"0\" leftmargin=\"0\" bottommargin=\"0\" rightmargin=\"0\"> " + strNewLine);
        sbHTML.Append(strTab + strTab + "<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\">" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + "<tr>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + strTab + "<td style=\"" + style_td_title + "\" valign=\"middle\" colspan=\"3\"><b>Bienvenido a: " + ServiceFullName + "</b><img border=\"0\" src=\"" + Settings.ApplicationBaseURL + "imagenes/logo_small.gif\"></td>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + "</tr>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + "<tr>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + strTab + "<td style=\"" + style_td_content + "\" colspan=\"3\">" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + strTab + strTab + Sexo.ToString() == "F" ? "Estimada" : "Estimado " + this.m_Nombre + " " + this.m_Apellidos + ":<br>usted ha sido registrado como usuario del " + ServiceFullName + "." + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + strTab + strTab + "A continuación le notificamos los datos necesarios para hacer uso de éste." + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + strTab + "</td>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + "</tr>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + "<tr>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + strTab + "<td style=\"" + style_td + "\" width=\"20%\" valign=\"top\"><b>Nombre</b></td>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + strTab + "<td style=\"" + style_td + "\" width=\"30%\" valign=\"top\"><b>Valor</b></td>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + strTab + "<td style=\"" + style_td + "\" width=\"50%\" valign=\"top\"><b>Explicación</b></td>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + "</tr>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + "<tr>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + strTab + "<td style=\"" + style_td + "\" width=\"20%\" valign=\"top\">Dirección de corrreo:</td>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + strTab + "<td style=\"" + style_td + "\" width=\"30%\" valign=\"top\">" + this.m_Email + "</td>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + strTab + "<td style=\"" + style_td + "\" width=\"50%\" valign=\"top\">La dirección de correo se empleará como nombre de usuario.</td>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + "</tr>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + "<tr>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + strTab + "<td style=\"" + style_td + "\" width=\"20%\" valign=\"top\">Contraseña:</td>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + strTab + "<td style=\"" + style_td + "\" width=\"30%\" valign=\"top\">" + Crypto.Decrypt(this.m_Passwd) + "</td>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + strTab + "<td style=\"" + style_td + "\" width=\"50%\" valign=\"top\">Contraseña de acceso.</td>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + "</tr>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + "<tr>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + strTab + "<td valign=\"middle\" colspan=\"3\"><hr size=\"1\" color=\"#47639a\" width=\"100%\"></td>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + "</tr>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + "<tr>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + strTab + "<td style=\"" + style_td + "\" valign=\"middle\" colspan=\"3\">" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + strTab + strTab + "Puede utilizar la aplicación <a href=\"" + AzuPassProfileMgrURL + "\" target=\"_blank\">Administrador de perfiles AzuPass <img border=\"0\" src=\"" + Settings.AzuPassProfileMgrURL + "/imagenes/logo_small.gif\"></a>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + strTab + strTab + "para modificar datos de su perfil, su contraseña y dirección electrónica según como estime necesario y también para ver un registro de sus propias visitas a otros servicio o aplicaciones que hagan uso de " + Settings.ServiceShortName + "." + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + strTab + "</td>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + "</tr>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + "<tr>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + strTab + "<td style=\"" + style_td + "\" valign=\"middle\" colspan=\"3\">" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + strTab + strTab + "<br>Atentamente,<br>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + strTab + strTab + "Administradores del servicio: " + ServiceAdmins + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + strTab + "</td>" + strNewLine);
        sbHTML.Append(strTab + strTab + strTab + "</tr>" + strNewLine);
        sbHTML.Append(strTab + strTab + "</table>" + strNewLine);
        sbHTML.Append("<br>" + strNewLine);
        sbHTML.Append(strTab + strTab + Settings.getHTML_FootCopyright() + strNewLine);
        sbHTML.Append(strTab + "</body>" + strNewLine);
        sbHTML.Append("</html>");


        /*
        MailAddress from = new MailAddress("ben@contoso.com");
        MailAddress to = new MailAddress("Jane@contoso.com");
        MailMessage message = new MailMessage(from, to);
        message.Subject = "Using the SmtpClient class.";
        message.Body = @"Using this feature, you can send an e-mail message from an application very easily.";
        SmtpClient client = new SmtpClient(server);
        Console.WriteLine("Sending an e-mail message to {0} by using SMTP host {1} port {2}.",
        to.ToString(), client.Host, client.Port);
        client.Send(message);
        */

        System.Net.Mail.MailMessage mailMsg = new System.Net.Mail.MailMessage(
          Settings.SMTPService.User,
          this.m_Email,
          "Notificación de registro: " + Settings.ServiceShortName,
          @sbHTML.ToString());

        mailMsg.BodyEncoding = System.Text.Encoding.GetEncoding("iso-8859-1");
        mailMsg.Priority = System.Net.Mail.MailPriority.Normal;
        mailMsg.Headers["Reply-To"] = "no-replay";
        //mailMsg.Subject = "Notificación de registro: " + wsSettings.ServiceShortName;
        //mailMsg.AlternateViews.Clear();
        mailMsg.AlternateViews.Add(System.Net.Mail.AlternateView.CreateAlternateViewFromString(@sbHTML.ToString(), System.Text.Encoding.GetEncoding("iso-8859-1"), System.Net.Mime.MediaTypeNames.Text.Html));

        //mailMsg.BodyFormat = System.Net.Mail.MailFormat.Html;
        //mailMsg.UrlContentBase = strContentBase;
        //mailMsg.Body = @sbHTML.ToString();
        System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(Settings.SMTPService.Server);
        try
        {
          if (Settings.SMTPService.Auth)
          {
            System.Net.NetworkCredential basicAuthenticationInfo = new System.Net.NetworkCredential(
              Settings.SMTPService.User,
              Settings.SMTPService.Passwd);

            client.UseDefaultCredentials = false;
            client.Credentials = basicAuthenticationInfo;
          }
        }
        catch (System.Exception Ex)
        {
          ProcessError processError = new ProcessError();
          processError.GuardarError(Ex, "Perfil", "sendWellcomeMessage");
          processError = null;
          // No hacer nada más para cuando no se pueden establecer los parámetros de Authenticación SMTP.
        }
        client.Send(mailMsg);
        //System.Net.Mail.SmtpMail.Send(mailMsg);
        mailMsg = null;
        return true;
      }
      catch (System.Exception Ex)
      {
        // Registrar Error aqu;i de manera detallada a través de la clase ProcessError.
        ProcessError processError = new ProcessError();
        processError.GuardarError(Ex, "Perfil", "sendWellcomeMessage", "Administrador de perfiles AzuPass", Settings.AzuPassProfileMgrURL, this.ID);
        processError = null;
        return false;
      }
    }

    public bool ResendPasswd(string e_mail)
    {
      System.Object[] Args = { null, e_mail.ToLower() };
      Perfil perfil = new Perfil();
      // Devuelve el ID del Perfil al que pertenece el Email.
      System.Object result = perfil.getValue("IDxEmail", Args);
      //System.Diagnostics.Debug.WriteLine(result);
      if (result == System.DBNull.Value)
      {
        throw new System.Exception("No existe ningún perfil que corresponda con el Correo-e: \"" + e_mail + "\", por tanto no se ha enviado la contraseña a dicha dirección.");
      }
      perfil.ID = (int)result;
      return perfil.sendWelcomeEmail();
    }

    /// <summary>
    /// Validar credenciales del usuario. Se emplea para cuando se van a realizar operaciones que lo requieran.
    /// Ejemplo: para cambiar los datos del perfil del usuario es necesario que sea identificado correctamente.
    /// Este devolverá el ID del usuario para el cual se están validando las credenciales si la validación es 
    /// exitosa, sino devolverá cero(0).
    /// </summary>
    /// <param name="Email"></param>
    /// <param name="Passwd"></param>
    /// <returns>ID del usuario para el cual se están validando las credenciales si la validación es exitosa, 
    /// de lo contrario devuelve cero(0).</returns>
    public int Identificar(string Email, string Passwd, bool isForLogOn)
    {
      System.Object[] Args = { null, Email, Crypto.Encrypt(Passwd), isForLogOn };
      System.Object result = this.getValue("IDxEmailPasswd", Args);

      if (result != System.DBNull.Value)
      {
        return (int)result;
      }
      else
      {
        return 0;
      }
    }


    /// <summary>
    /// Validar las credenciales del usuario para inicio de sesión, éste registrará también el 
    /// suceso en la tabla "tbl_RegistroUso" a nombre del usuario que inicia sesión y devilverá
    /// el ID del usuario que está iniciando sesión.
    /// </summary>
    /// <param name="e_mail"></param>
    /// <param name="Passwd"></param>
    /// <param name="clientAppName">Nombre de la aplicación cliente desde donde se intenta iniciar sesión.</param>
    /// <param name="clientAppURL">URL de la aplicación cliente desde donde se intenta iniciar sesión.</param>
    /// <returns></returns>
    public int LogOn(string e_mail, string Passwd, string clientAppName, string clientAppURL)
    {
      // Validar la URL de la aplicación que se está utilizando.


      System.Object[] Args = { null, e_mail, Passwd, clientAppName, clientAppURL };
      System.Object result = this.getValue("IDxEmailPasswd_Plus", Args);
      if (result != System.DBNull.Value)
      {
        return (int)result;
      }
      else
      {
        return 0;
      }
    }


    /// <summary>
    /// Devuelve un DataSet con la lista con el nombre y URL de los sitios favoritos del usuario al 
    /// que pertenezca el perfil según el IdPerfil que se pasa como parámetro. El número de registros
    /// está limitado por el valor que se pase en listTop.
    /// </summary>
    /// <param name="IdPerfil"></param>
    /// <param name="listTop"></param>
    /// <returns></returns>
    public System.Data.DataSet getFavoritos(int IdPerfil, int listTop)
    {
      //IdPerfil_Favoritos
      System.Data.DataSet ds = this.getFiltered("IdPerfil_Favoritos", IdPerfil);
      /*
      ds.Tables[0].Columns.Remove("Desde");
      ds.Tables[0].Columns.Remove("Hasta");
      */

      if ((ds.Tables[0].Rows.Count > listTop) & (listTop > 0))
      {
        System.Data.DataSet dsResult = ds.Clone();
        for (int i = 0; i <= (listTop - 1); i++)
        {
          dsResult.Tables[0].NewRow();
          dsResult.Tables[0].Rows.Add(ds.Tables[0].Rows[i].ItemArray);
        }
        dsResult.Tables[0].TableName = "Favoritos";
        return dsResult;
      }

      ds.Tables[0].TableName = "Favoritos";
      return ds;
    }


    #region "PROCEDIMIENTOS PARA VALIDACIONES"


    /// <summary>
    /// Comprueba el formato de la dirección electrónica que se pasa como valor del parámetro e_mail y verifica que 
    /// pertenezca a uno de los dominios de correo en el Web.config o se encuentre dentro de un subdominio si corresponde.
    /// </summary>
    /// <param name="e_mail">Dirección de correo que se necesita comprobar.</param>
    /// <returns>TRUE si la dirección de correo es válida, en otro caso FALSE.</returns>
    public static bool isValidEmail(string e_mail)
    {
      bool validEmail = false;
      string[] regxEmail = Settings.regExpForEmail;
      for (int i = 0; i <= (regxEmail.Length - 1); i++)
      {
        //System.Diagnostics.Debug.WriteLine("Email: " + e_mail + " Patern: " + rgxEmail[i]);
        if (System.Text.RegularExpressions.Regex.IsMatch(e_mail, regxEmail[i]))
        {
          validEmail = true;
          break;
        }
      }
      return validEmail;
    }


    /// <summary>
    /// Comprueba si existe la dirección de correo que se pasa como valor del parámetro "e_mail"
    /// </summary>
    /// <param name="e_mail">Dirección de correo que se necesita comprobar.</param>
    /// <returns>Entero que indica la cantidad de ocurrencias encontradas.</returns>
    public static bool existEmail(string e_mail)
    {
      System.Object[] Args = { null, e_mail.ToLower() };
      Perfil perfil = new Perfil();
      // Devuelve el ID del Perfil al que pertenece el Email.
      System.Object result = perfil.getValue("IDxEmail", Args);
      //System.Diagnostics.Debug.WriteLine(result);
      perfil = null;
      return (result == System.DBNull.Value ? false : true);
    }


    /// <summary>
    /// Comprueba que el valor de la fecha que se pasa en el parámetro FechaNac sea tan atrás en el tiempo como 
    /// años se especifiquen en el Web.config para hacer uso de este servicio.
    /// </summary>
    /// <param name="FechaNac"></param>
    /// <returns></returns>
    public static bool valFechaNac(System.DateTime FechaNac)
    {
      if (FechaNac.AddYears(Settings.MinAgeToUseThisService) > System.DateTime.Now)
      {
        return false;
      }
      return true;
    }

    
    #endregion "FIN DE PROCEDIMIENTOS PARA VALIDACIONES"

    #endregion "FIN DE PROCEDIMIENTOS PÚBLICOS"

    #region "PROCEDIMIENTOS OBSOLETOS"

    #endregion "FIN DE PROCEDIMIENTOS OBSOLETOS"

  } // Fin de la clase Perfil

} // Fin del namespace
