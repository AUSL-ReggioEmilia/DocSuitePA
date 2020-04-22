using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Templates;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Templates
{
    public class TemplateCollaborationDocumentRepositoryMap : EntityTypeConfiguration<TemplateCollaborationDocumentRepository>
    {
        public TemplateCollaborationDocumentRepositoryMap() : base()
        {
            ToTable("TemplateCollaborationDocumentRepositories");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdTemplateCollaborationDocumentRepository")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.ChainType)
                .HasColumnName("ChainType")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional();

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);
            #endregion

            #region [ Configure Navigation Properties ]
            HasRequired(t => t.TemplateCollaboration)
                .WithMany(t => t.TemplateCollaborationDocumentRepositories)
                .Map(m => m.MapKey("IdTemplateCollaboration"));

            HasRequired(t => t.TemplateDocumentRepository)
                .WithMany(t => t.TemplateCollaborationDocumentRepositories)
                .Map(m => m.MapKey("IdTemplateDocumentRepository"));
            #endregion
        }
    }
}
