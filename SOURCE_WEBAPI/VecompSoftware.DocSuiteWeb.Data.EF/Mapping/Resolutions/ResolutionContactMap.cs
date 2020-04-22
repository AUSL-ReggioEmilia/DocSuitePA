using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Resolutions
{
    public class ResolutionContactMap : EntityTypeConfiguration<ResolutionContact>
    {
        public ResolutionContactMap()
            : base()
        {
            ToTable("ResolutionContact");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.ComunicationType)
                .HasColumnName("ComunicationType")
                .IsRequired();

            Property(x => x.IdResolution)
                .HasColumnName("idResolution")
                .IsRequired();

            Property(x => x.Incremental)
                .HasColumnName("Incremental")
                .IsOptional();

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional();

            Property(x => x.RegistrationUser)
               .HasColumnName("RegistrationUser")
               .IsOptional();

            Property(x => x.Timestamp)
               .HasColumnName("Timestamp")
               .IsRequired();

            Ignore(x => x.EntityShortId)
                .Ignore(x => x.RegistrationDate)
                .Ignore(x => x.EntityId);


            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Contact)
                .WithMany(t => t.ResolutionContacts)
                .Map(m => m.MapKey("IDContact"));

            HasRequired(t => t.Resolution)
                .WithMany(t => t.ResolutionContacts)
                .Map(m => m.MapKey("UniqueIdResolution"));

            #endregion
        }
    }
}
