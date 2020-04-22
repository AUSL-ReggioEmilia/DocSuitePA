
using System;
using System.Collections.Generic;
using System.Linq;

namespace VecompSoftware.DocSuiteWeb.AVCP.Import
{
    public class TorinoDocumentHandler : DocumentHandler
  {
    public TorinoDocumentHandler(string dictServizioFilename, string dictContraenteFilename)
      : base(dictServizioFilename, dictContraenteFilename)
    {
    }

    protected override List<ImportRowGara> GaraRowConverter(List<DocumentRow> rows)
    {
      List<ImportRowGara> gareList = new List<ImportRowGara>();
      List<ImportAzienda> aziendeList;

      //group by CIG
      var lotti = rows.GroupBy(p => p.Values.CIG,
        (key, items) => new { CIG = key, Rows = items.ToList() });

      //crea nuova lista accorpando righe stesso cig
      List<DocumentRow> newList = new List<DocumentRow>();
      foreach (var lotto in lotti)
      {
        DocumentRow docRow = lotto.Rows[0];
        List<string> aggList = new List<string>();
        decimal ImportoAggiudicazione = 0;

        foreach (var row in lotto.Rows)
        {
          string cf = String.Empty;
          if (!string.IsNullOrEmpty(row.Values.CodiceFiscaleAggiudicatario))
            cf = row.Values.CodiceFiscaleAggiudicatario;

          string cfe = String.Empty;
          if (!string.IsNullOrEmpty(row.Values.CodiceFiscaleAggiudicatarioEstero))
            cfe = CfEstero + row.Values.CodiceFiscaleAggiudicatarioEstero;

          string res = string.Format("{0}{1}{2}", 
            row.Values.Aggiudicatario ?? "", 
            cf != "" ? "#" +cf : "",
            cfe != "" ? "#" + cfe : ""
          );

          if (!string.IsNullOrEmpty(res))
            aggList.Add(res);

          ImportoAggiudicazione += row.Values.ImportoAggiudicazione ?? 0;
        }

        docRow.Values.Aggiudicatario = aggList.Count > 0 ? string.Join(" / ", aggList) : String.Empty;
        docRow.Values.ImportoAggiudicazione = ImportoAggiudicazione;
        newList.Add(docRow);
      }

      foreach (var row in newList)
      {
        int annoProvvedimento = 0;
        string codiceProvvedimento = String.Empty;
        int numeroProvvedimento = 0;
        string CIG = String.Empty;

        if (!ParseProvvedimento(row.Values.Provvedimento, out annoProvvedimento, out codiceProvvedimento, out numeroProvvedimento))
        {
          this.listNotify.Add(new Notification
          {
            ErrorID = (int)Notification.ND_Error.ER_FIELD_ERROR,
            Message = string.Format("Record n.{0} - Provvedimento '{1}' non valido", row.RowIndex, row.Values.Provvedimento)
          });
        }

        if (!TryConvertCodiceProvvedimento(codiceProvvedimento, out codiceProvvedimento))
        {
          this.listNotify.Add(new Notification
          {
            ErrorID = (int)Notification.ND_Error.ER_FIELD_ERROR,
            Message = string.Format("Record n.{0} - Codice Servizio '{1}' non valido", row.RowIndex, codiceProvvedimento)
          });
        }

        sceltaContraenteType contraenteType;
        string contraente = (row.Values.SceltaContraente ?? "").Trim();
        if (!TryConvertContraente(contraente, out contraenteType))
        {
          this.listNotify.Add(new Notification
          {
            ErrorID = (int)Notification.ND_Error.ER_FIELD_ERROR,
            Message = string.Format("Record n.{0} - Scelta contraente '{1}' non valida", row.RowIndex, contraente)
          });
        }

        if (!string.IsNullOrEmpty(row.Values.Partecipante) && !TryParseAzienda(row.Values.Partecipante, out aziendeList))
        {
          this.listNotify.Add(new Notification
          {
            ErrorID = (int)Notification.ND_Error.ER_FIELD_ERROR,
            Message = string.Format("Record n.{0} - Campo Partecipanti '{1}' non valido", row.RowIndex, row.Values.Partecipante)
          });
        }

        if (!string.IsNullOrEmpty(row.Values.Aggiudicatario) && !TryParseAzienda(row.Values.Aggiudicatario, out aziendeList))
        {
          this.listNotify.Add(new Notification
          {
            ErrorID = (int)Notification.ND_Error.ER_FIELD_ERROR,
            Message = string.Format("Record n.{0} - Campo Aggiudicatari '{1}' non valido", row.RowIndex, row.Values.Aggiudicatario)
          });
        }

        if (!TryParseCIG(row.Values.CIG, out CIG))
        {
          this.listNotify.Add(new Notification
          {
            ErrorID = (int)Notification.ND_Error.ER_FIELD_ERROR,
            Message = string.Format("Record n.{0} - CIG '{1}' non valido", row.RowIndex, row.Values.CIG ?? "null")
          });
        }

        gareList.Add(new ImportRowGara
        {
          DocumentKey = row.Values.Provvedimento,
          IsValid = true,
          AnnoProvvedimento = annoProvvedimento,
          CodiceProvvedimento = codiceProvvedimento,
          NumeroProvvedimento = numeroProvvedimento,
          DataProvvedimento = row.Values.DataProvvedimento ?? DateTime.MinValue,
          AnnoRiferimento = row.Values.AnnoRiferimento ?? 0,
          CIG = CIG,
          CodiceFiscaleProponente = row.Values.CodiceFiscaleProponente ?? "",
          DenominazioneProponente = row.Values.DenominazioneProponente ?? "",
          Oggetto = row.Values.Oggetto ?? "",
          SceltaContraente = contraenteType,
          Partecipante = row.Values.Partecipante ?? "",
          Aggiudicatario = row.Values.Aggiudicatario ?? "",
          DurataDal = row.Values.DurataDal ?? DateTime.MinValue,
          DurataAl = row.Values.DurataAl ?? DateTime.MinValue,
          ImportoAggiudicazione = row.Values.ImportoAggiudicazione ?? 0,
          DataAggiornamento = DateTime.Today
        });
      }

      return gareList;
    }


    protected override List<ImportRowPagamento> PagamentoRowConverter(List<DocumentRow> rows)
    {
      List<ImportRowPagamento> pagList = new List<ImportRowPagamento>();
      foreach (var row in rows)
      {
        string CIG;

        if (!TryParseCIG(row.Values.CIG, out CIG))
        {
          this.listNotify.Add(new Notification
          {
            ErrorID = (int)Notification.ND_Error.ER_FIELD_ERROR,
            Message = string.Format("Record n.{0} - CIG '{1}' non valido", row.RowIndex, row.Values.CIG ?? "null")
          });
        }

        DateTime dt = row.Values.DataEmissione ?? DateTime.MinValue;
        string documentKey = string.Format("{0}/{1}", dt.Month, dt.Year);

        pagList.Add(new ImportRowPagamento
        {
          CIG = CIG,
          DocumentKey = documentKey,
          ImportoLiquidato = row.Values.ImportoLiquidato ?? 0
        });

      }

      return pagList;
    }


  }
}
