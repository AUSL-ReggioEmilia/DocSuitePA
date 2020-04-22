using BiblosDS.Library.Common.DB;
using BiblosDS.Library.Common.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Services
{
  public partial class DocumentService
  {
    #region DocumentUnit

    /// <summary>
    /// Ricerca DocumentUnit
    /// </summary>
    /// <param name="IdDocumentUnit">Chiave DocumentUnit</param>
    /// <returns>DocumentUnit</returns>
    public static DocumentUnit UdsGetDocumentUnit(Guid idDocumentUnit)
    {
      return DbProvider.UdsGetDocumentUnit(idDocumentUnit);
    }


    /// <summary>
    /// Ricerca DocumentUnit - Ritorno ReadOnly e Preservate
    /// </summary>
    /// <param name="IdDocumentUnit">Chiave DocumentUnit</param>
    /// <returns>DocumentUnit</returns>
    public static DocumentUnitExt UdsGetDocumentUnitExt(Guid idDocumentUnit)
    {
      bool isReadOnly;
      bool isPreserved;

      var unit = DbProvider.UdsGetDocumentUnit(idDocumentUnit, out isReadOnly, out isPreserved);

      return new DocumentUnitExt
      {
        Unit = unit,
        IsReadOnly = isReadOnly,
        IsPreserved = isPreserved
      };
    }


    /// <summary>
    /// Ricerca documentUnit per fascicolo
    /// </summary>
    /// <param name="uriFascicle">Fascicolo</param>
    /// <returns>Elenco delle document unit per fascicolo</returns>
    public static BindingList<DocumentUnit> UdsGetDocumentUnitsByFascicle(string uriFascicle)
    {
      return DbProvider.UdsGetDocumentUnitsByFascicle(uriFascicle);
    }

        /// <summary>
        /// Ricerca DocumentUnit
        /// </summary>
        /// <param name="IdDocumentUnit">Chiave DocumentUnit</param>
        /// <returns>DocumentUnit</returns>
        public static DocumentUnit UdsGetDocumentUnitByIdentifier(string identifierDocumentUnit)
        {
            return DbProvider.UdsGetDocumentUnitByIdentifier(identifierDocumentUnit);
        }

        /// <summary>
        /// Ricerca catena Guid dei documenti legati alla document unit
        /// </summary>
        /// <param name="IdDocumentUnit">Chiave DocumentUnit</param>
        /// <returns>Catena ID documenti</returns>
        public static BindingList<DocumentUnitChain> UdsGetUnitChain(Guid idDocumentUnit)
    {
      return DbProvider.UdsGetUnitChain(idDocumentUnit);
    }


    /// <summary>
    /// Ricerca i documenti per DocumentUnit
    /// </summary>
    /// <param name="IdDocumentUnit">Chiave DocumentUnit</param>
    /// <returns>Ritorna lista dei documenti</returns>
    public static BindingList<Document> UdsGetUnitDocuments(Guid idDocumentUnit)
    {
      return DbProvider.UdsGetUnitDocuments(idDocumentUnit);
    }


    /// <summary>
    /// Ricerca i documenti per fascicolo
    /// </summary>
    /// <param name="uriFascicle">Fascicolo</param>
    /// <returns>Ritorna lista dei documenti</returns>
    public static BindingList<Document> UdsGetUnitDocumentsByFascicle(string uriFascicle)
    {
      return DbProvider.UdsGetUnitDocumentsByFascicle(uriFascicle);
    }


    /// <summary>
    /// Apre a modifiche una DocumentUnit e le relative Aggregate a meno che una queste non sia già stata Preservata
    /// </summary>
    /// <param name="IdDocumentUnit">Chiave DocumentUnit</param>
    /// <returns>DocumentUnit</returns>
    public static DocumentUnit UdsOpenDocumentUnit(Guid idDocumentUnit)
    {
      return DbProvider.UdsOpenDocumentUnit(idDocumentUnit);
    }


    /// <summary>
    /// Aggiunge una nuova DocumentUnit
    /// </summary>
    /// <param name="unit">DocumentUnit da aggiungere</param>
    /// <returns>DocumentUnit inserita</returns>
    public static DocumentUnit UdsAddDocumentUnit(DocumentUnit unit)
    {
      return DbProvider.UdsAddDocumentUnit(unit);
    }


    /// <summary>
    /// Aggiorna una DocumentUnit esistente
    /// </summary>
    /// <param name="unit">DocumentUnit da aggiornare</param>
    /// <returns>Ritorna document unit aggiornata</returns>
    public static DocumentUnit UdsUpdateDocumentUnit(DocumentUnit unit)
    {
      return DbProvider.UdsUpdateDocumentUnit(unit);
    }


    /// <summary>
    /// Collega Documenti ad una DocumentUnit esistente
    /// </summary>
    /// <param name="idDocumentUnit"></param>
    /// <param name="documents"></param>
    /// <param name="checkUnitExist">Verifica l'esistenza della document unit prima di collegare i documenti (default true)</param>
    /// <returns>N.di documenti collegati</returns>
    public static int UdsDocumentUnitAddDocuments(Guid idDocumentUnit, DocumentUnitChain[] documents)
    {
      return DbProvider.UdsDocumentUnitAddDocuments(idDocumentUnit, documents);
    }


    /// <summary>
    /// Aggiunge una nuova DocumentUnit e collega i documenti collegati passati
    /// </summary>
    /// <param name="unit">DocumentUnit da inserire</param>
    /// <param name="documents">Elenco dei riferimenti ai documenti da collegare</param>
    /// <returns>DocumentUnit creata</returns>
    public static DocumentUnit UdsAddDocumentUnitWithDocuments(DocumentUnit unit, DocumentUnitChain[] documents)
    {
      EntityProvider provider = DbProvider;

      using (DbTransaction tran = provider.BeginNoSave())
      {
        try
        {
          var unitRes = provider.UdsAddDocumentUnit(unit);
          if (unitRes != null)
            provider.UdsDocumentUnitAddDocuments(unitRes.IdDocumentUnit, documents, false);

          provider.SaveChanges();
          tran.Commit();
          return unitRes;
        }
        catch
        {
          try
          {
            tran.Rollback();
          }
          catch
          {
          }
          throw;
        }
      }
    }


    /// <summary>
    /// Elimina una document unit. Rimuove anche i riferimenti dei documenti alla DocumentUnit dalla tabella DocumentUnitChain
    /// </summary>
    /// <param name="idDocumentUnit"></param>
    /// <returns>Ritorna n. di record complessivamente eliminati</returns>
    public static int UdsDeleteDocumentUnit(Guid idDocumentUnit, bool detachDocuments = true)
    {
      EntityProvider provider = DbProvider;

      using (DbTransaction tran = provider.BeginNoSave())
      {
        try
        {
          //detach dei documenti
          if (detachDocuments)
          {
            var docChain = DbProvider.UdsGetUnitChain(idDocumentUnit);
            foreach (var item in docChain)
              DetachDocument(item.Document);
          }

          provider.UdsDeleteDocumentUnitChain(idDocumentUnit);
          provider.UdsDeleteDocumentUnit(idDocumentUnit);
          int count = provider.SaveChanges();
          tran.Commit();
          return count;
        }
        catch
        {
          try
          {
            tran.Rollback();
          }
          catch
          {
          }
          throw;
        }
      }
    }


    /// <summary>
    /// Elimina tutti i riferimenti ai documenti della document unit passata
    /// </summary>
    /// <param name="idDocumentUnit"></param>
    /// <returns>Ritorna il n. di riferimenti eliminati</returns>
    public static int UdsDeleteDocumentUnitChain(Guid idDocumentUnit, bool detachDocuments = true)
    {
      //detach dei documenti
      if (detachDocuments)
      {
        var docChain = DbProvider.UdsGetUnitChain(idDocumentUnit);
        foreach (var item in docChain)
          DetachDocument(item.Document);
      }

      return DbProvider.UdsDeleteDocumentUnitChain(idDocumentUnit);
    }

    #endregion

    #region DocumentUnitAggregate


    /// <summary>
    /// Ricerca Aggregato (Fasciolo)
    /// </summary>
    /// <param name="IdAggregate"></param>
    /// <returns>DocumentUnitAggregate (Fascicolo)</returns>
    public static DocumentUnitAggregate UdsGetDocumentUnitAggregate(Guid idAggregate)
    {
      return DbProvider.UdsGetDocumentUnitAggregate(idAggregate);
    }


    /// <summary>
    /// Ricerca DocumentUnitAggregate - Ritorna anche se ReadOnly e Preserved
    /// </summary>
    /// <param name="idDocumentUnit"></param>
    /// <param name="isReadOnly"></param>
    /// <param name="isPreserved"></param>
    /// <returns>Ritorna DocumentUnitAggregate individuato</returns>
    public static DocumentUnitAggregateExt UdsGetDocumentUnitAggregateExt(Guid idAggregate)
    {
      bool isReadOnly;
      bool isPreserved;

      var aggregate = DbProvider.UdsGetDocumentUnitAggregate(idAggregate, out isReadOnly, out isPreserved);

      return new DocumentUnitAggregateExt
      {
        Aggregate = aggregate,
        IsReadOnly = isReadOnly,
        IsPreserved = isPreserved
      };
    }


    /// <summary>
    /// Ricerca lista di DocumentUnit per Aggregato
    /// </summary>
    /// <param name="idAggregate"></param>
    /// <returns>Ritorna lista di document unit per aggregato</returns>
    public static BindingList<DocumentUnit> UdsGetAggregateChain(Guid idAggregate)
    {
      return DbProvider.UdsGetAggregateChain(idAggregate);
    }


    /// <summary>
    /// Apre una DocumentUnitAggregate a meno che una queste non sia già stata Preservata e tutte le relative DocumentUnit 
    /// </summary>
    /// <param name="idDocumentUnit"></param>
    public static DocumentUnitAggregate UdsOpenDocumentUnitAggregate(Guid idAggregate)
    {
      return DbProvider.UdsOpenDocumentUnitAggregate(idAggregate);
    }


    /// <summary>
    /// Aggiunge una nuova DocumentUnitAggregate
    /// </summary>
    /// <param name="aggregate"></param>
    /// <returns>Ritorna l'aggregato aggiunto</returns>
    public static DocumentUnitAggregate UdsAddDocumentUnitAggregate(DocumentUnitAggregate aggregate)
    {
      return DbProvider.UdsAddDocumentUnitAggregate(aggregate);
    }


    /// <summary>
    /// Aggiorna una DocumentUnitAggregate esistente
    /// </summary>
    /// <param name="aggregate">Aggregato da aggiornare</param>
    /// <returns>Ritorna aggregato aggiornato</returns>
    public static DocumentUnitAggregate UdsUpdateDocumentUnitAggregate(DocumentUnitAggregate aggregate)
    {
      return DbProvider.UdsUpdateDocumentUnitAggregate(aggregate);
    }


    /// <summary>
    /// Aggiunge DocumentUnit all'aggregato (fascicolo) selezionato
    /// </summary>
    /// <param name="idAggregate">Chiave aggregato</param>
    /// <param name="units">elenco delle document unit (ID's)</param>
    /// <param name="checkAggregateExist">Verifica esistenza aggregato</param>
    /// <returns>N. di document unit aggregate</returns>
    public static int UdsDocumentAggregateAddUnits(Guid idAggregate, Guid[] unitsId)
    {
      return DbProvider.UdsDocumentAggregateAddUnits(idAggregate, unitsId);
    }


    /// <summary>
    /// Elimina aggregato (fascicolo) - Elimina anche tutti i riferimenti alle DocumentiUnit aggregate.
    /// </summary>
    /// <param name="idAggregate">Chiave aggregato</param>
    /// <returns>N. di rercord complessivamente eliminati</returns>
    public static int UdsDeleteDocumentUnitAggregate(Guid idAggregate)
    {
      EntityProvider provider = DbProvider;

      using (DbTransaction tran = provider.BeginNoSave())
      {
        try
        {
          provider.UdsDeleteDocumentUnitAggregateChain(idAggregate);
          provider.UdsDeleteDocumentUnitAggregate(idAggregate);

          int count = provider.SaveChanges();
          tran.Commit();
          return count;
        }
        catch
        {
          try
          {
            tran.Rollback();
          }
          catch
          {
          }
          throw;
        }
      }
    }


    /// <summary>
    /// Elimina tutti i riferimenti di DocumentUnit all'aggregato passato 
    /// </summary>
    /// <param name="idAggregate"></param>
    /// <returns>N. di riferimenti eliminati</returns>
    public static int UdsDeleteDocumentUnitAggregateChain(Guid idAggregate)
    {
      return DbProvider.UdsDeleteDocumentUnitAggregateChain(idAggregate);
    }

    #endregion

  }
}
