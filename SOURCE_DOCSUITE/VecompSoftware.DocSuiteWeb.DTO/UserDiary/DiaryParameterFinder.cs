using System;
using VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.DocSuiteWeb.DTO.UserDiary
{
    public class DiaryParameterFinder
    {
        public DateTime? DateFrom { get; set; } = null;
        public DateTime? DateTo { get; set; } = null;
        public LogKindEnum? TypeLog { get; set; } = null;
        public String Subject { get; set; } = String.Empty;
    }
}
