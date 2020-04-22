using System;

namespace VecompSoftware.DocSuiteWeb.API
{
    internal class OrgDeptDTO : IOrgDeptDTO
    {

        #region [ Properties ]

        public string CodUO { get; set; }
        public string CodUOPadre { get; set; }
        public string Nome { get; set; }
        public DateTime? DataCreazione { get; set; }
        public DateTime? DataDismissione { get; set; }
        public DateTime? DataUltimoUpdate { get; set; }
        public DateTime? DataUltimoJeep { get; set; }

        #endregion

    }
}
