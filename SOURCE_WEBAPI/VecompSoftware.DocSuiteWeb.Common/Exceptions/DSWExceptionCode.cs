
namespace VecompSoftware.DocSuiteWeb.Common.Exceptions
{
    public enum DSWExceptionCode : int
    {
        Invalid = -1,

        #region DB Context
        DB_Anomaly = 1000,
        DB_Validation = 1001,
        DB_SyntaxError = 1002,
        DB_ConnectionError = 1003,
        DB_EntityStateError = 1004,
        DB_ConcurrencyError = 1005,
        DB_TransactionError = 1006,
        DB_ModelMappingError = 1007,
        #endregion

        #region UnitOfWork
        UW_Anomaly = 2000,
        UW_Validation = 2001,
        UW_EntityStateError = 2002,
        #endregion

        #region Validation
        VA_Anomaly = 3000,
        VA_RulesetValidation = 3001,
        VA_Mapper = 3002,
        VA_CustomValidatorObject = 3003,
        VA_CustomValidatorRelation = 3004,
        #endregion

        #region Service
        SS_Anomaly = 4000,
        SS_RulesetValidation = 4001,
        SS_Mapper = 4002,
        SS_NotAllowedOperation = 4003,
        #endregion

        #region Workflow
        WF_Anomaly = 5000,
        WF_RulesetValidation = 5001,
        WF_Mapper = 5002,
        #endregion

        #region Security
        SC_Anomaly = 6000,
        SC_RulesetValidation = 6001,
        SC_Mapper = 6002,
        SC_Parameters = 6003,
        SC_InvalidAccount = 6004,
        SC_NotFoundAccount = 6005,
        #endregion

        #region Document
        DM_Anomaly = 7000,
        DM_Validation = 7001,
        DM_ConnectionError = 7002,
        DM_Parameters = 7003,
        DM_NotAllowedOperation = 7004,
        #endregion

    }
}
