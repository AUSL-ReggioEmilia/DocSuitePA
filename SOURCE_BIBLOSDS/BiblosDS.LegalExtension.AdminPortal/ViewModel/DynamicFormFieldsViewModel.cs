using System;
using System.Collections.Generic;

namespace BiblosDS.LegalExtension.AdminPortal.ViewModel
{
    public class DynamicFormFieldsViewModel
    {
        public DynamicFormFieldsViewModel()
        {
            DynamicControlViewModels = new List<DynamicControlViewModel>();
            FromDate = new DateTime(DateTime.Today.Year, 1, 1);
            ToDate = DateTime.Today;
        }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public ICollection<DynamicControlViewModel> DynamicControlViewModels { get; set; }
    }
}