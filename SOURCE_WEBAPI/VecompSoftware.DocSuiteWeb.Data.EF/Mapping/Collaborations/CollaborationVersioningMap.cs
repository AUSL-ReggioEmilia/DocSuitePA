using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Collaborations
{
    public class CollaborationVersioningMap : EntityTypeConfiguration<CollaborationVersioning>
    {
        public CollaborationVersioningMap() : base()
        {
            ToTable("CollaborationVersioning");
            //Primary Key

            HasKey(k => k.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdCollaborationVersioning")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired()
                .HasMaxLength(256);

            Property(t => t.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.CollaborationIncremental)
                .HasColumnName("CollaborationIncremental");

            Property(x => x.Incremental)
                .HasColumnName("Incremental");

            Property(x => x.IdDocument)
                .HasColumnName("idDocument")
                .IsRequired();

            Property(x => x.DocumentName)
                .HasColumnName("DocumentName")
                .IsRequired();

            Property(x => x.IsActive)
                .HasColumnName("IsActive")
                .IsRequired();

            Property(x => x.CheckedOut)
                .HasColumnName("CheckedOut")
                .IsOptional();

            Property(x => x.CheckOutUser)
                .HasColumnName("CheckOutUser")
                .IsOptional();

            Property(x => x.CheckOutSessionId)
                .HasColumnName("CheckOutSessionId")
                .IsOptional();

            Property(x => x.CheckOutDate)
                .HasColumnName("CheckOutDate")
                .IsOptional();

            Property(x => x.DocumentChecksum)
                .HasColumnName("DocumentChecksum")
                .IsOptional();

            Property(x => x.DocumentGroup)
                .HasColumnName("DocumentGroup")
                .IsOptional();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.LastChangedDate)
                .Ignore(x => x.LastChangedUser)
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.EntityId);
            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Collaboration)
                .WithMany(t => t.CollaborationVersionings)
                .Map(m => m.MapKey("IdCollaboration"));
            #endregion
        }
    }
}
