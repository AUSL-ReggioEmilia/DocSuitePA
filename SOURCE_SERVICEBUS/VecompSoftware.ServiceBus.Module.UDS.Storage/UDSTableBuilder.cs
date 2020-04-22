using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.Helpers.UDS;
using VecompSoftware.ServiceBus.Module.UDS.Storage.Smo;

namespace VecompSoftware.ServiceBus.Module.UDS.Storage
{

    /// <summary>
    /// Builder delle tabelle per lo storage UDS.
    /// Consente di creare la tabella principale della UDS e tutte le tabelle di relazione (Documents, Contacts, ecc...)
    /// </summary>
    public class UDSTableBuilder
    {
        private readonly string _tableName = string.Empty;
        private readonly string _dbSchema = string.Empty;
        private readonly UDSModel _uds;

        public const string DSWIdCategoryPK = "idCategory";
        public const string DSWCategoryTableName = "Category";

        public const string UDSTablePrefix = "UDS_T_";
        public const string UDSRepositoriesTableName = "UDSRepositories";
        public const string UDSRepositoriesSequenceCurrentNumberField = "SequenceCurrentNumber";
        public const string UDSRepositoriesSequenceCurrentYearField = "SequenceCurrentYear";
        public const string UDSRepositoriesPK = "IdUDSRepository";

        public const string UDSPK = "UDSId";
        public const string UDSDocumentsPK = "UDSDocumentId";

        public const string UDSDocumentsFK = "IdDocument";
        public const string UDSRepositoryFK = "IdUDSRepository";
        public const string UDSIdCategoryFK = "IdCategory";
        public const string UDSFK = "UDSId";

        public const string UDSRegistrationDateField = "RegistrationDate";
        public const string UDSRegistrationUserField = "RegistrationUser";
        public const string UDSLastChangedDateField = "LastChangedDate";
        public const string UDSLastChangedUserField = "LastChangedUser";
        public const string UDSYearField = "_year";
        public const string UDSNumberField = "_number";
        public const string UDSSubjectField = "_subject";
        public const string UDSStatusField = "_status";
        public const string UDSTimestampField = "Timestamp";
        public const string UDSCancelMotivationField = "_cancelMotivation";

        public const string UDSDocumentsDocumentTypeField = "DocumentType";
        public const string UDSDocumentsDocumentNameField = "DocumentName";
        public const string UDSContactsContactManualField = "ContactManual";
        public const string UDSContactsContactTypeField = "ContactType";
        public const string UDSContactsContactLabelField = "ContactLabel";
        public const string UDSAuthorizationsAuthorizationLabelField = "AuthorizationLabel";
        public const string UDSDocumentsDocumentLabelField = "DocumentLabel";

        public const string UDSLogDateField = "LogDate";
        public const string UDSSystemUserField = "SystemUser";
        public const string UDSSystemComputerField = "SystemComputer";
        public const string UDSLogTypeField = "LogType";
        public const string UDSLogDescriptionField = "LogDescription";
        public const string UDSSeverityField = "Severity";

        public string DbSchema => _dbSchema;
        public string UDSTableName => _tableName;
        public string UDSName => _uds.Model.Title;

        public string UDSDocumentsTableName => string.Concat(_tableName, "_Documents");
        public string UDSTableNameWithoutPrefix => _tableName.Remove(0, UDSTablePrefix.Length);


        public UDSTableBuilder(UDSModel uds, string dbSchema = "dbo")
        {
            _uds = uds;
            _tableName = string.Concat(UDSTablePrefix, Utils.SafeSQLName(uds.Model.Title));
            _dbSchema = dbSchema;
        }

        public void CreateUDSTable(SmoContext ctx)
        {
            Table tb = new Table(ctx.DbInstace, _tableName, _dbSchema);

            Column col = new Column(tb, UDSPK, DataType.UniqueIdentifier)
            {
                Nullable = false
            };
            tb.Columns.Add(col);

            //colonne dei metadati
            if (_uds.Model.Metadata != null && _uds.Model.Metadata.Length > 0)
            {
                foreach (Section section in _uds.Model.Metadata.Where(f => f.Items != null))
                {
                    foreach (FieldBaseType field in section.Items)
                    {
                        AddField(field, tb);
                    }
                }
            }

            col = new Column(tb, UDSRepositoryFK, DataType.UniqueIdentifier)
            {
                Nullable = false
            };
            tb.Columns.Add(col);

            //colonne di default
            AddDateTimeOffsetField(UDSRegistrationDateField, tb);

            col = new Column(tb, UDSRegistrationUserField, DataType.NVarChar(256))
            {
                Nullable = false
            };
            tb.Columns.Add(col);

            AddDateTimeOffsetField(UDSLastChangedDateField, tb, true);

            col = new Column(tb, UDSLastChangedUserField, DataType.NVarChar(256))
            {
                Nullable = true
            };
            tb.Columns.Add(col);

            col = new Column(tb, UDSYearField, DataType.SmallInt)
            {
                Nullable = false
            };
            tb.Columns.Add(col);

            col = new Column(tb, UDSNumberField, DataType.Int)
            {
                Nullable = false
            };
            tb.Columns.Add(col);

            col = new Column(tb, UDSSubjectField, DataType.NVarChar(4000))
            {
                Nullable = true
            };
            tb.Columns.Add(col);

            col = new Column(tb, UDSIdCategoryFK, DataType.SmallInt)
            {
                Nullable = false
            };
            tb.Columns.Add(col);

            col = new Column(tb, UDSStatusField, DataType.SmallInt)
            {
                Nullable = false
            };
            col.AddDefaultConstraint().Text = "1"; // Active
            tb.Columns.Add(col);

            col = new Column(tb, UDSCancelMotivationField, DataType.NVarChar(1024))
            {
                Nullable = true
            };
            tb.Columns.Add(col);

            col = new Column(tb, UDSTimestampField, DataType.Timestamp)
            {
                Nullable = false
            };
            tb.Columns.Add(col);

            //Create the table on the instance of SQL Server.
            tb.ValidateSchema();
            tb.Create();

            tb.AddPrimaryKey(UDSPK);
            tb.AddForeignKey(UDSIdCategoryFK, DSWCategoryTableName, "dbo", DSWIdCategoryPK);
            tb.AddForeignKey(UDSRepositoryFK, UDSRepositoriesTableName, _dbSchema, UDSRepositoryFK);
            tb.AddClusterIndex(UDSRegistrationDateField);
            tb.AddIndex(new List<string>() { UDSYearField, UDSNumberField });
        }


        public void UpdateUDSTable(SmoContext ctx)
        {
            Table tb = ctx.GetTable(_tableName);
            if (tb == null)
            {
                throw new Exception(string.Concat("Table ", _tableName, " not found in methods UpdateUDSTable"));
            }

            //colonne dei metadati
            if (_uds.Model.Metadata != null && _uds.Model.Metadata.Length > 0)
            {
                foreach (Section section in _uds.Model.Metadata)
                {
                    foreach (FieldBaseType field in section.Items.Where(f => !ctx.ColumnExist(tb, f.ColumnName, true)))
                    {
                        AddField(field, tb);
                    }
                }
            }

            tb.ValidateSchema();
            tb.Alter();
        }

        public void CreateUDSDocumentsTable(SmoContext ctx)
        {
            Table tb = new Table(ctx.DbInstace, UDSDocumentsTableName, _dbSchema);

            Column col = new Column(tb, UDSDocumentsPK, DataType.UniqueIdentifier)
            {
                Nullable = false
            };
            tb.Columns.Add(col);

            col = new Column(tb, UDSFK, DataType.UniqueIdentifier)
            {
                Nullable = false
            };
            tb.Columns.Add(col);

            col = new Column(tb, UDSDocumentsFK, DataType.UniqueIdentifier)
            {
                Nullable = false
            };
            tb.Columns.Add(col);

            col = new Column(tb, UDSDocumentsDocumentNameField, DataType.NVarChar(256))
            {
                Nullable = true
            };
            tb.Columns.Add(col);

            col = new Column(tb, UDSDocumentsDocumentTypeField, DataType.SmallInt)
            {
                Nullable = false
            };
            tb.Columns.Add(col);

            col = new Column(tb, UDSDocumentsDocumentLabelField, DataType.NVarChar(256))
            {
                Nullable = true
            };
            tb.Columns.Add(col);

            AddDateTimeOffsetField(UDSRegistrationDateField, tb);

            tb.ValidateSchema();
            tb.Create();

            tb.AddPrimaryKey(UDSDocumentsPK);
            tb.AddForeignKey(UDSFK, UDSTableName, _dbSchema, UDSPK);
            tb.AddIndex(UDSFK);
            tb.AddClusterIndex(UDSRegistrationDateField);
        }


        private void AddField(FieldBaseType field, Table tb)
        {
            if (field.GetType() == typeof(TextField))
            {
                AddTextCol(field, tb);
                return;
            }

            if (field.GetType() == typeof(LookupField))
            {
                AddTextCol(field, tb);
                return;
            }

            if (field.GetType() == typeof(DateField))
            {
                AddDateCol(field, tb);
                return;
            }

            if (field.GetType() == typeof(NumberField))
            {
                AddNumberCol(field, tb);
                return;
            }

            if (field.GetType() == typeof(BoolField))
            {
                AddBoolCol(field, tb);
                return;
            }
            if (field.GetType() == typeof(EnumField))
            {
                AddEnumCol(field, tb);
                return;
            }
            if (field.GetType() == typeof(StatusField))
            {
                AddEnumCol(field, tb);
                return;
            }
        }

        private void AddTextCol(FieldBaseType field, Table tb)
        {
            Column col = new Column(tb, field.ColumnName, DataType.NVarChar(4000))
            {
                Nullable = true
            };
            tb.Columns.Add(col);
        }


        private void AddDateCol(FieldBaseType field, Table tb)
        {
            Column col = new Column(tb, field.ColumnName, DataType.DateTimeOffset(7))
            {
                Nullable = true
            };
            tb.Columns.Add(col);
        }


        private void AddNumberCol(FieldBaseType field, Table tb)
        {
            Column col = new Column(tb, field.ColumnName, DataType.Float)
            {
                Nullable = true
            };
            tb.Columns.Add(col);
        }


        private void AddBoolCol(FieldBaseType field, Table tb)
        {
            Column col = new Column(tb, field.ColumnName, DataType.Bit)
            {
                Nullable = true
            };
            tb.Columns.Add(col);
        }

        private void AddEnumCol(FieldBaseType field, Table tb)
        {
            Column col = new Column(tb, field.ColumnName, DataType.NVarChar(256))
            {
                Nullable = true
            };
            tb.Columns.Add(col);
        }

        private void AddDateTimeOffsetField(string columnName, Table tb, bool nullable = false)
        {
            Column col = new Column(tb, columnName, DataType.DateTimeOffset(7))
            {
                Nullable = nullable
            };
            col.AddDefaultConstraint().Text = "sysdatetimeoffset()";
            tb.Columns.Add(col);
        }

    }
}
