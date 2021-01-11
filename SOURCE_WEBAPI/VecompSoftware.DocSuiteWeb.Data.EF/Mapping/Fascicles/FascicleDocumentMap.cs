using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Fascicles
{
    public class FascicleDocumentMap : EntityTypeConfiguration<FascicleDocument>
    {
        public FascicleDocumentMap()
            : base()
        {
            ToTable("FascicleDocuments");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdFascicleDocument")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

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

            Property(x => x.ChainType)
                .HasColumnName("ChainType")
                .IsRequired();

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            Property(x => x.IdArchiveChain)
                .HasColumnName("IdArchiveChain")
                .IsRequired();

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);

            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Fascicle)
                .WithMany(t => t.FascicleDocuments)
                .Map(p => p.MapKey("IdFascicle"));

            //nella 8.75 sarà da mettere required
            HasOptional(t => t.FascicleFolder)
                .WithMany(t => t.FascicleDocuments)
                .Map(m => m.MapKey("IdFascicleFolder"));
            #endregion
        }
    }
}
