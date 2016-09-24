﻿using System;

namespace TEICOCF.WebServices
{
	/// <summary>
	/// Descripción breve de Perfil.
	/// </summary>
	public class CatOcupacional : Entidad
	{
		public CatOcupacional() : base(EntityTableName)
		{
		}
		/*
		public CatOcupacional(string tblName) : base(tblName)
		{

		}
		*/

		private static string EntityTableName = "tbl_CatOcupacional";

		protected sealed override void ActualizarCampos()
		{

		}

		public bool existByID(int IdCatOcupacional)
		{
			System.Object[] Args = {IdCatOcupacional};
			System.Object result = this.daCom().getValue("pa_" + this.tblName + "_GvID", Args);
			return (result==System.DBNull.Value?false:true);
		}

	}
}
