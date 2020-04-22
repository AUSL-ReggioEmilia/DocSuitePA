using PetaPoco;
using System.Collections.Generic;

namespace VecompSoftware.ServiceBus.Module.UDS.Storage.Relations
{
    /// <summary>
    /// Base class con operazioni crud per entità
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UDSBaseEntity<T> where T : new()
    {
        private readonly string primaryKey;

        public UDSBaseEntity(string primaryKey)
        {
            this.primaryKey = primaryKey;
        }

        //entity instance operation
        public object Insert(Database db, string dbSchema, string tableName) { return db.Insert(dbSchema, tableName, primaryKey, false, this); }
        public void Save(Database db, string tableName) { db.Save(tableName, primaryKey, this); }
        public int Update(Database db, string dbSchema, string tableName) { return db.Update(dbSchema, tableName, primaryKey, this); }
        public int Update(Database db, string dbSchema, string tableName, IEnumerable<string> columns) { return db.Update(dbSchema, tableName, primaryKey, this, columns); }
        public int Delete(Database db, string tableName) { return db.Delete(tableName, primaryKey, this); }
    }
}
