using System.Collections.Generic;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLPC.COVID19.Models
{
    public class RequestModel
    {
        public RequestModel()
        {
            DocumentUnits = new List<DocumentUnitModel>();
        }

        public AccountModel User { get; set; }
        public ICollection<DocumentUnitModel> DocumentUnits { get; set; }
    }
}
