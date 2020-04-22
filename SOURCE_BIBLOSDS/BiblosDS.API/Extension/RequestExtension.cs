namespace BiblosDS.API
{
    public static class RequestExtension
    {
        public static CodiceErrore CheckRichiestaChiave(this RequestBase request, ResponseBase response, string chiave)
        {
            if (string.IsNullOrEmpty(chiave))
            {
                return CodiceErrore.ChiaveDocumentoNonDefinita;
            }
            if (string.IsNullOrEmpty(request.TipoDocumento))
            {
                return CodiceErrore.ClasseDocumentaleNonDefinita;
            }
            if (request is CreaDocumentoRequest)
            {
                if (Helpers.ExistDocumentChiave(request.IdCliente, request.Chiave))
                    return CodiceErrore.ChiaveDocumentoDuplicata;
            }
            if (request is CreaLegameDocumentiChiaveRequest && string.IsNullOrEmpty((request as CreaLegameDocumentiChiaveRequest).ChiaveDocumentoLink))
            {
                return CodiceErrore.ChiaveDocumentoNonDefinita;
            }
            if (request is CreaLegameDocumentiChiaveRequest && string.IsNullOrEmpty((request as CreaLegameDocumentiChiaveRequest).TipoDocumentoLink))
            {
                return CodiceErrore.ClasseDocumentaleNonDefinita;
            }
            if (request is CreaLegameDocumentiChiaveRequest)
            {
                if (!Helpers.ExistDocumentByChiave(request, request.Chiave))
                    return CodiceErrore.DocumentoNonDefinito;
                if (!Helpers.ExistDocumentByChiave(request, (request as CreaLegameDocumentiChiaveRequest).ChiaveDocumentoLink))
                    return CodiceErrore.DocumentoNonDefinito;
            }
            return request.CheckRequest(response);
        }

        public static CodiceErrore CheckRequest(this RequestBase request, ResponseBase response)
        {
            if (request is RequestIdBase && !(request is CreaDocumentoRequest))
            {
                if (!Helpers.ExistDocumentById(request, (request as RequestIdBase).IdDocumento))
                    return CodiceErrore.DocumentoNonDefinito;
            }           
            if (request is CreaLegameDocumentiRequest)
            {
                if (!Helpers.ExistDocumentById(request, (request as CreaLegameDocumentiRequest).IdDocumento))
                    return CodiceErrore.DocumentoNonDefinito;
                if (!Helpers.ExistDocumentById(request, (request as CreaLegameDocumentiRequest).IdDocumentoLink))
                    return CodiceErrore.DocumentoNonDefinito;
            }
            if (string.IsNullOrEmpty(request.IdCliente))
            {
                return CodiceErrore.IdClienteErrato;
            }

            if (response.TokenInfo.IdCliente != request.IdCliente)
            {
                return CodiceErrore.UtenteNonAbilitatoPerIlCliente;
            }

            if (string.IsNullOrEmpty(request.IdClient))
            {
                return CodiceErrore.IdClientErrato;
            }

            //if (string.IsNullOrEmpty(request.IdRichiesta))
            //{
            //    return CodiceErrore.IdRichiestaErrata;
            //}

            return CodiceErrore.NessunErrore;
        }
    }
}