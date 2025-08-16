// CÓDIGO COMPLETO DO FormatadorNoticiaService.cs
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using ReportWebMvc.Models; // Atualize o using para o namespace dos Models

namespace ReportWebMvc.Services
{
    // As classes para desserializar o JSON da Gemini.
    public class Content { public List<Part> parts { get; set; } public string role { get; set; } }
    public class Part { public string text { get; set; } }
    public class GeminiCandidate { public Content content { get; set; } }
    public class GeminiRoot { public List<GeminiCandidate> candidates { get; set; } }

    public class FormatadorNoticiaService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;

        public FormatadorNoticiaService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiKey = configuration["Gemini:ApiKey"]!;
        }

        // O método principal que gera o HTML foi renomeado para refletir o novo Model
        public async Task<string> GerarHtmlCompletoAsync(ReportDataViewModel reportData)
        {
            string paragrafosProcessados = "";
            string textoNavegacao = "";

            for (int i = 0; i < reportData.Noticias.Count; i++)
            {
                var noticia = reportData.Noticias[i];

                string? textoDaApi = await ChamarGeminiApiAsync(noticia.Texto!);
                if (string.IsNullOrEmpty(textoDaApi))
                {
                    textoDaApi = noticia.Texto;
                }

                string pattern = @"https?:\/\/[^\s]+";
                MatchCollection links = Regex.Matches(noticia.Texto!, pattern);

                string blocoHtmlNoticia = FormatarBlocoHtml(textoDaApi!, noticia.Titulo!, noticia.Subtitulo!, i + 1, links);
                paragrafosProcessados += blocoHtmlNoticia;
                textoNavegacao += GerarLinkNavegacao(noticia.Titulo!, i + 1);
            }

            string htmlFinal = MontarTemplateFinal(reportData, textoNavegacao, paragrafosProcessados);
            return htmlFinal;
        }

        // O restante do código (ChamarGeminiApiAsync, FormatarBlocoHtml, GerarLinkNavegacao, MontarTemplateFinal)
        // é EXATAMENTE O MESMO da versão Razor Pages. Apenas certifique-se que o último
        // método, MontarTemplateFinal, aceite `ReportDataViewModel` como parâmetro.
        private async Task<string?> ChamarGeminiApiAsync(string texto)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new Exception("Chave da API do Gemini não configurada em appsettings.json");
            }

            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_apiKey}";
            var requestData = new
            {
                contents = new[] {
                    new { parts = new[] {
                        new { text = $"Separe cada parágrafo por um '|'. Certifique-se de que nenhum parágrafo ultrapasse 700 caracteres. Elimine espaços extras entre os parágrafos. Substitua **penas** links reais (URLs válidas que comecem com 'http' ou 'www.') pelo símbolo '|@'. Não crie links fictícios nem altere partes do texto que não contenham links. Mantenha a estrutura original do texto, sem reescrever frases ou palavras, mesmo que não façam sentido. Apenas me retorne a resposta processada. Segue o texto: '{texto}'" }
                    }}
                }
            };

            var httpClient = _httpClientFactory.CreateClient();
            string jsonRequest = JsonConvert.SerializeObject(requestData);
            StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var geminiResponse = JsonConvert.DeserializeObject<GeminiRoot>(responseBody);
                return geminiResponse?.candidates?.FirstOrDefault()?.content?.parts?.FirstOrDefault()?.text;
            }

            return null; // Retorna nulo se a chamada falhar
        }

        // Lógica do seu FormatarTexto.cs
        private string FormatarBlocoHtml(string txt, string tituloPrincipal, string subTitulo, int tema, MatchCollection links)
        {
            txt = txt.Replace("@", "<div style=\"text-align: center; font-size: 18px;\"><img src=\"@\" alt=\"\" height=\"400\"></div>");

            foreach (Match match in links)
            {
                int index = txt.IndexOf("@");
                if (index != -1)
                {
                    txt = txt.Substring(0, index) + match.Value + txt.Substring(index + 1);
                }
            }

            string[] paragrafos = txt.Split('|');
            string paragrafosTxt = "";

            string pattern = @"\([^)]*\)"; // Regex para capturar o autor.
            Match autor = Regex.Match(tituloPrincipal, pattern);
            string tituloPrincipalFormatado = Regex.Replace(tituloPrincipal, pattern, ""); // Removendo o autor do título principal.

            foreach (var par in paragrafos)
            {
                paragrafosTxt += $"\n                        <p style='font-size: 18px; color: #8b8279;'>{par}</p>";
            }

            return $@"
                <div>
                    <div style='padding: 20px; color: #8b8279; font-size: 14px; line-height: 1.6; text-align: justify;'>
                    <h2 style='font-size: 30px; color: #96d600; margin-bottom: 10px;'>
                        <a name='tema{tema}'></a>
                        {tituloPrincipalFormatado}
                        <br>
                        <i>{autor.Value}</i>
                    </h2>
                        <h3 style='font-size: 23px; color: #797979; margin-bottom: 10px;'>{subTitulo}</h3>
                            {paragrafosTxt}
                        <a href='#menu-rapido' style='color: #416ce2; text-decoration: none; font-size: 20px;'>Voltar ao topo</a>
                    </div>
                </div>
";
        }

        // Lógica do seu Program.cs para os links
        private string GerarLinkNavegacao(string titulo, int index)
        {
            return $@"<div><div style='text-align: center; font-size: 18px;'><p style='margin: 10px 0;'><a href='#tema{index}' style='color: #416ce2; text-decoration: none;'>{titulo}</a></p></div></div>";
        }
        private string MontarTemplateFinal(ReportDataViewModel data, string nav, string content)
        {
            return $@"
                <!DOCTYPE html>
                <html lang='pt-br'><head><meta charset='UTF-8'><title>Report Semanal</title></head>
                <body style='margin: 0; padding: 0; background-color: #fff; font-family: Roboto, sans-serif;'>
                    <div width='100%' style='background-color: #ffffff; padding: 20px; font-family: Roboto, sans-serif;'>
                        <div align='center'>
                            <div width='100%' style='background-color: #ffffff; border-radius: 8px; overflow: hidden;'>
                                <div align='center' style='background-color: #ffffff; padding: 20px;'>
                                    <img src='https://media.licdn.com/dms/image/C4E0BAQHg6skx7qepXA/company-logo_200_200/0/1630575744260/rbinvestimentos_logo?e=2147483647&v=beta&t=CsiTFWSfHetHC7xWz8UWBTAnfnSiBRbPmRe67GaNbjo' alt='Logo RB Investimentos' style='max-width: 100%; height: auto;'>
                                </div>
                                <div align='center' style='padding-top: 20px; color: #8b8279;'>
                                    <h1 style='margin: 0; font-size: 30px; color: #8b8279;' id='menu-rapido'><a name='menu-rapido'></a>RB Investimentos</h1>
                                    <h2 style='margin: 10px 0; font-size: 29px; color: #8b8279;'>Report Semanal {data.DiaIReport}-{data.DiaFReport}/{data.MesReport}/{data.AnoReport}</h2>
                                </div>
                                {nav}
                                {content}
                            </div>
                        </div>
                    </div>
                </body></html>";
        }
    }
}