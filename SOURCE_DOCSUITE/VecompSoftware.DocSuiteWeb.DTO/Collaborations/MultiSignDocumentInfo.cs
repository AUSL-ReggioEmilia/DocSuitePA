using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using VecompSoftware.Services.Biblos;
using VecompSoftware.Services.Biblos.Models;

namespace VecompSoftware.DocSuiteWeb.DTO.Collaborations
{
    public class MultiSignDocumentInfo
    {
        #region [ Constructors ]
        public MultiSignDocumentInfo()
        {
            Signers = new List<string>();
        }

        public MultiSignDocumentInfo(DocumentInfo doc)
            : this()
        {
            this.DocumentInfo = doc;
        }

        public MultiSignDocumentInfo(FileInfo fileInfo)
            : this()
        {
            this.DocumentInfo = new FileDocumentInfo(fileInfo);
            this.IdOwner = DocumentInfo.Name.Substring(0, DocumentInfo.Name.LastIndexOf("§"));
            this.DocumentInfo.Name = DocumentInfo.Name.Substring(DocumentInfo.Name.LastIndexOf("§") + 1);
        }

        public MultiSignDocumentInfo(NameValueCollection items)
            : this()
        {
            this.DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(items);
            this.IdOwner = items["IdOwner"];
            this.Description = items["Description"];
            this.DocType = items["DocType"];
            this.Mandatory = Convert.ToBoolean(items["Mandatory"]);
            this.MandatorySelectable = Convert.ToBoolean(items["MandatorySelectable"]);
            this.GroupCode = items["GroupCode"];
            this.EffectiveSigner= items["EffectiveSigner"];
        }

        #endregion

        #region [ Properties ]
        public string IdOwner { get; set; }
        public string DocType { get; set; }
        public string Description { get; set; }
        public bool Mandatory { get; set; }
        public bool MandatorySelectable { get; set; }
        public string GroupCode { get; set; }
        public string EffectiveSigner { get; set; }
        public DocumentInfo DocumentInfo { get; set; }
        public ICollection<string> Signers { get; set; }

        public string Serialized
        {
            get
            {
                NameValueCollection temp = ToQueryString();
                return String.Join("&", temp.AllKeys.Select(t => String.Format("{0}={1}", t, temp[t])).ToList());
            }
        }

        #endregion

        #region [ Methods ]

        public NameValueCollection ToQueryString()
        {
            NameValueCollection tor = DocumentInfo.ToQueryString();
            tor.Add("DocType", DocType);
            tor.Add("Description", Description);
            tor.Add("IdOwner", IdOwner);
            tor.Add("Mandatory", Mandatory.ToString());
            tor.Add("MandatorySelectable", MandatorySelectable.ToString());
            tor.Add("GroupCode", GroupCode);
            tor.Add("EffectiveSigner", EffectiveSigner);
            return tor;
        }
        #endregion

    }
}
