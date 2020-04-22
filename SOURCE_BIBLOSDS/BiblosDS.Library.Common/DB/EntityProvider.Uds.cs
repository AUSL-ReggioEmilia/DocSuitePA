
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Objects.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.DB
{
  public partial class EntityProvider
  {

    /// <summary>
    /// Ricerca DocumentUnit
    /// </summary>
    /// <param name="IdDocumentUnit">Chiave DocumentUnit</param>
    /// <returns>DocumentUnit</returns>
    public DocumentUnit UdsGetDocumentUnit(Guid idDocumentUnit)
    {
      try
      {
        return UdsGetDocumentUnitModel(idDocumentUnit).Convert();
      }
      finally
      {
        Dispose();
      }
    }


    /// <summary>
    /// Ricerca DocumentUnit - Ritorna anche se ReadOnly
    /// </summary>
    /// <param name="idDocumentUnit"></param>
    /// <param name="isReadOnly"></param>
    /// <returns></returns>
    public DocumentUnit UdsGetDocumentUnit(Guid idDocumentUnit, out bool isReadOnly, out bool isPreserved)
    {
      try
      {
        return UdsGetDocumentUnitModel(idDocumentUnit, out isReadOnly, out isPreserved).Convert();
      }
      finally
      {
        Dispose();
      }
    }

        /// <summary>
        /// Ricerca DocumentUnit
        /// </summary>
        /// <param name="IdDocumentUnit">Chiave DocumentUnit</param>
        /// <returns>DocumentUnit</returns>
        public DocumentUnit UdsGetDocumentUnitByIdentifier(string identifierDocumentUnit)
        {
            try
            {
                return UdsGetDocumentUnitByIdentifierModel(identifierDocumentUnit).Convert();
            }
            finally
            {
                Dispose();
            }
        }

        /// <summary>
        /// Apre a modifiche una DocumentUnit e le relative Aggregate a meno che una queste non sia già stata Preservata
        /// </summary>
        /// <param name="idDocumentUnit"></param>
        public DocumentUnit UdsOpenDocumentUnit(Guid idDocumentUnit)
    {
      bool isReadOnly;
      bool isPreserved;

      var docUnit = UdsGetDocumentUnitModel(idDocumentUnit, out isReadOnly, out isPreserved);

      //già aperta
      if (!isReadOnly)
        return docUnit.Convert();

      //preservata, non si può fare
      if (isPreserved)
        throw DocumentUnitAggregateException.Preserved();

      //apre la unit
      docUnit.CloseDate = null;
  
      //apre gli aggregaati
      var aggregates = UdsGetUnitAggregates(docUnit.IdDocumentUnit);
      foreach (var aggregate in aggregates)
      {
        aggregate.CloseDate = null;
      }

      if (requireSave)
        this.db.SaveChanges();

      return docUnit.Convert();
    }



    /// <summary>
    /// Ricerca documentUnit per fascicolo
    /// </summary>
    /// <param name="uriFascicle">Fascicolo</param>
    /// <returns>Elenco delle document unit per fascicolo</returns>
    public BindingList<DocumentUnit> UdsGetDocumentUnitsByFascicle(string uriFascicle)
    {
      try
      {
        var query = this.db.DocumentUnit.Where(x => x.UriFascicle == uriFascicle).OrderByDescending(x => x.InsertDate);
        var retval = new BindingList<DocumentUnit>();

        foreach (var item in query)
        {
          retval.Add(item.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
        }

        return retval;
      }
      finally
      {
        Dispose();
      }
    }


    /// <summary>
    /// Ricerca catena Guid dei documenti legati alla document unit
    /// </summary>
    /// <param name="IdDocumentUnit">Chiave DocumentUnit</param>
    /// <returns>Catena ID documenti</returns>
    public BindingList<DocumentUnitChain> UdsGetUnitChain(Guid idDocumentUnit)
    {
      try
      {
        BindingList<DocumentUnitChain> list = new BindingList<DocumentUnitChain>();
        var query = this.db.DocumentUnitChain
          .Include("Document")
          .Include("DocumentUnit")
          .Where(x => x.IdDocumentUnit == idDocumentUnit);

        foreach (var item in query)
        {
          list.Add(item.Convert());
        }
        return list;
      }
      finally
      {
        Dispose();
      }
    }


    /// <summary>
    /// Ricerca i documenti per DocumentUnit
    /// </summary>
    /// <param name="IdDocumentUnit">Chiave DocumentUnit</param>
    /// <returns>Ritorna lista dei documenti</returns>
    public BindingList<Document> UdsGetUnitDocuments(Guid idDocumentUnit)
    {
      try
      {
        var docs = this.db.DocumentUnitChain
                  .Where(x => x.IdDocumentUnit == idDocumentUnit)
                  .Select(x => x.IdParentBiblos).ToArray();

        var response = GetDocumentsPaged(docs);
        return response.Documents;
      }
      finally
      {
        Dispose();
      }
    }


    /// <summary>
    /// Ricerca lista di DocumentUnit per Aggregato
    /// </summary>
    /// <param name="idAggregate"></param>
    /// <returns>Ritorna lista di document unit per aggregato</returns>
    public BindingList<DocumentUnitAggregate> UdsGetUnitAggregates(Guid idDocumentUnit)
    {
      try
      {
        var unit = UdsGetDocumentUnitModel(idDocumentUnit);

        BindingList<DocumentUnitAggregate> list = new BindingList<DocumentUnitAggregate>();
        foreach (var item in unit.DocumentUnitAggregate)
        {
          list.Add(item.Convert());
        }
        return list;
      }
      finally
      {
        Dispose();
      }
    }



    /// <summary>
    /// Ricerca i documenti per fascicolo
    /// </summary>
    /// <param name="uriFascicle">Fascicolo</param>
    /// <returns>Ritorna lista dei documenti</returns>
    public BindingList<Document> UdsGetUnitDocumentsByFascicle(string uriFascicle)
    {
      try
      {
        var docs = this.db.DocumentUnitChain
                  .Where(x => x.DocumentUnit.UriFascicle == uriFascicle)
                  .Select(x => x.IdParentBiblos).ToArray();

        var response = GetDocumentsPaged(docs);
        return response.Documents;
      }
      finally
      {
        Dispose();
      }
    }


    /// <summary>
    /// Aggiunge una nuova DocumentUnit
    /// </summary>
    /// <param name="unit">DocumentUnit da aggiungere</param>
    /// <returns>DocumentUnit inserita</returns>
    public DocumentUnit UdsAddDocumentUnit(DocumentUnit unit)
    {
      try
      {
        if (unit == null)
          throw DocumentUnitException.NotFound();

        var entity = new Model.DocumentUnit
        {
          IdDocumentUnit = Guid.NewGuid(),
          InsertDate = DateTime.Now,
          CloseDate = null,
          Identifier = unit.Identifier ?? "",
          Subject = unit.Subject ?? "",
          Classification = unit.Classification ?? "",
          UriFascicle = unit.UriFascicle ?? "",
          XmlDoc = unit.XmlDoc ?? ""
        };

        this.db.DocumentUnit.AddObject(entity);
        if (requireSave)
          this.db.SaveChanges();

        var ret = entity.Convert(0, 3);
        return ret;
      }
      finally
      {
        Dispose();
      }
    }


    /// <summary>
    /// Aggiorna una DocumentUnit esistente
    /// </summary>
    /// <param name="unit">DocumentUnit da aggiornare</param>
    /// <returns>Ritorna document unit aggiornata</returns>
    public DocumentUnit UdsUpdateDocumentUnit(DocumentUnit item)
    {
      try
      {
        var docUnit = UdsTryEditDocumentUnit(item.IdDocumentUnit);

        docUnit.CloseDate = item.CloseDate;
        docUnit.Subject = item.Subject;
        docUnit.Classification = item.Classification;
        docUnit.UriFascicle = item.UriFascicle;
        docUnit.XmlDoc = item.XmlDoc;

        if (requireSave)
          this.db.SaveChanges();

        var ret = docUnit.Convert(0, 3);
        return ret;
      }
      finally
      {
        Dispose();
      }
    }


    /// <summary>
    /// Collega Documenti ad una DocumentUnit esistente
    /// </summary>
    /// <param name="idDocumentUnit"></param>
    /// <param name="documents"></param>
    /// <param name="checkUnitExist">Verifica l'esistenza della document unit prima di collegare i documenti (default true)</param>
    /// <returns>N.di documenti collegati</returns>
    public int UdsDocumentUnitAddDocuments(Guid idDocumentUnit, DocumentUnitChain[] documents, bool checkUnitExist = true)
    {
      try
      {
        //trova document unit
        if (checkUnitExist)
          UdsTryEditDocumentUnit(idDocumentUnit);

        //determina se i documenti passati sono tutti capo catena validi
        var docs = documents.Select(x => x.IdParentBiblos).ToArray();

        var query = this.db.Document.Where(
         x => !x.IdParentBiblos.HasValue &&
         x.IsVisible == 1 &&
         docs.Contains(x.IdDocument)).ToArray();

        var notFounds = query.Where(x => !docs.Contains(x.IdDocument)).Select(x => x.IdDocument.ToString()).ToArray();
        if (notFounds.Count() > 0)
          throw DocumentUnitException.InvalidDocuments(notFounds);

        foreach (var doc in documents)
        {
          var entity = new Model.DocumentUnitChain
          {
            IdDocumentUnit = idDocumentUnit,
            IdParentBiblos = doc.IdParentBiblos,
            Name = doc.Name
          };
          this.db.DocumentUnitChain.AddObject(entity);
        }

        int count = 0;
        if (requireSave)
          count = this.db.SaveChanges();

        return count;
      }
      finally
      {
        Dispose();
      }
    }


    /// <summary>
    /// Elimina una document unit. Prima di poter rimuovere la document unit deveono essere rimosse le relative foreign key nella DocumentUnitChain
    /// </summary>
    /// <param name="idDocumentUnit"></param>
    /// <returns>Ritorna n. DocumentUnit eliminate (1 se trovato)</returns>
    public int UdsDeleteDocumentUnit(Guid idDocumentUnit)
    {
      try
      {
        var docUnit = UdsTryEditDocumentUnit(idDocumentUnit);
        this.db.DocumentUnit.DeleteObject(docUnit);

        int count = 0;
        if (requireSave)
          count = this.db.SaveChanges();

        return count;
      }
      finally
      {
        Dispose();
      }
    }



    /// <summary>
    /// Elimina tutti i riferimenti ai documenti della document unit passata
    /// </summary>
    /// <param name="idDocumentUnit"></param>
    /// <returns>Ritorna il n. di riferimenti eliminati</returns>
    public int UdsDeleteDocumentUnitChain(Guid idDocumentUnit)
    {
      try
      {
        UdsTryEditDocumentUnit(idDocumentUnit);

        var query = this.db.DocumentUnitChain.Where(x => x.IdDocumentUnit == idDocumentUnit);
        foreach (var item in query)
        {
          this.db.DocumentUnitChain.DeleteObject(item);
        }

        int count = 0;
        if (requireSave)
          count = this.db.SaveChanges();

        return count;
      }
      finally
      {
        Dispose();
      }
    }



    /// <summary>
    /// Ricerca Aggregato (Fasciolo)
    /// </summary>
    /// <param name="IdAggregate"></param>
    /// <returns>DocumentUnitAggregate (Fascicolo)</returns>
    public DocumentUnitAggregate UdsGetDocumentUnitAggregate(Guid idAggregate)
    {
      try
      {
        return UdsGetDocumentUnitAggregateModel(idAggregate).Convert();
      }
      finally
      {
        Dispose();
      }
    }


    /// <summary>
    /// Ricerca DocumentUnitAggregate - Ritorna anche se ReadOnly e Preserved
    /// </summary>
    /// <param name="idDocumentUnit"></param>
    /// <param name="isReadOnly"></param>
    /// <param name="isPreserved"></param>
    /// <returns></returns>
    public DocumentUnitAggregate UdsGetDocumentUnitAggregate(Guid idAggregate, out bool isReadOnly, out bool isPreserved)
    {
      try
      {
        return UdsGetDocumentUnitAggregateModel(idAggregate, out isReadOnly, out isPreserved).Convert();
      }
      finally
      {
        Dispose();
      }
    }


    /// <summary>
    /// Apre una DocumentUnitAggregate a meno che una queste non sia già stata Preservata e tutte le relative DocumentUnit 
    /// </summary>
    /// <param name="idDocumentUnit"></param>
    public DocumentUnitAggregate UdsOpenDocumentUnitAggregate(Guid idAggregate)
    {
      bool isReadOnly;
      bool isPreserved;

      var aggregate = UdsGetDocumentUnitAggregateModel(idAggregate, out isReadOnly, out isPreserved);

      //già aperta
      if (!isReadOnly)
        return aggregate.Convert();

      //preservata, non si può fare
      if (isPreserved)
        throw DocumentUnitAggregateException.Preserved();

      //apre la unit
      aggregate.CloseDate = null;

      //apre le units
      var units = UdsGetAggregateChain(aggregate.IdAggregate);
      foreach (var unit in units)
      {
        unit.CloseDate = null;
      }

      if (requireSave)
        this.db.SaveChanges();

      return aggregate.Convert();
    }




    /// <summary>
    /// Ricerca lista di DocumentUnit per Aggregato
    /// </summary>
    /// <param name="idAggregate"></param>
    /// <returns>Ritorna lista di document unit per aggregato</returns>
    public BindingList<DocumentUnit> UdsGetAggregateChain(Guid idAggregate)
    {
      try
      {
        var aggregate = UdsGetDocumentUnitAggregateModel(idAggregate);

        BindingList<DocumentUnit> list = new BindingList<DocumentUnit>();
        foreach (var item in aggregate.DocumentUnit)
        {
          list.Add(item.Convert());
        }
        return list;
      }
      finally
      {
        Dispose();
      }
    }


    /// <summary>
    /// Aggiunge una nuova DocumentUnitAggregate
    /// </summary>
    /// <param name="aggregate"></param>
    /// <returns>Ritorna l'aggregato aggiunto</returns>
    public DocumentUnitAggregate UdsAddDocumentUnitAggregate(DocumentUnitAggregate aggregate)
    {
      try
      {
        if (aggregate == null)
          throw DocumentUnitAggregateException.NotFound();

        var entity = new Model.DocumentUnitAggregate
        {
          IdAggregate = Guid.NewGuid(),
          AggregationType = aggregate.AggregationType,
          CloseDate = aggregate.CloseDate,
          PreservationDate = aggregate.PreservationDate,
          XmlFascicle = aggregate.XmlFascicle
        };

        this.db.DocumentUnitAggregate.AddObject(entity);
        if (requireSave)
          this.db.SaveChanges();

        var ret = entity.Convert(0, 3);
        return ret;
      }
      finally
      {
        Dispose();
      }
    }


    /// <summary>
    /// Aggiorna una DocumentUnitAggregate esistente
    /// </summary>
    /// <param name="idAggregate">Chiave Aggregato da aggiornare</param>
    /// <param name="closeDate"></param>
    /// <param name="xmlFascicle"></param>
    /// <param name="aggregationType"></param>
    /// <param name="preservationDate"></param>
    /// <returns>Ritorna aggregato da aggiornare</returns>
    public DocumentUnitAggregate UdsUpdateDocumentUnitAggregate(DocumentUnitAggregate item)
    {
      try
      {
        var aggregate = UdsTryEditDocumentUnitAggregate(item.IdAggregate);

        aggregate.CloseDate = item.CloseDate;
        aggregate.XmlFascicle = item.XmlFascicle;
        aggregate.AggregationType = item.AggregationType;
        aggregate.PreservationDate = item.PreservationDate;

        if (requireSave)
          this.db.SaveChanges();

        var ret = aggregate.Convert(0, 3);
        return ret;
      }
      finally
      {
        Dispose();
      }
    }


    /// <summary>
    /// Aggiunge DocumentUnit all'aggregato (fascicolo) selezionato
    /// </summary>
    /// <param name="idAggregate">Chiave aggregato</param>
    /// <param name="units">elenco delle document unit (ID's)</param>
    /// <returns>N. di document unit aggregate</returns>
    public int UdsDocumentAggregateAddUnits(Guid idAggregate, Guid[] units)
    {
      try
      {
        var aggregate = UdsTryEditDocumentUnitAggregate(idAggregate);

        var unitsEntities = this.db.DocumentUnit.Where(x => units.Contains(x.IdDocumentUnit)).ToArray();
        foreach (var entity in unitsEntities)
        {
          aggregate.DocumentUnit.Add(entity);
        }

        int count = 0;
        if (requireSave)
          count = this.db.SaveChanges();

        return count;
      }
      finally
      {
        Dispose();
      }
    }


    /// <summary>
    /// Elimina aggregato (fascicolo) - Devono essere prima eliminati dalla DocumentUnitAggregateChain i riferimenti  al fascicolo
    /// </summary>
    /// <param name="idAggregate">Chiave aggregato</param>
    /// <returns>N. di aggregati eliminati (1 se trovato)</returns>
    public int UdsDeleteDocumentUnitAggregate(Guid idAggregate)
    {
      try
      {
        var aggregate = UdsTryEditDocumentUnitAggregate(idAggregate);
        this.db.DocumentUnitAggregate.DeleteObject(aggregate);

        int count = 0;
        if (requireSave)
          count = this.db.SaveChanges();

        return count;
      }
      finally
      {
        Dispose();
      }
    }


    /// <summary>
    /// Elimina tutti i riferimenti di DocumentUnit all'aggregato passato 
    /// </summary>
    /// <param name="idAggregate"></param>
    /// <returns>N. di riferimenti eliminati</returns>
    public int UdsDeleteDocumentUnitAggregateChain(Guid idAggregate)
    {
      try
      {
        var aggregate = UdsTryEditDocumentUnitAggregate(idAggregate);

        var units = aggregate.DocumentUnit.Select(x => x.IdDocumentUnit).ToArray();
        var unitsEntities = this.db.DocumentUnit.Where(x => units.Contains(x.IdDocumentUnit)).ToArray();
        foreach (var entity in unitsEntities)
        {
          aggregate.DocumentUnit.Remove(entity);
        }

        int count = 0;
        if (requireSave)
          count = this.db.SaveChanges();

        return count;
      }
      finally
      {
        Dispose();
      }
    }


    #region Helpers

    private Model.DocumentUnit UdsGetDocumentUnitModel(Guid idDocumentUnit, out bool isReadOnly, out bool isPreserved)
    {
      isReadOnly = false;
      isPreserved = false;

      var unit = this.db.DocumentUnit
        .Include("DocumentUnitAggregate")
        .SingleOrDefault(p => p.IdDocumentUnit == idDocumentUnit);

      if (unit == null)
        throw DocumentUnitException.NotFound();

      if (unit.CloseDate != null)
        isReadOnly = true;

      foreach (var aggregate in unit.DocumentUnitAggregate)
      {
        isReadOnly = isReadOnly || (aggregate.CloseDate != null || aggregate.PreservationDate != null);
        isPreserved = isPreserved || aggregate.PreservationDate != null;
      }

      return unit;
    }


    private Model.DocumentUnit UdsGetDocumentUnitModel(Guid idDocumentUnit)
    {
      bool isReadOnly;
      bool isPreserved;

      return UdsGetDocumentUnitModel(idDocumentUnit, out isReadOnly, out isPreserved);
    }

        private Model.DocumentUnit UdsGetDocumentUnitByIdentifierModel(string identifierDocumentUnit)
        {
            var unit = this.db.DocumentUnit              
              .SingleOrDefault(p => p.Identifier == identifierDocumentUnit);

            if (unit == null)
                throw DocumentUnitException.NotFound();

            return unit;
        }

        private Model.DocumentUnit UdsTryEditDocumentUnit(Guid idDocumentUnit)
    {
      bool isReadOnly;
      bool isPreserved;

      var docUnit = UdsGetDocumentUnitModel(idDocumentUnit, out isReadOnly, out isPreserved);
      if (isReadOnly)
        throw DocumentUnitException.ReadOnly();

      return docUnit;
    }


    private Model.DocumentUnitAggregate UdsGetDocumentUnitAggregateModel(Guid idAggregate, out bool isReadOnly, out bool isPreserved)
    {
      isReadOnly = false;
      isPreserved = false;

      var aggregate = this.db.DocumentUnitAggregate
        .Include("DocumentUnit")
        .SingleOrDefault(p => p.IdAggregate == idAggregate);

      if (aggregate == null)
        throw DocumentUnitAggregateException.NotFound();

      isReadOnly = (aggregate.CloseDate != null || aggregate.PreservationDate != null);
      isPreserved = aggregate.PreservationDate != null;

      return aggregate;
    }


    private Model.DocumentUnitAggregate UdsGetDocumentUnitAggregateModel(Guid idAggregate)
    {
      bool isReadOnly = false;
      bool isPreserved = false;
      return UdsGetDocumentUnitAggregateModel(idAggregate, out isReadOnly, out isPreserved);
    }


    private Model.DocumentUnitAggregate UdsTryEditDocumentUnitAggregate(Guid idAggregate)
    {
      bool isReadOnly;
      bool isPreserved;

      var aggregate = UdsGetDocumentUnitAggregateModel(idAggregate, out isReadOnly, out isPreserved);
      if (isReadOnly)
        throw DocumentUnitAggregateException.ReadOnly();

      return aggregate;
    }

    #endregion

  }

}
