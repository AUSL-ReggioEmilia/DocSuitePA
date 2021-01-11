using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.DocumentArchives
{ 
    public class DocumentSeriesItemLinkMap : EntityTypeConfiguration<DocumentSeriesItemLink>
    {
        public DocumentSeriesItemLinkMap()
            : base()
        {
            ToTable("DocumentSeriesItemLinks");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.EntityId)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.LinkType)
                .HasColumnName("LinkType")
                .IsRequired();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
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

            Ignore(x => x.EntityShortId);
            #endregion

            #region [ Configure Navigation Properties ]
            HasRequired(t => t.DocumentSeriesItem)
                .WithMany(t => t.DocumentSeriesItemLinks)
                .Map(m => m.MapKey("UniqueIdDocumentSeriesItem"));

            HasRequired(t => t.Resolution)
                .WithMany(t => t.DocumentSeriesItemLinks)
                .Map(m => m.MapKey("UniqueIdResolution"));
            #endregion
        }
    }
}
