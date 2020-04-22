using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiblosDS.Library.Common.Enums;

namespace BiblosDS.Library.Common.Converters
{
    public interface IConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tmpFile">File da convertire</param>
        /// <param name="fileExtension">Estenzione del file</param>
        /// <param name="extReq">Estenzione richiesta</param>
        byte[] Convert(byte[] fileSource, string fileExtension, string extReq, AttachConversionMode mode);

        string GetVersion();
    }
}
