using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using BiblosDS.Library.Common.Exceptions;

namespace BiblosDS.API
{
    public partial class DocumentoFacade    
    {
        private static void ParseBiblosDSFaultException(ResponseBase response, FaultException<BiblosDsException> faultEx)
        {
            if (faultEx.Detail.Code == Library.Common.Exceptions.FaultCode.DocumentNotFound_Exception)
                response.CodiceEsito = CodiceErrore.IdDocumentoNonTrovato;
            else if (faultEx.Detail.Code == Library.Common.Exceptions.FaultCode.Attribute_Exception)
                response.CodiceEsito = CodiceErrore.AttributoNonValido;
            else if (faultEx.Detail.Code == Library.Common.Exceptions.FaultCode.AttributeRequired_Exception)
                response.CodiceEsito = CodiceErrore.AttributeRequired;
            else if (faultEx.Detail.Code == Library.Common.Exceptions.FaultCode.DocumentPrimaryKey_Exception)
                response.CodiceEsito = CodiceErrore.PrimaryKeyErrata;
            else if (faultEx.Detail.Code == Library.Common.Exceptions.FaultCode.DocumentNotReadyForAttach_Exception)
                response.CodiceEsito = CodiceErrore.StatoDocumentoNonValido;
            else if (faultEx.Detail.Code == Library.Common.Exceptions.FaultCode.FileNotFound_Exception)
                response.CodiceEsito = CodiceErrore.FileInesistente;
            else if (faultEx.Detail.Code == Library.Common.Exceptions.FaultCode.DocumentWithoutContent_Exception)
                response.CodiceEsito = CodiceErrore.ProfileOnly;
            else if (faultEx.Detail.Code == Library.Common.Exceptions.FaultCode.DocumentConnectionExists_Exception)
                response.CodiceEsito = CodiceErrore.LegameDocumentiDefinito;
            else if (faultEx.Detail.Code == Library.Common.Exceptions.FaultCode.DocumentAttachNotFound_Exception)
                response.CodiceEsito = CodiceErrore.AllegatoNonPresente;
            else
                response.CodiceEsito = CodiceErrore.ErroreGenericoBiblosDS;
            response.MessaggioErrore = faultEx.StackTrace;
            response.MessaggioEsito = faultEx.Message;
        }
    }
}
