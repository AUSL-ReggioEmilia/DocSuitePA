using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.DocumentArchives
{
    public class DocumentSeriesItemMap : EntityTypeConfiguration<DocumentSeriesItem>
    {
        public DocumentSeriesItemMap()
            : base()
        {
            ToTable("DocumentSeriesItem");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.EntityId)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.Year)
                .HasColumnName("Year")
                .IsOptional();

            Property(x => x.Number)
                .HasColumnName("Number")
                .IsOptional();

            Property(x => x.IdMain)
                .HasColumnName("IdMain")
                .IsOptional();

            Property(x => x.IdAnnexed)
                .HasColumnName("IdAnnexed")
                .IsOptional();

            Property(x => x.IdUnpublishedAnnexed)
                .HasColumnName("IdUnpublishedAnnexed")
                .IsOptional();

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

            Property(x => x.PublishingDate)
                .HasColumnName("PublishingDate")
                .IsOptional();

            Property(x => x.RetireDate)
                .HasColumnName("RetireDate")
                .IsOptional();

            Property(x => x.Subject)
               .HasColumnName("Subject")
               .IsOptional();

            Property(x => x.IdDocumentSeriesSubsection)
                .HasColumnName("IdDocumentSeriesSubsection")
                .IsOptional();

            Property(x => x.Status)
                .HasColumnName("Status")
                .IsOptional();

            Property(x => x.Priority)
                .HasColumnName("Priority")
                .IsOptional();

            Property(x => x.DematerialisationChainId)
                .HasColumnName("DematerialisationChainId")
                .IsOptional();

            Property(x => x.HasMainDocument)
                .HasColumnName("HasMainDocument")
                .IsOptional();

            Property(x => x.ConstraintValue)
                .HasColumnName("ConstraintValue")
                .IsOptional();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.WorkflowAutoComplete)
                .Ignore(x => x.WorkflowName)
                .Ignore(x => x.IdWorkflowActivity)
                .Ignore(x => x.WorkflowActions);

            #endregion

            #region [ Configure Navigation Properties ]

            HasOptional(t => t.DocumentSeries)
                .WithMany(t => t.DocumentSeriesItems)
                .Map(m => m.MapKey("IdDocumentSeries"));

            HasOptional(t => t.Location)
                .WithMany(t => t.DocumentSeriesItems)
                .Map(m => m.MapKey("IdLocation"));

            HasOptional(t => t.LocationAnnexed)
                .WithMany(t => t.DocumentSeriesItemAnnexes)
                .Map(m => m.MapKey("IdLocationAnnexed"));

            HasOptional(t => t.LocationUnpublishedAnnexed)
                .WithMany(t => t.DocumentSeriesItemUnpublishedAnnexes)
                .Map(m => m.MapKey("IdLocationUnpublishedAnnexed"));

            HasOptional(t => t.Category)
                .WithMany(t => t.DocumentSeriesItems)
                .Map(m => m.MapKey("IdCategory"));

            HasMany(t => t.Messages)
                    .WithMany(t => t.DocumentSeriesItems)
                    .Map(m => m.ToTable("DocumentSeriesItemMessage")
                               .MapLeftKey("UniqueIdDocumentSeriesItem")
                               .MapRightKey("IdMessage"));
            #endregion
        }
    }
}
