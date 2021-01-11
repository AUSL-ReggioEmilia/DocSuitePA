using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.StampConforme.Models.SecureDocument;

namespace VecompSoftware.StampaConforme.Services.DOU
{
    public class DOUResponseHelper
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public DOUResponseHelper()
        {

        }
        #endregion

        #region [ Methods ]
        public bool ValidateResponse(DOUService.DOUResponse response)
        {
            return response.status == (int)DOUResponseStatus.Ok;
        }

        public string ReadStatusMessage(DOUService.DOUResponse response)
        {
            string message = string.Empty;
            DOUResponseStatus status = (DOUResponseStatus)response.status;
            switch (status)
            {
                case DOUResponseStatus.Ok:
                    message = "OK";
                    break;
                case DOUResponseStatus.InputParameterError:
                    message = "Errore inserimento parametri di input al WS";
                    break;
                case DOUResponseStatus.InvalidDocumentType:
                    message = "Il file in input non e’ tra quelli elaborabili dal WS";
                    break;
                case DOUResponseStatus.CallerVerifyError:
                    message = "Errore verifica del chiamante";
                    break;
                case DOUResponseStatus.NotAuthorized:
                    message = "Chiamante non autorizzato";
                    break;
                case DOUResponseStatus.SaveDataToDbError:
                    message = "Errore durante il salvataggio dei dati del PDF sul DB";
                    break;
                case DOUResponseStatus.SaveDataToDbGenericError:
                    message = "Errore generico durante il salvataggio dei dati del PDF sul DB";
                    break;
                case DOUResponseStatus.VerifyFromDbError:
                    message = "Errore durante la verifica dei dati del PDF su DB";
                    break;
                case DOUResponseStatus.VerifyFromDbGenericError:
                    message = "Errore generico durante la verifica dei dati del PDF su DB";
                    break;
                case DOUResponseStatus.DeleteTemporaryDataError:
                    message = "Errore durante l’eliminazione dei dati temporanei del PDF";
                    break;
                case DOUResponseStatus.DeleteTemporaryDataGenericError:
                    message = "Errore generico durante l’eliminazione dei dati temporanei del PDF";
                    break;
                case DOUResponseStatus.DOUManagerInitializationError:
                    message = "Errore nell’inizializzazione della classe DOUManager";
                    break;
                case DOUResponseStatus.DOUManagerInitializationGenericError:
                    message = "Errore generico nell’inizializzazione della classe DOUManager";
                    break;
                case DOUResponseStatus.PDFNotRegistered:
                    message = "PDF non registrato su database";
                    break;
                case DOUResponseStatus.ReadArchiveConfigurationFromDbError:
                    message = "Errore durante il prelevamento delle configurazioni di archiviazione da DB";
                    break;
                case DOUResponseStatus.ReadArchiveConfigurationFromDbGenericError:
                    message = "Errore generico durante il prelevamento delle configurazioni di archiviazione da DB";
                    break;
                case DOUResponseStatus.ArchiverInitializationError:
                    message = "Errore durante l’inizializzazione dell’oggetto Archiver";
                    break;
                case DOUResponseStatus.ArchiverInitializationGenericError:
                    message = "Errore generico durante l’inizializzazione dell’oggetto Archiver";
                    break;
                case DOUResponseStatus.ArchiveDocumentError:
                    message = "Errore durante l’archiviazione del documento";
                    break;
                case DOUResponseStatus.ArchiveDocumentGenericError:
                    message = "Errore generico durante l’archiviazione del documento";
                    break;
                case DOUResponseStatus.SaveArchiveDataToDbError:
                    message = "Errore durante il salvataggio dei dati di archiviazione su DB";
                    break;
                case DOUResponseStatus.SaveArchiveDataToDbGenericError:
                    message = "Errore generico durante il salvataggio dei dati di archiviazione su DB";
                    break;
                case DOUResponseStatus.ArchiveDataNotSaved:
                    message = "Dati di archiviazione non salvati su DB";
                    break;
                case DOUResponseStatus.GenerateDocumentHashError:
                    message = "Errore durante il calcolo della hash del documento";
                    break;
                case DOUResponseStatus.SignDocumentHashError:
                    message = "Errore durante la firma della hash del documento";
                    break;
                case DOUResponseStatus.SignDocumentHashGenericError:
                    message = "Errore generico durante la firma della hash del documento";
                    break;
                case DOUResponseStatus.VerifyDocumentError:
                    message = "Errore durante la verifica del file archiviato tramite l’ID di archiviazione";
                    break;
                case DOUResponseStatus.VerifyDocumentGenericError:
                    message = "Errore generico durante la verifica del file archiviato tramite l’ID di archiviazione";
                    break;
                case DOUResponseStatus.ArchiveDataNotFound:
                    message = "Nessun dato archiviazione con l’ID archiviazione ricevuto";
                    break;
                case DOUResponseStatus.ReadConfigurationFromDbError:
                    message = "Errore durante l’ottenimento delle configurazioni da DB";
                    break;
                case DOUResponseStatus.ReadConfigurationFromDbGenericError:
                    message = "Errore generico durante l’ottenimento delle configurazioni da DB";
                    break;
                case DOUResponseStatus.IdArchiveMatchError:
                    message = "ID archiviazione in input non corrisponde con quello estratto dal QRCode";
                    break;
                case DOUResponseStatus.IdServiceMathcError:
                    message = "ID service in input non corrispondente a quello associato al pdf";
                    break;
                case DOUResponseStatus.TemporaryArchiveError:
                    message = "Errore durante l’archiviazione temporanea del documento";
                    break;
                case DOUResponseStatus.ConfigurationInitializeError:
                    message = "Errore durante l’inizializzazione degli oggetti di configurazione";
                    break;
                case DOUResponseStatus.QRCodeReadError:
                    message = "Errore durante l’estrazione dei dati dal QRCode";
                    break;
            }
            return message;
        }
        #endregion
    }
}
