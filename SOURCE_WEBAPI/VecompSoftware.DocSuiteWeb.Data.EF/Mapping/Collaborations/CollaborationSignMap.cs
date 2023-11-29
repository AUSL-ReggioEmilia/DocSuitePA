using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Collaborations
{
    public class CollaborationSignMap : EntityTypeConfiguration<CollaborationSign>
    {
        public CollaborationSignMap() : base()
        {
            ToTable("CollaborationSigns");
            //Primary Key
            HasKey(k => k.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdCollaborationSign")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

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

            Property(x => x.Incremental)
                .HasColumnName("Incremental")
                .IsRequired();

            Property(x => x.IsActive)
                .HasColumnName("IsActive")
                .IsRequired();

            Property(x => x.IdStatus)
                .HasColumnName("idStatus")
                .IsOptional();

            Property(x => x.SignUser)
                .HasColumnName("SignUser")
                .IsOptional();

            Property(x => x.SignName)
                .HasColumnName("SignName")
                .IsOptional();

            Property(x => x.SignEmail)
                .HasColumnName("SignEMail")
                .IsOptional();

            Property(x => x.SignDate)
                .HasColumnName("SignDate")
                .IsOptional();

            Property(x => x.IsRequired)
                .HasColumnName("IsRequired")
                .IsOptional();

            Property(x => x.IsAbsent)
                .HasColumnName("IsAbsent")
                .IsOptional();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);
            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Collaboration)
                .WithMany(t => t.CollaborationSigns)
                .Map(m => m.MapKey("IdCollaboration"));
            #endregion
        }
    }
}
