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

        [HttpPost]
        public async Task<IActionResult> Index(ReportDataViewModel reportData)
        {
            // A l�gica de remover not�cias vazias continua a mesma
            reportData.Noticias.RemoveAll(n =>
                string.IsNullOrWhiteSpace(n.Titulo) &&
                string.IsNullOrWhiteSpace(n.Texto));

            if (reportData.Noticias.Count == 0)
            {
                // Retorna um JSON indicando o erro
                return Json(new { success = false, message = "Voc� deve preencher pelo menos uma not�cia." });
            }

            // Chama o servi�o para gerar o HTML
            string htmlCompleto = await _formatadorService.GerarHtmlCompletoAsync(reportData);

            // EM VEZ DE RETORNAR UM ARQUIVO, RETORNA UM JSON COM O C�DIGO
            return Json(new { success = true, htmlContent = htmlCompleto });
        }
    }
}