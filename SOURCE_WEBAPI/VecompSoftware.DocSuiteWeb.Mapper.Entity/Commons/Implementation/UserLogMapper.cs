using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons
{
    public class UserLogMapper : BaseEntityMapper<UserLog, UserLog>, IUserLogMapper
    {
        public UserLogMapper()
        {

        }

        public override UserLog Map(UserLog entity, UserLog entityTransformed)
        {
            #region [ Base ]
            entityTransformed.SystemUser = entity.SystemUser;
            entityTransformed.SystemServer = entity.SystemServer;
            entityTransformed.SystemComputer = entity.SystemComputer;
            entityTransformed.AccessNumber = entity.AccessNumber;
            entityTransformed.PrevOperationDate = entity.PrevOperationDate;
            entityTransformed.SessionId = entity.SessionId;
            entityTransformed.AdvancedScanner = entity.AdvancedScanner;
            entityTransformed.AdvancedViewer = entity.AdvancedViewer;
            entityTransformed.UserMail = entity.UserMail;
            entityTransformed.MobilePhone = entity.MobilePhone;
            entityTransformed.DefaultAdaptiveSearchControls = entity.DefaultAdaptiveSearchControls;
            entityTransformed.AdaptiveSearchStatistics = entity.AdaptiveSearchStatistics;
            entityTransformed.AdaptiveSearchEvaluated = entity.AdaptiveSearchEvaluated;
            entityTransformed.PrivacyLevel = entity.PrivacyLevel;
            entityTransformed.CurrentTenantId = entity.CurrentTenantId;
            entityTransformed.UserProfile = entity.UserProfile;
            entityTransformed.UserPrincipalName = entity.UserPrincipalName;
            #endregion

            return entityTransformed;
        }
    }
}
