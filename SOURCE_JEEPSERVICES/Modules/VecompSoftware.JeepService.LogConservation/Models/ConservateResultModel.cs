using System.Collections.Generic;

namespace VecompSoftware.JeepService.LogConservation.Models
{
    public class ConservateResultModel
    {
        public ConservateResultModel()
        {
            Errors = new List<string>();
        }

        public ICollection<string> Errors { get; set; }

        public bool HasError
        {
            get
            {
                return Errors != null && Errors.Count > 0;
            }
        }
    }
}
