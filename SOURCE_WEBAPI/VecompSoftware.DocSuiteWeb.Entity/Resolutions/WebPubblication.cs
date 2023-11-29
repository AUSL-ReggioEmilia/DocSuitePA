using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.Entity.Resolutions
{
    public class WebPublication : DSWBaseEntity
    {
        #region [ Constructor ]
        public WebPublication() : this(Guid.NewGuid()) { }
        public WebPublication(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]
        public string ExternalKey { get; set; }
        public int? Status { get; set; }
        public int? IDLocation { get; set; }
        public int? IDDocument { get; set; }
        public int? EnumDocument { get; set; }
        public string Descrizione { get; set; }
        public bool? IsPrivacy { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual Resolution Resolution { get; set; }
        #endregion
    }
}
