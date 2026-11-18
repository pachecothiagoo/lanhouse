using LanHouse.Reservas.DTO;
using LanHouse.Reservas.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace LanHouse.Reservas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController : ApiBaseController
    {
        private readonly ServReserva _servReserva;

        public ReservasController()
        {
            _servReserva = new ServReserva();
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] ReservaCriarDTO dto)
        {
            try
            {
                var criada = await _servReserva.Criar(dto);
                return Created($"/api/reservas/{criada.Id}", criada);
            }
            catch (Exception e) { return TratarErro(e); }
        }

        [HttpGet]
        public IActionResult Listar([FromQuery] int? clienteId, [FromQuery] string? status)
        {
            try { return Ok(_servReserva.Listar(clienteId, status)); }
            catch (Exception e) { return TratarErro(e); }
        }

        [HttpGet("{id:int}")]
        public IActionResult Obter(int id)
        {
            try { return Ok(_servReserva.Obter(id)); }
            catch (Exception e) { return TratarErro(e); }
        }

        [HttpPost("{id:int}/iniciar-sessao")]
        public async Task<IActionResult> IniciarSessao(int id)
        {
            try { return Ok(await _servReserva.IniciarSessao(id)); }
            catch (Exception e) { return TratarErro(e); }
        }

        [HttpPost("{id:int}/finalizar")]
        public async Task<IActionResult> Finalizar(int id)
        {
            try { return Ok(await _servReserva.Finalizar(id)); }
            catch (Exception e) { return TratarErro(e); }
        }

        [HttpPost("{id:int}/cancelar")]
        public async Task<IActionResult> Cancelar(int id)
        {
            try { return Ok(await _servReserva.Cancelar(id)); }
            catch (Exception e) { return TratarErro(e); }
        }
    }
}
