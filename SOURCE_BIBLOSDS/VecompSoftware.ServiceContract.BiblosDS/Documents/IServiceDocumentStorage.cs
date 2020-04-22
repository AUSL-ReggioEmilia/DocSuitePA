using System;
using System.ServiceModel;
using BiblosDS.Library.Common.Objects;

namespace VecompSoftware.ServiceContract.BiblosDS.Documents
{
    [ServiceContract]
    public interface IServiceDocumentStorage : IBiblosDSServiceContract
    {
        [OperationContract]
        Guid AddDocument(Document Document);


        [OperationContract]
        Guid AddAttachToDocument(DocumentAttach Document);

        [OperationContract]
        Document GetDocument(Document Document);

        [OperationContract]
        DocumentAttach GetDocumentAttach(DocumentAttach Document);
        
        /// <summary>
        /// Get a file with a specific name in the same Storage of the document
        /// </summary>
        /// <param name="Document"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [OperationContract]
        Document GetDocumentConformAttach(Document Document, string fileName);

        [OperationContract]
        void DeleteDocument(Guid IdDocument);

        [OperationContract]
        void RestoreAttribute(Guid IdDocument);

        [OperationContract]
        void WriteAttribute(Document Document);

        [OperationContract]
        bool CheckIntegrity(Document Document);

        [OperationContract]
        bool IsAlive();

        [OperationContract]
        void InitializeStorage(DocumentStorage storage);

        /// <summary>
        /// Elimina il contenuto degli indici fulltext di uno specifico documento, rimuovendo i documenti salvati nelle specifiche filetable.
        /// </summary>
        /// <param name="document">Documento da gestire</param>
        [OperationContract]
        void DeleteFullTextDocumentData(Document document);

        /// <summary>
        /// Dato un documento esistente, vengono salvati i documenti necessari all'indicizzazione fulltext nelle specifiche filetable.
        /// </summary>
        /// <param name="document">Documento da gestire</param>
        [OperationContract]
        void WriteFullTextDocumentData(Document document);
    }    
}
