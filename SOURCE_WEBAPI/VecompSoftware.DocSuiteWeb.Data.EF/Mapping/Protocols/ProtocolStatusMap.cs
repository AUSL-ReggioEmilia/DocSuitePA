using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Protocols
{
    public class ProtocolStatusMap : EntityTypeConfiguration<ProtocolStatus> 
    {
        public ProtocolStatusMap()
            : base()
        {
            ToTable("ProtocolStatus");
            //Primary Key
            HasKey(t => t.EntityId);

            #region [ Configure Properties ]

            Property(x => x.EntityId)
                .HasColumnName("Incremental")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(x => x.Status)
                .HasColumnName("ProtocolStatus")
                .IsRequired();

            Property(x => x.ProtocolStatusDescription)
                .HasColumnName("ProtocolStatusDescription")
                .IsRequired();

            Property(x => x.BackColor)
                .HasColumnName("BackColor")
                .IsOptional();

            Property(x => x.ForeColor)
                .HasColumnName("ForeColor")
                .IsOptional();

            Ignore(x => x.UniqueId);
            Ignore(x => x.EntityShortId);
            Ignore(x => x.RegistrationDate);
            Ignore(x => x.RegistrationUser);
            Ignore(x => x.LastChangedDate);
            Ignore(x => x.LastChangedUser);
            Ignore(x => x.Timestamp);

            #endregion
        }
    }
}
