
namespace VecompSoftware.DocSuiteWeb.Model.Securities
{
    public struct HostIdentify
    {
        #region [Constructor]

        public HostIdentify(string host, string serviceName)
        {
            Host = host;
            ServiceName = serviceName;
        }

        #endregion

        #region [Properties]

        /// <summary>
        /// Host name
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        ///  Service name of host identification
        /// </summary>
        public string ServiceName { get; set; }

        #endregion
    }
}
