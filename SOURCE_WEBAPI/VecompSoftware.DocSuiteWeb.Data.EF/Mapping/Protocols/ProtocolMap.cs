using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Protocols
{
    public class ProtocolMap : EntityTypeConfiguration<Protocol>
    {
        public ProtocolMap()
            : base()
        {
            ToTable("Protocol");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Number)
               .HasColumnName("Number")
               .IsRequired();

            Property(x => x.Year)
               .HasColumnName("Year")
               .IsRequired();

            Property(x => x.Object)
               .HasColumnName("Object")
               .IsRequired();

            Property(x => x.ObjectChangeReason)
               .HasColumnName("ObjectChangeReason")
               .IsOptional();

            Property(x => x.DocumentDate)
               .HasColumnName("DocumentDate")
               .IsOptional();

            Property(x => x.DocumentProtocol)
               .HasColumnName("DocumentProtocol")
               .IsOptional();

            Property(x => x.IdDocument)
               .HasColumnName("idDocument")
               .IsOptional();

            Property(x => x.IdAttachments)
               .HasColumnName("idAttachments")
               .IsOptional();

            Property(x => x.DocumentCode)
               .HasColumnName("DocumentCode")
               .IsRequired();

            Property(x => x.IdStatus)
               .HasColumnName("idStatus")
               .IsRequired();

            Property(x => x.LastChangedReason)
               .HasColumnName("LastChangedReason")
               .IsOptional();

            Property(x => x.AlternativeRecipient)
               .HasColumnName("AlternativeRecipient")
               .IsOptional();

            Property(x => x.CheckPublication)
               .HasColumnName("CheckPublication")
               .IsOptional();

            Property(x => x.JournalDate)
               .HasColumnName("JournalDate")
               .IsOptional();

            Property(x => x.ConservationStatus)
               .HasColumnName("ConservationStatus")
               .IsRequired();

            Property(x => x.LastConservationDate)
               .HasColumnName("LastConservationDate")
               .IsOptional();

            Property(x => x.HasConservatedDocs)
               .HasColumnName("HasConservatedDocs")
               .IsOptional();

            Property(x => x.IdAnnexed)
               .HasColumnName("idAnnexed")
               .IsOptional();

            Property(x => x.HandlerDate)
               .HasColumnName("HandlerDate")
               .IsOptional();

            Property(x => x.Modified)
               .HasColumnName("Modified")
               .IsOptional();

            Property(x => x.IdHummingBird)
               .HasColumnName("IdHummingBird")
               .IsOptional();

            Property(x => x.ProtocolCheckDate)
               .HasColumnName("ProtocolCheckDate")
               .IsOptional();

            Property(x => x.TdIdDocument)
               .HasColumnName("TdIdDocument")
               .IsOptional();

            Property(x => x.TDError)
               .HasColumnName("TDError")
               .IsOptional();

            Property(x => x.DocAreaStatus)
               .HasColumnName("DocAreaStatus")
               .IsOptional();

            Property(x => x.DocAreaStatusDesc)
               .HasColumnName("DocAreaStatusDesc")
               .IsOptional();

            Property(x => x.IdProtocolKind)
               .HasColumnName("idProtocolKind")
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

            Property(x => x.IdProtocolJournalLog)
              .HasColumnName("IdProtocolJournalLog")
              .IsOptional();

            Property(x => x.DematerialisationChainId)
                .HasColumnName("DematerialisationChainId")
                .IsOptional();

            Property(x => x.Timestamp)
              .HasColumnName("Timestamp")
              .IsRequired();

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.WorkflowName)
                .Ignore(x => x.IdWorkflowActivity)
                .Ignore(x => x.WorkflowActions);
            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Location)
                    .WithMany(t => t.Protocols)
                    .Map(m => m.MapKey("idLocation"));

            HasOptional(t => t.AttachLocation)
                    .WithMany(t => t.AttachProtocols)
                    .Map(m => m.MapKey("idAttachLocation"));

            HasRequired(t => t.Container)
                    .WithMany(t => t.Protocols)
                    .Map(m => m.MapKey("idContainer"));

            HasRequired(t => t.Category)
                    .WithMany(t => t.Protocols)
                    .Map(m => m.MapKey("IdCategoryAPI"));

            HasRequired(t => t.ProtocolType)
                    .WithMany(t => t.Protocols)
                    .Map(m => m.MapKey("idType"));

            HasOptional(t => t.DocType)
                    .WithMany(t => t.Protocols)
                    .Map(m => m.MapKey("idDocType"));

            HasMany(t => t.Messages)
                    .WithMany(t => t.Protocols)
                    .Map(m => m.ToTable("ProtocolMessage")
                               .MapLeftKey("UniqueIdProtocol")
                               .MapRightKey("IdMessage"));

            HasMany(t => t.DocumentSeriesItems)
                    .WithMany(t => t.Protocols)
                    .Map(m => m.ToTable("ProtocolDocumentSeriesItems")
                               .MapLeftKey("UniqueIdProtocol")
                               .MapRightKey("UniqueIdDocumentSeriesItem"));

            #endregion
        }
    }
}
