namespace ReportWebMvc.Models
{
    public class ReportDataViewModel
    {
        public string? DiaIReport { get; set; }
        public string? DiaFReport { get; set; }
        public string? MesReport { get; set; }
        public string? AnoReport { get; set; }
        public List<NoticiaData> Noticias { get; set; } = new List<NoticiaData>();
    }

    public class NoticiaData
    {
        public string? Titulo { get; set; }
        public string? Subtitulo { get; set; }
        public string? Texto { get; set; }
    }
}