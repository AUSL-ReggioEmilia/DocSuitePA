using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Desks;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Desks
{
    public class DeskCollaborationMap : EntityTypeConfiguration<DeskCollaboration>
    {
        public DeskCollaborationMap()
            : base()
        {
            // Table
            ToTable("DeskCollaborations");
            // Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdDeskCollaboration")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Ignore(x => x.LastChangedDate)
                .Ignore(x => x.LastChangedUser)
                .Ignore(x => x.RegistrationUser)
                .Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);
            #endregion

            #region [ Configure Navigation Properties ]
            HasRequired(t => t.Desk)
                .WithMany(t => t.DeskCollaborations)
                .Map(m => m.MapKey("IdDesk"));

            HasRequired(t => t.Collaboration)
                .WithOptional(t => t.DeskCollaboration)
                .Map(m => m.MapKey("IdCollaboration"));
            #endregion
        }
    }
}
