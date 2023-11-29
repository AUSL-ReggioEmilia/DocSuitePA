using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Collaborations
{
    public class CollaborationDraftMap : EntityTypeConfiguration<CollaborationDraft>
    {
        public CollaborationDraftMap()
            : base()
        {
            ToTable("CollaborationDraft");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Data)
               .HasColumnName("Data")
               .IsRequired();

            Property(x => x.Description)
               .HasColumnName("Description")
               .IsRequired();

            Property(x => x.DraftType)
               .HasColumnName("DraftType")
               .IsRequired();

            Property(x => x.IsActive)
               .HasColumnName("IsActive")
               .IsRequired();

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.RegistrationUser)
               .HasColumnName("RegistrationUser")
               .IsRequired();

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);
            #endregion

            #region [ Configure Navigation Properties ]
            HasOptional(t => t.DocumentUnit)
                .WithOptionalDependent()
                .Map(m => m.MapKey("IdDocumentUnit"));

            HasOptional(t => t.Collaboration)
                .WithOptionalDependent()
                .Map(m => m.MapKey("IdCollaboration"));
            #endregion
        }
    }
}
