using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using VecompSoftware.Commons.CompEd.Structs;

namespace VecompSoftware.Commons.CompEd
{
    // tag
    /// <summary>
    /// Classe di Wrapper DigitalSign
    /// Contiene i metodi per sbustare i file p7m
    /// Contiene i metodi per avere per visualizzare in formato XML 
    /// le informazioni relative la firma del documento
    /// </summary>

    public class P7Mmanager
    {
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
        // TimeStamp
        const string TOK_XML_TAG_OUT_TIMESTAMP = "TimeStamp";
        const string TOK_XML_TAG_OUT_ROWTS = "RowTs";
        const string TOK_XML_TAG_OUT_TS_ITIS = "%TS_TimeStamp%";
        const string TOK_XML_TAG_OUT_TS_LEASE = "%TS_Lease%";
        const string TOK_XML_TAG_OUT_TS_EXPIRE = "%TS_Expire%";
        const string TOK_XML_TAG_OUT_TS_ISSUER = "%TS_Issuer%";
        const string TOK_XML_TAG_OUT_TS_SUBJECT = "%TS_Subject%";
        const string TOK_XML_TAG_OUT_GENERAL_TIMESTAMP = "%TimeStamp%";
        const string TOK_XML_TAG_OUT_GENERAL_NTIMESTAMP = "%NumberTimeStamp%";
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
        const string TOK_XML_TAG_OUT_DESCRIPTION = "%Description%";
        const string TOK_XML_TAG_OUT_FILENAME = "%FileName%";
        const string TOK_XML_TAG_OUT_DATE = "%Date%";

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
        static int count = 0;

        public string LastErr = "";
        object CurrentBuf = null;
        XmlDocument xmlDoc;
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(P7Mmanager));

        public P7Mmanager()
        {
            try
            {
                string db = "";
                oDigest = new CCypher.Digest();
                try
                {
                    oDigest.GetDefaultConnectionString(out db);
                    oDigest.OpenDb(db);
                }
                catch (Exception ex) { LastErr = ex.Message + " " + db; }
                count++;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message + " " + exp.StackTrace);
            }
        }

        public void Close()
        {
            if (oDigest != null)
            {
                try
                {
                    oDigest.CloseDb();
                    Marshal.ReleaseComObject(oDigest);
                    oDigest = null;
                }
                catch (Exception ex) { LastErr = ex.Message; }
            }
        }

        public void dispose()
        {
            //if(--count==0)
            if (oDigest != null)
                try
                {
                    oDigest.CloseDb();
                    Marshal.ReleaseComObject(oDigest);
                    oDigest = null;
                }
                catch (Exception ex) { LastErr = ex.Message; }
        }

        public string GetOriginalFileName(byte[] p7m)
        {
            string fileName, retInfo;
            int nHandle = 0;
            oDigest.P7kLoadFromBuf(p7m, 1, out nHandle);
            oDigest.P7kGetDescription(nHandle, out fileName,out retInfo);
            return fileName;
        }

        public String GetExpiryDate(string fileName, byte[] p7m, out SimplyCert firstExp, out SimplyCert[] SimCrtLst)
        {
            firstExp = new SimplyCert();
            SimCrtLst = null;
            string LastErr = "";

            try
            {
                int nHandle = 0, nHandleTSfile = 0, nCountSignature = 0, nCountTimeStamp = 0;

                if (string.Compare(fileName.Substring(fileName.Length - 3, 3), "P7M", true) == 0)
                {
                    oDigest.P7kLoadFromBuf(p7m, 1, out nHandle);
                    if (nHandle == 0)
                    {
                        int ErrCode;
                        CCypher.enumErrorClass ErrClass;
                        oDigest.GetLastError(out ErrClass, out ErrCode, out LastErr);

                    }
                    else
                    {
                        oDigest.P7kGetSignatureCount(nHandle, out nCountSignature);
                        SimCrtLst = new SimplyCert[nCountSignature];

                        for (int j = 0; j < nCountSignature; j++)
                        {
                            int nHandleX509 = 0;
                            Object vtCertBuf = null;
                            CCypher.enumHashType nEnumHashType = CCypher.enumHashType.HTC_SHA1;

                            oDigest.P7kGetSignerInfo(nHandle, j, out nEnumHashType, out vtCertBuf);
                            oDigest.x509LoadFromBuf(vtCertBuf, out nHandleX509);

                            //oDigest.x509GetInfo(nHandleX509,CCypher.enumCertInfoCategory.CC_GENERAL,CCypher.enumCertInfoItem.CI_SERIALNUMBER,out sRetInfo);
                            //oDigest.x509GetInfo(nHandleX509,CCypher.enumCertInfoCategory.CC_SUBJECT,CCypher.enumCertInfoItem.CI_NAME,out sRetInfo);
                            //oDigest.x509GetInfo(nHandleX509,CCypher.enumCertInfoCategory.CC_SUBJECT,CCypher.enumCertInfoItem.CI_FISCALCODE,out sRetInfo);
                            //oDigest.x509GetInfo(nHandleX509,CCypher.enumCertInfoCategory.CC_GENERAL,CCypher.enumCertInfoItem.CI_DATEBIRTH,out sRetInfo);
                            //oDigest.x509GetInfo(nHandleX509,CCypher.enumCertInfoCategory.CC_GENERAL,CCypher.enumCertInfoItem.CI_VALID_FROM,out sRetInfo);

                            string sRetInfo = "";
                            oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_GENERAL, CCypher.enumCertInfoItem.CI_VALID_TO, out sRetInfo);
                            DateTime dt = new DateTime(int.Parse(sRetInfo.Substring(6, 4)), int.Parse(sRetInfo.Substring(3, 2)), int.Parse(sRetInfo.Substring(0, 2)));
                            oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_SUBJECT, CCypher.enumCertInfoItem.CI_NAME, out sRetInfo);

                            if (DateTime.Compare(firstExp.Expiry, dt) > 0 || j == 0)
                            {
                                firstExp.Type = TOK_TypeSign;
                                firstExp.Name = sRetInfo;
                                firstExp.Expiry = dt;
                            }

                            SimCrtLst[j].Type = TOK_TypeSign;
                            SimCrtLst[j].Name = sRetInfo;
                            SimCrtLst[j].Expiry = dt;
                            oDigest.x509Free(nHandleX509);
                        }
                        oDigest.P7kFree(nHandle);
                    }
                }
                else
                {	// marche temporali

                    oDigest.P7xLoadFromBuf(p7m, out nHandleTSfile);
                    if (nHandleTSfile == 0)
                    {
                        int ErrCode;
                        CCypher.enumErrorClass ErrClass;
                        oDigest.GetLastError(out ErrClass, out ErrCode, out LastErr);
                    }
                    else
                    {
                        oDigest.P7xGetTSCount(nHandleTSfile, out nCountTimeStamp);
                        SimCrtLst = new SimplyCert[nCountTimeStamp];

                        for (int j = 0; j < nCountTimeStamp && LastErr.Length == 0; j++)
                        {
                            int nHandleTS = 0;

                            oDigest.P7xGetTS(nHandleTSfile, j, out nHandleTS);
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

                                if (DateTime.Compare(firstExp.Expiry, dt) > 0 || j == 0)
                                {
                                    firstExp.Type = TOK_TypeTS;
                                    firstExp.Name = (j + 1).ToString();
                                    firstExp.Expiry = dt;
                                }

                                SimCrtLst[j].Type = TOK_TypeTS;
                                SimCrtLst[j].Name = (j + 1).ToString();
                                SimCrtLst[j].Expiry = dt;

                                oDigest.tsFree(nHandleTS);
                            }
                        }
                        oDigest.P7xFree(nHandleTSfile);
                    }
                }
            }
            catch (Exception e)
            {
                LastErr = e.Message;
            }
            return LastErr;
        }

        public String GetAllExpiryDate(byte[] p7m, out SimplyCert firstExp, out Hashtable SimCrtLst)
        {
            firstExp = new SimplyCert();
            SimCrtLst = new Hashtable();
            return RecGetAllExpiryDate(1, p7m, ref firstExp, ref SimCrtLst);
        }

        int ele = 1;
        public String RecGetAllExpiryDate(int lev, byte[] p7m, ref SimplyCert firstExp, ref Hashtable SimCrtLst)
        {
            string LastErr = "";

            try
            {
                int nHandle = 0, nCountSignature = 0, nCountTimeStamp = 0;

                oDigest.P7kLoadFromBuf(p7m, 1, out nHandle);
                if (nHandle > 0)
                {
                    oDigest.P7kGetSignatureCount(nHandle, out nCountSignature);
                    //SimCrtLst=new SimplyCert[nCountSignature];

                    for (int j = 0; j < nCountSignature; j++)
                    {
                        int nHandleX509 = 0;
                        Object vtCertBuf = null;
                        CCypher.enumHashType nEnumHashType = CCypher.enumHashType.HTC_SHA1;

                        oDigest.P7kGetSignerInfo(nHandle, j, out nEnumHashType, out vtCertBuf);
                        oDigest.x509LoadFromBuf(vtCertBuf, out nHandleX509);

                        string sRetInfo = "", Name = "", fc = "", des = "", role = "", iss = "", email = "";
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

                            if ((DateTime.Compare(firstExp.Expiry, dt) > 0 || j == 0) && firstExp.Type != TOK_TypeTS)
                            {
                                firstExp.Level = lev;
                                firstExp.Type = TOK_TypeTS;
                                firstExp.Name = (j + 1).ToString();
                                firstExp.Expiry = dt;
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

        private string ExtractKeyFromString(string sBuffer, string sKey)
        {
            string sRet = "";
            char cLF = '\x000A';

            int nStartToken = sBuffer.IndexOf(sKey);
            if (nStartToken >= 0)
            {
                int nEndToken = sBuffer.IndexOf(cLF, nStartToken);
                if (nEndToken >= 0)
                    sRet = sBuffer.Substring(nStartToken + sKey.Length, nEndToken - nStartToken - sKey.Length);
            }

            return sRet;
        }

        private void SetSingleAttribute(ref XmlElement xmlEl, string sKey, string sValue)
        {
            // imposta solo se 
            // non è nullo
            // e se già non esiste
            sValue = sValue.Trim();
            if (sValue != "")
            {
                if (xmlEl.GetAttribute(sKey) == "")
                    xmlEl.SetAttribute(sKey, sValue);
            }
        }

        private bool SetInformation(CS nType, string[] sValue, string nSignature, ref XmlElement xmlElSig)
        {
            bool bRet = false;
            string[] sInfo;
            bool bFound = false;
            XmlElement xmlEl = null;

            switch (nType)
            {
                case CS.ISSUER:
                    for (int j = 0; j < xmlElSig.ChildNodes.Count && !bFound; j++)
                        if (xmlElSig.ChildNodes.Item(j).Name == XML_TOKEN.TOK_XML_TAG_ISSUER)
                        {
                            xmlEl = (XmlElement)xmlElSig.ChildNodes.Item(j);
                            bFound = true;
                        }
                    if (!bFound)
                    {
                        xmlEl = xmlDoc.CreateElement(XML_TOKEN.TOK_XML_TAG_ISSUER);
                        xmlEl.SetAttribute(XML_TOKEN.TOK_XML_TAG_ATT_SIGNATURE, nSignature);
                        xmlElSig.AppendChild(xmlEl);
                    }
                    for (int j = 0; j <= sValue.GetUpperBound(0); j++)
                    {
                        sInfo = sValue[j].Split('=');
                        // Organization
                        if (sInfo[0].Trim() == TOK_SIG_ISSUER_ATT_O && sInfo.Length > 1)
                            SetSingleAttribute(ref xmlEl, XML_TOKEN.TOK_XML_TAG_ISSUER_ATT_O, sInfo[1].Trim());
                        // Organization Unit
                        if (sInfo[0].Trim() == TOK_SIG_ISSUER_ATT_OU && sInfo.Length > 1)
                            SetSingleAttribute(ref xmlEl, XML_TOKEN.TOK_XML_TAG_ISSUER_ATT_OU, sInfo[1].Trim());
                        // COUNTRY
                        else if (sInfo[0].Trim() == TOK_SIG_ISSUER_ATT_C && sInfo.Length > 1)
                            SetSingleAttribute(ref xmlEl, XML_TOKEN.TOK_XML_TAG_ISSUER_ATT_C, sInfo[1].Trim());
                        // CN
                        else if (sInfo[0].Trim() == TOK_SIG_ISSUER_ATT_CN && sInfo.Length > 1)
                            SetSingleAttribute(ref xmlEl, XML_TOKEN.TOK_XML_TAG_ISSUER_ATT_CN, sInfo[1].Trim());
                    }
                    break;
                case CS.SUBJECT:
                    for (int j = 0; j < xmlElSig.ChildNodes.Count && !bFound; j++)
                        if (xmlElSig.ChildNodes.Item(j).Name == XML_TOKEN.TOK_XML_TAG_SUBJECT)
                        {
                            xmlEl = (XmlElement)xmlElSig.ChildNodes.Item(j);
                            bFound = true;
                        }
                    if (!bFound)
                    {
                        xmlEl = xmlDoc.CreateElement(XML_TOKEN.TOK_XML_TAG_SUBJECT);
                        xmlEl.SetAttribute(XML_TOKEN.TOK_XML_TAG_ATT_SIGNATURE, nSignature);
                        xmlElSig.AppendChild(xmlEl);
                    }
                    for (int j = 0; j <= sValue.GetUpperBound(0); j++)
                    {
                        sInfo = sValue[j].Split('=');
                        // Organization
                        if (sInfo[0].Trim() == TOK_SIG_SUBJECT_ATT_O && sInfo.Length > 1)
                            SetSingleAttribute(ref xmlEl, XML_TOKEN.TOK_XML_TAG_SUBJECT_ATT_O, sInfo[1].Trim());
                        // Country
                        if (sInfo[0].Trim() == TOK_SIG_SUBJECT_ATT_C && sInfo.Length > 1)
                            SetSingleAttribute(ref xmlEl, XML_TOKEN.TOK_XML_TAG_SUBJECT_ATT_C, sInfo[1].Trim());
                        // cognome nome CF
                        if (sInfo[0].Trim() == TOK_SIG_SUBJECT_ATT_CN && sInfo.Length > 1)
                            SetSingleAttribute(ref xmlEl, XML_TOKEN.TOK_XML_TAG_SUBJECT_ATT_CN, sInfo[1].Trim());
                        // ibrido
                        if (sInfo[0].Trim() == TOK_SIG_SUBJECT_ATT_D && sInfo.Length > 1)
                        {
                            switch (sInfo.Length)
                            {
                                // data di nascita
                                case 2:
                                    SetSingleAttribute(ref xmlEl, XML_TOKEN.TOK_XML_TAG_SUBJECT_ATT_D, sInfo[1].Trim());
                                    break;
                                // cognome
                                case 3:
                                    SetSingleAttribute(ref xmlEl, XML_TOKEN.TOK_XML_TAG_SUBJECT_ATT_CO, sInfo[2].Trim());
                                    break;
                            }
                        }
                        // nome
                        if (sInfo[0].Trim() == TOK_SIG_SUBJECT_ATT_N && sInfo.Length > 1)
                            SetSingleAttribute(ref xmlEl, XML_TOKEN.TOK_XML_TAG_SUBJECT_ATT_N, sInfo[1].Trim());
                    }
                    break;
                case CS.SERIAL_NUMBER:
                    for (int j = 0; j < xmlElSig.ChildNodes.Count && !bFound; j++)
                        if (xmlElSig.ChildNodes.Item(j).Name == XML_TOKEN.TOK_XML_TAG_DETAILS)
                        {
                            xmlEl = (XmlElement)xmlElSig.ChildNodes.Item(j);
                            bFound = true;
                        }
                    if (!bFound)
                    {
                        xmlEl = xmlDoc.CreateElement(XML_TOKEN.TOK_XML_TAG_DETAILS);
                        xmlEl.SetAttribute(XML_TOKEN.TOK_XML_TAG_ATT_SIGNATURE, nSignature);
                        xmlElSig.AppendChild(xmlEl);
                    }
                    // Serial Number
                    sInfo = sValue[0].Split('=');
                    SetSingleAttribute(ref xmlEl, XML_TOKEN.TOK_XML_TAG_DETAILS_ATT_SN, sInfo[0].Trim());

                    break;
                case CS.NOT_BEFORE:
                    for (int j = 0; j < xmlElSig.ChildNodes.Count && !bFound; j++)
                        if (xmlElSig.ChildNodes.Item(j).Name == XML_TOKEN.TOK_XML_TAG_DETAILS)
                        {
                            xmlEl = (XmlElement)xmlElSig.ChildNodes.Item(j);
                            bFound = true;
                        }
                    if (!bFound)
                    {
                        xmlEl = xmlDoc.CreateElement(XML_TOKEN.TOK_XML_TAG_DETAILS);
                        xmlEl.SetAttribute(XML_TOKEN.TOK_XML_TAG_ATT_SIGNATURE, nSignature);
                        xmlElSig.AppendChild(xmlEl);
                    }
                    // Not Before
                    sInfo = sValue[0].Split('=');
                    SetSingleAttribute(ref xmlEl, XML_TOKEN.TOK_XML_TAG_DETAILS_ATT_LEASE, sInfo[0].Trim());

                    break;
                case CS.NOT_AFTER:
                    for (int j = 0; j < xmlElSig.ChildNodes.Count && !bFound; j++)
                        if (xmlElSig.ChildNodes.Item(j).Name == XML_TOKEN.TOK_XML_TAG_DETAILS)
                        {
                            xmlEl = (XmlElement)xmlElSig.ChildNodes.Item(j);
                            bFound = true;
                        }
                    if (!bFound)
                    {
                        xmlEl = xmlDoc.CreateElement(XML_TOKEN.TOK_XML_TAG_DETAILS);
                        xmlEl.SetAttribute(XML_TOKEN.TOK_XML_TAG_ATT_SIGNATURE, nSignature);
                        xmlElSig.AppendChild(xmlEl);
                    }
                    // Not After
                    sInfo = sValue[0].Split('=');
                    SetSingleAttribute(ref xmlEl, XML_TOKEN.TOK_XML_TAG_DETAILS_ATT_EXPIRE, sInfo[0].Trim());

                    break;
                case CS.DESCRIPTION:
                    for (int j = 0; j < xmlElSig.ChildNodes.Count && !bFound; j++)
                        if (xmlElSig.ChildNodes.Item(j).Name == XML_TOKEN.TOK_XML_TAG_SUBJECT)
                        {
                            xmlEl = (XmlElement)xmlElSig.ChildNodes.Item(j);
                            bFound = true;
                        }
                    if (!bFound)
                    {
                        xmlEl = xmlDoc.CreateElement(XML_TOKEN.TOK_XML_TAG_SUBJECT);
                        xmlEl.SetAttribute(XML_TOKEN.TOK_XML_TAG_ATT_SIGNATURE, nSignature);
                        xmlElSig.AppendChild(xmlEl);
                    }
                    // Descrizione
                    for (int j = 0; j <= sValue.GetUpperBound(0); j++)
                    {
                        sInfo = sValue[j].Split('=');
                        // nome
                        if (sInfo[0].Trim() == TOK_SIG_DESCRIPTION_ATT_N && sInfo.Length > 1)
                            SetSingleAttribute(ref xmlEl, XML_TOKEN.TOK_XML_TAG_SUBJECT_ATT_N, sInfo[1].Trim());
                        // Cognome
                        else if (sInfo[0].Trim() == TOK_SIG_DESCRIPTION_ATT_C && sInfo.Length > 1)
                            SetSingleAttribute(ref xmlEl, XML_TOKEN.TOK_XML_TAG_SUBJECT_ATT_CO, sInfo[1].Trim());
                        // Data di Nascita
                        else if (sInfo[0].Trim() == TOK_SIG_DESCRIPTION_ATT_D && sInfo.Length > 1)
                            SetSingleAttribute(ref xmlEl, XML_TOKEN.TOK_XML_TAG_SUBJECT_ATT_D, sInfo[1].Trim());
                    }
                    break;
                case CS.FISCAL_CODE:
                    for (int j = 0; j < xmlElSig.ChildNodes.Count && !bFound; j++)
                        if (xmlElSig.ChildNodes.Item(j).Name == XML_TOKEN.TOK_XML_TAG_SUBJECT)
                        {
                            xmlEl = (XmlElement)xmlElSig.ChildNodes.Item(j);
                            bFound = true;
                        }
                    if (!bFound)
                    {
                        xmlEl = xmlDoc.CreateElement(XML_TOKEN.TOK_XML_TAG_SUBJECT);
                        xmlEl.SetAttribute(XML_TOKEN.TOK_XML_TAG_ATT_SIGNATURE, nSignature);
                        xmlElSig.AppendChild(xmlEl);
                    }
                    // Fiscal Code
                    sInfo = sValue[0].Split('=');
                    SetSingleAttribute(ref xmlEl, XML_TOKEN.TOK_XML_TAG_SUBJECT_ATT_FC, sInfo[0].Trim());

                    break;
            }

            return bRet;
        }

        private bool IsThisDocumentSigned(ref string sStringcode64Document)
        {
            if (string.IsNullOrEmpty(sStringcode64Document))
                return false;
            string sVerify = sStringcode64Document.Substring(0, 10);
            bool bRet = false;            
            if (sVerify.Substring(0, 2) == "MI" || sVerify.Substring(0, 2) == "TW")
                bRet = true;

            return bRet;
        }

        // Estrazione Informazioni in Formato XML
        public string ExtractSignatureInfo(ref Object objP7M, int iParentDocument)
        {
            // Dichiarazioni
            string sRet = "";
            int nHandle = 0;
            int nHandleX509 = 0;
            Object vtCertBuf = null;
            string sRetInfo = "";
            XmlElement xmlElDoc;
            XmlElement xmlElSig;
            XmlElement xmlNestedSig;
            string sSignatureNumber = "";

            CCypher.enumHashType nEnumHashType = CCypher.enumHashType.HTC_SHA1;

            try
            {
                // wrapper ultimo in memoria
                if (objP7M == null)
                    objP7M = CurrentBuf;
                else
                    CurrentBuf = objP7M;

                xmlDoc = new XmlDocument();
                //xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0","UTF-8","yes"));

                // Intestazione
                xmlElDoc = xmlDoc.CreateElement(XML_TOKEN.TOK_XML_TAG_DOCUMENT);
                xmlDoc.AppendChild(xmlElDoc);
                // Lettura Informazioni
                oDigest.P7kLoadFromBuf(objP7M, 1, out nHandle);

                int signatureCount = 0;
                oDigest.P7kGetSignatureCount(nHandle, out signatureCount);

                if (signatureCount == 0)
                {
                    try
                    {
                        // try to load as P7x
                        string db;
                        oDigest.GetDefaultConnectionString(out db);
                        oDigest.OpenDb(db);

                        oDigest.P7xLoadFromBuf(objP7M, out nHandle);
                        if (nHandle > 0)
                        {
                            int ErrCode;
                            string LastErr = "";
                            CCypher.enumErrorClass ErrClass;
                            oDigest.GetLastError(out ErrClass, out ErrCode, out LastErr);

                            int nCountTimeStamp;
                            oDigest.P7xGetTSCount(nHandle, out nCountTimeStamp);
                            xmlElDoc.SetAttribute(XML_TOKEN.TOK_XML_TAG_DOCUMENT_ATT_NTIMESTAMP, nCountTimeStamp.ToString());

                            for (int j = 1; j <= nCountTimeStamp; j++)
                            {
                                int nHandleTS = 0;

                                sSignatureNumber = iParentDocument.ToString() + "." + j.ToString();
                                // firma i-esima
                                xmlElSig = xmlDoc.CreateElement(XML_TOKEN.TOK_XML_TAG_TIMESTAMP);
                                xmlElDoc.AppendChild(xmlElSig);
                                xmlElSig.SetAttribute(XML_TOKEN.TOK_XML_TAG_ATT_TIMESTAMP, sSignatureNumber);

                                oDigest.P7xGetTS(nHandle, j - 1, out nHandleTS);
                                if (nHandleTS != 0)
                                {
                                    object DaT;
                                    oDigest.tsGetDateAndTime(nHandleTS, out DaT);
                                    oDigest.GetLastError(out ErrClass, out ErrCode, out LastErr);
                                    DateTime dt = (DateTime)DaT;
                                    xmlElSig.SetAttribute(XML_TOKEN.TOK_XML_TAG_DETAILS_ATT_LEASE, dt.ToString("dd/MM/yyyy"));
                                    oDigest.tsGetExpieryDateAndTime(nHandleTS, out DaT);
                                    oDigest.GetLastError(out ErrClass, out ErrCode, out LastErr);
                                    if (DaT != null)
                                    {
                                        dt = (DateTime)DaT;
                                        xmlElSig.SetAttribute(XML_TOKEN.TOK_XML_TAG_DETAILS_ATT_EXPIRE, dt.ToString("dd/MM/yyyy"));
                                    }
                                    string str;
                                    oDigest.tsGetTSPInfo(nHandleTS, CCypher.enumCertInfoCategory.CC_SUBJECT, CCypher.enumCertInfoItem.CI_NAME, out str);
                                    xmlElSig.SetAttribute(XML_TOKEN.TOK_XML_TAG_SUBJECT, str);
                                    oDigest.tsGetTSPInfo(nHandleTS, CCypher.enumCertInfoCategory.CC_ISSUER, CCypher.enumCertInfoItem.CI_NAME, out str);
                                    xmlElSig.SetAttribute(XML_TOKEN.TOK_XML_TAG_ISSUER, str);
                                    
                                    oDigest.tsFree(nHandleTS);
                                }
                            }
                            oDigest.P7xFree(nHandle);
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                    }
                }
                else
                {
                    XmlNode innerSignature;
                    // numero firme
                    xmlElDoc.SetAttribute(XML_TOKEN.TOK_XML_TAG_DOCUMENT_ATT_NSIGNATURE, signatureCount.ToString());
                    for (int j = 1; j <= signatureCount; j++)
                    {
                        sSignatureNumber = iParentDocument.ToString() + "." + j.ToString();
                        // firma i-esima
                        xmlElSig = xmlDoc.CreateElement(XML_TOKEN.TOK_XML_TAG_SIGNATURE);
                        xmlElDoc.AppendChild(xmlElSig);
                        xmlElSig.SetAttribute(XML_TOKEN.TOK_XML_TAG_ATT_SIGNATURE, sSignatureNumber);
                       
                        nHandleX509 = 0;
                        vtCertBuf = null;
                        oDigest.P7kGetSignerInfo(nHandle, j - 1, out nEnumHashType, out vtCertBuf);
                        oDigest.x509LoadFromBuf(vtCertBuf, out nHandleX509);
                        sRetInfo = "";
                        // impostazione XML

                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_ALL, CCypher.enumCertInfoItem.CI_NA, out sRetInfo);
                        SetInformation(CS.ISSUER, ExtractKeyFromString(sRetInfo, TOK_SIG_ISSUER).Split(','), sSignatureNumber, ref xmlElSig);
                        SetInformation(CS.SERIAL_NUMBER, ExtractKeyFromString(sRetInfo, TOK_SIG_SERIAL_NUMBER).Split(','), sSignatureNumber, ref xmlElSig);
                        SetInformation(CS.SUBJECT, ExtractKeyFromString(sRetInfo, TOK_SIG_SUBJECT).Split(','), sSignatureNumber, ref xmlElSig);
                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_GENERAL, CCypher.enumCertInfoItem.CI_DESCRIPTION, out sRetInfo);
                        SetInformation(CS.DESCRIPTION, sRetInfo.Replace("/", ", ").Split(','), sSignatureNumber, ref xmlElSig);                        			


                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_GENERAL, CCypher.enumCertInfoItem.CI_SERIALNUMBER, out sRetInfo);
                        locSetSingleAttribute(XML_TOKEN.TOK_XML_TAG_SUBJECT, XML_TOKEN.TOK_XML_TAG_DETAILS_ATT_SN, sRetInfo);

                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_SUBJECT, CCypher.enumCertInfoItem.CI_NAME, out sRetInfo);
                        locSetSingleAttribute(XML_TOKEN.TOK_XML_TAG_SUBJECT, XML_TOKEN.TOK_XML_TAG_SUBJECT_ATT_NAME, sRetInfo);

                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_SUBJECT, CCypher.enumCertInfoItem.CI_FISCALCODE, out sRetInfo);
                        locSetSingleAttribute(XML_TOKEN.TOK_XML_TAG_SUBJECT, XML_TOKEN.TOK_XML_TAG_SUBJECT_ATT_FC, sRetInfo);

                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_GENERAL, CCypher.enumCertInfoItem.CI_DATEBIRTH, out sRetInfo);
                        if (sRetInfo != null)
                            locSetSingleAttribute(XML_TOKEN.TOK_XML_TAG_SUBJECT, XML_TOKEN.TOK_XML_TAG_SUBJECT_ATT_D, sRetInfo);

                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_GENERAL, CCypher.enumCertInfoItem.CI_VALID_FROM, out sRetInfo);
                        locSetSingleAttribute(XML_TOKEN.TOK_XML_TAG_DETAILS, XML_TOKEN.TOK_XML_TAG_DETAILS_ATT_LEASE, sRetInfo);

                        oDigest.x509GetInfo(nHandleX509, CCypher.enumCertInfoCategory.CC_GENERAL, CCypher.enumCertInfoItem.CI_VALID_TO, out sRetInfo);
                        locSetSingleAttribute(XML_TOKEN.TOK_XML_TAG_DETAILS, XML_TOKEN.TOK_XML_TAG_DETAILS_ATT_EXPIRE, sRetInfo);
                        
                        try
                        {
                            CCypher.enumASN1Type cct;
                            object obj;
                            oDigest.P7kGetSignAttribute(nHandle, 0, j - 1, "unstructuredName", out cct, out obj);
                            string sTmp = (string)obj;
                            string[] atts = sTmp.Split(';');

                            for (int i = 0; i < atts.Length; i++)
                            {
                                if (atts[i].Length > XML_TOKEN.TOK_XML_TAG_PKCS7_FileHeaderDes.Length &&
                                  string.Compare(atts[i], 0, XML_TOKEN.TOK_XML_TAG_PKCS7_FileHeaderDes, 0,
                                  XML_TOKEN.TOK_XML_TAG_PKCS7_FileHeaderDes.Length, true) == 0)
                                {
                                    string strVal = atts[i].Substring(XML_TOKEN.TOK_XML_TAG_PKCS7_FileHeaderDes.Length + 1);
                                    locSetSingleAttribute(XML_TOKEN.TOK_XML_TAG_DETAILS, XML_TOKEN.TOK_XML_TAG_DESCRIPTION, strVal == "" ? "-" : strVal);
                                }
                                //xmlElSig.SetAttribute(XML_TOKEN.TOK_XML_TAG_DESCRIPTION, atts[i].Substring(XML_TOKEN.TOK_XML_TAG_PKCS7_FileHeaderDes.Length));
                                if (atts[i].Length > XML_TOKEN.TOK_XML_TAG_PKCS7_FileName.Length &&
                                  string.Compare(atts[i], 0, XML_TOKEN.TOK_XML_TAG_PKCS7_FileName, 0,
                                  XML_TOKEN.TOK_XML_TAG_PKCS7_FileName.Length, true) == 0)
                                    locSetSingleAttribute(XML_TOKEN.TOK_XML_TAG_DETAILS, XML_TOKEN.TOK_XML_TAG_DATE, atts[i].Substring(XML_TOKEN.TOK_XML_TAG_PKCS7_FileName.Length + 1));
                                //xmlElSig.SetAttribute(XML_TOKEN.TOK_XML_TAG_DATE, atts[i].Substring(XML_TOKEN.TOK_XML_TAG_PKCS7_FileName.Length));
                            }
                        }
                        catch (Exception ex) { string err = ex.Message; }

                        oDigest.x509Free(nHandleX509);

                        innerSignature = ExtractCounterSignatureInfo(nHandle, j - 1, 0, iParentDocument);

                        if (innerSignature != null)
                        {
                            xmlNestedSig = xmlDoc.CreateElement(TOK_XML_TAG_OUT_SIGNATURE_NESTED);
                            xmlNestedSig.InnerXml = innerSignature.OuterXml;
                            xmlElSig.InnerXml += xmlNestedSig.OuterXml;
                        }
                    }
                    string ss1, ss2;
                    oDigest.P7kGetDescription(nHandle, out ss1, out ss2);
                    xmlElDoc.SetAttribute(XML_TOKEN.TOK_XML_TAG_DESCRIPTION, ss2);
                    xmlElDoc.SetAttribute(XML_TOKEN.TOK_XML_TAG_FILENAME, ss1);

                    oDigest.P7kFree(nHandle);
                }

                // Se il documento interno è firmato:
                // verifica sui primi 10 byte
                string sInnerDocument = ExtractDocumentFromBuffer(null, true);
                if (IsThisDocumentSigned(ref sInnerDocument))
                {
                    // contiene un documento firmato!!!!!
                    XmlDocument xmlInnerDoc = new XmlDocument();
                    XmlNode NodeChieldSignature;
                    XmlDocument xmlDocInner = new XmlDocument();
                    P7Mmanager p7mChildDocument = new P7Mmanager();

                    //byte[] abInnerDocument = System.Convert.FromBase64String(sInnerDocument);
                    //Object oInnerDocument = abInnerDocument;
                    Object oInnerDocument = (Object)System.Convert.FromBase64String(sInnerDocument);
                    string sInnerXML = ConvertFromCode64StringToString(p7mChildDocument.ExtractSignatureInfo(ref oInnerDocument, iParentDocument + 1));
                    xmlDocInner.LoadXml(sInnerXML);
                    NodeChieldSignature = xmlDoc.ImportNode(xmlDocInner.FirstChild, true);
                    xmlElDoc.AppendChild(NodeChieldSignature);
                    p7mChildDocument.Close();
                }

                // chiudo documento
                MemoryStream memStream = new MemoryStream();
                //ASCIIEncoding AE = new ASCIIEncoding();
                UTF8Encoding AE = new UTF8Encoding();
                string xmlStr = xmlDoc.DocumentElement.OuterXml;
                byte[] buf = AE.GetBytes(xmlStr);
                memStream.Write(buf, 0, buf.Length); //xmlStr.Length
                Byte[] buffer = memStream.ToArray();
                sRet = System.Convert.ToBase64String(buffer, 0, buffer.Length);
                memStream.Close();

            }
            catch (System.Exception exp)
            {
                throw new Exception(exp.Message + " " + exp.StackTrace);
            }

            return sRet;
        }

        /// <summary>
        /// Estrazione controfirme in formato XmlNode.
        /// </summary>
        /// <param name="handleP7M">Handle al file p7m.</param>
        /// <param name="mainSignatureIndex">Indice zero-based della firma principale (controfirmata).</param>
        /// <param name="counterSignatureIndex">Indice zero-based della controfirma.</param>
        /// <param name="parentDocument"></param>
        /// <returns></returns>
        private XmlNode ExtractCounterSignatureInfo(int handleP7M, int mainSignatureIndex, int counterSignatureIndex, int parentDocument)
        {
            var currentXmlDoc = xmlDoc;
            XmlNode retval = null;
            string sRetInfo;
            int handleX509, handleCounterSignature;
            CCypher.enumHashType hashType;
            object vtCertBuf;
            var innerSignatures = string.Empty;

            try
            {
                xmlDoc = new XmlDocument();
                oDigest.P7kGetCountersignature(handleP7M, mainSignatureIndex, out handleCounterSignature);
                var xmlElement = xmlDoc.CreateElement(XML_TOKEN.TOK_XML_TAG_SIGNATURE);
                xmlDoc.AppendChild(xmlElement);

                var sSignatureNumber = parentDocument + "." + (mainSignatureIndex + 1) + "." + (counterSignatureIndex + 1);

                xmlElement.SetAttribute(XML_TOKEN.TOK_XML_TAG_ATT_SIGNATURE, sSignatureNumber);

                oDigest.P7kGetSignerInfo(handleCounterSignature, mainSignatureIndex, out hashType, out vtCertBuf);
                if (vtCertBuf != null)
                {
                    oDigest.x509LoadFromBuf(vtCertBuf, out handleX509);

                    try
                    {
                        oDigest.x509GetInfo(handleX509, CCypher.enumCertInfoCategory.CC_ALL, CCypher.enumCertInfoItem.CI_NA, out sRetInfo);
                        SetInformation(CS.ISSUER, ExtractKeyFromString(sRetInfo, TOK_SIG_ISSUER).Split(','), sSignatureNumber, ref xmlElement);
                        SetInformation(CS.SERIAL_NUMBER, ExtractKeyFromString(sRetInfo, TOK_SIG_SERIAL_NUMBER).Split(','), sSignatureNumber, ref xmlElement);
                        SetInformation(CS.SUBJECT, ExtractKeyFromString(sRetInfo, TOK_SIG_SUBJECT).Split(','), sSignatureNumber, ref xmlElement);
                        oDigest.x509GetInfo(handleX509, CCypher.enumCertInfoCategory.CC_GENERAL, CCypher.enumCertInfoItem.CI_DESCRIPTION, out sRetInfo);
                        SetInformation(CS.DESCRIPTION, sRetInfo.Replace("/", ", ").Split(','), sSignatureNumber, ref xmlElement);


                        oDigest.x509GetInfo(handleX509, CCypher.enumCertInfoCategory.CC_GENERAL, CCypher.enumCertInfoItem.CI_SERIALNUMBER, out sRetInfo);
                        locSetSingleAttribute(XML_TOKEN.TOK_XML_TAG_SUBJECT, XML_TOKEN.TOK_XML_TAG_DETAILS_ATT_SN, sRetInfo);

                        oDigest.x509GetInfo(handleX509, CCypher.enumCertInfoCategory.CC_SUBJECT, CCypher.enumCertInfoItem.CI_NAME, out sRetInfo);
                        locSetSingleAttribute(XML_TOKEN.TOK_XML_TAG_SUBJECT, XML_TOKEN.TOK_XML_TAG_SUBJECT_ATT_NAME, sRetInfo);

                        oDigest.x509GetInfo(handleX509, CCypher.enumCertInfoCategory.CC_SUBJECT, CCypher.enumCertInfoItem.CI_FISCALCODE, out sRetInfo);
                        locSetSingleAttribute(XML_TOKEN.TOK_XML_TAG_SUBJECT, XML_TOKEN.TOK_XML_TAG_SUBJECT_ATT_FC, sRetInfo);

                        oDigest.x509GetInfo(handleX509, CCypher.enumCertInfoCategory.CC_GENERAL, CCypher.enumCertInfoItem.CI_DATEBIRTH, out sRetInfo);
                        if (sRetInfo != null)
                            locSetSingleAttribute(XML_TOKEN.TOK_XML_TAG_SUBJECT, XML_TOKEN.TOK_XML_TAG_SUBJECT_ATT_D, sRetInfo);

                        oDigest.x509GetInfo(handleX509, CCypher.enumCertInfoCategory.CC_GENERAL, CCypher.enumCertInfoItem.CI_VALID_FROM, out sRetInfo);
                        locSetSingleAttribute(XML_TOKEN.TOK_XML_TAG_DETAILS, XML_TOKEN.TOK_XML_TAG_DETAILS_ATT_LEASE, sRetInfo);

                        oDigest.x509GetInfo(handleX509, CCypher.enumCertInfoCategory.CC_GENERAL, CCypher.enumCertInfoItem.CI_VALID_TO, out sRetInfo);
                        locSetSingleAttribute(XML_TOKEN.TOK_XML_TAG_DETAILS, XML_TOKEN.TOK_XML_TAG_DETAILS_ATT_EXPIRE, sRetInfo);

                        try
                        {
                            CCypher.enumASN1Type cct;
                            object obj;
                            oDigest.P7kGetSignAttribute(handleCounterSignature, 0, mainSignatureIndex, "unstructuredName", out cct, out obj);
                            string sTmp = (string)obj;
                            string[] atts = sTmp.Split(';');

                            for (int i = 0; i < atts.Length; i++)
                            {
                                if (atts[i].Length > XML_TOKEN.TOK_XML_TAG_PKCS7_FileHeaderDes.Length &&
                                  string.Compare(atts[i], 0, XML_TOKEN.TOK_XML_TAG_PKCS7_FileHeaderDes, 0,
                                  XML_TOKEN.TOK_XML_TAG_PKCS7_FileHeaderDes.Length, true) == 0)
                                {
                                    string strVal = atts[i].Substring(XML_TOKEN.TOK_XML_TAG_PKCS7_FileHeaderDes.Length + 1);
                                    locSetSingleAttribute(XML_TOKEN.TOK_XML_TAG_DETAILS, XML_TOKEN.TOK_XML_TAG_DESCRIPTION, strVal == "" ? "-" : strVal);
                                }
                                //xmlElSig.SetAttribute(XML_TOKEN.TOK_XML_TAG_DESCRIPTION, atts[i].Substring(XML_TOKEN.TOK_XML_TAG_PKCS7_FileHeaderDes.Length));
                                if (atts[i].Length > XML_TOKEN.TOK_XML_TAG_PKCS7_FileName.Length &&
                                  string.Compare(atts[i], 0, XML_TOKEN.TOK_XML_TAG_PKCS7_FileName, 0,
                                  XML_TOKEN.TOK_XML_TAG_PKCS7_FileName.Length, true) == 0)
                                    locSetSingleAttribute(XML_TOKEN.TOK_XML_TAG_DETAILS, XML_TOKEN.TOK_XML_TAG_DATE, atts[i].Substring(XML_TOKEN.TOK_XML_TAG_PKCS7_FileName.Length + 1));
                                //xmlElSig.SetAttribute(XML_TOKEN.TOK_XML_TAG_DATE, atts[i].Substring(XML_TOKEN.TOK_XML_TAG_PKCS7_FileName.Length));
                            }
                        }
                        catch (Exception ex) { logger.Error(ex); }
                    }
                    finally
                    {
                        try { oDigest.x509Free(handleX509); }
                        catch { }
                    }
                }
                else
                {
                    xmlDoc = null; //Non ci sono controfirme.
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                xmlDoc = null;
            }

            if (xmlDoc != null)
                retval = xmlDoc.Clone();

            xmlDoc = currentXmlDoc;

            return retval;
        }

        public void locSetSingleAttribute(string nodeName, string attrName, string val)
        {
            XmlNodeList NodeList = xmlDoc.GetElementsByTagName(nodeName);
            for (int j = 0; j < NodeList.Count; j++)
            {
                XmlElement xmlEl = (XmlElement)NodeList.Item(j);
                SetSingleAttribute(ref xmlEl, attrName, val);
            }
        }
        // Da un file P7M Passato come Oggetto ritorna il contenuto
        // in una stringa convertita in Code64
        public string ExtractSignatureInfoFromBuffer(Object objP7M)
        {
            return ExtractSignatureInfo(ref objP7M, 1);
        }

        // Da un file P7M passato come come stringa in Code64 ritorna il contenuto
        // in una stringa convertita in Code64
        public string ExtractSignatureInfoFromCode64(string sCode64string)
        {
            // Dichiarazioni
            string sRet = "";
            try
            {
                if (sCode64string == null)
                    sRet = ExtractSignatureInfoFromBuffer(null);
                else
                {
                    byte[] abDocument = System.Convert.FromBase64String(sCode64string);
                    sRet = ExtractSignatureInfoFromBuffer(abDocument);
                }
            }
            catch (System.Exception exp)
            {
                throw new Exception(exp.Message + " " + exp.StackTrace);
            }

            return sRet;
        }

        // Da un file P7M passato come path ritorna il contenuto
        // in una stringa convertita in Code64
        public string ExtractSignatureInfoFromFile(string sP7MFile)
        {
            // Dichiarazioni
            string sRet = "";
            try
            {
                if (sP7MFile == null)
                    sRet = ExtractSignatureInfoFromBuffer(null);
                else
                {
                    System.IO.FileStream fs = new System.IO.FileStream(sP7MFile, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    byte[] Input = new byte[fs.Length];
                    fs.Read(Input, 0, (int)fs.Length);
                    fs.Close();
                    sRet = ExtractSignatureInfoFromBuffer(Input);
                }
            }
            catch (System.Exception exp)
            {
                throw new Exception(exp.Message + " " + exp.StackTrace);
            }

            return sRet;
        }

        // Da un file P7M passato come object ritorna il contenuto
        // in una stringa convertita in Code64
        public string ExtractDocumentFromBuffer(Object InBuf)
        {
            string fileName;
            return ExtractDocumentFromBuffer(InBuf, false, out fileName);
        }

        public string ExtractDocumentFromBuffer(Object InBuf, out string fileName)
        {
            return ExtractDocumentFromBuffer(InBuf, false, out fileName);
        }

        public string ExtractDocumentFromBuffer(Object InBuf, bool bNotRecursive)
        {
            string fileName;
            return ExtractDocumentFromBuffer(InBuf, bNotRecursive, out fileName);
        }

        public string ExtractDocumentFromBuffer(Object InBuf, bool bNotRecursive, out string fileName)
        {
            fileName = string.Empty;
            // Dichiarazioni
            Object OutBuf = null;
            string sRet = "";
            string strDefaultConnection = "";
            int lHandle = 0;

            try
            {
                // wrapper ultimo in memoria
                if (InBuf == null)
                    InBuf = CurrentBuf;
                else
                    CurrentBuf = InBuf;

                oDigest.GetDefaultConnectionString(out strDefaultConnection);
                
                try { oDigest.CloseDb(); }
                catch { }

                oDigest.OpenDb(strDefaultConnection);                

                oDigest.P7kLoadFromBuf(InBuf, 1, out lHandle);

                int signatureCount = 0;
                
                oDigest.P7kGetSignatureCount(lHandle, out signatureCount);                
                string retInfo;
                object contentType;
                if (signatureCount == 0)
                {
                    oDigest.P7xLoadFromBuf(InBuf, out lHandle);
                    oDigest.P7xContentReadToBuf(lHandle, out OutBuf, 0);
                    oDigest.Base64Encode(OutBuf, out sRet);                       
                    oDigest.P7xFree(lHandle);
                }
                else
                {
                    oDigest.P7kContentReadToBuf(lHandle, out OutBuf, 0);
                    oDigest.Base64Encode(OutBuf, out sRet);                    
                    oDigest.P7kGetDescription(lHandle, out fileName, out retInfo);                    
                    oDigest.P7kFree(lHandle);
                }                
                //oDigest.CloseDb();

                // ricorsione se interno è un p7m
                // e non è stata richiesta la singola estrazione
                if (!bNotRecursive && IsThisDocumentSigned(ref sRet))
                {
                    P7Mmanager p7mChildDocument = new P7Mmanager();
                    sRet = p7mChildDocument.ExtractDocumentFromCode64(sRet, bNotRecursive, out fileName);
                    p7mChildDocument.Close();
                }

            }
            catch (Exception)
            {
                throw;
            }
            return sRet;
        }

        public static string GetContentType(byte[] content)
        {
            CCypher.DigitalSignViewer v = new CCypher.DigitalSignViewer();            
            CCypher.enumContentTypeEx type;
            string contentType="";
            v.GetContentTypeBuf(content, out type, out contentType);
            return contentType;
        }

        // Da un file P7M passato come stringa in Code64 ritorna il contenuto
        // in una stringa convertita in Code64
        public string ExtractDocumentFromCode64(string sCode64string, out string fileName)
        {
            return ExtractDocumentFromCode64(sCode64string, false, out fileName);
        }
        public string ExtractDocumentFromCode64(string sCode64string, bool bNotRecursive, out string fileName)
        {
            fileName = string.Empty;
            // Dichiarazioni
            string sRet = "";
            try
            {
                if (sCode64string == null)
                    sRet = ExtractDocumentFromBuffer(null, bNotRecursive);
                else
                {
                    byte[] abDocument = System.Convert.FromBase64String(sCode64string);
                    sRet = ExtractDocumentFromBuffer(abDocument, bNotRecursive, out fileName);
                }
            }
            catch (System.Exception exp)
            {
                throw new Exception(exp.Message + " " + exp.StackTrace);
            }

            return sRet;
        }

        // Da un file P7M passato come path ritorna il contenuto
        // in una stringa convertita in Code64
        public string ExtractDocumentFromFile(string sP7MFile)
        {
            return ExtractDocumentFromFile(sP7MFile, false);
        }
        public string ExtractDocumentFromFile(string sP7MFile, bool bNotRecursive)
        {
            // Dichiarazioni
            string sRet = "";
            try
            {
                if (sP7MFile == null)
                    sRet = ExtractDocumentFromBuffer(null, bNotRecursive);
                else
                {
                    FileStream fs = new System.IO.FileStream(sP7MFile, FileMode.Open, FileAccess.Read);
                    byte[] Input = new byte[fs.Length];
                    fs.Read(Input, 0, (int)fs.Length);
                    fs.Close();
                    sRet = ExtractDocumentFromBuffer(Input, bNotRecursive);
                }
            }
            catch (System.Exception exp)
            {
                throw new Exception(exp.Message + " " + exp.StackTrace);
            }

            return sRet;
        }

        private string GetNumberSignature(ref XmlDocument xmlDoc)
        {
            string sRet = "0";
            int iRet = 0;
            string sCurrentSignatureNumber = "0";
            int j = 0;
            XmlNode Node;
          
            XmlNodeList NodeList = xmlDoc.GetElementsByTagName(XML_TOKEN.TOK_XML_TAG_DOCUMENT);

            for (j = 0; j < NodeList.Count; j++)
            {
                Node = NodeList.Item(j);
                sCurrentSignatureNumber = ((XmlElement)Node).GetAttribute(XML_TOKEN.TOK_XML_TAG_DOCUMENT_ATT_NSIGNATURE);
                if (sCurrentSignatureNumber != "")
                    iRet += System.Convert.ToInt32(sCurrentSignatureNumber);
                else
                {
                    sCurrentSignatureNumber = ((XmlElement)Node).GetAttribute(XML_TOKEN.TOK_XML_TAG_DOCUMENT_ATT_NTIMESTAMP);
                    if (sCurrentSignatureNumber != "")
                        iRet += System.Convert.ToInt32(sCurrentSignatureNumber);
                }
            }
    
            sRet = Convert.ToString(iRet);

            return sRet;
        }

        private string GetNumberTimeStamp(ref XmlDocument xmlDoc)
        {
            string sRet = "0";
            int iRet = 0;
            string sCurrentSignatureNumber = "0";
            int j = 0;
            XmlNode Node;
            XmlNodeList NodeList = xmlDoc.GetElementsByTagName(XML_TOKEN.TOK_XML_TAG_DOCUMENT);

            for (j = 0; j < NodeList.Count; j++)
            {
                Node = NodeList.Item(j);
                sCurrentSignatureNumber = ((XmlElement)Node).GetAttribute(XML_TOKEN.TOK_XML_TAG_DOCUMENT_ATT_NTIMESTAMP);
                if (sCurrentSignatureNumber != "")
                    iRet += System.Convert.ToInt32(sCurrentSignatureNumber);
            }
            sRet = System.Convert.ToString(iRet);

            return sRet;
        }

        private int GetXPos(ref XmlElement Node)
        {
            // Posizione X
            int xPos = 0;
            string sTmp;
            sTmp = Node.GetAttribute(TOK_XML_TAG_OUT_XPOS);
            if (sTmp == "")
                xPos = 0;
            else
                xPos = Convert.ToInt32(sTmp);

            return xPos;
        }

        private int GetYPos(ref XmlElement Node)
        {
            // Posizione Y
            int yPos = 0;
            string sTmp;
            sTmp = Node.GetAttribute(TOK_XML_TAG_OUT_YPOS);
            if (sTmp == "")
                yPos = 0;
            else
                yPos = Convert.ToInt32(sTmp);

            return yPos;
        }        

        private string GetThisPath()
        {

            string sFullPath = Directory.GetCurrentDirectory(); // System.IO.Path.GetDirectoryName(Application.ExecutablePath); //System.IO.Directory.GetCurrentDirectory();
            sFullPath += "\\";
            return sFullPath;
        }       

        public string ParseXMLText(string str, XElement element, bool timespen)
        {
            int index = 0;
            string result = str;
            while ((index = str.IndexOf("%", index)) >= 0)
            {
                var startIndex = index;
                var endIndex = str.IndexOf("%", index + 1);
                string token = str.Substring(startIndex + 1, endIndex - startIndex - 1);
                string xmlToken = token;
                string value;
                if (timespen)
                {
                    xmlToken = token.IndexOf("_") > 0 ? token.Split('_')[1] : token;
                    value = element.Attribute(xmlToken) == null ? "" : element.Attribute(xmlToken).Value;
                }
                else
                {
                    XAttribute attribute = null;
                    if (token.IndexOf("_") > 0)
                    {
                        if (token.Split('_')[0] == "S")
                        {
                            if (element.Element("Subject") != null && element.Element("Subject").Attribute(token.Split('_')[1]) != null)
                                attribute = element.Element("Subject").Attribute(token.Split('_')[1]);
                            else if (element.Element("Details") != null && element.Element("Details").Attribute(token.Split('_')[1]) != null)
                                attribute = element.Element("Details").Attribute(token.Split('_')[1]);
                        }
                        else if (token.Split('_')[0] == "I")
                        {
                            if (element.Element("Issuer") != null)
                                attribute = element.Element("Issuer").Attribute(token.Split('_')[1]);
                        }
                        else if (token.Split('_')[0] == "D")
                        {
                            if (element.Element("Details") != null)
                                attribute = element.Element("Details").Attribute(token.Split('_')[1]);
                        }
                        else
                        {
                            throw new Exception("Token index 0 not valid " + token);
                        }
                    }else
                        attribute = element.Attribute(token);
                    value = attribute == null ? "" : attribute.Value;
                }                
                result = result.Replace("%" + token + "%", value);
                index = endIndex + 1;
            }
            return result;
        }              

        private int FindAndReplaceTagInString(ref XmlDocument XMLDocInfo, string TAG_NAME, string TAG_PARENT, string TAG_ATTRIBUTE, int SignatureNumber, ref string sOutput, ref int iLastDocument)
        {
            int nPos = 0;
            int j = 0;
            int k = 0;
            string sRet = "";
            XmlNodeList NodeList = null;
            if ((nPos = sOutput.IndexOf(TAG_NAME, 0)) >= 0)
            {
                // per TAG_NAME 
                // XML_TOKEN.TOK_XML_TAG_ATT_SIGNATURE;
                // XML_TOKEN.TOK_XML_TAG_DOCUMENT_ATT_NSIGNATURE;
                // Avviene la sostituzione diretta!
                // Altrimenti mi sposto sull'elemento di firma i-esima
                if (TAG_NAME == TOK_XML_TAG_OUT_GENERAL_TIMESTAMP)
                    sRet = SignatureNumber.ToString();
                else if (TAG_NAME == TOK_XML_TAG_OUT_GENERAL_NTIMESTAMP)
                    sRet = GetNumberSignature(ref XMLDocInfo);
                else if (TAG_NAME == TOK_XML_TAG_OUT_GENERAL_SIGNATURE)
                    sRet = SignatureNumber.ToString();
                else if (TAG_NAME == TOK_XML_TAG_OUT_GENERAL_NSIGNATURE)
                    sRet = GetNumberSignature(ref XMLDocInfo);
                else
                {
                    // Numero di Documenti
                    int iTotalDocument = XMLDocInfo.GetElementsByTagName(XML_TOKEN.TOK_XML_TAG_DOCUMENT).Count;
                    // prendo le informazioni dal file XML
                    NodeList = XMLDocInfo.GetElementsByTagName(TAG_PARENT);

                    bool bFound = false;
                    int iTmpLastDocument = iLastDocument;
                    for (k = 0; k < iTotalDocument && !bFound; k++)
                    {
                        for (j = 0; j < NodeList.Count && !bFound; j++)
                        {
                            string sSignatureNumber =
                               iLastDocument.ToString() + "." + SignatureNumber.ToString();
                            // SignatureNumber - iTmpLastDocument
                            //sSignatureNumber =  iTmpLastDocument.ToString() + "." + (j + 1).ToString();

                            if (((XmlElement)NodeList.Item(j)).GetAttribute(XML_TOKEN.TOK_XML_TAG_ATT_SIGNATURE) == sSignatureNumber)
                            {
                                sRet = ((XmlElement)NodeList.Item(j)).GetAttribute(TAG_ATTRIBUTE);
                                bFound = true;
                                iLastDocument = iTmpLastDocument;
                            }
                            if (((XmlElement)NodeList.Item(j)).GetAttribute(XML_TOKEN.TOK_XML_TAG_ATT_TIMESTAMP) == sSignatureNumber)
                            {
                                sRet = ((XmlElement)NodeList.Item(j)).GetAttribute(TAG_ATTRIBUTE);
                                bFound = true;
                                iLastDocument = iTmpLastDocument;
                            }
                        }
                        if (!bFound && j != 0)
                        {
                            iTmpLastDocument++;
                        }
                    }
                }
                // check per label condizionale tra []
                try
                {
                    bool flag = false;
                    if (nPos > 0 && sOutput.Substring(nPos - 1, 1) == "]")
                    {
                        flag = true;
                        if (sRet.Length == 0)
                            sOutput = sOutput.Substring(0, sOutput.Substring(0, nPos).LastIndexOf("[")) + sOutput.Substring(nPos);
                    }
                    if (nPos + TAG_NAME.Length < sOutput.Length && sOutput.Substring(nPos + TAG_NAME.Length, 1) == "[")
                    {
                        flag = true;
                        if (sRet.Length > 0)
                            sOutput = sOutput.Substring(0, nPos + TAG_NAME.Length) + sOutput.Substring(sOutput.IndexOf("]", nPos));
                    }
                    if (flag)
                    {
                        sOutput = sOutput.Replace("[", "");
                        sOutput = sOutput.Replace("]", "");
                    }
                }
                catch (Exception ex) 
                {
                    logger.Error(ex);
                }
            }
            sOutput = sOutput.Replace(TAG_NAME, sRet);

            return nPos;
        }

        private string ParseXMLInformationForWMFFile(string sValue, ref XmlDocument XMLDocInfo, ref XmlDocument xmlDocTemplate, int NumberSignature, ref int iLastDocument, ref float fXIdent)
        {

            bool bFound = true;
            while (bFound)
            {
                // Description/File
                if (
                    FindAndReplaceTagInString(ref XMLDocInfo, TOK_XML_TAG_OUT_DESCRIPTION, XML_TOKEN.TOK_XML_TAG_DETAILS,
                                              XML_TOKEN.TOK_XML_TAG_DESCRIPTION, NumberSignature, ref sValue,
                                              ref iLastDocument) > 0)
                    bFound = true;
                else if (
                    FindAndReplaceTagInString(ref XMLDocInfo, TOK_XML_TAG_OUT_FILENAME,
                                              XML_TOKEN.TOK_XML_TAG_DETAILS, XML_TOKEN.TOK_XML_TAG_FILENAME,
                                              NumberSignature, ref sValue, ref iLastDocument) > 0)
                    bFound = true;
                // ISSUER
                // I_O
                else if (
                    FindAndReplaceTagInString(ref XMLDocInfo, TOK_XML_TAG_OUT_ISSUER_O,
                                              XML_TOKEN.TOK_XML_TAG_ISSUER, XML_TOKEN.TOK_XML_TAG_ISSUER_ATT_O,
                                              NumberSignature, ref sValue, ref iLastDocument) > 0)
                    bFound = true;
                // I_C
                else if (
                    FindAndReplaceTagInString(ref XMLDocInfo, TOK_XML_TAG_OUT_ISSUER_C,
                                              XML_TOKEN.TOK_XML_TAG_ISSUER,
                                              XML_TOKEN.TOK_XML_TAG_ISSUER_ATT_C, NumberSignature,
                                              ref sValue, ref iLastDocument) > 0)
                    bFound = true;
                // I_OU
                else if (
                    FindAndReplaceTagInString(ref XMLDocInfo, TOK_XML_TAG_OUT_ISSUER_OU,
                                              XML_TOKEN.TOK_XML_TAG_ISSUER,
                                              XML_TOKEN.TOK_XML_TAG_ISSUER_ATT_OU, NumberSignature,
                                              ref sValue, ref iLastDocument) > 0)
                    bFound = true;
                // I_CN
                else if (
                    FindAndReplaceTagInString(ref XMLDocInfo, TOK_XML_TAG_OUT_ISSUER_CN,
                                              XML_TOKEN.TOK_XML_TAG_ISSUER,
                                              XML_TOKEN.TOK_XML_TAG_ISSUER_ATT_CN, NumberSignature,
                                              ref sValue, ref iLastDocument) > 0)
                    bFound = true;
                // SUBJECT
                // S_NAME
                else if (
                    FindAndReplaceTagInString(ref XMLDocInfo, TOK_XML_TAG_OUT_SUBJECT_NAME,
                                              XML_TOKEN.TOK_XML_TAG_SUBJECT,
                                              XML_TOKEN.TOK_XML_TAG_SUBJECT_ATT_NAME,
                                              NumberSignature, ref sValue, ref iLastDocument) >
                    0)
                    bFound = true;
                // S_O
                else if (
                    FindAndReplaceTagInString(ref XMLDocInfo, TOK_XML_TAG_OUT_SUBJECT_O,
                                              XML_TOKEN.TOK_XML_TAG_SUBJECT,
                                              XML_TOKEN.TOK_XML_TAG_SUBJECT_ATT_O,
                                              NumberSignature, ref sValue, ref iLastDocument) >
                    0)
                    bFound = true;
                // S_C
                else if (
                    FindAndReplaceTagInString(ref XMLDocInfo, TOK_XML_TAG_OUT_SUBJECT_C,
                                              XML_TOKEN.TOK_XML_TAG_SUBJECT,
                                              XML_TOKEN.TOK_XML_TAG_SUBJECT_ATT_C,
                                              NumberSignature, ref sValue,
                                              ref iLastDocument) > 0)
                    bFound = true;
                // S_CN
                else if (
                    FindAndReplaceTagInString(ref XMLDocInfo,
                                              TOK_XML_TAG_OUT_SUBJECT_CN,
                                              XML_TOKEN.TOK_XML_TAG_SUBJECT,
                                              XML_TOKEN.TOK_XML_TAG_SUBJECT_ATT_CN,
                                              NumberSignature, ref sValue,
                                              ref iLastDocument) > 0)
                    bFound = true;
                // S_CO
                else if (
                    FindAndReplaceTagInString(ref XMLDocInfo,
                                              TOK_XML_TAG_OUT_SUBJECT_CO,
                                              XML_TOKEN.TOK_XML_TAG_SUBJECT,
                                              XML_TOKEN.
                                                  TOK_XML_TAG_SUBJECT_ATT_CO,
                                              NumberSignature, ref sValue,
                                              ref iLastDocument) > 0)
                    bFound = true;
                // S_N
                else if (
                    FindAndReplaceTagInString(ref XMLDocInfo,
                                              TOK_XML_TAG_OUT_SUBJECT_N,
                                              XML_TOKEN.TOK_XML_TAG_SUBJECT,
                                              XML_TOKEN.
                                                  TOK_XML_TAG_SUBJECT_ATT_N,
                                              NumberSignature, ref sValue,
                                              ref iLastDocument) > 0)
                    bFound = true;
                // S_D
                else if (
                    FindAndReplaceTagInString(ref XMLDocInfo,
                                              TOK_XML_TAG_OUT_SUBJECT_D,
                                              XML_TOKEN.
                                                  TOK_XML_TAG_SUBJECT,
                                              XML_TOKEN.
                                                  TOK_XML_TAG_SUBJECT_ATT_D,
                                              NumberSignature,
                                              ref sValue,
                                              ref iLastDocument) > 0)
                    bFound = true;
                // S_FC
                else if (
                    FindAndReplaceTagInString(ref XMLDocInfo,
                                              TOK_XML_TAG_OUT_SUBJECT_FC,
                                              XML_TOKEN.
                                                  TOK_XML_TAG_SUBJECT,
                                              XML_TOKEN.
                                                  TOK_XML_TAG_SUBJECT_ATT_FC,
                                              NumberSignature,
                                              ref sValue,
                                              ref iLastDocument) > 0)
                    bFound = true;
                // DETAILS
                // SN
                else if (
                    FindAndReplaceTagInString(ref XMLDocInfo,
                                              TOK_XML_TAG_OUT_DETAILS_SN,
                                              XML_TOKEN.
                                                  TOK_XML_TAG_DETAILS,
                                              XML_TOKEN.
                                                  TOK_XML_TAG_DETAILS_ATT_SN,
                                              NumberSignature,
                                              ref sValue,
                                              ref iLastDocument) >
                    0)
                    bFound = true;
                // Lease
                else if (
                    FindAndReplaceTagInString(
                        ref XMLDocInfo,
                        TOK_XML_TAG_OUT_DETAILS_LEASE,
                        XML_TOKEN.TOK_XML_TAG_DETAILS,
                        XML_TOKEN.
                            TOK_XML_TAG_DETAILS_ATT_LEASE,
                        NumberSignature, ref sValue,
                        ref iLastDocument) > 0)
                    bFound = true;
                // Expire
                else if (
                    FindAndReplaceTagInString(
                        ref XMLDocInfo,
                        TOK_XML_TAG_OUT_DETAILS_EXPIRE,
                        XML_TOKEN.TOK_XML_TAG_DETAILS,
                        XML_TOKEN.
                            TOK_XML_TAG_DETAILS_ATT_EXPIRE,
                        NumberSignature, ref sValue,
                        ref iLastDocument) > 0)
                    bFound = true;
                // GENERALI
                // NumberSignature
                else if (
                    FindAndReplaceTagInString(
                        ref XMLDocInfo,
                        TOK_XML_TAG_OUT_GENERAL_NSIGNATURE,
                        "", "", NumberSignature,
                        ref sValue,
                        ref iLastDocument) > 0)
                    bFound = true;
                // Signature
                else if (
                    FindAndReplaceTagInString(
                        ref XMLDocInfo,
                        TOK_XML_TAG_OUT_GENERAL_SIGNATURE,
                        "", "", NumberSignature,
                        ref sValue,
                        ref iLastDocument) > 0)
                    bFound = true;
                else
                    bFound = false;
            }


            // identazione se sottofirma
            int k = 0;
            if (iLastDocument > 1)
            {
                XmlNodeList NodeListIdent = xmlDocTemplate.GetElementsByTagName(TOK_XML_TAG_OUT_SIGNATURE);
                if (NodeListIdent.Count == 1)
                {
                    string sSpace = ((XmlElement)NodeListIdent.Item(0)).GetAttribute(TOK_XML_TAG_OUT_SIGNATURE_NESTED);
                    if (sSpace != "")
                    {
                        fXIdent += Convert.ToInt32(sSpace);
                    }
                }
            }
            char[] sArray = sValue.ToCharArray();
            sValue = "";
            for (k = 0; k < sArray.Length; k++)
            {
                if (sArray[k] != '\r' && sArray[k] != '\n')
                    sValue += sArray[k];
            }

            return sValue;
        }

        private string ParseXMLInformationForWMFFileTS(string sValue, ref XmlDocument XMLDocInfo, ref XmlDocument xmlDocTemplate, int NumberSignature, ref int iLastDocument, ref float fXIdent)
        {

            bool bFound = true;
            while (bFound)
            {
                // Description/File
                // TimeStamp
                if (
                    FindAndReplaceTagInString(ref XMLDocInfo, TOK_XML_TAG_OUT_TS_ITIS, XML_TOKEN.TOK_XML_TAG_TIMESTAMP,
                                              XML_TOKEN.TOK_XML_TAG_ISSUER, NumberSignature, ref sValue,
                                              ref iLastDocument) >
                    0)
                    bFound = true;
                else if (
                    FindAndReplaceTagInString(ref XMLDocInfo, TOK_XML_TAG_OUT_TS_LEASE, XML_TOKEN.TOK_XML_TAG_TIMESTAMP,
                                              XML_TOKEN.TOK_XML_TAG_DETAILS_ATT_LEASE, NumberSignature, ref sValue,
                                              ref iLastDocument) > 0)
                    bFound = true;
                else if (
                    FindAndReplaceTagInString(ref XMLDocInfo, TOK_XML_TAG_OUT_TS_EXPIRE,
                                              XML_TOKEN.TOK_XML_TAG_TIMESTAMP,
                                              XML_TOKEN.TOK_XML_TAG_DETAILS_ATT_EXPIRE, NumberSignature, ref sValue,
                                              ref iLastDocument) > 0)
                    bFound = true;
                else if (
                    FindAndReplaceTagInString(ref XMLDocInfo, TOK_XML_TAG_OUT_TS_SUBJECT,
                                              XML_TOKEN.TOK_XML_TAG_TIMESTAMP, XML_TOKEN.TOK_XML_TAG_SUBJECT,
                                              NumberSignature, ref sValue, ref iLastDocument) > 0)
                    bFound = true;
                else if (
                    FindAndReplaceTagInString(ref XMLDocInfo, TOK_XML_TAG_OUT_TS_ISSUER,
                                              XML_TOKEN.TOK_XML_TAG_TIMESTAMP, XML_TOKEN.TOK_XML_TAG_ISSUER,
                                              NumberSignature, ref sValue, ref iLastDocument) > 0)
                    bFound = true;
                else if (
                    FindAndReplaceTagInString(ref XMLDocInfo, TOK_XML_TAG_OUT_GENERAL_NTIMESTAMP, "", "",
                                              NumberSignature, ref sValue, ref iLastDocument) > 0)
                    bFound = true;
                else if (
                    FindAndReplaceTagInString(ref XMLDocInfo, TOK_XML_TAG_OUT_GENERAL_TIMESTAMP, "",
                                              "", NumberSignature, ref sValue, ref iLastDocument) > 0)
                    bFound = true;
                else
                    bFound = false;
            }


            // identazione se sottofirma
            int k = 0;
            if (iLastDocument > 1)
            {
                XmlNodeList NodeListIdent = xmlDocTemplate.GetElementsByTagName(TOK_XML_TAG_OUT_SIGNATURE);
                if (NodeListIdent.Count == 1)
                {
                    string sSpace = ((XmlElement)NodeListIdent.Item(0)).GetAttribute(TOK_XML_TAG_OUT_SIGNATURE_NESTED);
                    if (sSpace != "")
                    {
                        fXIdent += System.Convert.ToInt32(sSpace);
                    }
                }
            }
            char[] sArray = sValue.ToCharArray();
            sValue = "";
            for (k = 0; k < sArray.Length; k++)
            {
                if (sArray[k] != '\r' && sArray[k] != '\n')
                    sValue += sArray[k];
            }

            return sValue;
        }

        // Converte una stringa
        // in formato ascii in una stringa code64
        public string ConvertFromCode64StringToString(string sCode64String)
        {
            try
            {
                byte[] buffer = Convert.FromBase64String(sCode64String);
                //return  System.Text.Encoding.ASCII.GetString(buffer, 0, buffer.Length); 
                return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message + " " + exp.StackTrace);
            }
        }

        // Converte una stringa
        // in formato code64 in una stringa ascii
        public string ConvertFromStringToCode64String(string sString)
        {
            try
            {
                //System.Text.ASCIIEncoding utf = new System.Text.ASCIIEncoding();
                UTF8Encoding utf = new UTF8Encoding();
                byte[] buffer = utf.GetBytes(sString);
                return Convert.ToBase64String(buffer, 0, buffer.Length);
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message + " " + exp.StackTrace);
            }
        }

        // Scrive il contenuto di una stringa in code64
        // in un file
        public bool WriteDocumentToFile(string sCode64string, string sFileOutput)
        {

            bool bRet = false;
            try
            {
                byte[] abDocument = Convert.FromBase64String(sCode64string);
                FileStream fs = new FileStream(sFileOutput, FileMode.Create, FileAccess.Write);
                fs.Write(abDocument, 0, abDocument.GetLength(0));
                fs.Close();
                bRet = true;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message + " " + exp.StackTrace);
            }

            return bRet;
        }

        public static string Munge(string s)
        {
            string o = string.Empty;
            char[] c = s.ToCharArray();
            for (int i = 0; i < c.Length; i++)
                if ((c[i] >= 'a' && c[i] <= 'z') || (c[i] >= 'A' && c[i] <= 'Z') || (c[i] >= '0' && c[i] <= '9'))
                    o += c[i].ToString();
                else
                    o += "&#" + ((int)c[i]) + ";";
            return o;
        }

    }
}
