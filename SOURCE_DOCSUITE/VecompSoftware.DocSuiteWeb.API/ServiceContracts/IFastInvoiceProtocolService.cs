using System;
using System.ServiceModel;

namespace VecompSoftware.DocSuiteWeb.API
{
    [ServiceContract]
    public interface IFastInvoiceProtocolService
    {
        [OperationContract]
        bool IsAlive();

        [OperationContract]
        string GetAccountingSectionals();

        [OperationContract]
        string GetContacts(string searchCode);

        [OperationContract]
        string GetContainers();

        [OperationContract]
        string GetCategories();

        [OperationContract]
        string GetPECMailboxes();

        [OperationContract]
        string GetProtocolsToSend();

        [OperationContract]
        int CountProtocolReserved(DateTime from, DateTime to);

        [OperationContract]
        string InsertProtocol(string protocolDTO);

        [OperationContract]
        string SendMail(string mailDTO);

        [OperationContract]
        string SendProtocolMail(string mailDTO, string protocolDTO);
    }
}
