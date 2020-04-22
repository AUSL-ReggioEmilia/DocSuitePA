using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Templates;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Templates
{
    public class TemplateCollaborationMap : EntityTypeConfiguration<TemplateCollaboration>
    {
        public TemplateCollaborationMap()
            : base()
        {
            ToTable("TemplateCollaborations");
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdTemplateCollaboration")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();

            Property(x => x.Status)
                .HasColumnName("Status")
                .IsRequired();

            Property(x => x.DocumentType)
                .HasColumnName("DocumentType")
                .IsRequired();

            Property(x => x.IdPriority)
                .HasColumnName("IdPriority")
                .IsRequired();

            Property(x => x.Object)
                .HasColumnName("Object")
                .IsOptional();

            Property(x => x.Note)
                .HasColumnName("Note")
                .IsOptional();

            Property(x => x.IsLocked)
                .HasColumnName("IsLocked")
                .IsOptional();

            Property(x => x.WSDeletable)
                .HasColumnName("WSDeletable")
                .IsRequired();

            Property(x => x.WSManageable)
                .HasColumnName("WSManageable")
                .IsRequired();

            Property(x => x.JsonParameters)
                .HasColumnName("JsonParameters")
                .IsOptional();

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

            HasMany(t => t.Roles)
                .WithMany(t => t.TemplateCollaborations)
                .Map(m =>
               {
                   m.ToTable("TemplateCollaborationRoles");
                   m.MapLeftKey("IdTemplateCollaboration");
                   m.MapRightKey("idRole");
               });


            #endregion
        }
    }
}
