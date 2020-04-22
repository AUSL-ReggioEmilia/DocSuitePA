using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.DocumentUnits
{
    public class DocumentUnitChainMap : EntityTypeConfiguration<DocumentUnitChain>
    {
        public DocumentUnitChainMap()
            : base()
        {
            ToTable("DocumentUnitChains", "cqrs");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdDocumentUnitChain")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.DocumentName)
                .HasColumnName("DocumentName")
                .IsOptional();

            Property(x => x.ArchiveName)
                .HasColumnName("ArchiveName")
                .IsRequired();

            Property(x => x.IdArchiveChain)
                .HasColumnName("IdArchiveChain")
                .IsRequired();

            Property(x => x.ChainType)
                .HasColumnName("ChainType")
                .IsRequired();

            Property(x => x.DocumentLabel)
                .HasColumnName("DocumentLabel")
                .IsOptional();

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

            Ignore(x => x.EntityShortId)
                .Ignore(x => x.EntityId);

            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.DocumentUnit)
                .WithMany(t => t.DocumentUnitChains)
                .Map(m => m.MapKey("IdDocumentUnit"));

            #endregion
        }
    }
}
