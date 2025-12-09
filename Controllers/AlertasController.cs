using Microsoft.AspNetCore.Mvc;
using SeguridadPatrimonial_Backend.Models;

namespace SeguridadPatrimonial_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertasController : ControllerBase
    {
        // POST api/alertas
        [HttpPost]
        public IActionResult Post([FromBody] string mensajeCrudo)
        {
            if (string.IsNullOrWhiteSpace(mensajeCrudo))
                return BadRequest("Mensaje vacío.");

            // Crear objeto alerta ya parseado
            var alerta = Alerta.CrearDesdeMensaje(mensajeCrudo);

            // Guardar en SQL
            Alerta.Guardar(alerta);

            return Ok(new
            {
                message = "Alerta guardada correctamente",
                unidad = alerta.Unidad,
                tipo = alerta.Tipo,
                timestamp = alerta.TimestampWialon
            });
        }
    }
}
