using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Protocols
{
    public class ProtocolRoleMap : EntityTypeConfiguration<ProtocolRole>
    {
        public ProtocolRoleMap()
            : base()
        {
            ToTable("ProtocolRole");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                 .HasColumnName("UniqueId")
                 .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Year)
                .HasColumnName("Year")
                .IsRequired();

            Property(x => x.Number)
                .HasColumnName("Number")
                .IsRequired();

            Property(x => x.Rights)
                .HasColumnName("Rights")
                .IsOptional();

            Property(x => x.Note)
                .HasColumnName("Note")
                .IsOptional();

            Property(x => x.DistributionType)
                .HasColumnName("DistributionType")
                .IsOptional();

            Property(x => x.Type)
                .HasColumnName("Type")
                .IsOptional();

            Property(x => x.NoteType)
                .HasColumnName("NoteType")
                .IsOptional();

            Property(x => x.Status)
                .HasColumnName("Status")
                .IsOptional();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired();

            Property(t => t.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(t => t.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional();

            Property(t => t.Timestamp)
               .HasColumnName("Timestamp")
               .IsRequired();

            Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);


            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Protocol)
                .WithMany(t => t.ProtocolRoles)
                .Map(m => m.MapKey("UniqueIdProtocol"));

            HasRequired(t => t.Role)
                .WithMany(t => t.ProtocolRoles)
                .Map(m => m.MapKey("idRole"));

            #endregion
        }
    }
}
