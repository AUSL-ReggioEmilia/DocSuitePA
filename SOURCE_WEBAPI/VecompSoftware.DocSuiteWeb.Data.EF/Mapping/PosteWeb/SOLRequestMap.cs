using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.PosteWeb;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.PosteWeb
{
    // Note:
    // LOLRequest is a child of PosteOnLineRequest. 
    // PosteOnLineRequestMap a discriminator towards SOLRequest and it also maps common properties
    // This class is empty on purpose to keep the convention.
    public class SOLRequestMap : EntityTypeConfiguration<SOLRequest>
    {
        public SOLRequestMap()
        {
            #region [ Configure Properties ]
            #endregion

            #region [ Configure Navigation Properties ]
            #endregion
        }
    }
}
