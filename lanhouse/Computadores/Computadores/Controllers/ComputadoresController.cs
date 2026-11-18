using LanHouse.Computadores.DTO;
using LanHouse.Computadores.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace LanHouse.Computadores.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComputadoresController : ApiBaseController
    {
        private readonly ServComputador _servComputador;

        public ComputadoresController()
        {
            _servComputador = new ServComputador();
        }

        [HttpPost]
        public IActionResult Cadastrar([FromBody] ComputadorDTO dto)
        {
            try
            {
                var criado = _servComputador.Cadastrar(dto);
                return Created($"/api/computadores/{criado.Id}", criado);
            }
            catch (Exception e) { return TratarErro(e); }
        }

        [HttpGet]
        public IActionResult Listar([FromQuery] string? status)
        {
            try { return Ok(_servComputador.Listar(status)); }
            catch (Exception e) { return TratarErro(e); }
        }

        [HttpGet("{id:int}")]
        public IActionResult Obter(int id)
        {
            try { return Ok(_servComputador.Obter(id)); }
            catch (Exception e) { return TratarErro(e); }
        }

        [HttpPut("{id:int}/status")]
        public IActionResult AlterarStatus(int id, [FromBody] StatusDTO dto)
        {
            try { return Ok(_servComputador.AlterarStatus(id, dto.Status)); }
            catch (Exception e) { return TratarErro(e); }
        }

        [HttpPost("{id:int}/reservar")]
        public IActionResult Reservar(int id)
        {
            try { return Ok(_servComputador.Reservar(id)); }
            catch (Exception e) { return TratarErro(e); }
        }
    }
}
