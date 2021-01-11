using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Parameters
{
    public class ParameterEnv : DSWBaseEntity
    {

        #region [ Constructor ]
        public ParameterEnv() : this(Guid.NewGuid()) { }
        public ParameterEnv(Guid uniqueId)
            : base(uniqueId) { }
        #endregion

        #region [ Properties ]
        public string Name { get; set; }
        public string Value { get; set; }
        #endregion
    }
}
