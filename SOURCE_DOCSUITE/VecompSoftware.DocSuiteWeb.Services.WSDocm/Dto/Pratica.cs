using System;

namespace VecompSoftware.DocSuiteWeb.Services.WSDocm.Dto
{
    public class Pratica
    {
        #region Constructor

        public Pratica()
        {
            BiblosInfo = new BiblosInfo();
        }
        #endregion

        public BiblosInfo BiblosInfo { get; set; }

        public string XmlSource { get; set; }

        public string XmlToolbar { get; set; }

        public bool HideTreeView { get; set; }

        public string DocSuiteUrl { get; set; }

        public short? Year { get; set; }

        public int? Number { get; set; }

        public string ContainerName { get; set; }

        public string RoleName { get; set; }

        public int? CategoryCode { get; set; }

        public string CategoryFullCode { get; set; }

        public string CategoryName { get; set; }

        public string CategoryFullName { get; set; }

        public DateTime? StartDate { get; set; }

        public string ServiceNumber { get; set; }

        public string Name { get; set; }

        public string DocumentObject { get; set; }

        public string Manager { get; set; }

        public string Note { get; set; }

        public string ContactDescription { get; set; }

        public string FolderName { get; set; }

        public DateTime? FolderExpiryDate { get; set; }

        public string FolderExpiryDescription { get; set; }

        public int? FolderIncremental { get; set; }

        public string Status { get; set; }        
    }
}