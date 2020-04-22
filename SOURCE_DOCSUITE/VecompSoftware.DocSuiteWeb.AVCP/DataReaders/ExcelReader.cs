using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Excel;
using System.Linq;
using System.Data;

namespace VecompSoftware.DocSuiteWeb.AVCP.Import
{
    public class ExcelReader : IAVCPDataReader
    {
        private RecordTemplate profile;
        private string filename;
        private string templateFilename;

        public List<Notification> Notifications;

        public ExcelReader(string filename, string templateFilename)
        {
            this.filename = filename;
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

            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
            {
                Notifications.Add(new Notification
                {
                    ErrorID = (int)Notification.ND_Error.ER_FILE_NOT_FOUND,
                    Message = string.Format("{0} - File non trovato", filename)
                });
                return false;
            }

            FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read);
            IExcelDataReader reader = ExcelReaderFactory.CreateBinaryReader(stream);

            if (!reader.IsValid)
            {
                Notifications.Add(new Notification
                {
                    ErrorID = (int)Notification.ND_Error.ER_FILE_FORMAT,
                    Message = "Il formato del file Excel non è stato riconosciuto",
                    ExceptionMessage = reader.ExceptionMessage
                });
                return false;
            }

            if(profile.fields.Any(x => !string.IsNullOrEmpty(x.columnName)))
            {
                reader.IsFirstRowAsColumnNames = true;
                DataSet dataSet = reader.AsDataSet();
                for(int i = 0; i < profile.fields.Count(); i++)
                {
                    if (!string.IsNullOrEmpty(profile.fields[i].columnName) && dataSet.Tables[0].Columns[i].ColumnName != profile.fields[i].columnName)
                    {
                        Notifications.Add(new Notification
                        {
                            ErrorID = (int)Notification.ND_Error.ER_FILE_FORMAT,
                            Message = "Il formato del file Excel non è corretto",
                            ExceptionMessage = reader.ExceptionMessage
                        });
                        return false;
                    }
                }
            }

            //salta intestazioni
            int rowIdx = 1;
            reader.Read();

            while (reader.Read())
            {
                rowIdx++;
                int idx = 0;

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

                    object cellValue = reader.GetValue(idx++);
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
                        Type tp = Type.GetType("System." + field.type);
                        if (cellValue != null && cellValue.ToString().Trim() != "")
                        {
                            //se si tratta del nome file
                            if (field.isFilename)
                            {
                                string importFile = Path.Combine(Path.GetDirectoryName(filename), cellValue.ToString());

                                //controlla l'esistenza del file
                                if (!File.Exists(importFile))
                                    errors.Add(string.Format("Error:{0}-'{1}' file inesistente", (int)Notification.ND_Error.ER_FILE_NOT_FOUND, importFile));
                            }

                            if (tp == typeof(System.DateTime))
                            {
                                try
                                {
                                    cellValue = DateTime.FromOADate(reader.GetDouble(idx - 1));
                                }
                                catch (Exception)
                                {
                                    DateTime dt;
                                    if (!DateTime.TryParse(reader.GetString(idx - 1), out dt))
                                        throw new ApplicationException(string.Format("Errore campo '{0}' DataTime non riconosciuto", field.name));
                                    cellValue = dt;
                                }
                            }

                            value = Convert.ChangeType(cellValue, tp, CultureInfo.CreateSpecificCulture("it-IT"));
                            if (tp == typeof(System.String))
                                value = value.ToString().Trim();

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

            reader.Close();
            return Notifications.Count == 0;
        }
    }
}
