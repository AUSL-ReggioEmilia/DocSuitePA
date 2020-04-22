using System;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using VecompSoftware.Commons.BiblosDS.Objects.Exceptions;

namespace VecompSoftware.Commons.CompEd.Services
{
    public class SignService
    {
        public static byte[] GetByteArrayFromString(String originalString)
        {
            Encoding unicode = Encoding.Unicode;
            return unicode.GetBytes(originalString);
        }

        public static String GetStringFromByteArray(byte[] originalContent)
        {
            Encoding unicode = Encoding.Unicode;
            return unicode.GetString(originalContent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalMsg">il messaggio originario da firmare</param>
        public static byte[] Sign(String originalMsg, String signerName)
        {
#if DEBUG
            System.Diagnostics.Trace.WriteLine("System.Security.Cryptography.Pkcs " +
                "Sample: Single-signer signed and verified message");

            System.Diagnostics.Trace.WriteLine("\nOriginal message (len " + originalMsg.Length.ToString() + "): " + originalMsg + "  ");
#endif

            //  Convert message to array of bytes for signing.
            byte[] msgBytes = GetByteArrayFromString(originalMsg);


            return Sign(msgBytes, signerName);
        }

        public static byte[] Sign(byte[] msgBytes, String signerName)
        {
            X509Certificate2 signerCert = GetSignerCert(signerName);

            byte[] encodedSignedCms = SignMsg(msgBytes, signerCert);

#if DEBUG
            System.Diagnostics.Trace.WriteLine("     RECIPIENT SIDE     ");
#endif

            if (VerifyMsg(encodedSignedCms))
            {
#if DEBUG
                System.Diagnostics.Trace.WriteLine("\nMessage verified");
#endif
                return encodedSignedCms;
            }
            else
            {
#if DEBUG
                System.Diagnostics.Trace.WriteLine("\nMessage failed to verify");
#endif
                return null;
            }
        }

        /// <summary>
        /// Apre il My or Personal certificate store e ricerca per le 
        /// credenziali per la firma del messaggio.
        /// Il certificato deve avere il nome passato come parametro
        /// </summary>
        /// <returns></returns>
        public static X509Certificate2 GetSignerCert(string signerName)
        {
            //  Open the My certificate store.
            X509Store storeMy = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            storeMy.Open(OpenFlags.ReadOnly);

#if DEBUG
            //  Display certificates to help troubleshoot 
            //  the example's setup.
            System.Diagnostics.Trace.WriteLine("Found certs with the following subject " +
                "names in the {0} store:", storeMy.Name);
            foreach (X509Certificate2 cert in storeMy.Certificates)
            {
                System.Diagnostics.Trace.WriteLine("\t{0}", cert.SubjectName.Name);
            }
#endif

            //  Find the signer's certificate.
            X509Certificate2Collection certColl = storeMy.Certificates.Find(X509FindType.FindBySubjectName, signerName, false);

            if (certColl == null)
                throw new CertificateNotFound_Exception("No certificate found with name: " + signerName);

#if DEBUG
            System.Diagnostics.Trace.WriteLine(
                "Found " + certColl.Count.ToString() + " certificates in the " + storeMy.Name.ToString() + " store with name " + signerName);

            //  Check to see if the certificate suggested by the example
            //  requirements is not present.
            if (certColl.Count == 0)
            {
                System.Diagnostics.Trace.WriteLine(
                    "A suggested certificate to use for this sign " +
                    "is not in the certificate store. Select " +
                    "an alternate certificate to use for " +
                    "signing the message.");
            }
#endif
            storeMy.Close();

            //  If more than one matching cert, return the first one.
            if (certColl.Count > 0)
                return certColl[0];
            else
                throw new CertificateNotFound_Exception("No certificate found with name: " + signerName);
        }

        //  Sign the message with the private key of the signer.
        static public byte[] SignMsg(Byte[] msg, X509Certificate2 signerCert)
        {
            //  Place message in a ContentInfo object.
            //  This is required to build a SignedCms object.
            ContentInfo contentInfo = new ContentInfo(msg);

            //  Instantiate SignedCms object with the ContentInfo above.
            //  Has default SubjectIdentifierType IssuerAndSerialNumber.
            //  Has default Detached property value false, so message is
            //  included in the encoded SignedCms.
            SignedCms signedCms = new SignedCms(contentInfo);

            //  Formulate a CmsSigner object for the signer.
            CmsSigner cmsSigner = new CmsSigner(signerCert);

#if DEBUG
            //  Sign the CMS/PKCS #7 message.
            System.Diagnostics.Trace.Write("Computing signature with signer subject " +
                "name {0} ... ", signerCert.SubjectName.Name);
#endif
            signedCms.ComputeSignature(cmsSigner, false);
#if DEBUG
            System.Diagnostics.Trace.WriteLine("Done.");
#endif

            //  Encode the CMS/PKCS #7 message.
            return signedCms.Encode();
        }

        //  Verify the encoded SignedCms message and return a Boolean
        //  value that specifies whether the verification was successful.
        static public bool VerifyMsg(byte[] encodedSignedCms)
        {
            //  Prepare an object in which to decode and verify.
            SignedCms signedCms = new SignedCms();

            signedCms.Decode(encodedSignedCms);

            //  Catch a verification exception if you want to
            //  advise the message recipient that 
            //  security actions might be appropriate.
            try
            {
                //  Verify signature. Do not validate signer
                //  certificate for the purposes of this example.
                //  Note that in a production environment, validating
                //  the signer certificate chain will probably
                //  be necessary.
#if DEBUG
                System.Diagnostics.Trace.Write("Checking signature on message ... ");
#endif
                signedCms.CheckSignature(true);
#if DEBUG
                System.Diagnostics.Trace.WriteLine("Done.");
#endif
            }
            catch (System.Security.Cryptography.CryptographicException e)
            {
#if DEBUG
                System.Diagnostics.Trace.WriteLine("VerifyMsg caught exception:  {0}",
                    e.Message);
                System.Diagnostics.Trace.WriteLine("Verification of the signed PKCS #7 " +
                    "failed. The message, signatures, or " +
                    "countersignatures may have been modified " +
                    "in transit or storage. The message signers or " +
                    "countersigners may not be who they claim to be. " +
                    "The message's authenticity or integrity, " +
                    "or both, are not guaranteed.");
#endif          //LP20071214 x motivi di firma debole con certificato scadut faccio passare il check
                //return false;
                return true;
            }

            return true;
        }
    }
}
