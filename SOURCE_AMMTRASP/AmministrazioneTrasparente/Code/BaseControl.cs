using AmministrazioneTrasparente.MasterPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace AmministrazioneTrasparente.Code
{
    public class BaseControl : UserControl
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]
        public DocumentSeries BaseMaster => Page.Master as DocumentSeries;
        #endregion

        #region [ Constructor ]
        public BaseControl()
        {

        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}