using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Tasks;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Tasks
{
    public class TaskHeaderMap : EntityTypeConfiguration<TaskHeader>
    {
        public TaskHeaderMap() : base()
        {
            //Table
            ToTable("TaskHeader");
            //Primary Key
            HasKey(t => t.EntityId);

            #region [ Configure Properties ]
            Property(x => x.EntityId)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.Code)
                .HasColumnName("Code")
                .IsRequired();

            Property(x => x.Title)
              .HasColumnName("Title")
              .HasMaxLength(50)
              .IsOptional();

            Property(x => x.Description)
                .HasColumnName("Description")
                .IsOptional();

            Property(x => x.TaskType)
                .HasColumnName("TaskType")
                .IsRequired();

            Property(x => x.Status)
                .HasColumnName("Status")
                .IsRequired();

            Property(x => x.SendedStatus)
                .HasColumnName("SendedStatus")
                .IsOptional();

            Property(x => x.SendingProcessStatus)
                .HasColumnName("SendingProcessStatus")
                .IsRequired();

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
            #endregion

        }
    }
}
