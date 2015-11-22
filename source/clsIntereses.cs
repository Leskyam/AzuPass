using System;

namespace TEICOCF.WebServices
{
	/// <summary>
	/// Implementa las opciones específicas para el manejo de los registro de la tabla "tbl_Intereses". 
	/// Hereda de la clase "Entidad" algunos métodos como Agregar, Actualizar, Eliminar y el campo ID.
	/// </summary>
	public class Intereses : Entidad
	{
		#region "CONSTRUCTORES"

		/// <summary>
		/// Contructor. Carga el objeto como si se tratase del intento de agregar un nuevo 
		/// registro, con el valor de ID en cero(0). Pasa ala clase base "Entidad" el nombre
		/// de la tabla "tbl_Intereses", donde se almacenan los datos que maneja esta clase.
		/// </summary>
		public Intereses() : base (EntityTableName)
		{
			this.ID = 0;
		}

		/// <summary>
		/// Contructor. Carga el objeto con los valores que se corresponden con el ID que se 
		/// pasa como valor del parámetro Id y si el valor del ID no existe en la base de datos
		/// entonces carga el objeto como si se tratase del intento de agregar un nuevo registro.
		/// Pasa ala clase base "Entidad" el nombre de la tabla "tbl_Intereses", donde se almacenan 
		/// los datos que maneja esta clase.
		/// </summary>
		/// <param name="Id"></param>
		public Intereses(int Id) : base(EntityTableName)
		{
            this.ID = Id;
		}

		#endregion "FIN DE CONSTRUCTORES"

		#region "VARIABLES PRIVADAS"

		private static string EntityTableName = "tbl_Intereses";

		private int m_IdPerfil;
		private string m_Descripcion = string.Empty;

		#endregion "FIN DE VARIABLES PRIVADAS"

		#region "PROCEDIMIENTOS PROPERTY"

		/// <summary>
		/// ID del perfil del usuario al que pertenecen los intereses.
		/// </summary>
		public int IdPerfil
		{
			get {return this.m_IdPerfil;}
			set 
			{
				this.m_IdPerfil = value;
				this.dsEntidad.Tables[0].Rows[0]["IdPerfil"] = value;
			}
		}

		/// <summary>
		/// Descripción de los intereses en modo texto.
		/// </summary>
		public string Descripcion
		{
			get {return this.m_Descripcion;}
			set 
			{
				this.m_Descripcion = value;
				this.dsEntidad.Tables[0].Rows[0]["Descripcion"] = value;
			}
		}

		#endregion "FIN DE PROCEDIMIENTOS PROPERTY"

		#region "ENUMERACIONES"

		#endregion "FIN DE ENUMERACIONES"

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
				this.m_IdPerfil = System.Convert.ToInt32(data.GetValue(1));
				this.m_Descripcion = data.GetValue(2).ToString(); 
			}
		}

		#endregion "FIN DE PROCEDIMIENTOS PRIVADOS"

		#region "PROCEDIMIENTOS PUBLICOS"


		#endregion "FIN DE PROCEDIMIENTOS PUBLICOS"

	} // Fin de la clase

} // Fin del namespace
