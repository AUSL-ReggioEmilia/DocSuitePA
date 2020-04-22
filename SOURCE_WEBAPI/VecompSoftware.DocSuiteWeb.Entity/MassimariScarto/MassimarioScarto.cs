using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Entity.MassimariScarto
{
    public class MassimarioScarto : DSWBaseEntity
    {
        #region [ Constructor ]

        public MassimarioScarto() : this(Guid.NewGuid()) { }

        public MassimarioScarto(Guid uniqueId)
            : base(uniqueId)
        {
            Categories = new HashSet<Category>();
        }

        #endregion

        #region [ Properties ]

        public MassimarioScartoStatus Status { get; set; }

        public string Name { get; set; }

        public short? Code { get; set; }

        public string FullCode { get; set; }

        public string Note { get; set; }

        public short? ConservationPeriod { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public string MassimarioScartoPath { get; set; }

        public short MassimarioScartoLevel { get; set; }

        public string MassimarioScartoParentPath { get; set; }

        /// <summary>
        /// Proprietà fake per passare il parent id in fase di inserimento
        /// </summary>
        public Guid? FakeInsertId { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual ICollection<Category> Categories { get; set; }
        #endregion
    }
}
