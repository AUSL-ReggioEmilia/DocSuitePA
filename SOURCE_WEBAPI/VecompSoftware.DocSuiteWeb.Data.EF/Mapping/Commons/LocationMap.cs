using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Commons
{
    public class LocationMap : EntityTypeConfiguration<Location>
    {
        public LocationMap()
            : base()
        {
            ToTable("Location");
            //Primary Key
            HasKey(t => t.EntityShortId);

            #region [ Configure Properties ]

            Property(x => x.EntityShortId)
                .HasColumnName("idLocation")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();

            Property(x => x.DocumentServer)
                .HasColumnName("DocumentServer")
                .IsRequired();

            Property(x => x.ProtocolArchive)
                .HasColumnName("ProtBiblosDSDB")
                .IsOptional();

            Property(x => x.DossierArchive)
                .HasColumnName("DocmBiblosDSDB")
                .IsOptional();

            Property(x => x.ResolutionArchive)
                .HasColumnName("ReslBiblosDSDB")
                .IsOptional();

            Property(x => x.ConservationArchive)
                .HasColumnName("ConsBiblosDSDB")
                .IsOptional();

            Property(x => x.ConservationServer)
                .HasColumnName("ConservationServer")
                .IsOptional();

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .IsRequired();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.RegistrationDate)
                .Ignore(x => x.RegistrationUser)
                .Ignore(x => x.LastChangedDate)
                .Ignore(x => x.LastChangedUser)
                .Ignore(x => x.EntityId);
            #endregion

            #region [ Configure Navigation Properties ]
            #endregion
        }
    }
}
