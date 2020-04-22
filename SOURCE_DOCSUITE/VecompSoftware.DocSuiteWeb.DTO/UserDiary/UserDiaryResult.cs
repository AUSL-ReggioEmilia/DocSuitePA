using System;

namespace VecompSoftware.DocSuiteWeb.DTO.UserDiary
{
    public class UserDiaryResult
    {
        public int Year { get; set; }
        public int Number { get; set; }
        public string Object { get; set; }
        public string Codice { get; set; }
        public DateTime LogDate { get; set; }
        public string Type { get; set; }
        public int PI { get; set; }
        public int PS { get; set; }
        public int PD { get; set; }
        public int PZ { get; set; }
        public int PM { get; set; }
        public DateTime AdoptionDate { get; set; }
        public int IsHandled { get; set; }
    }
}
