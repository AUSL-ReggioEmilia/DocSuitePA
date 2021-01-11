using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Enums
{
    public enum LoggingSource
    {
        BiblosDS_WCF_DigitalSign = 1,
        BiblosDS_WCF_Documents = 2,
        BiblosDS_WCF_LegalExtension = 3,
        BiblosDS_WCF_StampaConforme = 4,
        BiblosDS_WCF_Storage = 5,
        BiblosDS_WindowsService_Alerting = 6,
        BiblosDS_WindowsService_WCFHost = 7,
        BiblosDS_WindowsService_WCFStampaConformeHost = 8,
        BiblosDS_WS = 9,
        BiblosDS_General = 10,
        BiblosDS_FileSystem = 11,
        BiblosDS_Sharepoint = 12,
        BiblosDS_SqlServer = 13,
        BiblosDS_CommonLibrary = 14,

    }
}
