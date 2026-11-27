namespace LanHouse.Clientes.DTO
{
    // Contrato recebido do microsserviço de Cobrança.
    public class SaldoDevedorDTO
    {
        public int ClienteId { get; set; }
        public decimal SaldoDevedor { get; set; }
    }
}
