using System.Collections.Generic;

namespace AmministrazioneTrasparente.Models
{
    public class ReportModel
    {
        public string Question { get; set; }
        public List<ResponseModel> Responses { get; set; }
    }
}