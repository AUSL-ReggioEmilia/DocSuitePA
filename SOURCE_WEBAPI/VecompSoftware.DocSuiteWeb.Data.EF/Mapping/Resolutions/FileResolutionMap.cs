using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Resolutions
{
    public class FileResolutionMap : EntityTypeConfiguration<FileResolution>
    {
        public FileResolutionMap()
            : base()
        {
            ToTable("FileResolution");
            //Primary Key
            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]

            Property(x => x.UniqueId)
                .HasColumnName("UniqueId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.EntityId)
                .HasColumnName("idResolution")
                .IsRequired();

            Property(x => x.IdResolutionFile)
                .HasColumnName("idResolutionFile")
                .IsOptional();

            Property(t => t.IdProposalFile)
                .HasColumnName("idProposalFile")
                .IsOptional();

            Property(t => t.IdAttachments)
                .HasColumnName("idAttachements")
                .IsOptional();

            Property(t => t.IdControllerFile)
                .HasColumnName("idControllerFile")
                .IsOptional();

            Property(t => t.IdAssumedProposal)
                .HasColumnName("idAssumedProposal")
                .IsOptional();

            Property(t => t.IdFrontespizio)
                .HasColumnName("idFrontespizio")
                .IsOptional();

            Property(t => t.IdPrivacyAttachments)
                .HasColumnName("idPrivacyAttachments")
                .IsOptional();

            Property(t => t.IdFrontalinoRitiro)
                .HasColumnName("idFrontalinoRitiro")
                .IsOptional();

            Property(t => t.IdAnnexes)
                .HasColumnName("idAnnexes")
                .IsOptional();

            Property(t => t.IdPrivacyPublicationDocument)
                .HasColumnName("idPrivacyPublicationDocument")
                .IsOptional();

            Property(t => t.IdMainDocumentsOmissis)
                .HasColumnName("idMainDocumentsOmissis")
                .IsOptional();

            Property(t => t.IdAttachmentsOmissis)
                .HasColumnName("idAttachmentsOmissis")
                .IsOptional();

            Property(t => t.IdUltimaPaginaFile)
                .HasColumnName("idUltimaPaginaFile")
                .IsOptional();

            Property(t => t.IdSupervisoryBoardFile)
                .HasColumnName("idSupervisoryBoardFile")
                .IsOptional();

            Property(t => t.DematerialisationChainId)
                .HasColumnName("DematerialisationChainId")
                .IsOptional();

            Ignore(x => x.EntityShortId)
                .Ignore(x => x.RegistrationDate)
                .Ignore(x => x.RegistrationUser)
                .Ignore(x => x.LastChangedDate)
                .Ignore(x => x.LastChangedUser);

            #endregion

            #region [ Navigation Properties ]

            HasRequired(t => t.Resolution)
                .WithRequiredDependent(t => t.FileResolution)
                .Map(m => m.MapKey("UniqueIdResolution"));

            #endregion
        }
    }
}
