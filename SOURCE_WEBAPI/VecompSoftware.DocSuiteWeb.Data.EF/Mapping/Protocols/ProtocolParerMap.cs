using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Protocols
{
    public class ProtocolParerMap : EntityTypeConfiguration<ProtocolParer>
    {
        public ProtocolParerMap()
            : base()
        {
            ToTable("ProtocolParer");
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

            Property(x => x.ArchiviedDate)
                .HasColumnName("ArchiviedDate")
                .IsOptional();

            Property(x => x.ParerUri)
                .HasColumnName("ParerUri")
                .IsOptional();

            Property(x => x.IsForArchive)
                .HasColumnName("IsForArchive")
                .IsOptional();

            Property(x => x.HasError)
                .HasColumnName("HasError")
                .IsOptional();

            Property(x => x.LastError)
                .HasColumnName("LastError")
                .IsOptional();

            Property(x => x.LastSendDate)
                .HasColumnName("LastSendDate")
                .IsOptional();

            Property(t => t.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(t => t.RegistrationUser)
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
                       .WithMany(t => t.ProtocolParers)
                       .Map(m => m.MapKey("UniqueIdProtocol"));

            #endregion
        }
    }
}
