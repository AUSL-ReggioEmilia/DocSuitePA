using System;
using System.Collections.Generic;
using System.Linq;
using BiblosDS.API.DocumentServiceReference;
using BiblosDS.API.Model;
using System.Security.Cryptography;
using BiblosDS.Library.Common.Objects;

namespace BiblosDS.API
{
    internal class Helpers
    {
        internal static DocumentAttribute[] GetBiblosDSMetadataStructure(Guid idArchive)
        {
            using (var client = new DocumentsClient())
            {
                return client.GetMetadataStructure(idArchive);
            }
        }

        internal static void SetDocumentKey(RequestBase request, Guid idDocumento, string chiave)
        {
            using (var db = new BiblosDS2010APIEntities())
            {
                DocumentKey documentKey = db.DocumentKeys.Where(x => x.DocumentClass == request.TipoDocumento && x.IdCustomer == request.IdCliente && x.Code == chiave).SingleOrDefault();
                if (documentKey == null)
                {
                    db.AddToDocumentKeys(new DocumentKey { DocumentClass = request.TipoDocumento, IdCustomer = request.IdCliente, Code = chiave, IdDocument = idDocumento, CreatedDate = DateTime.Now });
                    db.SaveChanges();
                }
                else
                    throw new Exception("Document alrady exists: "+ idDocumento + ", chiave: "+ chiave);
            }
        }

        internal static Guid? GetArchive(RequestBase request)
        {
            CustomerKey customerKey;
            using (var db = new BiblosDS2010APIEntities())
            {
                customerKey = db.CustomerKeys.Where(x => x.DocumentClass == request.TipoDocumento && x.IdCustomer == request.IdCliente).SingleOrDefault();
            }

            //Ritorna l'archivio legato al customer/document class
            return customerKey == null ? (Guid?) null : customerKey.IdArchive;
        }

        internal static bool ExistDocumentById(RequestBase request, Guid idDocumento)
        {
            bool exist = false;

            using (var db = new BiblosDS2010APIEntities())
            {
                exist = db.DocumentKeys.Any(x => x.IdCustomer == request.IdCliente && x.IdDocument == idDocumento);
            }

            //Ritorna il documento legato al customer/document class
            return exist;
        }

        internal static bool ExistDocumentByChiave(RequestBase request, string chiave)
        {
            bool exist = false;

            using (var db = new BiblosDS2010APIEntities())
            {
                exist = db.DocumentKeys.Any(x => x.IdCustomer == request.IdCliente && x.Code == chiave);
            }

            //Ritorna il documento legato al customer/document class
            return exist;
        }

        internal static bool ExistDocumentChiave(string idCliente, string chiave)
        {
            bool exist = false;

            using (var db = new BiblosDS2010APIEntities())
            {
                exist = db.DocumentKeys.Any(x => x.IdCustomer == idCliente && x.Code == chiave);
            }

            //Ritorna il documento legato al customer/document class
            return exist;
        }        

        internal static Guid? GetDocumentByChiave(RequestBase request, string chiave, out CodiceErrore errore)
        {
            return GetDocumentByChiave(request.TipoDocumento, request.IdCliente, chiave, out errore);
        }

        internal static Guid? GetDocumentByChiave(string tipoDocumento, string idCliente, string chiave, out CodiceErrore errore)
        {
            DocumentKey documentKey;
            errore = CodiceErrore.NessunErrore;
            using (var db = new BiblosDS2010APIEntities())
            {
                documentKey = db.DocumentKeys.Where(x => x.IdCustomer == idCliente && x.DocumentClass == tipoDocumento && x.Code == chiave).SingleOrDefault();
            }
            if (documentKey == null)
                return null;
            if (documentKey.IdCustomer != idCliente)
                errore = CodiceErrore.DocumentoNonDelCliente;
            //Ritorna il documento legato al customer/document class
            return documentKey == null ? null : documentKey.IdDocument;
        }

        internal static void DeleteDocumentKey(Guid idDocument)
        {            
            using (var db = new BiblosDS2010APIEntities())
            {
                var documentKey = db.DocumentKeys.Where(x => x.IdDocument == idDocument).SingleOrDefault();
                db.DeleteObject(documentKey);
                db.SaveChanges();
            }            
        }

        internal static void UpdateDocumentIdByIdDocument(Guid idDocument, Guid newIdDocument)
        {
            using (var db = new BiblosDS2010APIEntities())
            {
                var documentKey = db.DocumentKeys.Where(x => x.IdDocument == idDocument).SingleOrDefault();
                documentKey.IdDocument = newIdDocument;
                db.SaveChanges();
            }  
        }

        internal static string GetDocumentHash(DocumentContent Content)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            return GetStringFromBob(sha.ComputeHash(Content.Blob));
        }

        internal static string GetStringFromBob(byte[] Blob)
        {
            return System.Convert.ToBase64String(Blob);
        }

        internal static byte[] GetBlobFromString(string Base64Blob)
        {
            return System.Convert.FromBase64String(Base64Blob);
        }

        internal static TokenInfo ValidaToken(RequestBase request)
        {
            TokenInfo result = null;
            using (var db = new BiblosDS2010APIEntities())
            {
                var token = db.LoginTokens.Where(x => x.IdLoginToken == request.Token).SingleOrDefault();
                if (token != null)
                {
                    if (DateTime.Compare(DateTime.Now, token.DateExpire) > 0)
                    {
                        db.DeleteObject(token);
                        
                    }
                    else
                    {
                        result = new TokenInfo();
                        result.DataScadenza = DateTime.Now.AddMinutes(20);
                        result.Token = token.IdLoginToken;
                        result.IdCliente = token.IdCustomer;
                        token.DateExpire = result.DataScadenza;
                    }
                    db.SaveChanges();
                }                
            }
            return result;
        }

        internal static Dictionary<int, string> GetErrori()
        {
            var retval = new Dictionary<int, string>();
            var errorMsg = string.Empty;

            using (var db = new BiblosDS2010APIEntities())
            {
                var query = db.ErrorMessageDecodes;
                int errorCode;

                foreach (var item in query)
                {
                    try
                    {
                        if (!int.TryParse(item.ErrorCode, out errorCode))
                            throw new ApplicationException();

                        if (retval.ContainsKey(errorCode))
                        {
                            errorMsg = string.Format("GetErrori : Il codice d'errore \"{0}\" risulta duplicato in banca dati.", item.ErrorCode);
                            break;
                        }

                        retval.Add(errorCode, item.ErrorMessage);
                    }
                    catch
                    {
                        errorMsg = string.Format("Impossibile convertire il codice d'errore \"{0}\".", item.ErrorCode);
                        break;
                    }
                }

                if (!string.IsNullOrWhiteSpace(errorMsg))
                    throw new ApplicationException(errorMsg);
            }

            return retval;
        }

        
    }
}