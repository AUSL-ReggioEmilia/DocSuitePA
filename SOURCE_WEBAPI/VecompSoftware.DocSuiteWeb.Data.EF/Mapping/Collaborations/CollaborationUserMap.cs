using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Collaborations
{
    public class CollaborationUserMap : EntityTypeConfiguration<CollaborationUser>
    {
        public CollaborationUserMap() : base()
        {
            ToTable("CollaborationUsers");

            HasKey(k => k.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdCollaborationUser")
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

            Property(x => x.DestinationFirst)
                .HasColumnName("DestinationFirst")
                .IsOptional();

            Property(x => x.DestinationType)
                .HasColumnName("DestinationType")
                .IsOptional();

            Property(x => x.DestinationName)
                .HasColumnName("DestinationName")
                .IsOptional();

            Property(x => x.DestinationEmail)
                .HasColumnName("DestinationEMail")
                .IsOptional();

            Property(x => x.Account)
                .HasColumnName("Account")
                .IsOptional();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);
            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Collaboration)
                .WithMany(t => t.CollaborationUsers)
                .Map(m => m.MapKey("IdCollaboration"));

            HasOptional(t => t.Role)
                .WithMany(t => t.CollaborationUsers)
                .Map(m => m.MapKey("idRole"));

            #endregion
        }
    }
}
