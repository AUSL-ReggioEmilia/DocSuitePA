using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.DocumentArchives
{
    public class DocumentSeriesItemRoleMap : EntityTypeConfiguration<DocumentSeriesItemRole>
    {
        public DocumentSeriesItemRoleMap()
            : base()
        {
            ToTable("DocumentSeriesItemRole");
            //Primary Key
            HasKey(t => t.EntityId);

            #region [ Configure Properties ]
            Property(x => x.EntityId)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.LinkType)
                .HasColumnName("LinkType")
                .IsRequired();

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .IsRequired();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.RegistrationUser)
                .Ignore(x => x.RegistrationDate)
                .Ignore(x => x.LastChangedDate)
                .Ignore(x => x.LastChangedUser);
            #endregion

            #region [ Configure Navigation Properties ]
            HasRequired(t => t.DocumentSeriesItem)
                .WithMany(t => t.DocumentSeriesItemRoles)
                .Map(m => m.MapKey("UniqueIdDocumentSeriesItem"));

            HasRequired(t => t.Role)
                .WithMany(t => t.DocumentSeriesItemRoles)
                .Map(m => m.MapKey("IdRole"));
            #endregion
        }
    }
}
