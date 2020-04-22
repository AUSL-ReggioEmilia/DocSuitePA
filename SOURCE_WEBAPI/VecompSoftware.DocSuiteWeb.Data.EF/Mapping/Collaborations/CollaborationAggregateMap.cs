using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Collaborations
{
    public class CollaborationAggregateMap : EntityTypeConfiguration<CollaborationAggregate>
    {
        public CollaborationAggregateMap() : base()
        {
            ToTable("CollaborationAggregate");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdCollaborationAggregate")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.CollaborationDocumentType)
                .HasColumnName("CollaborationDocumentType")
                .IsRequired();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityShortId)
                .Ignore(x => x.EntityId)
                .Ignore(x => x.RegistrationDate)
                .Ignore(x => x.RegistrationUser)
                .Ignore(x => x.LastChangedDate)
                .Ignore(x => x.LastChangedUser);
            #endregion

            #region [ Configure Navigation Properties ]

            HasRequired(x => x.CollaborationFather)
                .WithMany(x => x.CollaborationAggregateFathers)
                .Map(m => m.MapKey("idCollaborationFather"));

            HasRequired(x => x.CollaborationChild)
                .WithMany(x => x.CollaborationAggregates)
                .Map(m => m.MapKey("idCollaborationChild"));


            #endregion
        }
    }
}
