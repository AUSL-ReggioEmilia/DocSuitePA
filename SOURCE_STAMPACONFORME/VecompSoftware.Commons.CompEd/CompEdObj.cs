using System;
using System.Security.Cryptography;
using VecompSoftware.Commons.BiblosDS.Objects.Enums;

namespace VecompSoftware.Commons.CompEd
{
    public class CompEdObj : IDisposable
    {
        // Oggetto Digital Sign
        CCypher.Digest oDigest;

        //XmlDocument xmlDoc;
        public CompEdObj()
        {
            try
            {
                oDigest = new CCypher.Digest();
                string db;
                oDigest.GetDefaultConnectionString(out db);
                oDigest.OpenDb(db);
            }
            catch (System.Exception exp)
            {
                throw new Exception(exp.Message + " " + exp.StackTrace);
            }
        }
        public bool Logon()
        {
            oDigest.Logon(1);
            return true;
        }
        public bool Logoff()
        {
            oDigest.Logon(0);
            return true;
        }

        public static byte[] CalculateBlobHash(byte[] Obj, ComputeHashType computeHash)
        {
            var docHash = new byte[0];
            if (computeHash == ComputeHashType.SHA256)
            {
                var sha = new SHA256CryptoServiceProvider();
                docHash = sha.ComputeHash(Obj);
            }
            else
            {
                var sha = new SHA1CryptoServiceProvider();
                docHash = sha.ComputeHash(Obj);
            }
            return docHash;
        }

        public int DigestInit(byte[] Sha1Hash)
        {
            int retHndl;
            oDigest.DigestInit(CCypher.enumHashType.HTC_SHA1, (object)Sha1Hash, out retHndl);
            return retHndl;
        }

        public void GetSmartCardCertificate(out int CertId, out int KeyId)
        {
            oDigest.GetSmartCardCertificate("TestApp", CCypher.enumKeyType.KTC_SIGN_AUTO, out CertId, out KeyId);
        }

        public void DigestGetSignedValue(int Handle, int CertId, out object digestData)
        {
            oDigest.DigestGetSignedValue(Handle, CertId, out digestData);
        }

        public void AddRawSignature(int Handle, object signed_digest, object signer_cert)
        {
            oDigest.P7kAddRawSignature(Handle, CCypher.enumHashType.HTC_SHA1, signed_digest, signer_cert);
        }

        public void P7mNew(out int Handle)
        {
            oDigest.P7kNew(out Handle);
        }

        public void P7mAddBlob(int Handle, Byte[] blob)
        {
            string fname = System.IO.Path.GetTempFileName();
            System.IO.File.WriteAllBytes(fname, blob);
            oDigest.P7kContentWriteFromFile(Handle, fname);
        }

        public void P7mAddTimeFileDescription(int Handle, string time, string filename, string description)
        {
            oDigest.P7kSetDescription(Handle, filename, description);
            oDigest.P7kSetSignAttribute(Handle, 0, "signingTime", CCypher.enumASN1Type.asn1_utctime, DateTime.Now.ToUniversalTime());
        }

        public void P7mContentInit(int Handle)
        {
            oDigest.P7kContentInit(Handle);
        }

        public void P7mSetType(int Handle)
        {
            oDigest.P7kSetType(Handle, CCypher.enumP7kType.P7kSigned);
        }

        public void P7mContentFinish(int Handle)
        {
            oDigest.P7kContentFinish(Handle);
        }

        public void P7kSaveToFile(int Handle, string OutputFile)
        {
            oDigest.P7kSaveToFile(Handle, OutputFile);
        }

        public byte[] P7kContentReadToBuf(int Handle)
        {
            object buf;
            oDigest.P7kSaveToBuf(Handle, out buf);
            return (byte[])buf;
        }

        public void VerifyLastError()
        {
            CCypher.enumErrorClass errCls;
            int errCod;
            string errDes;
            oDigest.GetLastError(out errCls, out errCod, out errDes);
            if (errCls > 0)
                throw new Exception("CompEd err: " + errCls.ToString() + "," + errCod.ToString() + "," + errDes);
        }

        public byte[] ExportCertificateBuf(int Handle)
        {
            object buf;
            oDigest.ExportCertificateBuf(Handle, out buf);
            return (byte[])buf;
        }

        public static string ByteToHex(byte[] buf)
        {
            string hexValue = "";
            for (int i = 0; i < buf.Length; i++)
                hexValue += buf[i].ToString("X2");
            return hexValue;
        }

        public string GetLastError()
        {
            CCypher.enumErrorClass errCls;
            int errCod;
            string errDes;
            oDigest.GetLastError(out errCls, out errCod, out errDes);
            return errCls.ToString() + "," + errCod.ToString() + "," + errDes;
        }

        public void GetFileInfo(bool isP7M, byte[] blob, out string Metadata)
        {
            int p7H;
            string cName = "", descr = "";

            if (isP7M)
            {
                oDigest.P7kLoadFromBuf((object)blob, 1, out p7H);
                oDigest.P7kGetDescription(p7H, out cName, out descr);
                //        object digestData;
                //        oDigest.DigestGetValue(p7H,out digestData);
                oDigest.P7kFree(p7H);
            }
            else
            {
                oDigest.P7xLoadFromBuf((object)blob, out p7H);
                oDigest.P7kGetDescription(p7H, out cName, out descr);
                oDigest.P7xFree(p7H);
            }

            CCypher.enumErrorClass errC;
            int ErrI;
            string ErrS;
            oDigest.GetLastError(out errC, out ErrI, out ErrS);

            Metadata = "<?xml version=\"1.0\" ?><file name=\"" + cName + "\" description=\"" + descr + "\"";
        }

        #region IDisposable Members

        ~CompEdObj()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (oDigest != null)
                    oDigest.CloseDb();
            }
        }
        public void Close()
        {
            Dispose();
        }


        #endregion

    }
}
