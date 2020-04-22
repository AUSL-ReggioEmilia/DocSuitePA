using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Commons
{
    public class IncrementalMap : EntityTypeConfiguration<Incremental>
    {
        public IncrementalMap()
            : base()
        {
            ToTable("Incremental");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Name)
                .HasColumnName("Name")
                .IsOptional();

            Property(x => x.IncrementalValue)
                .HasColumnName("Incremental")
                .IsOptional();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.RegistrationUser)
                .Ignore(x => x.RegistrationDate)
                .Ignore(x => x.LastChangedDate)
                .Ignore(x => x.LastChangedUser)
                .Ignore(x => x.EntityId);
            #endregion

            #region [ Configure Navigation Properties ]
            #endregion
        }
    }
}
