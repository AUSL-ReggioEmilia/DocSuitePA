using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.MassimariScarto;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.MassimariScarto
{
    public class MassimarioScartoMap : EntityTypeConfiguration<MassimarioScarto>
    {
        public MassimarioScartoMap()
            : base()
        {
            ToTable("MassimariScarto");

            HasKey(k => k.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("IdMassimarioScarto");

            Property(x => x.MassimarioScartoPath)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed)
                .IsOptional();

            Property(x => x.MassimarioScartoParentPath)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed)
                .IsOptional();

            Property(x => x.MassimarioScartoLevel)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed)
                .IsOptional();

            Property(x => x.FullCode)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed)
                .IsOptional();

            Property(x => x.FakeInsertId)
                .IsOptional();

            Property(x => x.Status)
                .IsRequired();

            Property(x => x.Code)
                .IsOptional();

            Property(x => x.ConservationPeriod)
                .IsOptional();

            Property(x => x.EndDate)
                .IsOptional();

            Property(x => x.Name)
                .IsRequired();

            Property(x => x.Note)
                .IsOptional();

            Property(x => x.StartDate)
                .IsRequired();

            Property(x => x.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsRequired();

            Property(x => x.RegistrationDate)
                .HasColumnName("RegistrationDate")
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

            Ignore(x => x.EntityId);
            Ignore(x => x.EntityShortId);


            MapToStoredProcedures(s => s.Insert(i => i.HasName("MassimarioScarto_Insert"))
                                            .Update(u => u.HasName("MassimarioScarto_Update")));

            #endregion

            #region [ Configure Navigation Properties ]
            #endregion
        }
    }
}
