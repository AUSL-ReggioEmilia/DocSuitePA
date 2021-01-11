using System.Collections.Generic;

namespace VecompSoftware.Common
{
    public interface ISignContent : IContent
    {

        #region [ Properties ]

        bool HasSignatures { get; }
        IEnumerable<SignInfo> Signatures { get; }

        #endregion

    }
}
