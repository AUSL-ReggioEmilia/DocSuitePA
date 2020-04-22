using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ComponentModel;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Enums;
using BiblosDS.Library.Common.Objects.Response;

namespace VecompSoftware.ServiceContract.BiblosDS.Preservations
{
    public partial interface IServicePreservation : IBiblosDSServiceContract
    {
        /// <summary>
        /// Ottiene la lista di tutti gli archivi - anche quelli non che non hanno alcuna conservazione associata, se richiesto.
        /// </summary>
        /// <param name="onlyPreservedArchives">Flag che specifica se si vuole la lista di TUTTI gli archivi o SOLO quelli che hanno almeno una conservazione associata.</param>
        /// <returns></returns>
        [OperationContract]
        BindingList<DocumentArchive> GetAdministratedArchives(bool onlyPreservedArchives);

        /// <summary>
        /// Ottiene la lista degli utenti abilitati alla conservazione legata ad un dato archivio, aventi un determinato ruolo.
        /// </summary>
        /// <param name="idRole">ID del ruolo di cui devono far parte gli utenti.</param>
        /// <param name="archiveName">Nome dell'archivio associato alla conservazione.</param>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationUser> GetPreservationUsersInArchiveByRole(Guid idRole, string archiveName);

        /// <summary>
        /// Ritorna l'elenco dei ruoli se idRole nullo - oppure il ruolo avente l'id = idRole (se esso è effettivamente presente in banca dati).
        /// </summary>
        /// <param name="idRole"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationRole> GetRoles(Guid? idRole);

        /// <summary>
        /// Aggiorna le informazioni di un utente.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="archiveName"></param>
        [OperationContract]
        void UpdatePreservationUser(PreservationUser user, string archiveName);

        /// <summary>
        /// Inserisce un nuovo utente.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="archiveName"></param>
        /// <returns></returns>
        [OperationContract]
        PreservationUser AddPreservationUser(PreservationUser user, string archiveName);

        /// <summary>
        /// Elimina tutti i ruoli associati ad un utente autorizzato alla conservazione.
        /// </summary>
        /// <param name="idPreservationUser"></param>
        [OperationContract]
        void DeletePreservationUserRolesByPreservationUser(Guid idPreservationUser);

        /// <summary>
        /// Cancella un utente abilitato alla conservazione.
        /// </summary>
        /// <param name="idPreservationUser"></param>
        [OperationContract]
        void DeletePreservationUser(Guid idPreservationUser);

        /// <summary>
        /// Aggiunge un nuovo ruolo associato ad un soggetto conservazione.
        /// </summary>
        /// <param name="userRole">Associazione soggetto - ruolo</param>
        /// <param name="archiveName">Nome archivio di destinazione.</param>
        /// <returns></returns>
        [OperationContract]
        PreservationUserRole AddPreservationUserRole(PreservationUserRole userRole, string archiveName);

        /// <summary>
        /// Aggiunge un nuovo scadenziario.
        /// </summary>
        /// <param name="sched"></param>
        /// <returns></returns>
        [OperationContract]
        PreservationSchedule AddPreservationSchedule(PreservationSchedule sched, string archiveName);

        /// <summary>
        /// Aggiorna uno scadenziario.
        /// </summary>
        /// <param name="sched"></param>
        [OperationContract]
        void UpdatePreservationSchedule(PreservationSchedule sched, string archiveName);

        /// <summary>
        /// Elimina uno scadenziario.
        /// </summary>
        /// <param name="idPreservationSchedule"></param>
        [OperationContract]
        void DeletePreservationSchedule(Guid idPreservationSchedule);

        /// <summary>
        /// Elimina un tipo task associato ad un dato scadenziario.
        /// </summary>
        /// <param name="idPreservationSchedule"></param>
        [OperationContract]
        void DeletePreservationSchedule_TaskTypeBySchedule(Guid idPreservationSchedule);

        /// <summary>
        /// Ottiene la lista dei tipi task + elenco dei ruoli ad essi associati (se richiesto).
        /// </summary>
        /// <param name="idPreservationUserRole"></param>
        /// <param name="archiveName"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationTaskType> GetTaskTypesByUserRole(Nullable<Guid> idPreservationUserRole, string archiveName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskType"></param>
        /// <param name="archiveName"></param>
        [OperationContract]
        void UpdatePreservationTaskType(PreservationTaskType taskType, string archiveName);

        /// <summary>
        /// Aggiunge un nuovo tipo task.
        /// </summary>
        /// <param name="taskType"></param>
        /// <param name="archiveName"></param>
        /// <returns></returns>
        [OperationContract]
        PreservationTaskType AddPreservationTaskType(PreservationTaskType taskType, string archiveName);

        /// <summary>
        /// Ritorna la lista dettagliata dei GRUPPI TASK, eventualmente limitata ad un massimo di  "maxReturnedValues" righe.
        /// </summary>
        /// <param name="idTaskGroup"></param>
        /// <param name="archiveName">Nome archivio di riferimento.</param>
        /// <param name="maxReturnedValues">Righe massime ritornate (se minore di 1 torna TUTTE le righe, non considerando il tetto massimo).</param>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationTaskGroup> GetDetailedPreservationTaskGroup(Nullable<Guid> idTaskGroup, string archiveName, int maxReturnedValues = 100);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservationTask"></param>
        /// <param name="newEstimatedExpiry"></param>
        /// <param name="archiveName"></param>
        [OperationContract]
        void UpdatePreservationTaskGroupExpiryByTask(Guid idPreservationTask, DateTime newEstimatedExpiry, string archiveName);

        /// <summary>
        /// Ottiene la lista delle conservazioni sostitutive per un dato utente (e, se richiesto, assegnate allo stesso task).
        /// </summary>
        /// <param name="idPreservationUser">Id utente.</param>
        /// <param name="taskName">Eventuale nome del task che accomuna tutte le conservazioni in db.</param>
        /// <param name="archiveName">Nome dell'archivio cui fanno riferimento le conservazioni sostitutive.</param>
        /// <returns>Lista delle conservazioni sostitutive in banca dati.</returns>
        [OperationContract]
        BindingList<Preservation> GetPreservationsByUserAndTask(Guid idPreservationUser, string taskName, string archiveName);

        /// <summary>
        /// Aggiunge una festivita'.
        /// </summary>
        /// <param name="holiday"></param>
        /// <param name="archiveName"></param>
        /// <returns></returns>
        [OperationContract]
        PreservationHoliday AddPreservationHoliday(PreservationHoliday holiday, string archiveName);

        /// <summary>
        /// Aggiorna una festivita'.
        /// </summary>
        /// <param name="holiday"></param>
        /// <param name="archiveName"></param>
        [OperationContract]
        void UpdatePreservationHoliday(PreservationHoliday holiday, string archiveName);

        /// <summary>
        /// Elimina una festivita'.
        /// </summary>
        /// <param name="idPreservationHoliday"></param>
        /// <param name="archiveName"></param>
        [OperationContract]
        void DeletePreservationHoliday(Guid idPreservationHoliday, string archiveName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservationAlertType"></param>
        /// <param name="archiveName"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationAlertType> GetPreservationAlertTypes(Guid? idPreservationAlertType, string archiveName);

        /// <summary>
        /// Aggiorna un tipo avviso.
        /// </summary>
        /// <param name="alertType"></param>
        /// <param name="archiveName"></param>
        [OperationContract]
        void UpdatePreservationAlertType(PreservationAlertType alertType, string archiveName);

        /// <summary>
        /// Aggiunge un nuovo tipo avviso.
        /// </summary>
        /// <param name="alertType"></param>
        /// <param name="archiveName"></param>
        /// <returns></returns>
        [OperationContract]
        PreservationAlertType AddPreservationAlertType(PreservationAlertType alertType, string archiveName);

        /// <summary>
        /// Elimina un tipo di avviso.
        /// </summary>
        /// <param name="idPreservationAlertType"></param>
        /// <param name="archiveName"></param>
        [OperationContract]
        void DeletePreservationAlertType(Guid idPreservationAlertType, string archiveName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservationAlertType"></param>
        /// <param name="idPreservationTaskType"></param>
        /// <param name="archiveName"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationAlertType> GetPreservationAlertTypesByTaskType(Nullable<Guid> idPreservationAlertType, Guid idPreservationTaskType, string archiveName);

        /// <summary>
        /// Aggiorna un ruolo.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="archiveName"></param>
        [OperationContract]
        void UpdatePreservationRole(PreservationRole role, string archiveName);

        /// <summary>
        /// Aggiunge un ruolo.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="archiveName"></param>
        /// <returns></returns>
        [OperationContract]
        PreservationRole AddPreservationRole(PreservationRole role, string archiveName);

        /// <summary>
        /// Aggiunge un parametro di conservazione.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="value"></param>
        /// <param name="archiveName">Nome archivio di destinazione - se non si tratta di un parametro generale, comune a tutti gli archivi.</param>
        [OperationContract]
        void AddPreservationParameter(string label, string value, string archiveName);

        /// <summary>
        /// Modifica il valore di un parametro di conservazione.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="value"></param>
        /// <param name="filterName">Eventuale LABEL da usare come filtro nella query di aggiornamento banca dati.</param>
        /// <param name="archiveName">Nome archivio di destinazione - se non si tratta di un parametro generale, comune a tutti gli archivi.</param>
        [OperationContract]
        void UpdatePreservationParameter(string label, string value, string filterName, string archiveName);

        /// <summary>
        /// Elimina un parametro di conservazione.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="archiveName">Nome archivio di destinazione - se non si tratta di un parametro generale, comune a tutti gli archivi.</param>
        [OperationContract]
        void DeletePreservationParameter(string label, string archiveName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idSchedule"></param>
        /// <param name="idTaskGroupType"></param>
        /// <param name="archiveName"></param>
        /// <returns></returns>
        [OperationContract]
        PreservationExpireResponse GetPreservationExpire(Guid idSchedule, Guid idTaskGroupType, string archiveName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservation"></param>
        /// <param name="archiveName"></param>
        [OperationContract]
        void UpdatePreservationAsSigned(Guid idPreservation, string archiveName);

        /// <summary>
        /// Aggiorna il percorso dei file legati ad una conservazione.
        /// </summary>
        /// <param name="idPreservation"></param>
        /// <param name="path"></param>
        /// <param name="archiveName"></param>
        [OperationContract]
        void UpdatePreservationPath(Guid idPreservation, string path, string archiveName);

        /// <summary>
        /// Elimina un gruppo task e tutte i task, gli avvisi, ecc. ad esso correlati.
        /// </summary>
        /// <param name="idTaskGroup"></param>
        /// <param name="archiveName"></param>
        [OperationContract]
        void DeletePreservationTaskGroup(Guid idTaskGroup, string archiveName);

        /// <summary>
        /// Aggiorna la descrizione di un tipo gruppo task.
        /// </summary>
        /// <param name="idTaskGroupType"></param>
        /// <param name="description"></param>
        [OperationContract]
        void UpdatePreservationTaskGroupTypeDescription(Guid idTaskGroupType, string description);

        /// <summary>
        /// Aggiunge un nuovo tipo gruppo task.
        /// </summary>
        /// <param name="groupType"></param>
        /// <param name="archiveName"></param>
        /// <returns></returns>
        [OperationContract]
        PreservationTaskGroupType AddPreservationTaskGroupType(PreservationTaskGroupType groupType, string archiveName);

        /// <summary>
        /// Ritorna l'elenco dei supporti ove sono state memorizzate delle conservazioni.
        /// </summary>
        /// <param name="idPreservationStorageDevice"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationStorageDevice> GetPreservationStorageDevices(Nullable<Guid> idPreservationStorageDevice);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservationStorageDevice"></param>
        /// <param name="minDate"></param>
        /// <param name="maxDate"></param>
        /// <param name="username"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="totalItems"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationStorageDevice> GetPreservationStorageDevicesFromDates(Guid? idPreservationStorageDevice, DateTime? minDate, DateTime? maxDate, string username, int skip, int take, out long totalItems);

        /// <summary>
        /// Ritorna l'elenco delle conservazioni associate ad un supporto.
        /// </summary>
        /// <param name="idPreservation"></param>
        /// <param name="idPreservationStorageDevice"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="totalItems"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationInStorageDevice> GetPreservationsInStorageDevices(Guid? idPreservation, Guid? idPreservationStorageDevice, int skip, int take, out int totalItems);

        /// <summary>
        /// Aggiunge una conservazione su supporto.
        /// </summary>
        /// <param name="toAdd"></param>
        /// <returns></returns>
        [OperationContract]
        PreservationInStorageDevice AddPreservationInStorageDevice(PreservationInStorageDevice toAdd);

        /// <summary>
        /// Elimina una conservazione su supporto.
        /// </summary>
        /// <param name="preservationInStorageDevice"></param>
        /// <returns></returns>
        [OperationContract]
        void DeletePreservationInStorageDevice(PreservationInStorageDevice preservationInStorageDevice);

        /// <summary>
        /// Elimina uno storage.
        /// </summary>
        /// <param name="preservationStorageDevice"></param>
        /// <returns></returns>
        [OperationContract]
        void DeletePreservationStorageDevice(PreservationStorageDevice preservationStorageDevice);

        /// <summary>
        /// Aggiunge un supporto per la conservazione.
        /// </summary>
        /// <param name="toAdd"></param>
        /// <returns></returns>
        [OperationContract]
        PreservationStorageDevice AddPreservationStorageDevice(PreservationStorageDevice toAdd);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservation"></param>
        /// <param name="preservationStatus"></param>
        /// <returns></returns>
        [OperationContract]
        PreservationStorageDeviceStatus PreservationStorageDeviceChangeStatus(Guid idPreservation, PreservationStatus preservationStatus);

        /// <summary>
        /// Elimina una riga di journaling dal database.
        /// </summary>
        /// <param name="idJournaling"></param>
        /// <param name="archiveName"></param>
        [OperationContract]
        void DeletePreservationJournaling(Guid idJournaling, string archiveName);

        /// <summary>
        /// Ritorna l'elenco dei supporti aventi una certa etichetta ed EVENTUALMENTE associati ad un dato archivio.
        /// </summary>
        /// <param name="label">Etichetta supporto</param>
        /// <param name="archiveName">[facoltativo] Nome archivio associato - puo' essere NULL o VUOTO</param>
        /// <returns></returns>
        [OperationContract]
        BindingList<PreservationStorageDevice> GetPreservationStorageDeviceFromLabel(string label, string archiveName);

        /// <summary>
        /// Aggiorna la data ultima verifica di un supporto.
        /// </summary>
        /// <param name="idStorageDevice">Id supporto.</param>
        /// <param name="verifyDate">Data ultima verifica (oppure NULL).</param>
        [OperationContract]
        void UpdatePreservationStorageDeviceLastVerifyDate(Guid idStorageDevice, Nullable<DateTime> verifyDate);
        /// <summary>
        /// Crea il file XML d'impronta archivio.
        /// </summary>
        /// <param name="idStorageDevice"></param>
        /// <returns>Esito creazione file.</returns>
        [OperationContract]
        bool CreateArchivePreservationMark(Guid idStorageDevice);
        /// <summary>
        /// Scarica il file d'impronta archivio dal server.
        /// </summary>
        /// <param name="idPreservationStorageDevice"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        [OperationContract]
        byte[] GetArchivePreservationMarkFile(Guid idPreservationStorageDevice, long skip, int take, out long fileSize);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservationStorageDevice"></param>
        /// <returns></returns>
        [OperationContract]
        byte[] GetClosingFilesTimeStampFile(Guid idPreservationStorageDevice, long skip, int take, out long fileSize);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservationStorageDevice"></param>
        /// <param name="timeStampedFile"></param>
        /// <param name="isInfoCamere"></param>
        /// <returns></returns>
        [OperationContract]
        bool TimeStampArchivePreservationMarkFile(Guid idPreservationStorageDevice, byte[] timeStampedFile, bool isInfoCamere);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPreservationStorageDevice"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        [OperationContract]
        byte[] GetTimeStampedArchivePreservationMarkFile(Guid idPreservationStorageDevice, long skip, int take, out long fileSize);
        /// <summary>
        /// Torna la stringa formato esadecimale del contenuto di un file criptato con SHA1.
        /// </summary>
        /// <param name="content">Contenuto del file.</param>
        /// <returns>Stringa criptata in formato esadecimale.</returns>
        [OperationContract]
        string GetSHA1Mark(byte[] content);

        /// <summary>
        /// Torna la stringa formato esadecimale del contenuto di un file criptato con SHA256.
        /// </summary>
        /// <param name="content">Contenuto del file.</param>
        /// <returns>Stringa criptata in formato esadecimale.</returns>
        [OperationContract]
        string GetSHA256Mark(byte[] content);

        /// <summary>
        /// Ritorna lo scadenziario predefinito.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        PreservationSchedule GetDefaultPreservationSchedule();

        /// <summary>
        /// Torna una lista di conservazioni, aventi data chiusura compresa fra "minDate" e "maxDate", estremi inclusi, a partire da un archivio.
        /// </summary>
        /// <param name="archiveName"></param>
        /// <param name="minDate"></param>
        /// <param name="maxDate"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <param name="totalItems"></param>
        /// <returns></returns>
        [OperationContract]
        BindingList<Preservation> GetPreservationsFromArchiveAndDates(string archiveName, DateTime minDate, DateTime maxDate, int take, int skip, out long totalItems);
        /// <summary>
        /// Carica su server un file di risposta dell'Agenzia delle Entrate.
        /// </summary>
        /// <param name="clientFileName">Nome del file sul client.</param>
        /// <param name="idPreservationStorageDevice">Id del supporto</param>
        /// <param name="fileContent">Blob (contenuto) del file di risposta.</param>
        /// <returns>Nome del file FORMATTATO secondo questa formula: [Guid univoco]_[Nome file sul client].[Estensione nome file sul client]</returns>
        [OperationContract]
        string UploadEntratelFile(string clientFileName, Guid idPreservationStorageDevice, byte[] fileContent);
        /// <summary>
        /// Scarica dal server il contenuto del file di risposta entratel.
        /// </summary>
        /// <param name="idPreservationStorageDevice">Id del supporto.</param>
        /// <param name="fileLenght">Lunghezza - in bytes - del file Entratel.</param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>Blob del file.</returns>
        [OperationContract]
        byte[] DownloadEntratelFile(Guid idPreservationStorageDevice, long skip, int take, out long fileLenght);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        [OperationContract]
        string GetCurrentArchivePreservationMarkXmlTemplate(string companyName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlContent"></param>
        /// <param name="companyName"></param>
        [OperationContract]
        void ChangeCurrentArchivePreservationMarkXmlTemplate(string xmlContent, string companyName);
    }
}
