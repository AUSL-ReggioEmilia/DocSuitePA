using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiblosDS.Library.Common.Objects;
using System.ComponentModel;

namespace BiblosDS.Library.IStorage
{
    /// <summary>
    /// Interfaccia comune per gli storage 
    /// </summary>
    public interface IStorage 
    {
        /// <summary>
        /// Insert of new document
        /// </summary>
        /// <param name="archive"></param>
        /// <param name="document"></param>
        /// <param name="attributeValues"></param>
        /// <returns></returns>
        void AddDocumentAttach(DocumentAttach attach);

        /// <summary>
        /// Insert of new document
        /// </summary>
        /// <param name="archive"></param>
        /// <param name="document"></param>
        /// <param name="attributeValues"></param>
        /// <returns></returns>
        void AddDocument(Document document, BindingList<DocumentAttributeValue> attributeValues);

        /// <summary>
        /// Insert attach with specific name        
        /// </summary>
        /// <param name="document"></param>
        /// <param name="name"></param>
        void AddConformAttach(Document document, DocumentContent content, string name);

        /// <summary>
        /// Retrive of a document
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        byte[] GetDocument(Document Document);

        /// <summary>
        /// Retrive a file with a specific name in the same 
        /// storage and storage area of the document
        /// </summary>
        /// <param name="Document"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        byte[] GetAttach(Document Document, string name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Document"></param>
        void DeleteDocument(Document Document);

        void DeleteDocumentWithoutAttributes(Document document);

        /// <summary>
        /// Perform the restore of the document attribute
        /// </summary>
        /// <param name="Document"></param>
        void RestoreAttribute(Document Document);

        /// <summary>
        /// Perform the write of the document attribute
        /// </summary>
        /// <param name="Document"></param>
        void WriteAttributes(Document Document);

        bool VerifyAttribute(Document Document, BindingList<DocumentAttributeValue> DbAttributes);

        void InitializeStorage(DocumentStorage storage);

        void DeleteFullTextDocuments(Document document);

        void WriteFullTextDocuments(Document document);

        void CopyTo(Document document, string destinationPath);
    }
}
