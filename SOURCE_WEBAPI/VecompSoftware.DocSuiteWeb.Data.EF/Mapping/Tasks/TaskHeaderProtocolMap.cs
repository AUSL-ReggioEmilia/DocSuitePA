using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Tasks;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Tasks
{
    public class TaskHeaderProtocolMap : EntityTypeConfiguration<TaskHeaderProtocol>
    {
        public TaskHeaderProtocolMap() : base()
        {
            //Table
            ToTable("TaskHeaderProtocol");
            //Primary Key
            HasKey(t => t.EntityId);

            #region [ Configure Properties ]
            Property(x => x.EntityId)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.Year)
                .HasColumnName("Year")
                .IsOptional();

            Property(x => x.Number)
              .HasColumnName("Number")
              .IsOptional();

            Ignore(x => x.UniqueId)
                .Ignore(x => x.RegistrationDate)
                .Ignore(x => x.RegistrationUser)
                .Ignore(x => x.LastChangedDate)
                .Ignore(x => x.LastChangedUser)
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.Timestamp);
            #endregion

            #region [ Configure Navigation Properties ]
            HasRequired(t => t.TaskHeader)
                .WithMany(t => t.TaskHeaderProtocols)
                .Map(m => m.MapKey("IdTaskHeader"));

            HasRequired(t => t.Protocol)
                .WithMany(t => t.TaskHeaderProtocols)
                .Map(m => m.MapKey("UniqueIdProtocol"));
            #endregion

        }
    }
}
