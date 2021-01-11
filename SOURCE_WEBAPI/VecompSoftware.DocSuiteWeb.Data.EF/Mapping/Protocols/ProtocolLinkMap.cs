using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Protocols
{
    public class ProtocolLinkMap : EntityTypeConfiguration<ProtocolLink>
    {
        public ProtocolLinkMap()
            : base()
        {
            ToTable("ProtocolLinks");

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

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired();

            Property(x => x.LinkType)
                .HasColumnName("LinkType")
                .IsOptional();

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional();

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            Ignore(x => x.EntityShortId)
                .Ignore(x => x.EntityId);

            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(t => t.Protocol)
                    .WithMany(t => t.ProtocolLinks)
                    .Map(t => t.MapKey("UniqueIdProtocolParent"));

            HasRequired(t => t.ProtocolLinked)
                    .WithMany(t => t.LinkedProtocols)
                    .Map(t => t.MapKey("UniqueIdProtocolSon"));

            #endregion
        }
    }
}
