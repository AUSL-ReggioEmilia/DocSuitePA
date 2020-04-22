using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Enums
{
    public enum LoggingOperationType
    {
        BiblosDS_General = 0,
        BiblosDS_GetAlive = 1,
        BiblosDS_InsertDocument = 2,
        BiblosDS_CheckInTransitoDocument = 3,
        BiblosWs_General = 4,
        BiblosWs_Compatibility = 5,
        BiblosDS_GetDocument = 6,
        BiblosDS_CheckInTransitoDocumentAttach = 7,
    }
}
