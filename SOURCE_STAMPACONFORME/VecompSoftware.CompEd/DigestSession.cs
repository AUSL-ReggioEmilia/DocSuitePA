using CCypher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using VecompSoftware.Commons;
using VecompSoftware.Commons.BiblosInterfaces;
using VecompSoftware.Commons.CommonEx;
using VecompSoftware.Commons.Infos;

namespace VecompSoftware.CompEd
{
    public class DigestSession : IDisposable
    {
        #region [ Fields ]

        private List<int> p7kHandles = new List<int>();
        private List<int> x509Handles = new List<int>();
        private List<int> p7xHandles = new List<int>();
        private List<int> tsHandles = new List<int>();
        private List<int> padesHandles = new List<int>();
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(DigestSession));
        // Flag di disposed object
        private bool disposed = false;

        #endregion

        #region [ Constructors ]

        public DigestSession(IContent info)
        {
            Document = info;
            Instance = new Digest();
            Instance.SetParam(enumPARAM.DS_GUI, false);
            Instance.SetParam(enumPARAM.DS_RAISE_EXCEPTIONS, true);
            string pStr = null;
            Instance.GetDefaultConnectionString(out pStr);
            Instance.OpenDb(pStr);            
        }
        public DigestSession(string fullName) : this(new ContentInfo(fullName, true)) { }

        #endregion

        #region [ Properties ]

        private IContent Document { get; set; }
        public Digest Instance { get; private set; }

        #endregion

        #region [ Methods ]

        public int P7kLoadFromBuf(byte[] buf)
        {
            int handle = 0;
            try
            {
                Instance.P7kLoadFromBuf((object)buf, 1, out handle);
                p7kHandles.Add(handle);
            }
            catch (COMException ex)
            {
                logger.Error(ex);
                return 0;
            }
            return handle;
        }
        public int P7kLoadFromBuf()
        {
            return P7kLoadFromBuf(Document.Content);
        }
        public byte[] P7kContentReadToBuf(int handle)
        {
            object buf = null;
            Instance.P7kContentReadToBuf(handle, out buf, 0);
            return (byte[])buf;
        }

        public bool HasParent()
        {
            var p7k = P7kLoadFromBuf();
            P7kFree(p7k);
            if (p7k > 0)
                return true;
            var p7x = P7xLoadFromBuf();
            P7xFree(p7x);
            if (p7x > 0)
                return true;
            return false;
        }
        public ContentInfo GetParent()
        {
            byte[] buf = null;
            switch (Document.Extension)
            {
                case PathUtil.EXTENSIONP7M:
                    var p7k = P7kLoadFromBuf();
                    if (p7k.Equals(0))
                        return null;
                    buf = P7kContentReadToBuf(p7k);
                    P7kFree(p7k);
                    break;

                case PathUtil.EXTENSIONPDF:
                    return null;

                default:
                    var p7x = P7xLoadFromBuf();
                    if (p7x.Equals(0))
                        return null;
                    buf = P7xContentReadToBuf(p7x);
                    P7xFree(p7x);
                    break;
            }

            if (buf.IsNullOrEmpty())
                return null;

            string resolved = Document.FileName.ToPath().GetFileNameResolveChain();
            if (this.Document.Extension.EqualsIgnoreCase(PathUtil.EXTENSIONP7M))
            {
                var contentType = new DigitalSignViewer().GetContentTypeBuf(buf, this.Document.FileName);
                if (contentType.EqualsIgnoreCase(PathUtil.EXTENSIONP7M))
                {
                    resolved = resolved + contentType;
                    return new ContentInfo(resolved, buf);
                }
            }

            if (!resolved.ToPath().HasExtension())
            {
                var contentType = new DigitalSignViewer().GetContentTypeBuf(buf, this.Document.FileName);
                resolved = resolved.ToPath().ChangeExtension(contentType);
            }
            return new ContentInfo(resolved, buf);
        }

        public int P7kGetCountersignature(int parentHandle, int num)
        {
            int handle = 0;
            try
            {
                Instance.P7kGetCountersignature(parentHandle, num, out handle);
                p7kHandles.Add(handle);
            }
            catch (COMException ex)
            {
                logger.Error(ex);
                return 0;
            }
            return handle;
        }
        public bool P7kSignatureHasTS(int handle, int index)
        {
            return Instance.P7kSignatureHasTS(handle, index);
        }
        public int P7kSignatureGetTS(int handle, int index)
        {
            var tsHandle = Instance.P7kSignatureGetTS(handle, index);
            tsHandles.Add(tsHandle);
            return tsHandle;
        }

        public int P7kGetSignatureCount(int handle)
        {
            int count = 0;
            Instance.P7kGetSignatureCount(handle, out count);
            return count;
        }
        public object P7kGetSignAttribute(int handle, bool signed, int num, string oid)
        {
            enumASN1Type type = 0;
            object data = null;
            try
            {
                Instance.P7kGetSignAttribute(handle, Convert.ToInt32(signed), num, oid, out type, out data);
            }
            catch (COMException ex)
            {
                logger.Error(ex);
                return null;
            }
            return data;
        }
        public object P7kGetSignAttribute(int handle, int num, string oid)
        {
            return P7kGetSignAttribute(handle, true, num, oid);
        }
        public byte[] P7kGetSignerInfo(int handle, int num)
        {
            enumHashType mdAlg = 0;
            object certBuf = null;
            Instance.P7kGetSignerInfo(handle, num, out mdAlg, out certBuf);
            return (byte[])certBuf;
        }
        public void P7kSetOption(int handle, enumP7kOption option, object value)
        {
            Instance.P7kSetOption(handle, option, value);
        }
        public dynamic P7kGetOption(int handle, enumP7kOption option)
        {
            return Instance.P7kGetOption(handle, option);
        }
        public bool P7kVerifySignature(int handle, int num)
        {
            int signatureVerification = 0;
            string signatureVerificationError = null;
            bool timestampPresence = false;
            int timestampVerification = 0;
            string timestampVerificationError = null;
            int certificateVerification = 0;
            string certificateVerificationError = null;

            Instance.P7kVerifySignature(handle, num, out signatureVerification, out signatureVerificationError, out timestampPresence, out timestampVerification, out timestampVerificationError, out certificateVerification, out certificateVerificationError);
            return signatureVerification.Equals(0) && timestampVerification.Equals(0) && certificateVerification.Equals(0);
        }

        public int X509LoadFromBuf(byte[] buf)
        {
            int handle = 0;
            Instance.x509LoadFromBuf((object)buf, out handle);
            x509Handles.Add(handle);
            return handle;
        }
        public bool X509Verify(int handle)
        {
            int result = 0;
            Instance.x509Verify(handle, 1, out result);
            return Convert.ToBoolean(result);
        }
        public string X509GetInfo(int handle, enumCertInfoCategory certInfoCategory, enumCertInfoItem certInfoItem)
        {
            string result = null;
            try
            {
                Instance.x509GetInfo(handle, certInfoCategory, certInfoItem, out result);
            }
            catch (COMException ex)
            {
                logger.Error(ex);
                return null;
            }
            return result;
        }

        public bool HasP7kSignatures()
        {
            var p7k = P7kLoadFromBuf();
            if (p7k.Equals(0))
            {
                P7kFree(p7k);
                return false;
            }

            var count = P7kGetSignatureCount(p7k);
            P7kFree(p7k);
            return count > 0;
        }
        public IEnumerable<P7kSignInfo> GetP7kSignatures()
        {
            var p7k = P7kLoadFromBuf();
            if (p7k.Equals(0))
                return Enumerable.Empty<P7kSignInfo>();

            var count = P7kGetSignatureCount(p7k);
            if (count.Equals(0))
                return Enumerable.Empty<P7kSignInfo>();

            var result = new List<P7kSignInfo>();
            for (int i = 0; i < count; i++)
            {
                var buf = P7kGetSignerInfo(p7k, i);
                var x509 = X509LoadFromBuf(buf);
                var sign = new P7kSignInfo(this, p7k, x509, i);
                X509Free(x509);
                sign.Children = GetP7kChildren(p7k, i);
                result.Add(sign);
            }
            P7kFree(p7k);
            return result;
        }
        public IEnumerable<SignInfo> GetP7kChildren(int pades, int parentHandle, int num, SignInfo.SignTypes signType)
        {
            var result = new List<SignInfo>();

            if (P7kSignatureHasTS(parentHandle, num))
            {
                var ts = P7kSignatureGetTS(parentHandle, num);
                result.Add(new TSSignInfo(this, parentHandle, ts, num));
                TSFree(ts);
            }

            var p7k = P7kGetCountersignature(parentHandle, num);
            if (p7k.Equals(0))
                return result;

            var count = P7kGetSignatureCount(p7k);
            if (count.Equals(0))
            {
                P7kFree(p7k);
                return result;
            }

            for (int i = 0; i < count; i++)
            {
                var buf = P7kGetSignerInfo(p7k, i);
                var x509 = X509LoadFromBuf(buf);
                var sign = new P7kSignInfo(this, pades, p7k, x509, i, signType);
                X509Free(x509);
                sign.Children = GetP7kChildren(pades, p7k, i, signType);
                result.Add(sign);
            }
            P7kFree(p7k);

            return result;
        }
        public IEnumerable<SignInfo> GetP7kChildren(int parentHandle, int num)
        {
            return GetP7kChildren(0, parentHandle, num, SignInfo.SignTypes.CAdES);
        }

        public int P7xLoadFromBuf(byte[] buf)
        {
            int handle = 0;
            try
            {
                Instance.P7xLoadFromBuf((object)buf, out handle);
                p7xHandles.Add(handle);
            }
            catch (COMException ex)
            {
                logger.Error(ex);
                return 0;
            }
            return handle;
        }
        public int P7xLoadFromBuf()
        {
            return P7xLoadFromBuf(Document.Content);
        }
        public byte[] P7xContentReadToBuf(int p7x)
        {
            object buf = null;
            Instance.P7xContentReadToBuf(p7x, out buf, 0);
            return (byte[])buf;
        }
        public int P7xGetTSCount(int handle)
        {
            int count = 0;
            Instance.P7xGetTSCount(handle, out count);
            return count;
        }
        public int P7xGetTS(int handle, int index)
        {
            int tsHandle = 0;
            Instance.P7xGetTS(handle, index, out tsHandle);
            tsHandles.Add(tsHandle);
            return tsHandle;
        }
        public DateTime TSGetDateAndTime(int handle)
        {
            object dataAndTime = null;
            Instance.tsGetDateAndTime(handle, out dataAndTime);
            return (DateTime)dataAndTime;
        }
        public DateTime TSGetExpieryDateAndTime(int handle)
        {
            object dataAndTime = null;
            Instance.tsGetExpieryDateAndTime(handle, out dataAndTime);
            return (DateTime)dataAndTime;
        }
        public string TSGetTSPInfo(int handle, enumCertInfoCategory certInfoCategory, enumCertInfoItem certInfoItem)
        {
            string result = null;
            Instance.tsGetTSPInfo(handle, certInfoCategory, certInfoItem, out result);
            return result;
        }

        public bool P7xVerify(int handle, DateTime date)
        {
            object dateAndTime = null;
            object expieryDateAndTime = null;
            Instance.P7xVerify(handle, out dateAndTime, out expieryDateAndTime);
            return expieryDateAndTime != null && date < (DateTime)expieryDateAndTime;
        }
        public bool P7xVerify(int handle)
        {
            return P7xVerify(handle, DateTime.Now);
        }
        public bool TSVerify(int handle, DateTime date)
        {
            object dataAndTime = null;
            Instance.tsGetExpieryDateAndTime(handle, out dataAndTime);
            return date < (DateTime)dataAndTime;
        }
        public bool TSVerify(int handle)
        {
            return TSVerify(handle, DateTime.Now);
        }

        public bool HasP7xSignatures()
        {
            var p7x = P7xLoadFromBuf();
            if (p7x.Equals(0))
            {
                P7xFree(p7x);
                return false;
            }

            var count = P7xGetTSCount(p7x);
            P7xFree(p7x);
            return count > 0;
        }
        public IEnumerable<TSSignInfo> GetP7xSignatures()
        {
            var p7x = P7xLoadFromBuf();
            if (p7x.Equals(0))
                return Enumerable.Empty<TSSignInfo>();

            var count = P7xGetTSCount(p7x);
            if (count.Equals(0))
                return Enumerable.Empty<TSSignInfo>();

            var result = new List<TSSignInfo>();
            for (int i = 0; i < count; i++)
            {
                var ts = P7xGetTS(p7x, i);
                result.Add(new TSSignInfo(this, p7x, ts, i));
                TSFree(ts);
            }
            P7xFree(p7x);
            return result;
        }

        public int PADES_OpenFromBuf(byte[] buf)
        {
            int handle = 0;
            try
            {
                Instance.PADES_OpenFromBuf((object)buf, out handle);
                padesHandles.Add(handle);
                return handle;
            }
            catch (COMException ex)
            {
                logger.Error(ex);
                return 0;
            }
        }
        public int PADES_OpenFromBuf()
        {
            return PADES_OpenFromBuf(Document.Content);
        }
        public int PADES_GetSignatureCount(int handle)
        {
            try
            {
                return Instance.PADES_GetSignatureCount(handle);
            }
            catch (COMException ex)
            {
                logger.Error(ex);
                return 0;
            }
        }
        public int PADES_GetSignatureObject(int handle, int index)
        {
            int p7kHandle = 0;
            Instance.PADES_GetSignatureObject(handle, index, out p7kHandle);
            return p7kHandle;
        }
        public bool PADES_Verify(int handle)
        {
            try
            {
                Instance.PADES_Verify(handle);
                return true;
            }
            catch { }
            return false;
        }

        public int GetLastError()
        {
            enumErrorClass errClass = enumErrorClass.ERR_NoError;
            int errCode = 0;
            string pErrDescription = null;
            Instance.GetLastError(out errClass, out errCode, out pErrDescription);
            return errCode;
        }

        public bool HasPADESSignatures()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            List<Task<bool>> tasks = new List<Task<bool>>();
            tasks.Add(new Task<bool>(() => ActionWaitHasPADESSignatures(cts.Token), cts.Token));
            tasks.Add(new Task<bool>(() => ActionHasPADESSignatures(cts), cts.Token));
            tasks.ForEach(t => t.Start());
            
            int taskCompleteId = Task.WaitAny(tasks.ToArray(), cts.Token);
            Task<bool> taskComplete = tasks.ElementAt(taskCompleteId);

            if(cts.Token.CanBeCanceled)
                cts.Cancel(false);   

            return taskComplete.Result;
        }
        public IEnumerable<P7kSignInfo> GetPADESSignatures()
        {
            var pades = PADES_OpenFromBuf();
            if (pades.Equals(0))
                return Enumerable.Empty<P7kSignInfo>();

            var iPADESCount = PADES_GetSignatureCount(pades);
            if (iPADESCount.Equals(0))
            {
                PADES_Free(pades);
                return Enumerable.Empty<P7kSignInfo>();
            }

            var result = new List<P7kSignInfo>();
            for (int iPADES = 0; iPADES < iPADESCount; iPADES++)
            {
                var p7k = PADES_GetSignatureObject(pades, iPADES);
                var p7kCount = P7kGetSignatureCount(p7k);
                for (int iP7k = 0; iP7k < p7kCount; iP7k++)
                {
                    var buf = P7kGetSignerInfo(p7k, iP7k);
                    var x509 = X509LoadFromBuf(buf);
                    var sign = new P7kSignInfo(this, pades, p7k, x509, iP7k, SignInfo.SignTypes.PAdES);
                    X509Free(x509);
                    sign.Children = GetP7kChildren(pades, p7k, iP7k, SignInfo.SignTypes.PAdES);
                    result.Add(sign);
                }
                P7kFree(p7k);
            }
            PADES_Free(pades);
            return result;
        }

        public DigestSession P7kFree(int handle)
        {
            try
            {
                Instance.P7kFree(handle);
            }
            catch (COMException ex)
            {
                logger.Error(ex);
            }
            p7kHandles.Remove(handle);
            return this;
        }
        public DigestSession X509Free(int handle)
        {
            try
            {
                Instance.x509Free(handle);
            }
            catch (COMException ex)
            {
                logger.Error(ex);
            }
            x509Handles.Remove(handle);
            return this;
        }
        public DigestSession P7xFree(int handle)
        {
            try
            {
                Instance.P7xFree(handle);
            }
            catch (COMException ex)
            {
                logger.Error(ex);
            }
            p7xHandles.Remove(handle);
            return this;
        }
        public DigestSession TSFree(int handle)
        {
            try
            {
                Instance.tsFree(handle);
            }
            catch (COMException ex)
            {
                logger.Error(ex);
            }
            tsHandles.Remove(handle);
            return this;
        }
        public DigestSession PADES_Free(int handle)
        {
            try
            {
                Instance.PADES_Free(handle);
            }
            catch (COMException ex)
            {
                logger.Error(ex);
            }
            padesHandles.Remove(handle);
            return this;
        }

        /// <summary>
        /// Implementazione pubblica del pattern Dispose chiamato dall'utente
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implementazione protetta del pattern Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                this.tsHandles.ForEach(h => TSFree(h));
                this.x509Handles.ForEach(h => X509Free(h));
                this.p7kHandles.ForEach(h => P7kFree(h));
                this.padesHandles.ForEach(h => PADES_Free(h));
                this.p7xHandles.ForEach(h => P7xFree(h));
                this.Document = null;
            }

            // Free any unmanaged objects here.
            Instance.CloseDb();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            
            /* Reference:
             * http://stackoverflow.com/questions/1827059/why-use-finalreleasecomobject-instead-of-releasecomobject
             */
            if (Marshal.FinalReleaseComObject(Instance) != 0 || Marshal.ReleaseComObject(Instance) != 0)
            {
                logger.Error("DigesSessione Instance is not release correctly!");                
            }
            
            Instance = null;
            disposed = true;
        }

        private bool ActionHasPADESSignatures(CancellationTokenSource token)
        {
            int pades = PADES_OpenFromBuf();
            if (pades.Equals(0))
            {
                PADES_Free(pades);
                return false;
            }

            int count = PADES_GetSignatureCount(pades);
            PADES_Free(pades);

            return count > 0;
        }

        private bool ActionWaitHasPADESSignatures(CancellationToken token)
        {
            Thread.Sleep(3000);
            return false;
        }
        #endregion

    }
}
