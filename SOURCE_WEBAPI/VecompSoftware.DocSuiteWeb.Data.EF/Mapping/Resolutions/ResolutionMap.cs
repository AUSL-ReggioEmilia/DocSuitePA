using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Resolutions
{
    public class ResolutionMap : EntityTypeConfiguration<Resolution>
    {
        public ResolutionMap()
            : base()
        {
            ToTable("Resolution");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.EntityId)
                .HasColumnName("IdResolution");

            Property(t => t.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional()
                .HasMaxLength(256);

            Property(x => x.Number)
               .HasColumnName("Number")
               .IsOptional();

            Property(x => x.Year)
               .HasColumnName("Year")
               .IsOptional();

            Property(x => x.Object)
               .HasColumnName("Object");

            Property(x => x.ServiceNumber)
               .HasColumnName("ServiceNumber");

            Property(x => x.AdoptionDate)
               .HasColumnName("AdoptionDate")
               .IsOptional();

            Property(x => x.AlternativeAssignee)
               .HasColumnName("AlternativeAssignee");

            Property(x => x.AlternativeManager)
               .HasColumnName("AlternativeManager");

            Property(x => x.AlternativeProposer)
               .HasColumnName("AlternativeProposer");

            Property(x => x.AlternativeRecipient)
               .HasColumnName("AlternativeRecipient");

            Property(x => x.AdoptionUser)
               .HasColumnName("AdoptionUser");

            Property(x => x.ConfirmDate)
               .HasColumnName("ConfirmDate")
               .IsOptional();

            Property(x => x.ConfirmUser)
               .HasColumnName("ConfirmUser");

            Property(x => x.EffectivenessDate)
               .HasColumnName("EffectivenessDate")
               .IsOptional();

            Property(x => x.EffectivenessUser)
               .HasColumnName("EffectivenessUser");

            Property(x => x.LeaveDate)
               .HasColumnName("LeaveDate")
               .IsOptional();

            Property(x => x.LeaveUser)
               .HasColumnName("LeaveUser");

            Property(x => x.ProposeDate)
               .HasColumnName("ProposeDate")
               .IsOptional();

            Property(x => x.ProposeUser)
               .HasColumnName("ProposeUser");

            Property(x => x.PublishingDate)
               .HasColumnName("PublishingDate")
               .IsOptional();

            Property(x => x.PublishingUser)
               .HasColumnName("PublishingUser");

            Property(x => x.ResponseDate)
               .HasColumnName("ResponseDate")
               .IsOptional();

            Property(x => x.ResponseUser)
               .HasColumnName("ResponseUser");

            Property(x => x.ServiceNumber)
               .HasColumnName("ServiceNumber");

            Property(x => x.WaitDate)
               .HasColumnName("WaitDate")
               .IsOptional();

            Property(x => x.WaitUser)
               .HasColumnName("WaitUser");

            Property(x => x.WarningDate)
               .HasColumnName("WarningDate")
               .IsOptional();

            Property(x => x.WarningUser)
               .HasColumnName("WarningUser");

            Property(x => x.WorkflowType)
               .HasColumnName("WorkflowType");

            Property(x => x.IdType)
               .HasColumnName("idType")
               .IsRequired();

            Property(x => x.LastChangedDate)
               .HasColumnName("LastChangedDate")
               .IsOptional();

            Property(x => x.Status)
                .HasColumnName("idStatus")
                .IsRequired();

            Property(x => x.InclusiveNumber)
                .HasColumnName("InclusiveNumber")
                .IsRequired();

            Property(x => x.WebPublicationDate)
                .HasColumnName("WebPublicationDate")
                .IsOptional();

            Property(x => x.Amount)
                .HasColumnName("Amount")
                .IsOptional();

            Property(x => x.UltimaPaginaDate)
                .HasColumnName("UltimaPaginaDate")
                .IsOptional();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.RegistrationDate)
                .Ignore(x => x.RegistrationUser)
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.WorkflowAutoComplete)
                .Ignore(x => x.WorkflowName)
                .Ignore(x => x.IdWorkflowActivity)
                .Ignore(x => x.WorkflowActions);

            #endregion

            #region [ Navigation Properties ]

            HasOptional(t => t.Category)
                .WithMany(t => t.Resolutions)
                .Map(m => m.MapKey("IdCategoryAPI"));

            HasRequired(t => t.Container)
                .WithMany(t => t.Resolutions)
                .Map(m => m.MapKey("idContainer"));

            HasOptional(t => t.ResolutionKind)
                .WithMany(t => t.Resolutions)
                .Map(m => m.MapKey("IdResolutionKind"));


            HasMany(t => t.Messages)
                    .WithMany(t => t.Resolutions)
                    .Map(m => m.ToTable("ResolutionMessage")
                               .MapLeftKey("UniqueIdResolution")
                               .MapRightKey("IdMessage"));

            HasRequired(t => t.WebPublication)
                .WithRequiredDependent(t => t.Resolution)
                .Map(m => m.MapKey("IDResolution"));
            
            #endregion
        }
    }
}
