using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Templates;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Templates
{
    internal class TemplateCollaborationUserMap : EntityTypeConfiguration<TemplateCollaborationUser>
    {
        public TemplateCollaborationUserMap()
            : base()
        {
            ToTable("TemplateCollaborationUsers");
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdTemplateCollaborationUser")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Account)
                .HasColumnName("Account")
                .IsOptional();

            Property(x => x.Incremental)
                .HasColumnName("Incremental")
                .IsRequired();

            Property(x => x.UserType)
                .HasColumnName("UserType")
                .IsOptional();

            Property(x => x.IsRequired)
                .HasColumnName("IsRequired")
                .IsRequired();

            Property(x => x.IsValid)
                .HasColumnName("IsValid")
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

            Ignore(x => x.EntityShortId)
                .Ignore(x => x.EntityId);
            #endregion

            #region [ Configure Navigation Properties ]
            HasRequired(x => x.TemplateCollaboration)
                .WithMany(x => x.TemplateCollaborationUsers)
                .Map(x => x.MapKey("IdTemplateCollaboration"));

            HasOptional(x => x.Role)
                .WithMany(x => x.TemplateCollaborationUsers)
                .Map(x => x.MapKey("IdRole"));
            #endregion
        }
    }
}
