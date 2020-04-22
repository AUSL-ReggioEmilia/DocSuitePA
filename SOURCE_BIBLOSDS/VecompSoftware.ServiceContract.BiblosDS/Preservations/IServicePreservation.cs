using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ComponentModel;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Objects.Response;
using BiblosDS.Library.Common.Objects.Enums;

namespace VecompSoftware.ServiceContract.BiblosDS.Preservations
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IServicePreservation" in both code and config file together.
    [ServiceContract(
    SessionMode = SessionMode.Required,
    CallbackContract = typeof(IServicePreservationCallback))]
    public partial interface IServicePreservation : IBiblosDSServiceContract
    {
        /// <summary>
        /// Ritorna l'elenco degli archivi sottoposti a sostitutiva.
        /// Se la datatable si chiama "Errore" allora si è verificato un errore e conterrà le informazioni relative all'errore.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationArchiveInfoResponse> GetPreservationArchives(string domainUserName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainUserName"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationUserRole> GetUserRoleInArchive(string domainUserName, string archiveName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="idArchive"></param>
        /// <returns></returns>
        [OperationContract]
        Dictionary<string, string> GetPreservationParameter(string archiveName);
        /// <summary>
        /// Ritorna il primo parametro avente label = "paramKey".
        /// </summary>
        /// <param name="paramKey"></param>
        /// <returns></returns>
        [OperationContract]
        string GetFirstPreservationParameter(string paramKey);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="archiveName"></param>
        /// <param name="domainUserName"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [OperationContract]
        bool IsUserInRoleOnArchive(string archiveName, string domainUserName, int roleId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainUserName"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [OperationContract]
        bool IsUserInRole(string domainUserName, int roleId);

        /// <summary>
        /// Lista gruppo task associati ad una conservazione su di un archivio
        /// </summary>
        /// <param name="archiveName"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationTaskGroup> GetPreservationTaskGroup(string archiveName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idTaskGroup"></param>
        /// <param name="idArchive"></param>
        [OperationContract]
        void SetTaskGroupClosed(Guid idTaskGroup, string archiveName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idTaskGroup"></param>
        /// <returns></returns>
        [OperationContract]
        Guid GetScheduleFromTaskGroup(Guid idTaskGroup);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idSchedule"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationSchedule> GetSchedule(Nullable<Guid> idSchedule = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservation"></param>
        [OperationContract]
        void ResetPreparedPreservation(Guid idPreservation);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<Document> FindPreparedPreservationObjects(DateTime dateFrom, DateTime dateTo, Guid idArchive);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservation"></param>
        /// <returns></returns>
        [OperationContract]
        string GetPreservationName(Guid idPreservation);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservation"></param>
        [OperationContract]
        void MarkPreservationAsSigned(Guid idPreservation, byte[] signedFile, byte[] timeStampFile);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservation"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        [OperationContract]
        PreservationInfoResponse AbortConservation(Guid idPreservation);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservation"></param>
        /// <param name="exceptions"></param>
        /// <returns></returns>
        [OperationContract]
        bool CheckPreservationExceptions(Guid idPreservation, out string exceptions);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservation"></param>
        /// <param name="fileName"></param>
        [OperationContract]
        void CreatePreservationIndexFile(Guid idPreservation, string fileName);


        [OperationContract]
        PreservationInfoResponse GetPreservationInfo(string archiveName, Nullable<Guid> idTaskGroup);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idArchive"></param>
        /// <param name="domainUser"></param>
        /// <returns></returns>
        [OperationContract]
        PreservationUser GetPreservationUserForArchive(string archiveName, string domainUser);
        /// <summary>
        /// Numero di documenti da preservare per archivio nell'itervallo di date.
        /// </summary>
        /// <param name="idArchive"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [OperationContract]
        int GetSelectedDocumentsNumber(string archiveName, DateTime startDate, DateTime endDate);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        BindingList<DocumentAttribute> GetAttributes(string archiveName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservation"></param>
        /// <param name="workingDirectory"></param>
        /// <returns></returns>
        [OperationContract]
        string CopyCompEDViewerIndexFile(Guid idPreservation, string workingDirectory);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservationHolidays"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationHoliday> GetHolidays(Nullable<Guid> idPreservationHolidays);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservation"></param>
        /// <param name="workingDir"></param>
        /// <param name="exceptions"></param>
        /// <returns></returns>
        [OperationContract]
        string CreatePreservationClosingFile(Guid idPreservation, string workingDir, string exceptions);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservationTaskGroup"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationTaskGroup> GetTaskGroup(Nullable<Guid> idPreservationTaskGroup = null);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAlert"></param>
        /// <param name="idTaskType"></param>
        /// <param name="idAlertType"></param>
        /// <param name="idTask"></param>
        /// <param name="orderByOffset"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationAlert> GetPreservationAlert(Nullable<Guid> idAlert, Nullable<Guid> idTaskType, Nullable<Guid> idAlertType, Nullable<Guid> idTask, bool orderByOffset = false);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idTaskGroupType"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationTaskGroupType> GetTaskGroupTypes(Nullable<Guid> idTaskGroupType);
        /// <summary>
        /// Ritorna l'utente - o la lista di utenti - associati ad un archivio.
        /// </summary>
        /// <param name="idUser">Eventuale utente associato all'archivio. Se nullo ritorna la lista di TUTTI gli utenti associati all'archivio.</param>
        /// <param name="archiveName">Nome archivio di riferimento.</param>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationUser> GetPreservationUser(Nullable<Guid> idUser, string archiveName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservationTaskType"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationTaskType> GetPreservationTaskTypes(Nullable<Guid> idPreservationTaskType);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idTaskGroup"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationTask> GetTasksFromTaskGroup(Guid idTaskGroup);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toAdd"></param>
        /// <param name="archiveName"></param>
        /// <returns></returns>
        [OperationContract]
        PreservationTaskGroup AddPreservationTaskGroup(PreservationTaskGroup toAdd, string archiveName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toAdd"></param>
        /// <param name="archiveName"></param>
        /// <returns></returns>
        [OperationContract]
        PreservationTask AddPreservationTask(PreservationTask toAdd, string archiveName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toAdd"></param>
        /// <returns></returns>
        [OperationContract]
        PreservationAlert AddPreservationAlert(PreservationAlert toAdd);

        /// <summary>
        /// Ritorna la conservazione associata avente id = idPreservation.
        /// </summary>
        /// <param name="idPreservation">Identificativo della conservazione.</param>
        /// <returns></returns>
        [OperationContract]
        Preservation GetPreservation(Guid idPreservation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservationSchedule"></param>
        /// <param name="archiveName"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationTaskType> GetPreservationTaskTypesAndPreservationScheduleTaskTypes(Nullable<Guid> idPreservationSchedule, string archiveName);

        /// <summary>
        /// Creazione della conservazione sostitutiva
        /// </summary>
        /// <param name="archiveName"></param>
        /// <param name="domainUser"></param>
        /// <param name="idGruppoTask"></param>
        /// <param name="dataInizio"></param>
        /// <param name="dataFine"></param>
        [OperationContract]
        PreservationInfoResponse CreatePreservation(string archiveName, string domainUser, Guid idGruppoTask, DateTime dataInizio, DateTime dataFine, bool simulateOnly);

        [OperationContract]
        PreservationInfoResponse CreatePreservationByTask(PreservationTask task);

        [OperationContract]
        PreservationFileInfoResponse GetPreservationClosingFileInfo(Guid preservationId);

        [OperationContract]
        void ClosePreservation(Guid idPreservation);

        /// <summary>
        /// Riapre l'ultima conservazione eventualmente storicizzata in banca dati.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        PreservationInfoResponse ResetPreservation(Guid idPreservation, string domainUser);
        /// <summary>
        /// Torna l'elenco di TUTTE le conservazioni.
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="totalPreservations"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<Preservation> GetPreservations(string archiveName, int take, int skip, out int totalPreservations);

        [OperationContract]
        BindingList<Preservation> GetPreservationsToSign(string archiveName, int take, int skip, out int totalPreservations);
        /// <summary>
        /// Ritorna l'elenco di tutte le attività di journaling presenti sul server.
        /// </summary>
        /// <param name="idJournalingActivity">Eventuale ID dell'attivita' da recuperare.</param>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationJournalingActivity> GetPreservationJournalingActivities(Nullable<Guid> idJournalingActivity);

        /// <summary>
        /// Aggiunta dell'attività di verifica
        /// </summary>
        /// <param name="idPreservation"></param>
        /// <param name="domainUser"></param>
        [OperationContract]
        void AddVerifyPreservationToJournaling(Guid idPreservation, string domainUser);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toUpdate"></param>
        [OperationContract]
        void UpdatePreservationJournaling(PreservationJournaling toUpdate);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toAdd"></param>
        /// <returns></returns>
        [OperationContract]
        PreservationJournaling AddPreservationJournaling(PreservationJournaling toAdd);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Nullable<DateTime> GetPreservationJournalingLastPrintManualActivityDate();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idStatus"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationStorageDeviceStatus> GetPreservationStorageDeviceStatus(Nullable<Guid> idStatus);
        /// <summary>
        /// Ritorna l'elenco dei messaggi di journaling associati ad una conservazione e/o ad un archivio.
        /// E' inoltre possibile specificare il filtro per le date attività.
        /// </summary>
        /// <param name="archiveName"></param>
        /// <param name="idPreservation"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="journalingsInArchive"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationJournaling> GetPreservationJournalings(string archiveName, Guid? idPreservation, DateTime? startDate, DateTime? endDate, int skip, int take, out int journalingsInArchive);
        /// <summary>
        /// Ritorna l'elenco delle conservazioni cui è associato un dato journal.
        /// </summary>
        /// <param name="idJournaling"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="preservationsCount"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<Preservation> GetPreservationsFromJournaling(Guid idJournaling, int skip, int take, out int preservationsCount);

        [OperationContract]
        BindingList<Preservation> GetPreservationsFromArchive(string archiveName, int take, int skip, out long totalItems);

        [OperationContract]
        Document GetPreservedDocument(Guid idPreservation, string name);
        /// <summary>
        /// Ritorna il tipo di codifica usata (SHA1 o SHA256) se applicabile.
        /// </summary>
        /// <param name="encryptedBuffer"></param>
        /// <returns></returns>
        [OperationContract]
        CryptoType GetUsedCryptography(byte[] encryptedBuffer);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePathName"></param>
        /// <param name="useSHA256"></param>
        /// <returns></returns>
        [OperationContract]
        string GetHashFromFile(string filePathName, bool useSHA256);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="useSHA256"></param>
        /// <returns></returns>
        [OperationContract]
        string GetHashFromBuffer(byte[] buffer, bool useSHA256);
        /// <summary>
        /// Ritorna il conteggio assoluto dei supporti archivio presenti in banca dati.
        /// </summary>
        /// <returns>Numero totale di supporti in banca dati.</returns>
        [OperationContract]
        int GetPreservationStorageDevicesCount();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idArchive"></param>
        /// <param name="companyName"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<ArchiveCompany> GetArchiveCompany(Guid? idArchive, string companyName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idArchive"></param>
        /// <param name="companyName"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<ArchiveCompany> GetArchiveCompanyByUser(Guid? idArchive, string companyName, string username);

        [OperationContract]
        bool IsAlive();
    }

    public interface IServicePreservationCallback
    {
        [OperationContract(IsOneWay = true)]
        void Pulse(string source, string message, int progressPercentage);
    }
}
