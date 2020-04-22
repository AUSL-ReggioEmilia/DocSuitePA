using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Desks;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Desks
{
    public class DeskMessageMap : EntityTypeConfiguration<DeskMessage>
    {
        public DeskMessageMap()
            : base()
        {
            // Table
            ToTable("DeskMessages");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdDeskMail")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityId)
                .Ignore(x => x.RegistrationUser)
                .Ignore(x => x.LastChangedDate)
                .Ignore(x => x.LastChangedUser)
                .Ignore(x => x.EntityShortId);
            #endregion


            #region [ Configure Navigation Properties ]
            HasRequired(t => t.Desk)
                .WithMany(t => t.DeskMessages)
                .Map(m => m.MapKey("IdDesk"));

            HasOptional(t => t.Message)
                .WithMany(t => t.DeskMessages)
                .Map(m => m.MapKey("IdMessageEmail"));
            #endregion
        }
    }
}
