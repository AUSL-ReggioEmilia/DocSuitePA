using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.DocumentUnits
{
    public class DocumentUnitMap : EntityTypeConfiguration<DocumentUnit>
    {
        public DocumentUnitMap()
            : base()
        {
            ToTable("DocumentUnits", "cqrs");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdDocumentUnit")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.EntityId)
               .HasColumnName("EntityId")
               .IsOptional();

            Property(x => x.Year)
                .HasColumnName("Year")
                .IsRequired();

            Property(x => x.Number)
                .HasColumnName("Number")
                .IsRequired();

            Property(x => x.DocumentUnitName)
                .HasColumnName("DocumentUnitName")
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

            Property(x => x.Title)
                .HasColumnName("Title")
                .IsRequired();

            Property(x => x.Subject)
                .HasColumnName("Subject")
                .IsOptional();

            Property(x => x.Environment)
                .HasColumnName("Environment")
                .IsRequired();

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            Property(x => x.Status)
                .HasColumnName("Status")
                .IsRequired();


            Ignore(x => x.EntityShortId)
                .Ignore(x => x.WorkflowName)
                .Ignore(x => x.IdWorkflowActivity)
                .Ignore(x => x.WorkflowActions);

            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Container)
                .WithMany(t => t.DocumentUnits)
                .Map(m => m.MapKey("IdContainer"));

            HasRequired(t => t.Category)
                .WithMany(t => t.DocumentUnits)
                .Map(m => m.MapKey("IdCategory"));

            HasMany(x => x.FascicleDocumentUnits)
                .WithRequired(x => x.DocumentUnit)
                .Map(x => x.MapKey("IdDocumentUnit"));

            HasOptional(t => t.Fascicle)
               .WithMany(t => t.DocumentUnits)
               .Map(m => m.MapKey("IdFascicle"));

            HasOptional(t => t.UDSRepository)
               .WithMany(t => t.DocumentUnits)
               .Map(m => m.MapKey("IdUDSRepository"));

            #endregion
        }
    }
}
