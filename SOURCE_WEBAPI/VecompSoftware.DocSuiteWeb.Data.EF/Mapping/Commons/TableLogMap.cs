using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Commons
{
    public class TableLogMap : EntityTypeConfiguration<TableLog>
    {
        public TableLogMap()
            : base()
        {
            ToTable("TableLog");
            //PrimaryKey
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdTableLog")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.LoggedEntityId)
                .HasColumnName("EntityId")
                .IsOptional();

            Property(x => x.LoggedEntityUniqueId)
                .HasColumnName("EntityUniqueId")
                .IsOptional();

            Property(x => x.TableName)
                .HasColumnName("TableName")
                .IsRequired();

            Property(x => x.RegistrationDate)
                .HasColumnName("LogDate")
                .IsRequired();

            Property(x => x.SystemComputer)
                .HasColumnName("SystemComputer")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("SystemUser")
                .IsRequired();

            Property(x => x.LogType)
                .HasColumnName("LogType")
                .IsRequired();

            Property(x => x.LogDescription)
                .HasColumnName("LogDescription")
                .IsRequired();

            Property(x => x.Hash)
               .HasColumnName("Hash")
               .IsOptional();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional();

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);
            #endregion
        }
    }
}
