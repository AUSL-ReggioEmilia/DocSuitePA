using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using VecompSoftware.DocSuite.SPID.Common.Helpers.SAML;
using VecompSoftware.DocSuite.SPID.Model.SAML;

namespace VecompSoftware.DocSuite.SPID.SAML.Test
{
    [TestClass]
    public class AuthRequestTest
    {
        [TestMethod]
        public void CreatePostRequestAndCheckResultNotEmpty()
        {
            using (X509Certificate2 cert = new X509Certificate2("spid-developer.pfx", ".", X509KeyStorageFlags.Exportable))
            {
                SamlRequestOption options = new SamlRequestOption()
                {
                    SPIDLevel = SamlAuthLevel.SpidL1,
                    SPDomain = "http://www.vecompsoftware.it",
                    Destination = "http://idp.test.it",
                    AssertionConsumerServiceIndex = 1,
                    AttributeConsumingServiceIndex = 1,
                    Certificate = cert
                };

                NullLoggerFactory log = new NullLoggerFactory();
                NullLoggerFactory logHelper = new NullLoggerFactory();
                //AuthRequest request = new AuthRequest(log, new SignatureHelper(logHelper));
                //string result = request.PostableSpidAuthRequest(options);
                //Assert.AreNotEqual(string.Empty, request);
            }            
        }

        [TestMethod]
        public void CreateRedirectRequestAndCheckResultNotEmpty()
        {
            using (X509Certificate2 cert = new X509Certificate2("spid-developer.pfx", ".", X509KeyStorageFlags.Exportable))
            {
                SamlRequestOption options = new SamlRequestOption()
                {
                    SPIDLevel = SamlAuthLevel.SpidL1,
                    SPDomain = "http://www.vecompsoftware.it",
                    Destination = "http://idp.test.it",
                    AssertionConsumerServiceIndex = 1,
                    AttributeConsumingServiceIndex = 1,
                    Certificate = cert
                };

                NullLoggerFactory log = new NullLoggerFactory();
                NullLoggerFactory logHelper = new NullLoggerFactory();
                //AuthRequest request = new AuthRequest(log, new SignatureHelper(logHelper));
                //string result = request.RedirectableSpidAuthRequest(options);
                //Assert.AreNotEqual(string.Empty, request);
            }            
        }
    }
}
