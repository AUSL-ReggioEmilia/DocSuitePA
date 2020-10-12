using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.PosteWeb;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.PosteWeb
{
    // Note:
    // PosteOnLineRequestMap a discriminator towards ROLRequest and it also maps common properties
    // Properties mapped here are complementary to properties mapped in PosteOnLineRequestMap
    public class ROLRequestMap : EntityTypeConfiguration<ROLRequest>
    {
        public ROLRequestMap()
        {
            #region [ Configure Properties ]
            
            //specific
            Property(x => x.DocumentName)
                .HasColumnName("DocumentName")
                .IsOptional();

            Property(x => x.DocumentMD5)
                .HasColumnName("DocumentMD5")
                .IsOptional();

            Property(x => x.DocumentPosteMD5)
                .HasColumnName("DocumentPosteMD5")
                .IsOptional();

            Property(x => x.DocumentPosteFileType)
                .HasColumnName("DocumentPosteFileType")
                .IsOptional();

            Property(x => x.IdArchiveChain)
                .HasColumnName("IdArchiveChain")
                .IsOptional();

            Property(x => x.IdArchiveChainPoste)
                .HasColumnName("IdArchiveChainPoste")
                .IsOptional();
            
            #endregion

            #region [ Configure Navigation Properties ]
            #endregion
        }
    }
}
