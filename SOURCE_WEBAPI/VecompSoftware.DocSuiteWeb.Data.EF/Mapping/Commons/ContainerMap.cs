using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Commons
{
    public class ContainerMap : EntityTypeConfiguration<Container>
    {
        public ContainerMap()
            : base()
        {
            ToTable("Container");
            //Primary Key
            HasKey(t => t.EntityShortId);

            #region [ Configure Properties ]

            Property(x => x.EntityShortId)
                .HasColumnName("idContainer");

            Property(x => x.Name)
                .HasColumnName("Name")
                .IsOptional();

            Property(x => x.Note)
                .HasColumnName("Note")
                .IsOptional();

            Property(x => x.isActive)
                .HasColumnName("isActive")
                .IsRequired();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsOptional();

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional();

            Property(x => x.idArchive)
                .HasColumnName("idArchive")
                .IsOptional();

            Property(x => x.Privacy)
                .HasColumnName("Privacy")
                .IsOptional();

            Property(x => x.HeadingFrontalino)
                .HasColumnName("HeadingFrontalino")
                .IsOptional();

            Property(x => x.HeadingLetter)
                .HasColumnName("HeadingLetter")
                .IsOptional();

            Property(x => x.PrivacyLevel)
               .HasColumnName("PrivacyLevel")
               .IsRequired();

            Property(x => x.PrivacyEnabled)
               .HasColumnName("PrivacyEnabled")
               .IsRequired();

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .IsRequired();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityId);

            MapToStoredProcedures();

            #endregion

            #region [ Configure Navigation Properties ]

            HasOptional(t => t.DocmLocation)
                .WithMany(t => t.DocmContainers)
                .Map(m => m.MapKey("DocmLocation"));

            HasOptional(t => t.ProtLocation)
                .WithMany(t => t.ProtContainers)
                .Map(m => m.MapKey("ProtLocation"));

            HasOptional(t => t.ReslLocation)
                .WithMany(t => t.ReslContainers)
                .Map(m => m.MapKey("ReslLocation"));

            HasOptional(t => t.DeskLocation)
                .WithMany(t => t.DeskContainers)
                .Map(m => m.MapKey("DeskLocation"));

            HasOptional(t => t.UDSLocation)
                .WithMany(t => t.UDSContainers)
                .Map(m => m.MapKey("UDSLocation"));

            HasOptional(t => t.ProtAttachLocation)
                .WithMany(t => t.ProtAttachContainers)
                .Map(m => m.MapKey("ProtAttachLocation"));

            HasOptional(t => t.DocumentSeriesLocation)
                .WithMany(t => t.DocumentSeriesContainers)
                .Map(m => m.MapKey("DocumentSeriesLocation"));

            HasOptional(t => t.DocumentSeriesAnnexedLocation)
                .WithMany(t => t.DocumentSeriesAnnexedContainers)
                .Map(m => m.MapKey("DocumentSeriesAnnexedLocation"));

            HasOptional(t => t.DocumentSeriesUnpublishedAnnexedLocation)
                .WithMany(t => t.DocumentSeriesUnpublishedAnnexedContainers)
                .Map(m => m.MapKey("DocumentSeriesUnpublishedAnnexedLocation"));

            #endregion
        }
    }
}
