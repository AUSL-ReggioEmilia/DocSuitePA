using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Fascicles
{
    public class FascicleMap : EntityTypeConfiguration<Fascicle>
    {
        public FascicleMap()
            : base()
        {
            ToTable("Fascicles");
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdFascicle")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Year)
                .HasColumnName("Year")
                .IsRequired();

            Property(x => x.Number)
                .HasColumnName("Number")
                .IsRequired();

            Property(x => x.Conservation)
                .HasColumnName("Conservation")
                .IsOptional();

            Property(x => x.StartDate)
                .HasColumnName("StartDate")
                .IsRequired();

            Property(x => x.EndDate)
                .HasColumnName("EndDate")
                .IsOptional();

            Property(x => x.Title)
                .HasColumnName("Title")
                .IsRequired();

            Property(x => x.Name)
                .HasColumnName("Name")
                .IsOptional();

            Property(x => x.FascicleObject)
                .HasColumnName("Object")
                .IsOptional();

            Property(x => x.Manager)
                .HasColumnName("Manager")
                .IsOptional();

            Property(x => x.Rack)
                .HasColumnName("Rack")
                .IsOptional();

            Property(x => x.Note)
                .HasColumnName("Note")
                .IsOptional();

            Property(x => x.FascicleType)
                .HasColumnName("FascicleType")
                .IsRequired();

            Property(x => x.VisibilityType)
                .HasColumnName("VisibilityType")
                .IsRequired();

            Property(x => x.MetadataValues)
                .HasColumnName("MetadataValues")
                .IsOptional();

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

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            Property(x => x.DSWEnvironment)
                .HasColumnName("DSWEnvironment")
                .IsOptional();

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);

            Ignore(x => x.WorkflowName)
                .Ignore(x => x.IdWorkflowActivity)
                .Ignore(x => x.WorkflowActions);

            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Category)
                .WithMany(t => t.Fascicles)
                .Map(m => m.MapKey("IdCategory"));

            HasMany(x => x.FascicleDocumentUnits)
                .WithRequired(x => x.Fascicle)
                .Map(x => x.MapKey("IdFascicle"));

            HasMany(t => t.Contacts)
                .WithMany(t => t.Fascicles)
                .Map(m =>
                {
                    m.ToTable("FascicleContacts");
                    m.MapLeftKey("IdFascicle");
                    m.MapRightKey("IdContact");
                });

            HasOptional(t => t.MetadataRepository)
                .WithMany(t => t.Fascicles)
                .Map(m => m.MapKey("IdMetadataRepository"));

            HasOptional(t => t.Container)
                .WithMany(t => t.Fascicles)
                .Map(m => m.MapKey("IdContainer"));

            HasOptional(t => t.ProcessFascicleTemplate)
                .WithMany(t => t.Fascicles)
                .Map(m => m.MapKey("IdProcessFascicleTemplate"));
            #endregion
        }
    }
}
