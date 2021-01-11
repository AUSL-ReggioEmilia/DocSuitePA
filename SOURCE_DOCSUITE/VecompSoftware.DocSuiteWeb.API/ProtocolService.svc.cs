using System;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.NHibernateManager;

namespace VecompSoftware.DocSuiteWeb.API
{
    public class ProtocolService : IProtocolService
    {
        #region [ Methods ]
        
        public bool IsAlive()
        {
            return true;
        }

        public string Insert(string protocolDTO)
        {
            try
            {
                var dto = protocolDTO.Deserialize<ProtocolDTO>();
                var result = Insert(dto);
                return result.SerializeAsResponse();
            }
            catch (Exception ex)
            {
                var response = new APIResponse<ProtocolDTO>(ex);
                return response.Serialize();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        internal static ProtocolDTO Insert(ProtocolDTO dto)
        {
            return FacadeFactory.Instance.ProtocolFacade.InsertProtocol(dto);
        }

        internal static ProtocolDTO InsertInvoice(ProtocolDTO dto)
        {
            return FacadeFactory.Instance.ProtocolFacade.InsertInvoiceProtocol(dto);
        }
        #endregion
    }
}
