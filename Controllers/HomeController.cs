using Microsoft.AspNetCore.Mvc;
using ReportWebMvc.Models; 
using ReportWebMvc.Services; 

namespace ReportWebMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly FormatadorNoticiaService _formatadorService;

        // O serviço é injetado no construtor do controller
        public HomeController(FormatadorNoticiaService formatadorService)
        {
            _formatadorService = formatadorService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Vamos começar com apenas um campo de notícia
            var viewModel = new ReportDataViewModel();
            viewModel.Noticias.Add(new NoticiaData());

            return View(viewModel);
        }

        // ACTION POST: Executada quando o formulário é enviado
        [HttpPost]
        public async Task<IActionResult> Index(ReportDataViewModel reportData)
        {
            // Remove notícias que não foram preenchidas pelo usuário
            reportData.Noticias.RemoveAll(n =>
                string.IsNullOrWhiteSpace(n.Titulo) &&
                string.IsNullOrWhiteSpace(n.Texto));

            if (reportData.Noticias.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Você deve preencher pelo menos uma notícia.");
                return View(reportData); // Retorna para a View com a mensagem de erro
            }

            // Chama o serviço para gerar o HTML
            string htmlCompleto = await _formatadorService.GerarHtmlCompletoAsync(reportData);

            var bytes = System.Text.Encoding.UTF8.GetBytes(htmlCompleto);
            return File(bytes, "text/html", "report.html");
        }
    }
}