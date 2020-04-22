using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VecompSoftware.DocSuiteWeb.Entity.Desks;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Desks
{
    public class DeskStoryBoardMap : EntityTypeConfiguration<DeskStoryBoard>
    {
        public DeskStoryBoardMap()
            : base()
        {

            ToTable("DeskStoryBoards");

            HasKey(t => t.UniqueId);

            #region [ Configure Properties ]
            Property(x => x.UniqueId)
                .HasColumnName("IdStoryBoard")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Comment)
                .HasColumnName("Comment")
                .IsOptional();

            Property(x => x.DateBoard)
                .HasColumnName("DateBoard")
                .IsOptional();

            Property(x => x.Author)
                .HasColumnName("Author")
                .IsOptional();

            Property(t => t.LastChangedDate)
                .HasColumnName("LastChangedDate")
                .IsOptional();

            Property(t => t.LastChangedUser)
                .HasColumnName("LastChangedUser")
                .IsOptional()
                .HasMaxLength(256);

            Property(t => t.RegistrationUser)
                .HasColumnName("RegistrationUser")
                .IsOptional()
                .HasMaxLength(256);

            Property(t => t.RegistrationDate)
                .HasColumnName("RegistrationDate")
                .IsRequired();

            Property(t => t.BoardType)
                .HasColumnName("BoardType")
                .IsOptional();

            Ignore(x => x.Timestamp)
                .Ignore(x => x.EntityId)
                .Ignore(x => x.EntityShortId);
            #endregion

            #region [ Configure Navigation Properties ]
            HasRequired(t => t.Desk)
                .WithMany(t => t.DeskStoryBoards)
                .Map(m => m.MapKey("IdDesk"));

            HasOptional(t => t.DeskRoleUser)
                .WithMany(t => t.DeskStoryBoards)
                .Map(m => m.MapKey("IdDeskRoleUser"));

            HasOptional(t => t.DeskDocumentVersion)
                .WithMany(t => t.DeskStoryBoards)
                .Map(m => m.MapKey("IdDeskDocumentVersion"));
            #endregion
        }
    }
}
