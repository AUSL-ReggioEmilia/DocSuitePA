using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.NHibernateManager;

namespace VecompSoftware.DocSuiteWeb.API
{
    public class CollaborationService : ICollaborationService
    {
        public bool IsAlive()
        {
            return true;
        }

        public string GetCollaborationsToAlert(bool checkExpiredCollaborations = false)
        {
            try
            {
                var collaborations = FacadeFactory.Instance.CollaborationFacade.GetCollaborationsExpired(checkExpiredCollaborations);
                if (!collaborations.Any())
                {
                    return new CollaborationDTO[] {}.SerializeAsResponse();
                }

                var dtoTransformArray = collaborations.ToArray();
                return dtoTransformArray.SerializeAsResponse();
            }
            catch (Exception ex)
            {
                var response = new APIResponse<CollaborationDTO>(ex);
                return response.Serialize();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }        
    }
}
