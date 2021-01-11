using Microsoft.SqlServer.Management.Smo;
using System.Collections.Generic;
using VecompSoftware.Helpers.UDS.Exceptions;

namespace VecompSoftware.ServiceBus.Module.UDS.Storage.Smo
{
    /// <summary>
    /// Metodi di estensione alla classe Table di SQL Server Management Objects 
    /// </summary>
    public static class SmoTableExtensions
    {
        public static void ValidateSchema(this Table tb)
        {
            //verifica nome tabella
            if (string.IsNullOrWhiteSpace(tb.Name))
            {
                throw new UDSTableValidateException("Il nome della tabella non è valido");
            }

            //verifica nomi duplicati nella collezione
            HashSet<string> distincts = new HashSet<string>();
            foreach (Column col in tb.Columns)
            {
                if (string.IsNullOrWhiteSpace(col.Name))
                {
                    throw new UDSTableValidateException("Colonna con nome vuoto non è consentito");
                }

                if (distincts.Contains(col.Name.ToLower()))
                {
                    throw new UDSTableValidateException(string.Format("Esiste già una colonna con questo nome: '{0}'", col.Name));
                }

                distincts.Add(col.Name);
            }
        }


        public static void AddPrimaryKey(this Table tb, string columnName)
        {
            //create primary key index
            Index pk = new Index(tb, string.Format("PK_{0}_{1}", tb.Name, columnName));

            IndexedColumn indexedColumn = new IndexedColumn(pk, columnName);
            pk.IndexedColumns.Add(indexedColumn);
            pk.IndexKeyType = IndexKeyType.DriPrimaryKey;
            pk.IsClustered = false;
            pk.Create();
        }


        public static void AddIndex(this Table tb, string columnName)
        {
            AddIndex(tb, new List<string>() { columnName });
        }

        public static void AddIndex(this Table tb, IEnumerable<string> columnNames)
        {
            Index idx = new Index(tb, string.Format("IDX_{0}_{1}", tb.Name, string.Join("_", columnNames)));

            foreach (string name in columnNames)
            {
                IndexedColumn indexedColumn = new IndexedColumn(idx, name);
                idx.IndexedColumns.Add(indexedColumn);
            }
            idx.IndexKeyType = IndexKeyType.None;
            idx.IsClustered = false;
            idx.Create();
        }

        public static void AddUniqueIndex(this Table tb, string columnName)
        {
            AddUniqueIndex(tb, new List<string>() { columnName });
        }

        public static void AddUniqueIndex(this Table tb, IEnumerable<string> columnNames)
        {
            Index idx = new Index(tb, string.Format("UX_{0}_{1}", tb.Name, string.Join("_", columnNames)));

            foreach (string name in columnNames)
            {
                IndexedColumn indexedColumn = new IndexedColumn(idx, name);
                idx.IndexedColumns.Add(indexedColumn);
            }
            idx.IndexKeyType = IndexKeyType.DriUniqueKey;
            idx.IsClustered = false;
            idx.IsUnique = true;
            idx.Create();
        }

        public static void AddClusterIndex(this Table tb, string columnName)
        {
            Index idx = new Index(tb, string.Format("IDX_{0}_{1}", tb.Name, columnName));

            IndexedColumn indexedColumn = new IndexedColumn(idx, columnName);
            idx.IndexedColumns.Add(indexedColumn);
            idx.IndexKeyType = IndexKeyType.None;
            idx.IsClustered = true;
            idx.Create();
        }


        public static void AddForeignKey(this Table tb, string columnName, string refTable, string refTableSchema, string refColumn)
        {
            ForeignKey fk = new ForeignKey(tb, string.Format("FK_{0}_{1}", tb.Name, columnName));

            ForeignKeyColumn fkc = new ForeignKeyColumn(fk, columnName, refColumn);
            fk.Columns.Add(fkc);
            fk.ReferencedTable = refTable;
            fk.ReferencedTableSchema = refTableSchema;
            fk.Create();
        }

    }
}
