using BiblosDS.Library.Common.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BiblosDS.LegalExtension.AdminPortal.Helpers
{
  [XmlRootAttribute(ElementName = "VerifyReport", IsNullable = false)]
  public class VerifyReport
  {
    private string sourceFile = "";

    public string verifyDate { get; set; }
    public string fromDate { get; set; }
    public string toDate { get; set; }

    [XmlElementAttribute("Archive")]
    public VerifyReportArchive[] Archives { get; set; }

    public static VerifyReport Load(string fileName)
    {
      var res = XmlFile<VerifyReport>.Load(fileName, "");
      res.sourceFile = fileName;
      return res;
    }

    public void Save()
    {
      XmlFile<VerifyReport>.Serialize(this, this.sourceFile);
    }

    public void SaveAs(string fileName)
    {
      this.sourceFile = fileName;
      XmlFile<VerifyReport>.Serialize(this, fileName);
    }
  }


  public class VerifyReportArchive
  {
    public string ArchiveName { get; set; }

    [XmlElementAttribute("VerifyReportPreservation")]
    public VerifyReportPreservation[] Preservations { get; set; }
  }

 
  public class VerifyReportPreservation
  {
    public string IdPreservation { get; set; }
    public CData Name { get; set; }
    public CData Result { get; set; }
    public CData Errors { get; set; }
  }
}
