using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.PosteWeb;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.PosteWeb
{
    // Note:
    // PosteOnLineRequestMap a discriminator towards TOLRequest and it also maps common properties
    // Properties mapped here are complementary to properties mapped in PosteOnLineRequestMap
    public class TOLRequestMap : EntityTypeConfiguration<TOLRequest>
    {
        public TOLRequestMap()
        {
            #region [ Configure Properties ]
            #endregion

            #region [ Configure Navigation Properties ]
            #endregion
        }
    }
}
