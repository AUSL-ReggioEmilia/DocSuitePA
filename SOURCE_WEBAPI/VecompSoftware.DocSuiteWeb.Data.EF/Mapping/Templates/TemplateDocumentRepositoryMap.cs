using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Templates;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Templates
{
    public class TemplateDocumentRepositoryMap : EntityTypeConfiguration<TemplateDocumentRepository>
    {
        public TemplateDocumentRepositoryMap() : base()
        {
            ToTable("TemplateDocumentRepositories");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdTemplateDocumentRepository")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Status)
               .HasColumnName("IdStatus")
               .IsOptional();

            Property(x => x.QualityTag)
               .HasColumnName("QualityTag")
               .IsRequired();

            Property(x => x.IdArchiveChain)
                .HasColumnName("IdArchiveChain")
                .IsRequired();

            Property(x => x.Name)
                .HasColumnName("Name")
                .IsOptional();

            Property(x => x.Object)
                .HasColumnName("Object")
                .IsOptional();

            Property(t => t.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(t => t.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional()
                .HasMaxLength(256);

            Property(t => t.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired()
                .HasMaxLength(256);

            Property(t => t.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.Version)
                .HasColumnName("Version")
                .IsRequired();

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);
            #endregion

            #region [ Configure Navigation Properties ]

            #endregion
        }
    }
}
