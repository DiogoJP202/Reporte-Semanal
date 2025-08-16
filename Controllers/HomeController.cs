using Microsoft.AspNetCore.Mvc;
using ReportWebMvc.Models; 
using ReportWebMvc.Services; 

namespace ReportWebMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly FormatadorNoticiaService _formatadorService;

        // O servi�o � injetado no construtor do controller
        public HomeController(FormatadorNoticiaService formatadorService)
        {
            _formatadorService = formatadorService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Vamos come�ar com apenas um campo de not�cia
            var viewModel = new ReportDataViewModel();
            viewModel.Noticias.Add(new NoticiaData());

            return View(viewModel);
        }

        // ACTION POST: Executada quando o formul�rio � enviado
        [HttpPost]
        public async Task<IActionResult> Index(ReportDataViewModel reportData)
        {
            // Remove not�cias que n�o foram preenchidas pelo usu�rio
            reportData.Noticias.RemoveAll(n =>
                string.IsNullOrWhiteSpace(n.Titulo) &&
                string.IsNullOrWhiteSpace(n.Texto));

            if (reportData.Noticias.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Voc� deve preencher pelo menos uma not�cia.");
                return View(reportData); // Retorna para a View com a mensagem de erro
            }

            // Chama o servi�o para gerar o HTML
            string htmlCompleto = await _formatadorService.GerarHtmlCompletoAsync(reportData);

            var bytes = System.Text.Encoding.UTF8.GetBytes(htmlCompleto);
            return File(bytes, "text/html", "report.html");
        }
    }
}