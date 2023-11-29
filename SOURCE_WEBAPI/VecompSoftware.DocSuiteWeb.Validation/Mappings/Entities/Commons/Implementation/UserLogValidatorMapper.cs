using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons
{
    public class UserLogValidatorMapper : BaseMapper<UserLog, UserLogValidator>, IUserLogValidatorMapper
    {
        public UserLogValidatorMapper() { }

        public override UserLogValidator Map(UserLog entity, UserLogValidator entityTransformed)
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
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.UserPrincipalName = entity.UserPrincipalName;
            #endregion

            #region [ Navigation Properties ]
            #endregion

            return entityTransformed;
        }

    }
}
