using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using VecompSoftware.DocSuiteWeb.Entity.PosteWeb;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.PosteWeb
{
    public class PosteOnLineRequestMap : EntityTypeConfiguration<PosteOnLineRequest>
    {
        public PosteOnLineRequestMap()
        {
            // Table
            ToTable("PosteOnLineRequest");

            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            
            
            Property(x => x.UniqueId)
             .HasColumnName("Id")
             .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.RequestId)
                .HasColumnName("IdRichiesta")
                .IsOptional();

            Property(x => x.GuidPoste)
                .HasColumnName("GuidPoste")
                .IsOptional();

            Property(x => x.IdOrdine)
                .HasColumnName("IdOrdine")
                .IsOptional();

            Property(x => x.Status)
                .HasColumnName("Status")
                .IsRequired();

            Property(x => x.StatusDescription)
                .HasColumnName("StatusDescrition")
                .IsOptional();

            Property(x => x.ErrorMessage)
                .HasColumnName("ErrorMsg")
                .IsOptional();

            Property(x => x.TotalCost)
                .HasColumnName("CostoTotale")
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

            Property(x => x.ExtendedProperties)
                .HasColumnName("ExtendedProperties")
                .IsOptional();

            Property(x => x.Timestamp)
               .HasColumnName("Timestamp")
               .IsRequired();

            Ignore(x => x.EntityId);
            Ignore(x => x.EntityShortId);

            #endregion

            #region [ Configure Discriminators ]

            Map<LOLRequest>(m =>
            {
                m.ToTable("PosteOnLineRequest");
                m.Requires("Type").HasValue<int>(1);
            });
            Map<ROLRequest>(m =>
            {
                m.ToTable("PosteOnLineRequest");
                m.Requires("Type").HasValue<int>(2);
            });
            Map<TOLRequest>(m =>
            {
                m.ToTable("PosteOnLineRequest");
                m.Requires("Type").HasValue<int>(3);
            });
            Map<SOLRequest>(m =>
            {
                m.ToTable("PosteOnLineRequest");
                m.Requires("Type").HasValue<int>(4);
            });

            #endregion

            #region [ Navigation Properties ]
  
            HasRequired(x => x.DocumentUnit)
                .WithMany(x => x.POLRequests)
                .Map(x => x.MapKey("IdDocumentUnit"));

            #endregion
        }
    }
}
