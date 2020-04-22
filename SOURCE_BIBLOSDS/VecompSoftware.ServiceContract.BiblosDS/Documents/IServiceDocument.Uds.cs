using System;
using System.ServiceModel;
using BiblosDS.Library.Common.Objects;
using System.ComponentModel;
using BiblosDS.Library.Common.Exceptions;

namespace VecompSoftware.ServiceContract.BiblosDS.Documents
{
  public partial interface IDocuments : IBiblosDSServiceContract
  {
    #region DocumentUnit Methods

    /// <summary>
    /// Ricerca DocumentUnit
    /// </summary>
    /// <param name="IdDocumentUnit">Chiave DocumentUnit</param>
    /// <returns>DocumentUnit</returns>
    [OperationContract, FaultContract(typeof(BiblosDsException))]        
    DocumentUnit UdsGetDocumentUnit(Guid idDocumentUnit);

    /// <summary>
    /// Ricerca DocumentUnit - Ritorno ReadOnly e Preservate
    /// </summary>
    /// <param name="IdDocumentUnit">Chiave DocumentUnit</param>
    /// <returns>DocumentUnit</returns>
    [OperationContract, FaultContract(typeof(BiblosDsException))]
    DocumentUnitExt UdsGetDocumentUnitExt(Guid idDocumentUnit);


    /// <summary>
    /// Ricerca documentUnit per fascicolo
    /// </summary>
    /// <param name="uriFascicle">Fascicolo</param>
    /// <returns>Elenco delle document unit per fascicolo</returns>
    [OperationContract, FaultContract(typeof(BiblosDsException))]
    BindingList<DocumentUnit> UdsGetDocumentUnitsByFascicle(string uriFascicle);

    /// <summary>
    /// Ricerca catena Guid dei documenti legati alla document unit
    /// </summary>
    /// <param name="IdDocumentUnit">Chiave DocumentUnit</param>
    /// <returns>Catena ID documenti</returns>
    [OperationContract, FaultContract(typeof(BiblosDsException))]
    BindingList<DocumentUnitChain> UdsGetUnitChain(Guid idDocumentUnit);


    /// <summary>
    /// Ricerca i documenti per DocumentUnit
    /// </summary>
    /// <param name="IdDocumentUnit">Chiave DocumentUnit</param>
    /// <returns>Ritorna lista dei documenti</returns>
    [OperationContract, FaultContract(typeof(BiblosDsException))]        
    BindingList<Document> UdsGetUnitDocuments(Guid idDocumentUnit);


    /// <summary>
    /// Ricerca i documenti per fascicolo
    /// </summary>
    /// <param name="uriFascicle">Fascicolo</param>
    /// <returns>Ritorna lista dei documenti</returns>
    [OperationContract, FaultContract(typeof(BiblosDsException))]
    BindingList<Document> UdsGetUnitDocumentsByFascicle(string uriFascicle);


    /// <summary>
    /// Apre a modifiche una DocumentUnit e le relative Aggregate a meno che una queste non sia già stata Preservata
    /// </summary>
    /// <param name="IdDocumentUnit">Chiave DocumentUnit</param>
    /// <returns>DocumentUnit</returns>
    [OperationContract, FaultContract(typeof(BiblosDsException))]
    DocumentUnit UdsOpenDocumentUnit(Guid idDocumentUnit);


    /// <summary>
    /// Aggiunge una nuova DocumentUnit
    /// </summary>
    /// <param name="unit">DocumentUnit da aggiungere</param>
    /// <returns>DocumentUnit inserita</returns>
    [OperationContract, FaultContract(typeof(BiblosDsException))]
    DocumentUnit UdsAddDocumentUnit(DocumentUnit unit);


    /// <summary>
    /// Aggiorna una DocumentUnit esistente
    /// </summary>
    /// <param name="unit">DocumentUnit da aggiornare</param>
    /// <returns>Ritorna document unit aggiornata</returns>
    [OperationContract, FaultContract(typeof(BiblosDsException))]
    DocumentUnit UdsUpdateDocumentUnit(DocumentUnit unit);


    /// <summary>
    /// Collega Documenti ad una DocumentUnit esistente
    /// </summary>
    /// <param name="idDocumentUnit"></param>
    /// <param name="documents"></param>
    /// <param name="checkUnitExist">Verifica l'esistenza della document unit prima di collegare i documenti (default true)</param>
    /// <returns>N.di documenti collegati</returns>
    [OperationContract, FaultContract(typeof(BiblosDsException))]
    int UdsDocumentUnitAddDocuments(Guid idDocumentUnit, DocumentUnitChain[] documents);


    /// <summary>
    /// Aggiunge una nuova DocumentUnit e collega i documenti collegati passati
    /// </summary>
    /// <param name="unit">DocumentUnit da inserire</param>
    /// <param name="documents">Elenco dei riferimenti ai documenti da collegare</param>
    /// <returns>DocumentUnit creata</returns>
    [OperationContract, FaultContract(typeof(BiblosDsException))]
    DocumentUnit UdsAddDocumentUnitWithDocuments(DocumentUnit unit, DocumentUnitChain[] documents);


    /// <summary>
    /// Elimina una document unit. Rimuove anche i riferimenti dei documenti alla DocumentUnit dalla tabella DocumentUnitChain
    /// </summary>
    /// <param name="idDocumentUnit"></param>
    /// <returns>Ritorna n. di record complessivamente eliminati</returns>
    [OperationContract, FaultContract(typeof(BiblosDsException))]
    int UdsDeleteDocumentUnitChain(Guid idDocumentUnit);


    /// <summary>
    /// Elimina tutti i riferimenti ai documenti della document unit passata
    /// </summary>
    /// <param name="idDocumentUnit"></param>
    /// <returns>Ritorna il n. di riferimenti eliminati</returns>
    [OperationContract, FaultContract(typeof(BiblosDsException))]
    int UdsDeleteDocumentUnit(Guid idDocumentUnit);

    #endregion

    #region DocumentUnitAggregate Methods

    /// <summary>
    /// Ricerca Aggregato (Fasciolo)
    /// </summary>
    /// <param name="IdAggregate"></param>
    /// <returns>DocumentUnitAggregate (Fascicolo)</returns>
    [OperationContract, FaultContract(typeof(BiblosDsException))]
    DocumentUnitAggregate UdsGetDocumentUnitAggregate(Guid idAggregate);

    /// <summary>
    /// Ricerca DocumentUnitAggregate - Ritorna anche se ReadOnly e Preserved
    /// </summary>
    /// <param name="idDocumentUnit"></param>
    /// <returns></returns>
    [OperationContract, FaultContract(typeof(BiblosDsException))]
    DocumentUnitAggregateExt UdsGetDocumentUnitAggregateExt(Guid idAggregate);


    /// <summary>
    /// Ricerca lista di DocumentUnit per Aggregato
    /// </summary>
    /// <param name="idAggregate"></param>
    /// <returns>Ritorna lista di document unit per aggregato</returns>
    [OperationContract, FaultContract(typeof(BiblosDsException))]
    BindingList<DocumentUnit> UdsGetAggregateChain(Guid idAggregate);


    /// <summary>
    /// Apre una DocumentUnitAggregate a meno che una queste non sia già stata Preservata e tutte le relative DocumentUnit 
    /// </summary>
    /// <param name="idDocumentUnit"></param>
    [OperationContract, FaultContract(typeof(BiblosDsException))]
    DocumentUnitAggregate UdsOpenDocumentUnitAggregate(Guid idAggregate);



    /// <summary>
    /// Aggiunge una nuova DocumentUnitAggregate
    /// </summary>
    /// <param name="aggregate"></param>
    /// <returns>Ritorna l'aggregato aggiunto</returns>
    [OperationContract, FaultContract(typeof(BiblosDsException))]
    DocumentUnitAggregate UdsAddDocumentUnitAggregate(DocumentUnitAggregate aggregate);


    /// <summary>
    /// Aggiorna una DocumentUnitAggregate esistente
    /// </summary>
    /// <param name="aggregate">Aggregato da aggiornare</param>
    /// <returns>Ritorna aggregato aggiornato</returns>
    [OperationContract, FaultContract(typeof(BiblosDsException))]
    DocumentUnitAggregate UdsUpdateDocumentUnitAggregate(DocumentUnitAggregate aggregate);


    /// <summary>
    /// Aggiunge DocumentUnit all'aggregato (fascicolo) selezionato
    /// </summary>
    /// <param name="idAggregate">Chiave aggregato</param>
    /// <param name="units">elenco delle document unit (ID's)</param>
    /// <param name="checkAggregateExist">Verifica esistenza aggregato</param>
    /// <returns>N. di document unit aggregate</returns>
    [OperationContract, FaultContract(typeof(BiblosDsException))]
    int UdsDocumentAggregateAddUnits(Guid idAggregate, Guid[] unitsId);


    /// <summary>
    /// Elimina aggregato (fascicolo) - Elimina anche tutti i riferimenti alle DocumentiUnit aggregate.
    /// </summary>
    /// <param name="idAggregate">Chiave aggregato</param>
    /// <returns>N. di rercord complessivamente eliminati</returns>
    [OperationContract, FaultContract(typeof(BiblosDsException))]
    int UdsDeleteDocumentUnitAggregate(Guid idAggregate);


    /// <summary>
    /// Elimina tutti i riferimenti di DocumentUnit all'aggregato passato 
    /// </summary>
    /// <param name="idAggregate"></param>
    /// <returns>N. di riferimenti eliminati</returns>
    [OperationContract, FaultContract(typeof(BiblosDsException))]
    int UdsDeleteDocumentUnitAggregateChain(Guid idAggregate);
 
    #endregion
  }
}
