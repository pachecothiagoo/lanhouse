using System.Net.Http.Json;
using System.Text.Json;

namespace LanHouse.Reservas.Infra
{
    /// <summary>
    /// Base para chamadas REST a outros microsserviços.
    /// A URL de cada serviço vem da configuração (Servicos:NomeDoServico),
    /// que no Render é definida por variável de ambiente (Servicos__NomeDoServico).
    /// </summary>
    public abstract class ServicoExternoBase
    {
        // Timeout alto: no plano Free do Render o serviço "dorme" e leva ~50s para acordar.
        private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(120) };

        private readonly string _nome;
        private readonly string _baseUrl;

        protected ServicoExternoBase(string nome)
        {
            _nome = nome;
            var url = GeradorDeServicos.Configuracao[$"Servicos:{nome}"];
            if (string.IsNullOrWhiteSpace(url))
                throw new InvalidOperationException($"URL do serviço '{nome}' não configurada (Servicos:{nome}).");
            _baseUrl = url.TrimEnd('/');
        }

        protected async Task<T> EnviarAsync<T>(HttpMethod metodo, string caminho, object? corpo = null)
        {
            using var resposta = await EnviarBrutoAsync(metodo, caminho, corpo);
            return (await resposta.Content.ReadFromJsonAsync<T>())!;
        }

        protected async Task EnviarAsync(HttpMethod metodo, string caminho, object? corpo = null)
        {
            using var resposta = await EnviarBrutoAsync(metodo, caminho, corpo);
        }

        private async Task<HttpResponseMessage> EnviarBrutoAsync(HttpMethod metodo, string caminho, object? corpo)
        {
            using var requisicao = new HttpRequestMessage(metodo, _baseUrl + caminho);
            if (corpo != null)
                requisicao.Content = JsonContent.Create(corpo);

            var resposta = await Http.SendAsync(requisicao);
            if (!resposta.IsSuccessStatusCode)
            {
                var texto = await resposta.Content.ReadAsStringAsync();
                var status = (int)resposta.StatusCode;
                resposta.Dispose();
                throw new Exception($"{_nome}: {ExtrairMensagem(texto)} (HTTP {status})");
            }
            return resposta;
        }

        private static string ExtrairMensagem(string corpo)
        {
            try
            {
                using var doc = JsonDocument.Parse(corpo);
                if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                    doc.RootElement.TryGetProperty("erro", out var erro))
                    return erro.GetString() ?? corpo;
            }
            catch (JsonException) { }
            return corpo;
        }
    }
}
