using System.Security.Cryptography;
using VecompSoftware.DocSuite.SPID.Common.Helpers.SAML;

namespace VecompSoftware.DocSuite.SPID.Common.Extensions
{
    internal static class RSACryptoServiceProviderExtension
    {
        internal static string GetSigningAlgorithmName(this RSACryptoServiceProvider rSACryptoService)
        {
            var rsaSha256Name = SignatureHelper.SIGNATURE_ALGORITHM_MORE_SHA256;
            return rsaSha256Name;            
        }
    }
}
