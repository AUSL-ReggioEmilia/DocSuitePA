using System.ComponentModel;
using VecompSoftware.JeepService.Common;

namespace VecompSoftware.JeepService
{
    public class OrgDeptImporterParameters : JeepParametersBase
    {

#if DEBUG
        [DefaultValue("Password=Passw0rd;Persist Security Info=True;User ID=DocSuite;Initial Catalog=Protocollo_IAM;Data Source=SVILUPPO")]
#endif
        public string ConnectionString { get; set; }

#if DEBUG
        [DefaultValue("http://sviluppo/dswapi-asmn/")]
#endif
        public string APIAddress { get; set; }

#if DEBUG
        [DefaultValue(500)]
#else
        [DefaultValue(100)]
#endif
        public int MaxResults { get; set; }

    }
}
