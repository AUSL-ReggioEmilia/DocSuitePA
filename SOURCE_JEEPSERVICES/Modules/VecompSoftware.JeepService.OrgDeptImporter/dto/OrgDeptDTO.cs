using System;
using System.Data;
using VecompSoftware.DocSuiteWeb.API;

namespace VecompSoftware.JeepService
{
    internal class OrgDeptDTO : IOrgDeptDTO
    {

        #region [ Constructors ]

        public OrgDeptDTO(DataRow row)
        {
            
            Id = row.Field<int>("Id");
            CodUO = row.Field<string>("CodUO");
            CodUOPadre = row.Field<string>("CodUOPadre");
            DataCreazione = row.Field<DateTime?>("DataCreazione");
            DataDismissione = row.Field<DateTime?>("DataDismissione");
            DataUltimoJeep = row.Field<DateTime?>("DataUltimoJeep");
            DataUltimoUpdate = row.Field<DateTime?>("DataUltimoUpdate");
            Nome = row.Field<string>("Nome");
        }

        #endregion

        #region [ Properties ]

        public int Id { get; set; }
        public string CodUO { get; set; }
        public string CodUOPadre { get; set; }
        public DateTime? DataCreazione { get; set; }
        public DateTime? DataDismissione { get; set; }
        public DateTime? DataUltimoJeep { get; set; }
        public DateTime? DataUltimoUpdate { get; set; }
        public string Nome { get; set; }

        #endregion

    }
}
