using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiblosDS.LegalExtension.AdminPortal.Models
{
  public class DocumentItem
  {
    public Guid IdDocument { get; set; }
    public string Name { get; set; }
    public string DocumentHash { get; set; }
    public DateTime? DateMain { get; set; }

  }
}