using System;

namespace VecompSoftware.DocSuiteWeb.Model.Securities
{
    public class TokenSecurityModel
    {
        #region [Constructor]

        public TokenSecurityModel()
        {

        }

        #endregion

        #region [Properties]

        /// <summary>
        /// Service's host where Token was generated
        /// </summary>
        public HostIdentify Host { get; set; }

        /// <summary>
        /// The workflow's name which Token was generated
        /// </summary>
        public string WorkflowName { get; set; }

        /// <summary>
        /// The workflow's id which Token was generated
        /// </summary>
        public Guid? AuthenticationId { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public Guid Token { get; set; }

        /// <summary>
        /// Token expiry date
        /// </summary>
        public DateTimeOffset? ExpiryDate { get; set; }

        #endregion
    }
}
