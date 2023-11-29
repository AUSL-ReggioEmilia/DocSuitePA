using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Tasks;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Tasks
{
    public class TaskDetailMap : EntityTypeConfiguration<TaskDetail>
    {
        public TaskDetailMap() : base()
        {
            //Table
            ToTable("TaskDetail");
            //Primary Key
            HasKey(t => t.EntityId);

            #region [ Configure Properties ]
            Property(x => x.EntityId)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.DetailType)
                .HasColumnName("DetailType")
                .IsRequired();

            Property(x => x.Title)
              .HasColumnName("Title")
              .HasMaxLength(500)
              .IsRequired();

            Property(x => x.Description)
                .HasColumnName("Description")
                .IsOptional();

            Property(x => x.ErrorDescription)
                .HasColumnName("ErrorDescription")
                .IsOptional();

            Property(x => x.RegistrationUser)
               .HasColumnName("RegistrationUser")
               .IsRequired()
               .HasMaxLength(256);

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .HasMaxLength(256)
                .IsOptional();

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            
            Ignore(x => x.UniqueId)
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.Timestamp);
            #endregion

            #region [ Configure Navigation Properties ]
            HasRequired(t => t.TaskHeader)
                .WithMany(t => t.TaskDetails)
                .Map(m => m.MapKey("IdTask"));
            #endregion

        }
    }
}
