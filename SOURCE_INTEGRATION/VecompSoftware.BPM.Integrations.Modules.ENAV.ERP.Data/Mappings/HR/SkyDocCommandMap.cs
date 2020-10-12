using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Entities.HR;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Mappings.HR
{
    public class SkyDocCommandMap : EntityTypeConfiguration<SkyDocCommand>
    {
        public SkyDocCommandMap()
        {
            //Table
            ToTable("SKYDOC_COMMANDS");
            // Primary Key
            HasKey(t => t.Id);

            #region [ Configure Properties ]
            Property(x => x.Id)
                .HasColumnName("IdCommand")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(p => p.DossierReference)
                .HasColumnName("Riferimento_dossier")
                .HasMaxLength(256)
                .IsOptional();

            Property(p => p.FascicleReference)
                .HasColumnName("Riferimento_fascicolo")
                .HasMaxLength(256)
                .IsOptional();

            Property(p => p.ResponsibleRoleMappingTag)
                .HasColumnName("Mapping_Tag_Esecutore")
                .HasMaxLength(256)
                .IsOptional();

            Property(p => p.AuthorizedRoleMappingTag)
                .HasColumnName("Mapping_Tag_Autorizzato")
                .HasMaxLength(256)
                .IsOptional();

            Property(p => p.CategoryId)
                .HasColumnName("Classificazione")
                .HasMaxLength(20)
                .IsOptional();

            Property(p => p.ContainerId)
                .HasColumnName("Contenitore")
                .HasMaxLength(256)
                .IsOptional();

            Property(p => p.Object)
                .HasColumnName("Oggetto")
                .HasMaxLength(256)
                .IsOptional();

            Property(p => p.Contact1)
                .HasColumnName("Contatto_01")
                .HasMaxLength(256)
                .IsOptional();

            Property(p => p.Contact2)
                .HasColumnName("Contatto_02")
                .HasMaxLength(256)
                .IsOptional();

            Property(p => p.Contact3)
                .HasColumnName("Contatto_03")
                .HasMaxLength(256)
                .IsOptional();

            Property(p => p.Contact4)
                .HasColumnName("Contatto_04")
                .HasMaxLength(256)
                .IsOptional();

            Property(p => p.Typology)
                .HasColumnName("Tipologia")
                .IsOptional();

            Property(p => p.InsertDate)
                .HasColumnName("DataInserimento")
                .IsRequired();

            Property(p => p.TenantId)
                .HasColumnName("Tenant_ID")
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            Property(p => p.WFSkyDocStarted)
                .HasColumnName("WF_SkyDocStarted")
                .IsOptional();

            Property(p => p.WFSkyDocId)
                .HasColumnName("WF_SkyDocID")
                .HasColumnType("uniqueidentifier")
                .IsOptional();

            Property(p => p.WFSkyDocStatus)
                .HasColumnName("WF_SkyDocStatus")
                .HasColumnType("smallint")
                .IsOptional();

            Property(x => x.CommandType)
                .HasColumnName("Discriminator")
                .IsRequired();
            #endregion

            #region [ Navigation Properties ]
            HasMany(p => p.Documents)
                .WithRequired(d => d.Command)
                .Map(d => d.MapKey("IdCommand"));
            #endregion
        }
    }
}
