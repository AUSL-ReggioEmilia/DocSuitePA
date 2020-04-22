using BiblosDS.Library.Common.Objects;

namespace BiblosDS.LegalExtension.AdminPortal.ViewModel.Ipda
{
    public class IpdaIndexViewModel
    {
        public Preservation Preservation { get; set; }
        public bool ToCreate { get; set; }
        public bool ToSign { get; set; }
        public bool ToClose { get; set; }        
    }
}