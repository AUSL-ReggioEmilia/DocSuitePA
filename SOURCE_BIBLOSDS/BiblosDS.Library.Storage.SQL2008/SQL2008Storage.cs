using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiblosDS.Library.IStorage;
using System.Data.SqlClient;
using System.IO;
using BiblosDS.Library.Common.Objects;
using Microsoft.Win32.SafeHandles;
using System.Data.SqlTypes;
using System.Data;
using System.ComponentModel;
using BiblosDS.Library.Common.Services;

namespace BiblosDS.Library.Storage.SQL
{
    /// <summary>
    /// storage di tipo SQL Server
    /// </summary>
    /// <remarks>
    /// utilizza due tabelle una per i contenuti documentali e una per gli attributi, 
    /// quella del contenuto documentale ha un campo di tipo FILESTREAM 
    /// entrambe le tabelle hanno il nome che inizia con il nome dell'archivio
    /// </remarks>
    public class SQL2008Storage : StorageBase 
    {
        /// <summary>
        /// Salvataggio del documento nelle tabelle del definitivo
        /// </summary>
        /// <param name="LocalFilePath"></param>
        /// <param name="Storage"></param>
        /// <param name="StorageArea"></param>
        /// <param name="Document"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        /// <remarks>piuttosto di trovarsi in situazioni, che non dovrebbero succedere, di documenti 
        /// nel transito aventi lo stesso nome di documenti nel definitivo e l'impossibilità di sovrascriverli,
        /// viene permesso la sovrascrittura
        /// </remarks> 
        protected override long SaveDocument(string LocalFilePath, BiblosDS.Library.Common.Objects.DocumentStorage Storage, BiblosDS.Library.Common.Objects.DocumentStorageArea StorageArea, BiblosDS.Library.Common.Objects.Document Document, System.ComponentModel.BindingList<BiblosDS.Library.Common.Objects.DocumentAttributeValue> attributeValue)
        {
            string connectionString = Storage.MainPath;
            string tableName = GetTableName(Storage, StorageArea);
            byte[] content = GetFileBytes(LocalFilePath);

            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                string sqlCreateTable = string.Format(@"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type in (N'U'))
                                                        BEGIN
                                                           CREATE TABLE {0} (
                                                           DocumentID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY ROWGUIDCOL,
                                                           Document VARBINARY (MAX) FILESTREAM NULL)
                                                        END", tableName);
                cnn.Open();
                using (SqlCommand cmd = new SqlCommand(sqlCreateTable, cnn))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                string sqlInsertBlob = string.Format(@"IF NOT EXISTS (SELECT DocumentID FROM {0} WHERE DocumentID = @DocumentID) 
                                                       BEGIN
                                                           INSERT INTO {1}(DocumentID, Document) VALUES (@DocumentID, @Document)  
                                                       END
                                                       ELSE BEGIN 
                                                           UPDATE {2} SET Document = @Document WHERE DocumentID = @DocumentID
                                                       END", tableName, tableName, tableName);
                using (SqlCommand cmd = new SqlCommand(sqlInsertBlob, cnn))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@DocumentID", Document.IdDocument);
                    cmd.Parameters.AddWithValue("@Document", content);
                    cmd.ExecuteNonQuery();
                } 
            }
            WriteAttributes(Document);
            return content.Length;
        }

        protected override byte[] LoadDocument(BiblosDS.Library.Common.Objects.Document Document)
        {
            SqlFileStream stream = null;
            string connectionString = Document.Storage.MainPath;
            string tableName = GetTableName(Document.Storage, Document.StorageArea);

            SqlTransaction transaction = null;            
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                string sqlPathFile = string.Format(@"SELECT Document.PathName(), GET_FILESTREAM_TRANSACTION_CONTEXT() 
                                                     FROM {0}
                                                     WHERE DocumentID = @DocumentID", tableName);

                cnn.Open();
                transaction = cnn.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    using (SqlCommand cmd = new SqlCommand(sqlPathFile, cnn, transaction))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@DocumentID", Document.IdDocument);
                        string path = string.Empty;
                        using (SqlDataReader reader = cmd.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                        {
                            if (reader.Read())
                            {
                                path = reader[0].ToString();
                                stream = new SqlFileStream(path, (byte[])reader.GetValue(1), FileAccess.Write, FileOptions.SequentialScan, 0);
                            }
                            else
                                throw new Exception("Document Id: " + Document.IdDocument + " not found.");
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch { }
                    throw;
                }                
            }
            byte[] contents = new byte[stream.Length];
            stream.Read(contents, 0, contents.Length);
            return contents;         
        }

        protected override void RemoveDocument(BiblosDS.Library.Common.Objects.Document Document)
        {
            string connectionString = Document.Storage.MainPath;
            string tableName = GetTableName(Document.Storage, Document.StorageArea);
            SqlTransaction transaction = null;
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                string sqlDelFile = string.Format(@"DELETE
                                                     FROM {0}
                                                     WHERE DocumentID = @DocumentID", tableName);

                cnn.Open();
                transaction = cnn.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    using (SqlCommand cmd = new SqlCommand(sqlDelFile, cnn, transaction))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@DocumentID", Document.IdDocument);
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch { }
                    throw;
                }
            }
        }

        protected override void SaveAttributes(Document Document)
        {
            string connectionString = Document.Storage.MainPath;
            string tableName = GetTableName(Document.Storage, Document.StorageArea) + "_Attributes" ;

            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                string sqlCreateTable = string.Format(@"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type in (N'U'))
                                                        BEGIN
                                                           CREATE TABLE {0} (
                                                           IdDocument UNIQUEIDENTIFIER NOT NULL,
                                                           IdAttribute UNIQUEIDENTIFIER NOT NULL, 
                                                           Value VARCHAR(MAX) NULL, 
                                                           InvariantValue VARCHAR(MAX) NULL )
                                                        END", tableName);
                cnn.Open();
                using (SqlCommand cmd = new SqlCommand(sqlCreateTable, cnn))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                cnn.Close(); 
            }


            string sqlAttributeAdd = string.Format(@"IF NOT EXISTS (SELECT IdDocument FROM {0} WHERE IdDocument = @IdDocument AND IdAttribute = @IdAttribute )
                    BEGIN 
                        INSERT INTO {0} (IdDocument, IdAttribute, Value, InvariantValue)
                        VALUES (@IdDocument, @IdAttribute, @Value, @InvariantValue)
                    END
                    ELSE
                    BEGIN 
                        UPDATE {0} SET Value = '@Value', InvariantValue = '@InvariantValue' 
                    END", tableName) ;
            
            SqlTransaction transaction = null;
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {             
                cnn.Open();
                transaction = cnn.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    foreach (DocumentAttributeValue item in Document.AttributeValues)
                    {
                        using (SqlCommand cmd = new SqlCommand(sqlAttributeAdd, cnn, transaction))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@IdDocument", Document.IdDocument);
                            cmd.Parameters.AddWithValue("@IdAttribute", item.Attribute.IdAttribute);
                            switch (item.Attribute.AttributeType)
                            {
                                case "System.String":
                                    cmd.Parameters.AddWithValue("@ValueString", item.Value);
                                    break;
                                case "System.DateTime":
                                    cmd.Parameters.AddWithValue("@ValueDateTime", item.Value);
                                    break;
                                case "System.Double":
                                    cmd.Parameters.AddWithValue("@ValueFloat", item.Value);
                                    break;
                                case "System.Int":
                                    cmd.Parameters.AddWithValue("@ValueInt", item.Value);
                                    break;
                                default:
                                    throw new BiblosDS.Library.Common.Exceptions.Attribute_Exception(string.Format("Type not found: {0}", item.Attribute.AttributeType));
                            }                            
                            cmd.ExecuteNonQuery();
                        }   
                    }                    
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // TODO Lgging                     
                    try
                    {
                        transaction.Rollback();
                    }
                    catch { }
                    throw;
                }
            }
        }

        #region private       

        static byte[] GetFileBytes(string filePath)
        {
            byte[] bytFile = null;
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                bytFile = new byte[fs.Length];
                fs.Read(bytFile, 0, bytFile.Length);
                fs.Close();
            }
            return bytFile;
        }

        private string GetTableName(DocumentStorage Storage, DocumentStorageArea StorageArea)
        {
            string table = Storage.Name;            
            if (!string.IsNullOrEmpty(StorageArea.Path))
            {
                table = StorageArea.Path;                
            }
            return table;
        }
        #endregion

        protected override BindingList<DocumentAttributeValue> LoadAttributes(Document Document)
        {
            throw new NotImplementedException();
        }     

        protected override long WriteFile(string saveFileName, DocumentStorage storage, DocumentStorageArea storageArea, Document document, DocumentContent content)
        {
            throw new NotImplementedException();
        }

        protected override byte[] LoadAttach(Document Document, string name)
        {
            throw new NotImplementedException();
        }
    }
}
