using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Conservations;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Conservations
{
    public class ConservationMap : EntityTypeConfiguration<Conservation>
    {
        public ConservationMap()
            : base()
        {
            ToTable("Conservations", "conservation");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdConservation")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.EntityType)
                .HasColumnName("EntityType")
                .IsRequired();

            Property(x => x.Status)
                .HasColumnName("Status")
                .IsRequired();

            Property(x => x.Message)
                .HasColumnName("Message")
                .IsOptional();

            Property(x => x.Type)
                .HasColumnName("Type")
                .IsRequired();

            Property(x => x.SendDate)
                .HasColumnName("SendDate")
                .IsOptional();

            Property(x => x.ConservationDate)
                .HasColumnName("ConservationDate")
                .IsOptional();

            Property(x => x.Uri)
                .HasColumnName("Uri")
                .IsOptional();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired();

            Property(x => x.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(x => x.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional();

            Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();


            Ignore(x => x.EntityShortId);
            Ignore(x => x.EntityId);

            #endregion

            #region [ Configure Navigation Properties ]

            #endregion
        }
    }
}
