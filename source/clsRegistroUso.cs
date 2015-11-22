using System;

namespace TEICOCF.WebServices
{
	/// <summary>
	/// Descripción breve de clsRegistroUso.
	/// </summary>
	public class RegistroUso : Entidad
	{
		#region "CONSTRUCTORES"

		/// <summary>
		/// Contructor. Carga el objeto como si se tratase del intento de agregar un nuevo 
		/// registro, con el valor de ID en cero(0). Pasa ala clase base "Entidad" el nombre
		/// de la tabla "tbl_RegistroUso", donde se almacenan los datos que maneja esta clase.
		/// </summary>
		public RegistroUso() : base(EntityTableName)
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
		public RegistroUso(int Id) : base(EntityTableName)
		{
			this.ID = Id;
		}

		#endregion "FIN DE CONTRUCTORES"

		#region "VARIABLES PRIVADAS"

		private static string EntityTableName = "tbl_RegistroUso";

		private int m_IdPerfil;
		private int m_IdApplication;
		private System.DateTime m_FechaHora;

		#endregion "FIN DE VARIABLES PRIVADAS"

		#region "PROCEDIMIENTOS PROPERTY"
		
		public int IdPerfil
		{
			get {return this.m_IdPerfil;}
			set 
			{
				this.m_IdPerfil = value;
				this.dsEntidad.Tables[0].Rows[0]["IdPerfil"] = value;
			}
		}

		public int IdApplication
		{
			get {return this.m_IdApplication;}
			set 
			{
				this.m_IdApplication = value;
				this.dsEntidad.Tables[0].Rows[0]["IdApplication"] = value;
			}
		}

		public System.DateTime FechaHora
		{
			get {return this.m_FechaHora;}
			set 
			{
				this.m_FechaHora = value;
				this.dsEntidad.Tables[0].Rows[0]["FechaHora"] = value;
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
				this.m_IdPerfil = System.Convert.ToInt32(data.GetValue(1));
				this.m_IdApplication = System.Convert.ToInt32(data.GetValue(2));
				this.m_FechaHora = System.Convert.ToDateTime(data.GetValue(3)); 
			}
		}


		#endregion "FIN DE PROCEDIMIENTOS PRIVADOS"

	
		#region "PROCEDIMIENTOS PÚBLICOS"
	
		/// <summary>
		/// Devuelve un DataSet con la lista con el nombre y URL de los sitios favoritos del usuario al 
		/// que pertenezca el perfil según el IdPerfil que se pasa como parámetro. El número de registros
		/// está limitado por el valor que se pase en listTop.
		/// </summary>
		/// <param name="IdPerfil"></param>
		/// <param name="listTop"></param>
		/// <returns></returns>
	
		public System.Data.DataSet getFavoritos(int listTop)
		{
			//IdPerfil_Favoritos
			System.Data.DataSet ds = this.getFiltered("_Favoritos");
			/*
			ds.Tables[0].Columns.Remove("Desde");
			ds.Tables[0].Columns.Remove("Hasta");
			*/

			if((ds.Tables[0].Rows.Count > listTop) & (listTop > 0))
			{
				System.Data.DataSet dsResult = ds.Clone();
				for(int i=0; i<=(listTop-1); i++)
				{
					dsResult.Tables[0].NewRow();
					dsResult.Tables[0].Rows.Add(ds.Tables[0].Rows[i].ItemArray);
				}
				dsResult.Tables[0].TableName = "ServicedApplications";
				return dsResult;
			}

			ds.Tables[0].TableName = "ServicedApplications";
			return ds;
		}

		
		#endregion "FIN DE PROCEDIMIENTOS PÚBLICOS"
	
	
	} // Fin de la clase.

} // Fin del namespace.
