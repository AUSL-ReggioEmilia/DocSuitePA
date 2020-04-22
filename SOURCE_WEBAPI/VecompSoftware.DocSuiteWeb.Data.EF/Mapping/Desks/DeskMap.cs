using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Desks;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Desks
{
    public class DeskMap : EntityTypeConfiguration<Desk>
    {
        public DeskMap() : base()
        {
            // Table
            ToTable("Desks");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdDesk")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Description)
                .HasColumnName("Description")
                .IsOptional();

            Property(x => x.ExpirationDate)
                .HasColumnName("ExpirationDate")
                .IsOptional();

            Property(x => x.Name)
                .HasColumnName("Name")
                .IsOptional();

            Property(x => x.Status)
                .HasColumnName("Status")
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
                .IsOptional()
                .HasMaxLength(256);

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Ignore(x => x.EntityId)
                .Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityShortId);
            #endregion


            #region [ Configure Navigation Properties ]
            #endregion
        }
    }
}
