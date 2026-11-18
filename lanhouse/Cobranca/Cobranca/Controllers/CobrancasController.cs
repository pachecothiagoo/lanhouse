using LanHouse.Cobranca.DTO;
using LanHouse.Cobranca.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace LanHouse.Cobranca.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CobrancasController : ApiBaseController
    {
        private readonly ServCobranca _servCobranca;

        public CobrancasController()
        {
            _servCobranca = new ServCobranca();
        }

        [HttpPost]
        public IActionResult Criar([FromBody] CobrancaCriarDTO dto)
        {
            try
            {
                var criada = _servCobranca.Criar(dto);
                return Created($"/api/cobrancas/{criada.Id}", criada);
            }
            catch (Exception e) { return TratarErro(e); }
        }

        [HttpGet]
        public IActionResult Listar([FromQuery] int? clienteId, [FromQuery] string? status)
        {
            try { return Ok(_servCobranca.Listar(clienteId, status)); }
            catch (Exception e) { return TratarErro(e); }
        }

        [HttpGet("{id:int}")]
        public IActionResult Obter(int id)
        {
            try { return Ok(_servCobranca.Obter(id)); }
            catch (Exception e) { return TratarErro(e); }
        }

        [HttpGet("cliente/{clienteId:int}/saldo")]
        public IActionResult SaldoDevedor(int clienteId)
        {
            try { return Ok(_servCobranca.SaldoDevedor(clienteId)); }
            catch (Exception e) { return TratarErro(e); }
        }

        [HttpPut("{id:int}/pagar")]
        public IActionResult Pagar(int id)
        {
            try { return Ok(_servCobranca.Pagar(id)); }
            catch (Exception e) { return TratarErro(e); }
        }

        [HttpPut("{id:int}/cancelar")]
        public IActionResult Cancelar(int id)
        {
            try { return Ok(_servCobranca.Cancelar(id)); }
            catch (Exception e) { return TratarErro(e); }
        }
    }
}
