using System;
using System.ServiceModel;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Enums;
using System.ComponentModel;
using BiblosDS.Library.Common.Objects.Response;
using BiblosDS.Library.Common.Exceptions;
using BiblosDS.Library.Common.Objects.Enums;
using System.Collections.Generic;

namespace VecompSoftware.ServiceContract.BiblosDS.Preservations
{
    /// <summary>
    /// Servizio pubblico di gestione conservazione sostitutiva
    /// </summary>
    [ServiceContract(Namespace = "http://Vecomp.BiblosDs.Preservation")]
    public interface IPreservation
    {
        /// <summary>
        /// Verifica una conservazione sostitutiva
        /// </summary>        
        /// <param name="idPreservation">Guid della conservazione sostitutiva</param>
        /// <param name="messages">array delle eventuali segnalazioni di anomalia riscontate</param>
        /// <returns>true se i controlli sono corretti</returns>
        /// <remarks>La verifica di una conservazione sostitutiva comprende la verifica della leggibilità del file di chiusura e dei file indice, 
        /// La verifica della falidità della marca temporale della chisura sostitutiva
        /// La corrispondenza delle hash dei documenti conservati rispetto a quelle riportate nel file di chiusura
        /// L'operazione è tracciata nella histroy della conservazione   
        /// </remarks>
        [OperationContract, FaultContract(typeof(BiblosDsException))]  
        bool VerifyPreservation(Guid idPreservation);

        /// <summary>
        /// Storia di una conservazione, riporta i fatti occorsi ad una conservazione sostitutiva
        /// </summary>
        /// <param name="idPreservation">Guid della conservazione sostitutiva</param>
        /// <returns>lista degli accadimenti di una conservazione sostitutiva</returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]  
        PreservationEvents[] PreservationHistory(Guid idPreservation);

        /// <summary>
        /// Controllo sullo stato del task di conservazione 
        /// </summary>
        /// <param name="idTask">Guid del task di conservazione</param>
        /// <returns>-2 = not yet ready, -1 = not found, 0 = have to be prepared, 1 = prepared, 2 = signed, 3 = timestamped, 4 = verified</returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]  
        int GetPreservationTaskStatus(Guid idTask);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idTask"></param>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]  
        bool PreparePreservationTask(Guid idTask);

        /// <summary>
        /// Identificativo della conservazione dato un task di conservazione
        /// </summary>
        /// <param name="idTask">Guid del task di conservazione</param>
        /// <returns>Guid della conservazione se è stata almeno preparata, null altrimenti</returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]  
        Guid GetIdPreservation(Guid idTask);

        /// <summary>
        /// ritorna l'importa Hash del file di chiusura di una conservazione
        /// </summary>
        /// <param name="idPreservation">Guid della conservazione</param>
        /// <returns></returns>
        /// <remarks>abilita la firma lato client della conservazione sostitutiva</remarks>
        [OperationContract, FaultContract(typeof(BiblosDsException))]  
        string GetClosingPreservationHash(Guid idPreservation);

        ///<summary>
        ///</summary>
        ///<returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]  
        bool ConfirmSignedPreservation();

        [OperationContract, FaultContract(typeof(BiblosDsException))]  
        DocumentContent GetPreservationAdEMark(Guid idPreservation, DocumentContentFormat outputFormat);

        [OperationContract, FaultContract(typeof(BiblosDsException))]  
        DocumentContent GetPreservationCloseFile(Guid idPreservation, DocumentContentFormat outputFormat);

        /// <summary>
        /// Crea uno o più task di conservazione sostitutiva.
        /// </summary>
        /// <param name="tasks">Lista di task da creare.</param>
        /// <returns>Task creati in caso di successo - tipicamente è utile gestire l'ID del task.</returns>
        [OperationContract]
        BindingList<PreservationTask> CreatePreservationTask(BindingList<PreservationTask> tasks);

        /// <summary>
        /// Aggiorna uno o più task di conservazione sostitutiva.
        /// E' possibile aggiornare, dal punto di vista della periodicità, anche i task eventualmente correlati.
        /// </summary>
        /// <param name="tasks">Lista dei task da aggiornare.</param>
        /// <param name="updateCorrelatedTasks">Se <code>TRUE</code> aggiorna la data di esecuzione stimata ANCHE dei task correlati.</param>
        /// <returns>Lista dei task aggiornati in caso di successo.</returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]  
        BindingList<PreservationTask> UpdatePreservationTask(BindingList<PreservationTask> tasks, bool updateCorrelatedTasks);

        /// <summary>
        /// Ritorna un singolo task.
        /// </summary>
        /// <param name="idTask"></param>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]  
        PreservationTask GetPreservationTask(Guid idTask);

        /// <summary>
        /// Lista dei task legati agli archivi passati
        /// </summary>
        /// <param name="archives"></param>
        /// <returns>
        /// 
        /// </returns>
        /// <remarks>
        /// Valutare necessità filtri su stato dei task: es. mi interessano solo i task chiusi o aperti.
        /// </remarks>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        PreservationTaskResponse GetPreservationTasksByArchive(BindingList<DocumentArchive> archives, int skip, int take);
        
        /// <summary>
        /// Ritorna la data più prossima di esecuzione prevista + la data di esecuzione schedulata più prossima 
        /// per i task di conservazione e di verifica appartenenti ad un dato archivio.
        /// </summary>
        /// <param name="idArchive">Id archivio</param>
        /// <param name="getOnlyVerifyDates">
        /// Questo flag specifica qualora si voglia recuperare le date del solo task 
        /// di verifica, lasciando perdere il task di conservazione.</param>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        BindingList<PreservationTaskDatesResponse> GetNextPreservationTaskDatesForArchive(Guid idArchive, BindingList<PreservationTaskTypes> types);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        PreservationInfoResponse ExecutePreservationTask(Guid idTask);

        /// <summary>
        /// Abilita un task di CONSERVAZIONE se il PIN di attivazione inserito corrisponde a quello assegnato al summenzionato task.
        /// Inoltre imposta la data di esecuzione del task di conservazione usando la seguente formula:
        ///  
        ///     data esecuzione = data esecuzione corrente + "mininumDaysOffset" giorni
        ///     
        /// </summary>
        /// <param name="idTaskToEnable">Id task di conservazione da attivare.</param>
        /// <param name="activationPin">Pin di attivazione (inviato all'utente tramite PEC).</param>
        /// <param name="mininumDaysOffset">Giorni minimi che intercorrono prima dell'esecuzione task.</param>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        bool EnablePreservationTaskByActivationPin(Guid idTaskToEnable, Guid activationPin, short mininumDaysOffset);


        [OperationContract, FaultContract(typeof(BiblosDsException))]
        IDictionary<Guid, BiblosDsException> UpdateDocumentMetadata(Guid idArchive, IDictionary<Guid, BindingList<DocumentAttributeValue>> documentAttributes);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        Preservation GetPreservationFromTask(Guid idTask);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void RemovePendigPreservation(Guid idArchive);
    }
}
