using LanHouse.Clientes.DTO;
using LanHouse.Clientes.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace LanHouse.Clientes.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ApiBaseController
    {
        private readonly ServCliente _servCliente;

        public ClientesController()
        {
            _servCliente = new ServCliente();
        }

        [HttpPost]
        public IActionResult Cadastrar([FromBody] ClienteDTO dto)
        {
            try
            {
                var criado = _servCliente.Cadastrar(dto);
                return Created($"/api/clientes/{criado.Id}", criado);
            }
            catch (Exception e) { return TratarErro(e); }
        }

        [HttpGet]
        public IActionResult Consultar([FromQuery] string? nome, [FromQuery] string? telefone)
        {
            try { return Ok(_servCliente.Consultar(nome, telefone)); }
            catch (Exception e) { return TratarErro(e); }
        }

        [HttpGet("{id:int}")]
        public IActionResult Obter(int id)
        {
            try { return Ok(_servCliente.Obter(id)); }
            catch (Exception e) { return TratarErro(e); }
        }

        [HttpPut("{id:int}/situacao")]
        public IActionResult AlterarSituacao(int id, [FromBody] SituacaoDTO dto)
        {
            try { return Ok(_servCliente.AlterarSituacao(id, dto.Situacao)); }
            catch (Exception e) { return TratarErro(e); }
        }

        [HttpGet("{id:int}/saldo-devedor")]
        public async Task<IActionResult> SaldoDevedor(int id)
        {
            try { return Ok(await _servCliente.SaldoDevedor(id)); }
            catch (Exception e) { return TratarErro(e); }
        }
    }
}
