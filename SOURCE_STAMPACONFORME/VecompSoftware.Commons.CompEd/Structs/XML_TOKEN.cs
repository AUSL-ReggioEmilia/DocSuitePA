
namespace VecompSoftware.Commons.CompEd.Structs
{
    public struct XML_TOKEN
    {
        // tag
        public const string TOK_XML_TAG_DOCUMENT = "Document";
        public const string TOK_XML_TAG_SIGNATURE = "Signature";
        public const string TOK_XML_TAG_TIMESTAMP = "TimeStamp";
        public const string TOK_XML_TAG_ISSUER = "Issuer";
        public const string TOK_XML_TAG_SUBJECT = "Subject";
        public const string TOK_XML_TAG_DETAILS = "Details";
        public const string TOK_XML_TAG_DESCRIPTION = "Description";
        public const string TOK_XML_TAG_FILENAME = "FileName";
        public const string TOK_XML_TAG_DATE = "Date";
        // attributi 
        public const string TOK_XML_TAG_ATT_SIGNATURE = "Signature";
        public const string TOK_XML_TAG_ATT_TIMESTAMP = "TimeStamp";
        public const string TOK_XML_TAG_DOCUMENT_ATT_NSIGNATURE = "NumberSignature";
        public const string TOK_XML_TAG_DOCUMENT_ATT_NTIMESTAMP = "NumberTimeStamp";
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
        // attributi sulla firma
        public const string TOK_XML_TAG_PKCS7_FileHeaderDes = "PKCS7-File-HeaderDescription";
        public const string TOK_XML_TAG_PKCS7_FileName = "Filename";
    }
}
