using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Globalization;
using System.IO;

namespace VecompSoftware.DocSuiteWeb.AVCP.Import
{
    public class DbReader : IAVCPDataReader
  {
    private RecordTemplate profile;
    private string connstr;
    private string sSQL;
    private string templateFilename;

    public List<Notification> Notifications;

    public DbReader(string connstr, string sSQL, string templateFilename)
    {
      this.connstr = connstr;
      this.sSQL = sSQL;
      this.templateFilename = templateFilename;
    }

    public bool Fetch(out List<DocumentRow> rows, out List<Notification> err)
    {
      this.Notifications = new List<Notification>();
      err = this.Notifications;
      rows = new List<DocumentRow>();

      //carica il profilo
      if (string.IsNullOrEmpty(templateFilename) || !File.Exists(templateFilename))
      {
        Notifications.Add(new Notification
        {
          ErrorID = (int)Notification.ND_Error.ER_TEMPLATE_NOT_FOUND,
          Message = string.Format("{0} - File non trovato", templateFilename)
        });
        return false;
      }
      this.profile = RecordTemplate.Load(templateFilename, "");

      using (OleDbConnection objConnection = new OleDbConnection(connstr))
      {
        OleDbCommand objCmd = new OleDbCommand(sSQL, objConnection);
        OleDbDataReader reader = null;

        objConnection.Open();
        using (reader = objCmd.ExecuteReader())
        {
          int rowIdx = 0;

          while (reader.Read() == true)
          {
            int idx = 0;

            rowIdx++;
            var row = new DocumentRow();
            row.RowIndex = rowIdx;

            List<string> errors = new List<string>();
            foreach (RecordTemplateField field in profile.fields)
            {
              if (idx >= reader.FieldCount)
              {
                if (!field.ignore)
                  errors.Add(string.Format("Error:{0}- Campo '{1}' non presente.", (int)Notification.ND_Error.ER_FILE_FORMAT, field.name));
                else
                  errors.Add(string.Format("Error:{0}- Formato file non corretto.", (int)Notification.ND_Error.ER_FILE_FORMAT));

                continue;
              }

              if (idx >= reader.FieldCount)
              {
                errors.Add(string.Format("Error:{0}-'{1}' richiesto", (int)Notification.ND_Error.ER_FIELD_REQUIRED, field.name));
                continue;
              }

              object cellValue = reader[field.name];
              idx++;
              if (field.ignore)
                continue;

              //controlla dato obbligatorio
              if (field.requested && (cellValue == null || string.IsNullOrEmpty(cellValue.ToString())))
              {
                errors.Add(string.Format("Error:{0}-'{1}' richiesto", (int)Notification.ND_Error.ER_FIELD_REQUIRED, field.name));
                continue;
              }

              //lettura valore
              object value;
              try
              {
                if (cellValue != null && cellValue.ToString().Trim() != "")
                {
                  //se si tratta del nome file
                  if (field.isFilename)
                  {
                    string importFile = cellValue.ToString();
                    //controlla l'esistenza del file
                    if (!File.Exists(importFile))
                      errors.Add(string.Format("Error:{0}-'{1}' file inesistente", (int)Notification.ND_Error.ER_FILE_NOT_FOUND, importFile));
                  }

                  Type tp = Type.GetType("System." + field.type);
                  value = Convert.ChangeType(cellValue, tp, CultureInfo.CreateSpecificCulture("it-IT"));

                  //imposta il valore
                  row.SetValue(field.name, value);
                }
                else
                  row.SetValue(field.name, null);
              }
              catch (Exception ex)
              {
                errors.Add(string.Format("Error:{0}-{1}", (int)Notification.ND_Error.ER_FIELD_ERROR, ex.Message));
              }
            }

            if (errors.Count > 0)
            {
              Notifications.Add(new Notification
              {
                ErrorID = (int)Notification.ND_Error.ER_FIELD_ERROR,
                Message = string.Format("Riga {0} - {1}", row.RowIndex, string.Join(" --- ", errors.ToArray())),
                ExceptionMessage = ""
              });
            }

            rows.Add(row);
          }
        }
      }

      return Notifications.Count == 0;
    }

  }
}
