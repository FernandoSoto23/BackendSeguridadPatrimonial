using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace SeguridadPatrimonial_Backend.Models
{
    public class Alerta
    {
        public int Id { get; set; }
        public string Unidad { get; set; } = "";
        public string Tipo { get; set; } = "";
        public string Mensaje { get; set; } = "";
        public DateTime? FechaIncidente { get; set; }
        public TimeSpan? HoraIncidente { get; set; }
        public DateTime? TimestampWialon { get; set; }

        // -------------------------------------------------------------
        // PARSER AUTOMÁTICO
        // -------------------------------------------------------------

        public static string ParseTipo(string mensaje)
        {
            mensaje = mensaje.ToUpper();

            if (mensaje.Contains("JAMMER")) return "JAMMER";
            if (mensaje.Contains("PÁNICO") || mensaje.Contains("PANICO")) return "PANICO";
            if (mensaje.Contains("SIN SEÑAL") || mensaje.Contains("CONNECTION LOSS")) return "SIN_SEÑAL";
            if (mensaje.Contains("DETENIDA") || mensaje.Contains("DETENIDO")) return "DETENIDA";

            return "OTRO";
        }

        public static string ParseUnidad(string mensaje)
        {
            Match m = Regex.Match(mensaje, @"M[-]?\d+");
            return m.Success ? m.Value : "DESCONOCIDA";
        }

        public static DateTime? ParseFechaHora(string mensaje)
        {
            Match m = Regex.Match(mensaje, @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}");

            if (m.Success)
                return DateTime.Parse(m.Value);

            return null;
        }

        // -------------------------------------------------------------
        // CONSTRUCTOR A PARTIR DEL MENSAJE CRUD0 DE WIALON
        // -------------------------------------------------------------

        public static Alerta CrearDesdeMensaje(string mensajeCrudo)
        {
            var timestamp = ParseFechaHora(mensajeCrudo);

            return new Alerta
            {
                Unidad = ParseUnidad(mensajeCrudo),
                Tipo = ParseTipo(mensajeCrudo),
                Mensaje = mensajeCrudo,
                FechaIncidente = timestamp?.Date,
                HoraIncidente = timestamp?.TimeOfDay,
                TimestampWialon = timestamp
            };
        }

        // -------------------------------------------------------------
        // INSERTAR EN SQL
        // -------------------------------------------------------------

        public static void Guardar(Alerta alerta)
        {
            try
            {
                Conexion.AbrirConexion();

                string sql = @"
                    INSERT INTO Alertas (unidad, tipo, mensaje, fecha_incidente, hora_incidente, timestamp_wialon)
                    VALUES (@unidad, @tipo, @mensaje, @fecha, @hora, @timestamp)
                ";

                using SqlCommand cmd = new SqlCommand(sql, Conexion.conx);

                cmd.Parameters.AddWithValue("@unidad", alerta.Unidad);
                cmd.Parameters.AddWithValue("@tipo", alerta.Tipo);
                cmd.Parameters.AddWithValue("@mensaje", alerta.Mensaje);
                cmd.Parameters.AddWithValue("@fecha", (object?)alerta.FechaIncidente ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@hora", (object?)alerta.HoraIncidente ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@timestamp", (object?)alerta.TimestampWialon ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
            finally
            {
                Conexion.CerrarConexion();
            }
        }
    }
}
