namespace VecompSoftware.StampaConforme.Models.Office.Excel
{
    public class SaveWorkbookToPdfRequest
    {
        public string SourceFilePath { get; set; }
        public string DestinationFilePath { get; set; }
        public bool ForcePortrait { get; set; }
        public int? FitToPagesTall { get; set; }
        public int? FitToPagesWide { get; set; }
    }
}
