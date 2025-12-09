using System;
using System.Data.SqlClient;

namespace SeguridadPatrimonial_Backend.Models
{
    public class Conexion
    {
        private static readonly string connectionString =
            "Server=sql.bsite.net\\MSSQL2016;Database=fernandosalazar23_;User Id=fernandosalazar23_;Password=jf529tsa1C#2312;";


        public static SqlConnection conx = new SqlConnection(connectionString);

        public static void AbrirConexion()
        {
            if (conx.State != System.Data.ConnectionState.Open)
            {
                conx.Open();
            }
        }

        public static void CerrarConexion()
        {
            if (conx.State == System.Data.ConnectionState.Open)
            {
                conx.Close();
            }
        }
    }
}
