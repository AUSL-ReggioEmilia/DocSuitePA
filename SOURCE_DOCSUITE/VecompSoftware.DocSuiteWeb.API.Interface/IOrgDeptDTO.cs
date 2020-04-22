using System;

namespace VecompSoftware.DocSuiteWeb.API
{
    public interface IOrgDeptDTO
    {

        #region [ Properties ]

        string CodUO { get; set; }
        string CodUOPadre { get; set; }
        string Nome { get; set; }
        DateTime? DataCreazione { get; set; }
        DateTime? DataDismissione { get; set; }
        DateTime? DataUltimoUpdate { get; set; }
        DateTime? DataUltimoJeep { get; set; }

        #endregion

    }

}
