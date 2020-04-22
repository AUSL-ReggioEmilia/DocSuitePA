using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Collaborations
{
    public class CollaborationMap : EntityTypeConfiguration<Collaboration>
    {
        public CollaborationMap() : base()
        {
            ToTable("Collaboration");
            //Primary Key
            HasKey(t => t.EntityId);

            #region [ Configure Properties ]
            Property(x => x.EntityId)
                .HasColumnName("IdCollaboration")
                .HasColumnType("numeric")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(t => t.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional()
                .HasMaxLength(256);

            Property(t => t.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired()
                .HasMaxLength(256);

            Property(t => t.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.DocumentType)
                .HasColumnName("DocumentType")
                .IsOptional();

            Property(x => x.AlertDate)
                .HasColumnName("AlertDate")
                .IsOptional();

            Property(x => x.IdPriority)
                .HasColumnName("IdPriority")
                .IsOptional();

            Property(x => x.IdStatus)
                .HasColumnName("IdStatus")
                .IsOptional();

            Property(x => x.MemorandumDate)
                .HasColumnName("MemorandumDate")
                .IsOptional();

            Property(x => x.Note)
                .HasColumnName("Note")
                .IsOptional();

            Property(x => x.Subject)
                .HasColumnName("Object")
                .IsOptional();

            Property(x => x.Year)
                .HasColumnName("Year")
                .IsOptional();

            Property(x => x.Number)
               .HasColumnName("Number")
               .IsOptional();

            Property(x => x.TemplateName)
               .HasColumnName("TemplateName")
               .IsRequired();

            Property(x => x.PublicationDate)
                .HasColumnName("PublicationDate")
                .IsOptional();

            Property(x => x.PublicationUser)
                .HasColumnName("PublicationUser")
                .IsOptional();

            Property(x => x.RegistrationEmail)
                .HasColumnName("RegistrationEMail")
                .IsOptional();

            Property(x => x.RegistrationName)
                .HasColumnName("RegistrationName")
                .IsOptional();

            Property(x => x.SignCount)
                .HasColumnName("SignCount")
                .IsOptional();

            Property(x => x.SourceProtocolNumber)
                .HasColumnName("SourceProtocolNumber")
                .IsOptional();

            Property(x => x.SourceProtocolYear)
                .HasColumnName("SourceProtocolYear")
                .IsOptional();

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .IsRequired();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.WorkflowName)
                .Ignore(x => x.IdWorkflowActivity)
                .Ignore(x => x.WorkflowActions);
            #endregion

            #region [ Configure Navigation Properties ]
            HasOptional(x => x.Desk)
                .WithMany(x => x.Collaborations)
                .Map(m => m.MapKey("IdDesk"));

            HasOptional(x => x.Location)
             .WithMany(x => x.Collaborations)
             .Map(m => m.MapKey("idLocation"));

            HasOptional(t => t.DocumentSeriesItem)
                .WithOptionalDependent()
                .Map(m => m.MapKey("IdDocumentSeriesItem"));

            HasOptional(t => t.Resolution)
                .WithOptionalDependent()
                .Map(m => m.MapKey("ResolutionUniqueId"));

            HasOptional(t => t.WorkflowInstance)
                .WithMany(t => t.Collaborations)
                .Map(m => m.MapKey("IdWorkflowInstance"));

            #endregion
        }
    }
}
