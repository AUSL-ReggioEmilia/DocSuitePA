using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Resolutions
{
    public class WebPublicationMap : EntityTypeConfiguration<WebPublication>
    {
        public WebPublicationMap()
            : base()
        {
            ToTable("WebPublication");
            //Primary Key
            HasKey(t => t.EntityId);

            #region [ Configure Properties ]
            Property(x => x.EntityId)
                .HasColumnName("Id");

            Property(x => x.ExternalKey)
                .HasColumnName("ExternalKey")
                .HasMaxLength(50)
                .IsOptional();

            Property(x => x.Status)
                .HasColumnName("Status")
                .IsOptional();

            Property(x => x.IDLocation)
                .HasColumnName("IDLocation")
                .IsOptional();

            Property(x => x.IDDocument)
                .HasColumnName("IDDocument")
                .IsOptional();

            Property(x => x.EnumDocument)
                .HasColumnName("EnumDocument")
                .IsOptional();

            Property(x => x.Descrizione)
                .HasColumnName("Descrizione")
                .HasMaxLength(255)
                .IsOptional();

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .HasMaxLength(256)
                .IsOptional();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .HasMaxLength(256)
                .IsRequired();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.IsPrivacy)
                .HasColumnName("IsPrivacy")
                .IsOptional();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.UniqueId);
            #endregion

            #region [ Navigation Properties ]
            #endregion
        }
    }
}
