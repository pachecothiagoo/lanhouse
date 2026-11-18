using Microsoft.AspNetCore.Mvc;

namespace LanHouse.Clientes.Controllers
{
    public abstract class ApiBaseController : ControllerBase
    {
        // Padroniza o formato de erro da API: { "erro": "mensagem" }
        protected IActionResult TratarErro(Exception e)
        {
            return e switch
            {
                KeyNotFoundException => NotFound(new { erro = e.Message }),
                HttpRequestException or TaskCanceledException => StatusCode(
                    StatusCodes.Status502BadGateway,
                    new { erro = "Serviço dependente indisponível ou demorou a responder: " + e.Message }),
                _ => BadRequest(new { erro = e.Message })
            };
        }
    }
}
