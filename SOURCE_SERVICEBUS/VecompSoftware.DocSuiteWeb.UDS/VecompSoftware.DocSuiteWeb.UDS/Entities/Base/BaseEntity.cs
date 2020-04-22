using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.UDS;

namespace VecompSoftware.DocSuiteWeb.UDS.Entities.Base
{

    public abstract class BaseEntity
    {
        [Key]
        public Guid UDSId { get; set; }

        public Guid IdUDSRepository { get; set; }

        [ForeignKey("IdUDSRepository")]
        public UDSRepository UDSRepository { get; set; }

        public short _year { get; set; }

        public int _number { get; set; }

        public string _subject { get; set; }

        public string _cancelMotivation { get; set; }

        public short IdCategory { get; set; }

        [ForeignKey("IdCategory")]
        public Category Category { get; set; }

        [ForeignKey("UDSId")]
        public virtual DocumentUnit DocumentUnit { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public short _status { get; set; }

        [Timestamp]
        [ConcurrencyCheck]
        public byte[] Timestamp { get; set; }
    }
}
