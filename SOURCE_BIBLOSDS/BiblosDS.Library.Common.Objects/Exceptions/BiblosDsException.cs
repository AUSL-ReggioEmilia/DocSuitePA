using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Exceptions
{
    [DataContract]
    public class BiblosDsException 
    {
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string StackTrace { get; set; }
        [DataMember]
        public FaultCode Code { get; set; }

        public BiblosDsException(Exception ex)
        {
            try
            {
                throw ex;
            }
            catch (Exceptions.Archive_Exception error)
            {
                Message = error.Message;
                StackTrace = error.StackTrace;
                Code = FaultCode.Archive_Exception;
            }
            catch (Exceptions.ArchiveStorage_Exception error)
            {
                Message = error.Message;
                StackTrace = error.StackTrace;
                Code = FaultCode.ArchiveStorage_Exception;
            }
            catch (Exceptions.Attribute_Exception error)
            {
                Message = error.Message;
                StackTrace = error.StackTrace;
                Code = FaultCode.Attribute_Exception;
            }
            catch (Exceptions.AttributeRequired_Exception error)
            {
                Message = error.Message;
                StackTrace = error.StackTrace;
                Code = FaultCode.AttributeRequired_Exception;
            }
            catch (Exceptions.DocumentCheckOut_Exception error)
            {
                Message = error.Message;
                StackTrace = error.StackTrace;
                Code = FaultCode.DocumentCheckOut_Exception;
            }
            catch (Exceptions.DocumentNotConvertible_Exception error)
            {
                Message = error.Message;
                StackTrace = error.StackTrace;
                Code = FaultCode.DocumentNotConvertible_Exception;
            }
            catch (Exceptions.DocumentNotFound_Exception error)
            {
                Message = error.Message;
                StackTrace = error.StackTrace;
                Code = FaultCode.DocumentNotFound_Exception;
            }
            catch (Exceptions.DocumentPrimaryKey_Exception error)
            {
                Message = error.Message;
                StackTrace = error.StackTrace;
                Code = FaultCode.DocumentPrimaryKey_Exception;
            }
            catch (Exceptions.DocumentReadOnly_Exception error)
            {
                Message = error.Message;
                StackTrace = error.StackTrace;
                Code = FaultCode.DocumentReadOnly_Exception;
            }
            catch (Exceptions.FileNotFound_Exception error)
            {
                Message = error.Message;
                StackTrace = error.StackTrace;
                Code = FaultCode.FileNotFound_Exception;
            }
            catch (Exceptions.FileNotUploaded_Exception error)
            {
                Message = error.Message;
                StackTrace = error.StackTrace;
                Code = FaultCode.FileNotUploaded_Exception;
            }
            catch (Exceptions.Permission_Exception error)
            {
                Message = error.Message;
                StackTrace = error.StackTrace;
                Code = FaultCode.Permission_Exception;
            }
            catch (Exceptions.StorageAreaConfiguration_Exception error)
            {
                Message = error.Message;
                StackTrace = error.StackTrace;
                Code = FaultCode.StorageAreaConfiguration_Exception;
            }
            catch (Exceptions.StorageConfiguration_Exception error)
            {
                Message = error.Message;
                StackTrace = error.StackTrace;
                Code = FaultCode.StorageConfiguration_Exception;
            }
            catch (Exceptions.StorageIsProcessingFile_Exception error)
            {
                Message = error.Message;
                StackTrace = error.StackTrace;
                Code = FaultCode.StorageIsProcessingFile_Exception;
            }
            catch (Exceptions.StorageNotFound_Exception error)
            {
                Message = error.Message;
                StackTrace = error.StackTrace;
                Code = FaultCode.StorageNotFound_Exception;
            }
            catch (DocumentNotReadyForAttach_Exception error)
            {
                Message = error.Message;
                StackTrace = error.StackTrace;
                Code = FaultCode.DocumentNotReadyForAttach_Exception;
            }
            catch (DocumentWithoutContent_Exception error)
            {
                Message = error.Message;
                StackTrace = error.StackTrace;
                Code = FaultCode.DocumentWithoutContent_Exception;
            }
            catch (DocumentConnectionExists_Exception error)
            {
                Message = error.Message;
                StackTrace = error.StackTrace;
                Code = FaultCode.DocumentConnectionExists_Exception;
            }
            catch (DocumentAttachNotFound_Exception error)
            {
                Message = error.Message;
                StackTrace = error.StackTrace;
                Code = FaultCode.DocumentAttachNotFound_Exception;
            }
            catch (Exception error)
            {
                Message = error.Message;
                StackTrace = error.StackTrace;
                Code = FaultCode.GENERIC_ERROR;
            }        
        }

        public BiblosDsException(string Message, FaultCode code)
        {
            this.Message = Message;
            this.Code = code;
        }

        public BiblosDsException(string Message, string StackTrace, FaultCode code)
        {
            this.Message = Message;
            this.StackTrace = StackTrace;
            this.Code = code;
        }
    }

    [DataContract]
    public enum FaultCode
    {
        [EnumMember]
        GENERIC_ERROR,
        [EnumMember]
        Archive_Exception,
        [EnumMember]
        ArchiveStorage_Exception,
        [EnumMember]
        Attribute_Exception,
        [EnumMember]
        AttributeRequired_Exception,
        [EnumMember]
        DocumentCheckOut_Exception,
        [EnumMember]
        DocumentNotConvertible_Exception,
        [EnumMember]
        DocumentNotFound_Exception,
        [EnumMember]
        DocumentPrimaryKey_Exception,
        [EnumMember]
        DocumentReadOnly_Exception,
        [EnumMember]
        FileNotFound_Exception,
        [EnumMember]
        FileNotUploaded_Exception,
        [EnumMember]
        Permission_Exception,
        [EnumMember]
        StorageAreaConfiguration_Exception,
        [EnumMember]
        StorageConfiguration_Exception,
        [EnumMember]
        StorageIsProcessingFile_Exception,
        [EnumMember]
        StorageNotFound_Exception,
        [EnumMember]
        DocumentNotReadyForAttach_Exception,
        [EnumMember]
        DocumentWithoutContent_Exception,
        [EnumMember]
        DocumentConnectionExists_Exception,
        [EnumMember]
        DocumentAttachNotFound_Exception,
    }

}
