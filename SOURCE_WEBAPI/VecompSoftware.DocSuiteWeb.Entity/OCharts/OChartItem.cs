using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;

namespace VecompSoftware.DocSuiteWeb.Entity.OCharts
{

    public class OChartItem : DSWBaseEntity
    {
        #region [ Constructor ]

        public OChartItem() : this(Guid.NewGuid()) { }

        public OChartItem(Guid uniqueId)
            : base(uniqueId)
        {
            Children = new HashSet<OChartItem>();
            Contacts = new HashSet<Contact>();
            Roles = new HashSet<Role>();
            Mailboxes = new HashSet<PECMailBox>();
            OChartItemContainers = new HashSet<OChartItemContainer>(); ;
        }
        #endregion

        #region [ Properties ]
        public bool? Enabled { get; set; }
        public string Code { get; set; }
        public string FullCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public bool? Imported { get; set; }
        public string Acronym { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual OChart OChart { get; set; }
        public virtual OChartItem Parent { get; set; }
        public virtual ICollection<OChartItem> Children { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<OChartItemContainer> OChartItemContainers { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<PECMailBox> Mailboxes { get; set; }
        #endregion
    }
}
