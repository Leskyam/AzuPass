using System;

namespace TEICOCF.WebServices
{


	/// <summary>
	/// Descripción breve de Perfil.
	/// </summary>
	public abstract class Entidad
	{

		#region "CONTRUCTORES"

		public Entidad(string tblName)
		{
            this.tblName = tblName;
		}

		#endregion "FIN DE CONTRUCTORES"

		#region "VARIABLES PRIVADAS"

		private string m_tblName;
		protected int m_ID;
    protected System.Data.DataSet m_dsEntidad;
		private DataGen m_daCom = null;

		#endregion "FIN DE VARIABLES PRIVADAS"

		#region "PROCEDIMIENTOS PROPERTY"

		/// <summary>
		/// ID para este registro en la base de datos.
		/// </summary>
		public int ID
		{
			get{ return this.m_ID; }
			set
			{
				/*
				 	Revisar si es necesario aquí traer los DataTable que corresponden 
					para que la entidad sea editada o implementar esto en otro procedimiento 
					y traer aquí solo el registro de la entidad en cuestión.
				*/
				this.m_ID = value;
				this.m_dsEntidad = this.daCom().getDataSet("pa_" + this.tblName + "_GE", this.m_ID);
				if(this.m_dsEntidad.Tables[0].Rows.Count==0) // No existe registro con el ID establecido.
				{
					this.m_ID = 0;
					this.m_dsEntidad.Tables[0].Rows.Add(this.dsEntidad.Tables[0].NewRow());
				}
				else
				{
					// Para cuando el DataTable 0 del DataSet contiene el registro de un perfil existente.
					this.m_dsEntidad.Tables[0].Rows[0]["ID"] = this.m_ID;
					ActualizarCampos();
				}
			}
		}

		/// <summary>
		/// Nombre de la tabla en la base de datos.
		/// </summary>
		protected string tblName
		{
			get{ return this.m_tblName; }
			set {this.m_tblName = value;}
		}

		public System.Data.DataSet dsEntidad
		{
			get {return this.m_dsEntidad;}
			set {this.m_dsEntidad = value;}
		}
        
		#endregion "FIN DE PROCEDIMIENTOS PROPERTY"

		#region "PROCEDIMIENTOS PROTECTED"

		protected DataGen daCom()
		{
			if(null==this.m_daCom)
			{

				string ServerTypeConfigured = wsSettings.dbServerType.ToUpper();

				if(ServerTypeConfigured == wsSettings.DataServerTypes.MSSql.ToString().ToUpper())
				{
					this.m_daCom = new DataMSSql();
				}
				else if(ServerTypeConfigured == wsSettings.DataServerTypes.MySQL.ToString().ToUpper())
				{
					this.m_daCom = new DataMySQL();
				}
				else if(ServerTypeConfigured == wsSettings.DataServerTypes.PostgreSQL.ToString().ToUpper())
				{
					this.m_daCom = new DataPostgreSQL();
				}
				else
				{
					throw new System.Exception("El valor de la llave 'dbServerType' en el fichero de configuración 'Web.config' no es la correcta.");
				}

			}

			return m_daCom;

		}

		#endregion "FIN DE PROCEDIMIENTOS PROTECTED"

		#region "PROCEDIMINETOS PRIVADOS"

		/// <summary>
		/// Implementar en la clase que deriva de ésta un procedimiento para actualizar 
		/// los valores de los miembros que corresponden con los campos en la base de 
		/// datos para que contengan los valores del DataSet dsEntidad que es donde se 
		/// almacenan los datos del objeto "Entidad" actual.
		/// </summary>
		protected abstract void ActualizarCampos();


		#endregion "FIN DE PROCEDIMINETOS PRIVADOS"

		#region "PROCEDIMIENTOS PÚBLICOS"

		/// <summary>
		/// Devuelve el valor de ID recien agregago a la entidad en cuestión.
		/// </summary>
		/// <param name="dsPerfil"></param>
		/// <returns></returns>
		public virtual int Agregar()
		{
			if(this.ID>0){throw new System.Exception("El valor de la propiedad ID debe ser cero \"0\" para ejecutar el procedimiento \"Agregar\"");}
			System.Object[] info = this.m_dsEntidad.Tables[0].Rows[0].ItemArray;
			/*
				Asignar el ID retiornado a la propiedad ID para llenar el dsEntidad 
				y poder correr procedimiento para "ActualizarCampos" locales con los 
				valores del perfil en la base de datos.
			*/ 
			this.ID = this.daCom().Ejecutar("pa_" + this.tblName + "_I", info);
			// No es necesario porque corre al establecer el valor de la propiedad this.ID
			// ActualizarCampos();
			return this.ID;
		}

		/// <summary>
		/// Actualiza y devuelve el valor del ID actualizado.
		/// </summary>
		/// <returns></returns>
		public int Actualizar()
		{
			if(this.ID<=0){throw new System.Exception("El valor de la propiedad ID es incorrecto para utilizar el procedimiento \"Actualizar\"");}
			System.Object[] info = this.m_dsEntidad.Tables[0].Rows[0].ItemArray;
			// Asignar el ID devuelto a la propiedad para que se recarguen los valores en el DataSet dsEntidad y 
			// se actualicen los valores de los campos implementados en las clases que heredan de esta.
			this.ID = this.daCom().Ejecutar("pa_" + this.tblName + "_U", info);
			// Correr procedimiento para "ActualizarCampos" locales 
			// con los valores del perfil en la base de datos.
			// No es necesario porque corre al establecer el valor de la propiedad this.ID
			// ActualizarCampos();
			return this.ID;
		}

		/// <summary>
		/// Elimina el registro cuyo ID corresponda con el valor que almacena la propiedad "ID" y devuelve 
		/// true si procede.
		/// </summary>
		/// <returns></returns>
		public bool Eliminar()
		{
			if(this.ID<=0){throw new System.Exception("El valor de la propiedad ID es incorrecto para utilizar el procedimiento \"Eliminar\"");}
			int result = this.daCom().Ejecutar("pa_" + this.tblName + "_D", this.ID);
			if(result>0)
			{
				this.ID = 0;
				return true;
			}
			else
			{
				return false;
			}
			// Correr procedimiento para "ActualizarCampos" locales 
			// con los valores del perfil en la base de datos.
			// Esto no es necesario porque se ejecuta cuando corresponde cuando se establece el ID.
			// ActualizarCampos();
		}

		// OBTENER DATOS DE LA ENTIDAD EN CUESTIÓN

		/// <summary>
		/// Obtener, en un DataSet, lista para mostrar en cuadros combinados.
		/// </summary>
		/// <returns></returns>
		public System.Data.DataSet getList()
		{
			return this.daCom().getDataSet("pa_" + this.tblName + "_GL");
		}
		
		/// <summary>
		/// Traer un valor.
		/// </summary>
		/// <returns></returns>
		public System.Object getValue(string campo, params System.Object[] Args)
		{
			return this.daCom().getValue("pa_" + this.tblName + "_Gv" + campo, Args);
		}

		/// <summary>
		/// Traer filtrado
		/// </summary>
		/// <param name="filtro">Parte final del nombre del procedimineto que describe el filtro del mismo.</param>
		/// <returns></returns>
		public System.Data.DataSet getFiltered(string filtro)
		{
			return this.daCom().getDataSet("pa_" + this.tblName + "_Gx" + filtro);
		}
		
		/// <summary>
		/// Traer filtrado
		/// </summary>
		/// <param name="filtro">Parte final del nombre del procedimineto que describe el filtro del mismo.</param>
		/// <param name="Args">Arreglo object con los valores de los parámetro que espera el procedimiento almacenado.</param>
		/// <returns></returns>
		public System.Data.DataSet getFiltered(string filtro, params System.Object[] Args)
		{
			return this.daCom().getDataSet("pa_" + this.tblName + "_Gx" + filtro, Args);
		}

		#endregion "FIN DE PROCEDIMIENTOS PÚBLICOS"


		/*
		public Entidad()
		{
			//
			// TODO: agregar aquí la lógica del constructor
			//
		}
		public Entidad(string tblName)
		{
			this.tblName = tblName;
		}

		#region "VARIABLES"
    
		//Privadas
		public string tblName;
		private int m_ID;

		//Públicas
		private DataGen m_DaCom = null;
		public System.Data.DataSet dsDatos;

		#endregion "FIN DE VARIABLES"

		#region "PROCEDIMIENTOS PROPERTY"

		/// <summary>
		/// ID para este registro en la base de datos.
		/// </summary>
		public int ID
		{
			get{ return this.m_ID; }
			set
			{
				this.m_ID=value;
				dsDatos = this.DaCom().getDataSet("pa_" + this.tblName + "_GE", this.m_ID);
				if(this.dsDatos.Tables[0].Rows.Count==0)
				{
					this.dsDatos.Tables[0].Rows.Add(this.dsDatos.Tables[0].NewRow());
				}
			}
		}

		#endregion "FIN DE PROCEDIMIENTOS PROPERTY"

		#region "PROCEDIMIENTOS PÚBLICOS"
		public System.Data.DataSet getOne()
		{
			return this.DaCom().getDataSet("pa_" + this.tblName + "_G", this.m_ID);
		}
		public System.Data.DataSet getAll()
		{
			return this.DaCom().getDataSet("pa_" + this.tblName + "_GA");
		}

		#endregion "FIN DE PROCEDIMIENTOS PÚBLICOS"

		*/
	
	} // Fin de la clase

} // Fin del namespace
