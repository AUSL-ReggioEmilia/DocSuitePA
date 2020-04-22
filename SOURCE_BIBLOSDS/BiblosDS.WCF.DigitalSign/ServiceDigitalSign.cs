using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml;
using System.Collections;
using System.ComponentModel;

using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common;
using BiblosDS.Library.Common.Enums;
using BiblosDS.Library.Common.CompEd;
using BiblosDS.Library.Common.Objects.Enums;
using System.ServiceModel.Activation;
using BiblosDS.WCF.DigitalSign;
using VecompSoftware.ServiceContract.BiblosDS.Signs;

#if WCF_Documents
namespace BiblosDS.WCF.DigitalSign
{
#endif
#if WCF_Documents    
    public class ServiceDigitalSign : IServiceDigitalSign
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ServiceDigitalSign));
#else
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class DigitalSignature : IServiceDigitalSign
{
    log4net.ILog logger = log4net.LogManager.GetLogger(typeof(DigitalSignature));
#endif
    #region IServiceDigitalSign Members

    /// <summary>
    /// Funzione da chiamare solo per p7m\p7x
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="Content"></param>
    /// <param name="FirstCertificate"></param>
    /// <returns>BindingList<DocumentCertificate></returns>
    public BindingList<DocumentCertificate> GetAllExpireDates(string fileName, DocumentContent Content, out DocumentCertificate FirstCertificate)
    {
        try
        {
            logger.DebugFormat("GetAllExpireDates {0}", fileName);
            return new CompEdService().GetAllExpireDates(fileName, Content, out FirstCertificate);
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            throw;
        }
    }

    //TODO Verificare se lasciare o togliere.
    public DocumentContent CalculateBlobHash(DocumentContent Content, ComputeHashType computeHash)
    {
        return new DocumentContent(CompEdObj.CalculateBlobHash(Content.Blob, computeHash));
    }

    //TODO
    //Chiamo lo storage
    //Calcolo la Hash
    //Ritorna
    //public DocumentContent CalculateBlobHash(Document Document)

    /// <summary>
    /// 
    /// </summary>
    /// <param name="FileName"></param>
    /// <param name="SignerCert"></param>
    /// <param name="SignedDigest"></param>
    /// <param name="Content"></param>
    /// <returns>DocumentContent</returns>
    public DocumentContent AddRawSignature(string FileName, DocumentContent SignerCert, DocumentContent SignedDigest, DocumentContent Content)
    {
        using (CompEdObj p7mO = new CompEdObj())
        {
            int p7kH;
            p7mO.P7mNew(out p7kH);
            p7mO.VerifyLastError();

            p7mO.P7mSetType(p7kH);
            p7mO.VerifyLastError();

            p7mO.AddRawSignature(p7kH, SignedDigest.Blob, SignerCert.Blob);
            p7mO.VerifyLastError();

            p7mO.P7mAddTimeFileDescription(p7kH, "", FileName, "");
            p7mO.VerifyLastError();

            p7mO.P7mContentInit(p7kH);
            p7mO.VerifyLastError();

            p7mO.P7mAddBlob(p7kH, Content.Blob);
            p7mO.VerifyLastError();

            p7mO.P7mContentFinish(p7kH);
            p7mO.VerifyLastError();

            byte[] ret = p7mO.P7kContentReadToBuf(p7kH);
            p7mO.VerifyLastError();

            return new DocumentContent(ret);
        }
    }

    public DocumentContent GetContent(string fileName, DocumentContent Content)
    {
        try
        {
            logger.DebugFormat("GetContent {0}", fileName);
            using (CompEdLib p7m = new CompEdLib())
            {
                Boolean isP7M = String.Compare("P7M", fileName.Substring(fileName.Length - 3), true) == 0;
                String MetaData;
                DocumentContent result = new DocumentContent(p7m.GetContent(isP7M, Content.Blob, out MetaData));
                //Ritorniamo un Xml nel campo Description per permettere future integrazioni di tag XML
                //Sono informazioni che spesso non ci sono.
                result.Description = MetaData;
                return result;
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            throw;
        }
    }

    public DocumentContent TimeStampDocument(string FileName, DocumentContent Content, bool IsInfoCamereFormat)
    {
        DocumentContent newTimeStampDocument = null;

        CompEdLib p7m = null;
        try
        {
            p7m = new CompEdLib();
            newTimeStampDocument = new DocumentContent(p7m.P7xTimeStampDocument(IsInfoCamereFormat, FileName, Content.Blob));
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            if (p7m != null)
                p7m.Dispose();
        }
        return newTimeStampDocument;
    }

    /// <summary>
    /// Torna quante marche temporali hai a disposizione come stringa contentente il file XML
    /// </summary>
    /// <param name="Service"></param>
    /// <param name="User"></param>
    /// <param name="Password"></param>
    /// <returns>string</returns>
    public string GetTimeStampAvailable(string Service, string User, string Password)
    {
        return CompEdLib.GetTimeStampCount(Service, User, Password);
    }

    #endregion

    #region IServiceDigitalSign Support Members

    /// <summary>
    /// Indica che il componente Comped è installato e funzionante
    /// </summary>
    /// <returns>bool</returns>
    public bool IsAlive()
    {
        try
        {
            CompEdLib p7m = new CompEdLib();
            p7m.Dispose();

            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    #endregion


    public void TimeStampDigitalDocument(Guid idDocument, string timeStampAccount, decimal? version)
    {
        throw new NotImplementedException();
    }

    public void SignDigitalDocument(Guid idDocument, string docDigest, DocumentCertificate signerCert, decimal? version)
    {
        throw new NotImplementedException();
    }

    public int GetDigitalTimestampAvailable(string timestampAccount)
    {
        throw new NotImplementedException();
    }

    public string[] GetDocumentsHash(Guid[] idDocuments)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// ritorna true se il documentcontent è firmato
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public bool IsBlobSigned(DocumentContent content)
    {
        CompEdLib p7m = new CompEdLib();
        return p7m.GetSignatureCount(content.Blob) > 0;
    }
}
#if WCF_Documents
}
#endif