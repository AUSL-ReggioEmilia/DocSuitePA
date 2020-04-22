using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;
using System.Web;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using BiblosDS.Library.Common.CompEd.Services;
using System.Configuration;

namespace BiblosDS.WCF.DigitalSign
{
    /// <summary>
    /// Classe di Wrapper DigitalSign Comped
    /// Contiene i metodi per sbustare i file PAdES e CAdES
    /// Contiene i metodi per avere per visualizzare in formato XML 
    /// le informazioni relative la firma del documento
    /// </summary>

    public struct XML_TOKEN
    {
        // tag
        public const string TOK_XML_TAG_DOCUMENT = "Document";
        public const string TOK_XML_TAG_SIGNATURE = "Signature";
        public const string TOK_XML_TAG_ISSUER = "Issuer";
        public const string TOK_XML_TAG_SUBJECT = "Subject";
        public const string TOK_XML_TAG_DETAILS = "Details";
        // attributi
        public const string TOK_XML_TAG_ATT_SIGNATURE = "Signature";
        public const string TOK_XML_TAG_DOCUMENT_ATT_NSIGNATURE = "NumberSignature";
        public const string TOK_XML_TAG_ISSUER_ATT_OU = "OU";
        public const string TOK_XML_TAG_ISSUER_ATT_O = "O";
        public const string TOK_XML_TAG_ISSUER_ATT_C = "C";
        public const string TOK_XML_TAG_ISSUER_ATT_CN = "CN";
        public const string TOK_XML_TAG_SUBJECT_ATT_O = "O";
        public const string TOK_XML_TAG_SUBJECT_ATT_C = "C";
        public const string TOK_XML_TAG_SUBJECT_ATT_CN = "CN";
        public const string TOK_XML_TAG_SUBJECT_ATT_D = "D";
        public const string TOK_XML_TAG_SUBJECT_ATT_N = "N";
        public const string TOK_XML_TAG_SUBJECT_ATT_CO = "CO";
        public const string TOK_XML_TAG_SUBJECT_ATT_FC = "FC";
        public const string TOK_XML_TAG_SUBJECT_ATT_NAME = "NAME";
        public const string TOK_XML_TAG_DETAILS_ATT_SN = "SN";
        public const string TOK_XML_TAG_DETAILS_ATT_LEASE = "Lease";
        public const string TOK_XML_TAG_DETAILS_ATT_EXPIRE = "Expire";
    };

    public struct SimplyCert
    {
        public int Level;

        public string Type,
                    Name,
                    FiscalCode,
                    Role,
                    Description,
                    Issuer,
                    eMail,
                    SerialNumber, 
                    Version,
                    Id;

        public DateTime Expiry,
                    ValidFrom;

        public string HeaderInfo { get; set; }
    }

    public class CompEdLib : IDisposable
    {
        static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(CompEdLib));
        enum CS
        {
            ISSUER,
            SUBJECT,
            DESCRIPTION,
            SERIAL_NUMBER,
            NOT_BEFORE,
            NOT_AFTER,
            FISCAL_CODE
        }

        // CertType
        const string TOK_TypeSign = "Signature";
        const string TOK_TypeTS = "TimeStamp";

        const string TOK_SIGNATURE_FILE = "TEMPLATE.XML";
        // XML
        // tag
        const string TOK_XML_TAG_HEAD = "<?xml version=\"1.0\"?>";
        // enti
        const string TOK_XML_TAG_OUT_ISSUER_O = "%I_O%";
        const string TOK_XML_TAG_OUT_ISSUER_C = "%I_C%";
        const string TOK_XML_TAG_OUT_ISSUER_OU = "%I_OU%";
        const string TOK_XML_TAG_OUT_ISSUER_CN = "%I_CN%";
        const string TOK_XML_TAG_OUT_SUBJECT_O = "%S_O%";
        const string TOK_XML_TAG_OUT_SUBJECT_C = "%S_C%";
        const string TOK_XML_TAG_OUT_SUBJECT_CN = "%S_CN%";
        const string TOK_XML_TAG_OUT_SUBJECT_CO = "%S_CO%";
        const string TOK_XML_TAG_OUT_SUBJECT_N = "%S_N%";
        const string TOK_XML_TAG_OUT_SUBJECT_D = "%S_D%";
        const string TOK_XML_TAG_OUT_SUBJECT_FC = "%S_FC%";
        const string TOK_XML_TAG_OUT_SUBJECT_NAME = "%S_NAME%";
        const string TOK_XML_TAG_OUT_DETAILS_SN = "%S_SN%";
        const string TOK_XML_TAG_OUT_DETAILS_LEASE = "%S_Lease%";
        const string TOK_XML_TAG_OUT_DETAILS_EXPIRE = "%S_Expire%";
        const string TOK_XML_TAG_OUT_GENERAL_SIGNATURE = "%Signature%";
        const string TOK_XML_TAG_OUT_GENERAL_NSIGNATURE = "%NumberSignature%";

        const string TOK_XML_TAG_OUT_DOCUMENT = "Document";
        const string TOK_XML_TAG_OUT_TITLE = "Title";
        const string TOK_XML_TAG_OUT_SUBTITLE = "SubTitle";
        const string TOK_XML_TAG_OUT_ROW = "Row";
        const string TOK_XML_TAG_OUT_SIGNATURE = "Signature";
        const string TOK_XML_TAG_OUT_SIGNATURE_NESTED = "NestedSignature";
        const string TOK_XML_TAG_OUT_FONT = "Font";
        const string TOK_XML_TAG_OUT_FONT_FACE = "Face";
        const string TOK_XML_TAG_OUT_FONT_SIZE = "Size";
        const string TOK_XML_TAG_OUT_FONT_ITALIC = "Italic";
        const string TOK_XML_TAG_OUT_FONT_BOLD = "Bold";
        const string TOK_XML_TAG_OUT_FONT_STRIKEOUT = "Strikeout";
        const string TOK_XML_TAG_OUT_FONT_UNDERLINE = "Underline";
        const string TOK_XML_TAG_OUT_XPOS = "x";
        const string TOK_XML_TAG_OUT_YPOS = "y";

        // informazioni di ritorno da DigitalSign
        const string TOK_SIG_ISSUER = "Issuer:";
        const string TOK_SIG_ISSUER_ATT_OU = "OU";
        const string TOK_SIG_ISSUER_ATT_O = "O";
        const string TOK_SIG_ISSUER_ATT_C = "C";
        const string TOK_SIG_ISSUER_ATT_CN = "CN";
        const string TOK_SIG_SERIAL_NUMBER = "Serial Number:";
        const string TOK_SIG_SUBJECT = "Subject:";
        const string TOK_SIG_SUBJECT_ATT_O = "O";
        const string TOK_SIG_SUBJECT_ATT_C = "C";
        const string TOK_SIG_SUBJECT_ATT_CN = "CN";
        const string TOK_SIG_SUBJECT_ATT_D = "D";
        const string TOK_SIG_SUBJECT_ATT_N = "N";
        const string TOK_SIG_DESCRIPTION_ATT_N = "N";
        const string TOK_SIG_DESCRIPTION_ATT_C = "C";
        const string TOK_SIG_DESCRIPTION_ATT_D = "D";
        const string TOK_SIG_NOT_BEFORE = "Not Before:";
        const string TOK_SIG_NOT_AFTER = "Not After :";

        // Oggetto Digital Sign
        CCypher.Digest oDigest;

        //XmlDocument xmlDoc;
        public CompEdLib()
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

        /// <summary>
        /// ritorna il numero di firme contenute nel documento
        /// </summary>
        /// <param name="signedFile"></param>
        /// <returns></returns>
        /// <remarks>supporta PAdES</remarks>
        public int GetSignatureCount(byte[] signedFile)
        {
            int nHandle = 0, nCountSignature = 0;
            
            //Controlla in primis se il documento è PADES.
            try
            {
                oDigest.PADES_OpenFromBuf(signedFile, out nHandle);
                if (nHandle != 0)
                {
                    nCountSignature = oDigest.PADES_GetSignatureCount(nHandle);
                    oDigest.PADES_Free(nHandle);
                }
            }
            catch { }
            finally
            {
                try
                {
                    if (nHandle != 0)
                        oDigest.PADES_Free(nHandle);
                }
                catch { } 
            }

            // Controlla le firme CADES 
            try
            {
                oDigest.P7kLoadFromBuf(signedFile, 1, out nHandle);
                if (nHandle != 0)
                {
                    int nCount;
                    oDigest.P7kGetSignatureCount(nHandle, out nCount);
                    nCountSignature += nCount;
                }
            }
            catch { }
            finally
            {
                try
                {
                    oDigest.P7kFree(nHandle);
                }
                catch { } 
            }
            
            return nCountSignature;
        }

        /// <summary>
        /// torna 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="p7m"></param>
        /// <param name="firstExp">il primo certificato che scade</param>
        /// <param name="SimCrtLst">elenco dei certificati di firma</param>
        /// <returns></returns>
        /// <remarks>supporta PAdES</remarks>
        public String GetExpiryDate(string fileName, byte[] p7m, out SimplyCert firstExp, out SimplyCert[] SimCrtLst)
        {
            firstExp = new SimplyCert();
            Hashtable HSimCrtLst = new Hashtable();
            string result = RecGetAllExpiryDate(1, p7m, ref firstExp, ref HSimCrtLst);
            SimCrtLst = new SimplyCert[HSimCrtLst.Count]; 
            HSimCrtLst.Values.CopyTo(SimCrtLst, 0);

            return result; 
        }

        public String GetAllExpiryDate(byte[] p7m, out SimplyCert firstExp, out Hashtable SimCrtLst)
        {
            firstExp = new SimplyCert();
            SimCrtLst = new Hashtable();
            return RecGetAllExpiryDate(1, p7m, ref firstExp, ref SimCrtLst);
        }

        /// <summary>
        /// Funzione interna ricorsiva che ritorna la hashtable dei certificati del modulo 
        /// </summary>
        int ele = 1;
        public String RecGetAllExpiryDate(int lev, byte[] p7m, ref SimplyCert firstExp, ref Hashtable SimCrtLst)
        {
            string LastErr = "";

            try
            {
                int nHandle = 0, nCountSignature = 0, nCountTimeStamp = 0 , nCountSignaturePDF = 0;
                int nHandlePDF = 0, nP7KHandle = 0 ; 

                oDigest.PADES_OpenFromBuf(p7m, out nHandlePDF);
                if (nHandlePDF > 0)
                {
                    nCountSignaturePDF = oDigest.PADES_GetSignatureCount(nHandlePDF); 
                    for (int j = 0; j < nCountSignature; j++)
                    {
                        int nHandleX509 = 0;
                        Object vtCertBuf = null;

                        oDigest.PADES_GetSignatureObject(nHandlePDF, j, out nP7KHandle);

                        CCypher.enumHashType nEnumHashType = CCypher.enumHashType.HTC_SHA1;
                        CCypher.enumASN1Type cct;
                        object obj;
                        oDigest.P7kGetSignAttribute(nP7KHandle, 0, j, "unstructuredName", out cct, out obj);
                        string tmpHeader = obj as string;
                        oDigest.P7kGetSignerInfo(nP7KHandle, j, out nEnumHashType, out vtCertBuf);
                        oDigest.x509LoadFromBuf(vtCertBuf, out nHandleX509);

                        string sRetInfo = "", Name = "", fc = "", des = "", role = "", iss = "", org = "", email = "", serial = "" , vers = "", id = "";
                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_GENERAL, CCypher.enumCertInfoItem.CI_VALID_TO, out sRetInfo);
                        DateTime dt = new DateTime(int.Parse(sRetInfo.Substring(6, 4)), int.Parse(sRetInfo.Substring(3, 2)), int.Parse(sRetInfo.Substring(0, 2)));
                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_GENERAL, CCypher.enumCertInfoItem.CI_VALID_FROM, out sRetInfo);
                        DateTime dtvf = new DateTime(int.Parse(sRetInfo.Substring(6, 4)), int.Parse(sRetInfo.Substring(3, 2)), int.Parse(sRetInfo.Substring(0, 2)));
                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_SUBJECT, CCypher.enumCertInfoItem.CI_NAME, out Name);
                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_SUBJECT, CCypher.enumCertInfoItem.CI_FISCALCODE, out fc);
                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_ISSUER, CCypher.enumCertInfoItem.CI_NAME, out iss);
                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_ALL, CCypher.enumCertInfoItem.CI_DESCRIPTION, out des);
                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_SUBJECT, CCypher.enumCertInfoItem.CI_ROLE, out role);
                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_SUBJECT, CCypher.enumCertInfoItem.CI_EMAIL, out email);
                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_GENERAL, CCypher.enumCertInfoItem.CI_SERIALNUMBER, out serial);
                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_GENERAL, CCypher.enumCertInfoItem.CI_VERSION, out vers);
                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_GENERAL, CCypher.enumCertInfoItem.CI_ID, out id);

                        if ((DateTime.Compare(firstExp.Expiry, dt) > 0 || j == 0) && firstExp.Type != TOK_TypeTS)
                        {
                            firstExp.Level = lev;
                            firstExp.Type = TOK_TypeSign;
                            firstExp.Name = Name;
                            firstExp.Expiry = dt;
                            firstExp.FiscalCode = fc;
                            firstExp.Role = role;
                            firstExp.Description = des;
                            firstExp.Issuer = iss;
                            firstExp.eMail = email;
                            firstExp.ValidFrom = dtvf;
                            firstExp.HeaderInfo = tmpHeader;
                            firstExp.SerialNumber = serial;
                            firstExp.Version = vers;
                            firstExp.Id = id; 
                        }

                        SimplyCert sc = new SimplyCert();
                        sc.Level = lev;
                        sc.Type = TOK_TypeSign;
                        sc.Name = Name;
                        sc.Expiry = dt;
                        sc.FiscalCode = fc;
                        sc.ValidFrom = dtvf;
                        sc.Description = des;
                        sc.Issuer = iss;
                        sc.Role = role;
                        sc.eMail = email;
                        sc.HeaderInfo = tmpHeader;
                        sc.SerialNumber = serial;
                        sc.Version = vers;
                        sc.Id = id; 
                        SimCrtLst.Add((ele++).ToString(), (object)sc);
                            
                        oDigest.x509Free(nHandleX509);
                        oDigest.P7kFree(nP7KHandle) ;

                        // nei PDF non c'è ricorsione sui contenuti
                    }

                    if (nHandlePDF != 0) 
                        oDigest.PADES_Free(nHandlePDF) ; 
                }

                // Inizio CAdES
                oDigest.P7kLoadFromBuf(p7m, 1, out nHandle);
                if (nHandle > 0)
                {
                    oDigest.P7kGetSignatureCount(nHandle, out nCountSignature);

                    for (int j = 0; j < nCountSignature; j++)
                    {
                        int nHandleX509 = 0;
                        Object vtCertBuf = null;
                        CCypher.enumHashType nEnumHashType = CCypher.enumHashType.HTC_SHA1;
                        CCypher.enumASN1Type cct;
                        object obj;
                        oDigest.P7kGetSignAttribute(nHandle, 0, j, "unstructuredName", out cct, out obj);
                        string tmpHeader = obj as string;
                        oDigest.P7kGetSignerInfo(nHandle, j, out nEnumHashType, out vtCertBuf);
                        oDigest.x509LoadFromBuf(vtCertBuf, out nHandleX509);

                        string sRetInfo = "", Name = "", fc = "", des = "", role = "", iss = "", org = "", email = "", serial = "" , vers = "", id = "";
                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_GENERAL, CCypher.enumCertInfoItem.CI_VALID_TO, out sRetInfo);
                        DateTime dt = new DateTime(int.Parse(sRetInfo.Substring(6, 4)), int.Parse(sRetInfo.Substring(3, 2)), int.Parse(sRetInfo.Substring(0, 2)));
                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_GENERAL, CCypher.enumCertInfoItem.CI_VALID_FROM, out sRetInfo);
                        DateTime dtvf = new DateTime(int.Parse(sRetInfo.Substring(6, 4)), int.Parse(sRetInfo.Substring(3, 2)), int.Parse(sRetInfo.Substring(0, 2)));
                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_SUBJECT, CCypher.enumCertInfoItem.CI_NAME, out Name);
                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_SUBJECT, CCypher.enumCertInfoItem.CI_FISCALCODE, out fc);
                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_ISSUER, CCypher.enumCertInfoItem.CI_NAME, out iss);
                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_ALL, CCypher.enumCertInfoItem.CI_DESCRIPTION, out des);
                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_SUBJECT, CCypher.enumCertInfoItem.CI_ROLE, out role);
                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_SUBJECT, CCypher.enumCertInfoItem.CI_EMAIL, out email);
                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_GENERAL, CCypher.enumCertInfoItem.CI_SERIALNUMBER, out serial);
                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_GENERAL, CCypher.enumCertInfoItem.CI_VERSION, out vers);
                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_GENERAL, CCypher.enumCertInfoItem.CI_ID, out id);

                        if ((DateTime.Compare(firstExp.Expiry, dt) > 0 || j == 0) && firstExp.Type != TOK_TypeTS)
                        {
                            firstExp.Level = lev;
                            firstExp.Type = TOK_TypeSign;
                            firstExp.Name = Name;
                            firstExp.Expiry = dt;
                            firstExp.FiscalCode = fc;
                            firstExp.Role = role;
                            firstExp.Description = des;
                            firstExp.Issuer = iss;
                            firstExp.eMail = email;
                            firstExp.ValidFrom = dtvf;
                            firstExp.HeaderInfo = tmpHeader;
                            firstExp.SerialNumber = serial;
                            firstExp.Version = vers;
                            firstExp.Id = id; 
                        }

                        SimplyCert sc = new SimplyCert();
                        sc.Level = lev;
                        sc.Type = TOK_TypeSign;
                        sc.Name = Name;
                        sc.Expiry = dt;
                        sc.FiscalCode = fc;
                        sc.ValidFrom = dtvf;
                        sc.Description = des;
                        sc.Issuer = iss;
                        sc.Role = role;
                        sc.eMail = email;
                        sc.HeaderInfo = tmpHeader;
                        sc.SerialNumber = serial;
                        sc.Version = vers;
                        sc.Id = id; 

                        SimCrtLst.Add((ele++).ToString(), (object)sc);
                        oDigest.x509Free(nHandleX509);
                    }
                    object OutBuf;
                    oDigest.P7kContentReadToBuf(nHandle, out OutBuf, 0);
                    oDigest.P7kFree(nHandle);
                    RecGetAllExpiryDate(lev + 1, (byte[])OutBuf, ref firstExp, ref SimCrtLst);
                }

                // marche temporali
                oDigest.P7xLoadFromBuf(p7m, out nHandle);
                if (nHandle > 0)
                {
                    oDigest.P7xGetTSCount(nHandle, out nCountTimeStamp);
                    //SimCrtLst=new SimplyCert[nCountTimeStamp];

                    for (int j = 0; j < nCountTimeStamp && LastErr.Length == 0; j++)
                    {
                        int nHandleTS = 0;

                        oDigest.P7xGetTS(nHandle, j, out nHandleTS);
                        if (nHandleTS == 0)
                        {
                            int ErrCode;
                            CCypher.enumErrorClass ErrClass;
                            oDigest.GetLastError(out ErrClass, out ErrCode, out LastErr);
                        }
                        else
                        {
                            object DaT;
                            oDigest.tsGetDateAndTime(nHandleTS, out DaT);
                            DateTime dt = (DateTime)DaT;
                            oDigest.tsGetExpieryDateAndTime(nHandleTS, out DaT);
                            DateTime dte = (DateTime)DaT;

                            if ((DateTime.Compare(firstExp.Expiry, dte) > 0 || j == 0) && firstExp.Type != TOK_TypeTS)
                            {
                                firstExp.Level = lev;
                                firstExp.Type = TOK_TypeTS;
                                firstExp.Name = (j + 1).ToString();
                                firstExp.Expiry = dte;
                            }

                            SimplyCert sc = new SimplyCert();
                            sc.Level = lev;
                            sc.Type = TOK_TypeTS;
                            oDigest.tsGetTSPInfo(nHandleTS, CCypher.enumCertInfoCategory.CC_SUBJECT, CCypher.enumCertInfoItem.CI_NAME, out sc.Name);
                            oDigest.tsGetTSPInfo(nHandleTS, CCypher.enumCertInfoCategory.CC_ISSUER, CCypher.enumCertInfoItem.CI_NAME, out sc.Issuer);
                            oDigest.tsGetTSPInfo(nHandleTS, CCypher.enumCertInfoCategory.CC_ALL, CCypher.enumCertInfoItem.CI_NAME, out sc.Description);
                            //sc.Name=(j+1).ToString();
                            sc.Expiry = dte;
                            sc.ValidFrom = dt;
                            SimCrtLst.Add((ele++).ToString(), (object)sc);

                            oDigest.tsFree(nHandleTS);
                        }
                    }
                    object OutBuf;
                    oDigest.P7xContentReadToBuf(nHandle, out OutBuf, 0);
                    oDigest.P7xFree(nHandle);
                    RecGetAllExpiryDate(lev + 1, (byte[])OutBuf, ref firstExp, ref SimCrtLst);
                }
            }
            catch (Exception e)
            {
                LastErr = e.Message;
            }
            return LastErr;
        }        

        public byte[] GetContent(bool isP7M, byte[] blob, out string Metadata)
        {
            int p7H;
            string fName = Path.GetTempFileName();

            FileStream fs = new FileStream(fName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(blob, 0, blob.Length);
            fs.Close();

            string cName = "", descr = "";
            object OutBuf;

            if (isP7M)
            {
                oDigest.P7kLoadFromFile(fName, 1, out p7H);
                oDigest.P7kContentReadToBuf(p7H, out OutBuf, 0);
                oDigest.P7kGetDescription(p7H, out cName, out descr);
                //oDigest.P7k
                oDigest.P7kFree(p7H);
            }
            else
            {
                oDigest.P7xLoadFromFile(fName, out p7H);
                oDigest.P7xContentReadToBuf(p7H, out OutBuf, 0);
                oDigest.P7xFree(p7H);
                File.Delete(fName);
            }

            CCypher.enumErrorClass errC;
            int ErrI;
            string ErrS;
            oDigest.GetLastError(out errC, out ErrI, out ErrS);

            Metadata = "<?xml version=\"1.0\" ?><file name=\"" + cName + "\" description=\"" + descr + "\"></file>";

            File.Delete(fName);
            return (byte[])OutBuf;
        }

        public byte[] P7xTimeStampDocument(bool InfoCamereFormat, string FileName, byte[] blobIn)
        {
            string ErrS;
            int ceHndl, ErrI, idTsProv = 0;
            CCypher.enumErrorClass errC;

            oDigest.P7xNew(out ceHndl);
            oDigest.P7xContentWriteFromBuf(ceHndl, (object)blobIn, FileName);
            oDigest.GetLastError(out errC, out ErrI, out ErrS);
            if (ErrS.Length > 0)
                throw new Exception(ErrS);

            oDigest.P7xSetFormat(ceHndl,
                                 InfoCamereFormat ? CCypher.enumP7xFormatType.p7xDike : CCypher.enumP7xFormatType.p7xDigitalSign,
                                 false, false);
            oDigest.GetLastError(out errC, out ErrI, out ErrS);
            if (ErrS.Length > 0)
                throw new Exception(ErrS);

            oDigest.P7xAddTS(ceHndl, idTsProv);
            oDigest.GetLastError(out errC, out ErrI, out ErrS);
            if (ErrS.Length > 0)
                throw new Exception(ErrS);

            object OutBuf;// = new object();
            oDigest.P7xSaveToBuf(ceHndl, out OutBuf);
            oDigest.GetLastError(out errC, out ErrI, out ErrS);
            if (ErrS.Length > 0)
                throw new Exception(ErrS);

            return ((byte[])OutBuf);
        }

        static public string GetTimeStampCount(string service, string user, string password)
        {
            logger.DebugFormat("GetTimeStampCount {0}", service);
            string ret = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<TimeStampCount>",
                   err = "",
                   parameters = "";

            string sUrl = "";
            switch (service.ToUpper())
            {
                case "INFOCAMERE":
                    sUrl = ConfigurationManager.AppSettings["UrlmarcheInfoCamere"];
                    // parameters: name1=value1&name2=value2	
                    parameters = "fTIPO=CONTA-MARCA&fUSER=" + user + "&fPSW=" + password;
                    break;
                default:
                    err = "<error>servizio sconosciuto</error>";
                    break;
            }
            WebRequest webRequest = null;

            if (err.Length == 0)
            {
                webRequest = WebRequest.Create(sUrl);
                if (ConfigurationManager.AppSettings["EnableProxy"] != null && ConfigurationManager.AppSettings["EnableProxy"].ToString().Equals("true", StringComparison.CurrentCultureIgnoreCase))
                {
                    logger.DebugFormat("GetTimeStampCount ProxyUrl: {0}", ConfigurationManager.AppSettings["ProxyUrl"].ToString());
                    WebProxy proxy = new WebProxy(ConfigurationManager.AppSettings["ProxyUrl"].ToString());
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ProxyUsername"].ToString()))
                        proxy.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["ProxyUsername"].ToString(), ConfigurationManager.AppSettings["ProxyPassword"].ToString(), ConfigurationManager.AppSettings["ProxyDomain"].ToString());
                    else
                        proxy.Credentials = CredentialCache.DefaultCredentials;
                    webRequest.Proxy = proxy;
                }
                
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Method = "POST";
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(parameters);
                Stream os = null;
                try
                { // send the Post
                    webRequest.ContentLength = bytes.Length;   //Count bytes to send
                    os = webRequest.GetRequestStream();
                    os.Write(bytes, 0, bytes.Length);         //Send it
                }
                catch (WebException ex)
                {
                    err += "<error><![CDATA[" + ex.Message + "]]><error>";
                }
                finally
                {
                    if (os != null)
                        os.Close();
                }
            }

            if (err.Length == 0)
                try
                { // get the response
                    WebResponse webResponse = webRequest.GetResponse();
                    if (webResponse == null)
                        err += "<error><![CDATA[webResponse==null]]><error>";
                    else
                    {
                        StreamReader sr = new StreamReader(webResponse.GetResponseStream());
                        string r = sr.ReadToEnd();
                        logger.DebugFormat("GetTimeStampCount Result: {0}", r);
                        switch (service.ToUpper())
                        {
                            case "INFOCAMERE":
                                //MSGNO = 100
                                //DESCR = Marche per alessandro.giachi@vecompsoftware.it; disponibili 48 consumate 52

                                ret += "<raw><![CDATA[" + r + "]]><raw>";
                                string[] r1 = r.Split(';');
                                if (r1.Length >= 1)
                                {
                                    string[] r2 = r1[1].Trim().Split(' ');
                                    ret += "<status available=\"" + (r2.Length >= 1 ? r2[1] : "ND") + "\" used=\"" + (r2.Length >= 3 ? r2[3] : "ND") + "\" />";
                                }else
                                    ret += "<status available=\"ND\" used=\"ND\" />";
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (WebException ex)
                {
                    err += "<error><![CDATA[" + ex.Message + "]]><error>";
                }

            return ret + err + "</TimeStampCount>";
        }

        public bool GetFileInfo(string format, byte[] blob, out string MetaData)
        {
            var compedObj = new CompEdObj();
            bool retval = false;

            try
            {
                switch (format.ToUpper())
                {
                    case "P7M":
                        compedObj.GetFileInfo(true, blob, out MetaData);
                        break;
                    case "P7X":
                        compedObj.GetFileInfo(false, blob, out MetaData);
                        break;
                    default:
                        MetaData = "<?xml version=\"1.0\" ?><file name=\"\" description=\"\"";
                        break;
                }

                SHA1 sha = new SHA1CryptoServiceProvider();
                byte[] docHash = sha.ComputeHash(blob);

                MetaData += " containerHash=\"" + CompEdObj.ByteToHex(docHash) + "\"></file>";

                retval = true;
            }
            finally
            {
                compedObj.Dispose();
            }

            return retval;
        }

        public byte[] SoftSign(string CertificateName, string fname, byte[] blob)
        {
            return SignService.Sign(blob, CertificateName);
        }

        #region IDisposable Members

        ~CompEdLib()
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
