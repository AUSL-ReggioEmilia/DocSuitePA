using System.Collections.Generic;
using System.Collections.ObjectModel;
using VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.DocSuiteWeb.DTO.DocumentSeries
{
    public class DocumentSeriesInsertModel
    {
        #region [ Constructor ]
        public DocumentSeriesInsertModel()
        {
            this.UnPublishedAnnexed = new Collection<string>();
            this.Annexed = new Collection<string>();
            this.SectorMembershipAuthorizations = new Collection<Role>();
            this.KnowledgeAuthorizations = new Collection<Role>();
        }
        #endregion

        #region [ Properties ]
        public int? SubSection { get; set; }
        public ICollection<string> Annexed { get; set; }
        public ICollection<string> UnPublishedAnnexed { get; set; }
        public string Object { get; set; }
        public ICollection<Role> SectorMembershipAuthorizations { get; set; }
        public ICollection<Role> KnowledgeAuthorizations { get; set; }

        #endregion
    }
}
