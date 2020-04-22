using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.DTO.Commons
{
    public enum MiscellaneaDocumentStatus
    {
        /// <summary>
        /// Non modificato
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Eliminato
        /// </summary>
        Deleted = 1,
        /// <summary>
        /// Aggiunto
        /// </summary>
        Added = Deleted * 2,
        /// <summary>
        /// Modificato
        /// </summary>
        Modified = Added * 2
    }
}
